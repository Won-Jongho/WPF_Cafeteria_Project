using System;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace ManagerUI.Tcp
{

    public class ClientConnector
    {
        static TcpClient client;
        static StreamReader reader;
        static StreamWriter writer;

        // 메뉴별 가격표
        private static readonly Dictionary<string, int> priceMap = new Dictionary<string, int>
            {
                { "에스프레소", 1000 },
                { "아메리카노", 2000 },
                { "헤이즐넛아메리카노", 2500 },
                { "카페라떼", 3500 },
                { "바닐라라떼", 4000 }
                // 필요 시 메뉴 추가
            };

        public void Networking(string host, int port)
        {
            client = new TcpClient(host, port);
            NetworkStream stream = client.GetStream();

            reader = new StreamReader(stream, Encoding.UTF8);
            writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true };

            RegisterToServer(); // 연결 후 type role 전송

            // 수신 스레드 시작
            Thread receiveThread = new Thread(ReceiveFromServer);
            receiveThread.IsBackground = true;
            receiveThread.Start();
        }

        private void RegisterToServer()
        {
            string registerMsg = "{\"type\":\"register\", \"role\":\"stock\"}";
            writer.WriteLine(registerMsg);
            Debug.WriteLine("[클라이언트 등록] " + registerMsg);
        }
        private void ReceiveFromServer()
        {
            while (true)
            {
                try
                {
                    string msg = reader.ReadLine();
                    if (msg == null) break;

                    Debug.WriteLine($"\n[서버 → 클라이언트] {msg}");

                    JObject json = JObject.Parse(msg);

                    SaveOrderToCsv(json);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("[수신 오류] " + ex.Message);
                    break;
                }
            }
        }

        static void SaveOrderToCsv(JObject json)
        {
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "sales.csv");
            string date = DateTime.Now.ToString("yyyy-MM-dd");

            // 파일 내용 읽어서 기존의 매출 리스트를 로드
            List<string> lines = File.Exists(filePath) ? File.ReadAllLines(filePath).ToList() : new List<string>();

            // 만약 파일이 비어있으면 헤더 추가
            if (lines.Count == 0)
            {
                lines.Add("menu,quantity,price,date"); // 헤더 추가
            }

            JArray items = (JArray)json["items"];

            foreach (var item in items)
            {
                string menu = item["menu"]?.ToString() ?? "";
                int quantity = item["quantity"]?.Value<int>() ?? 0;
                int price = priceMap.ContainsKey(menu) ? priceMap[menu] : 0;

                // 기존 데이터에서 해당 날짜, 품목을 찾기
                bool itemExists = false;

                for (int i = 1; i < lines.Count; i++) // 첫 번째 줄은 헤더라서 건너뛰기
                {
                    var columns = lines[i].Split(',');

                    string existingDate = columns[3]; // 기존 날짜는 4번째 칼럼
                    string existingMenu = columns[0]; // 기존 품목은 1번째 칼럼
                    int existingQuantity = int.Parse(columns[1]); // 기존 수량은 2번째 칼럼

                    // 같은 날짜와 품목이 있다면 수량 추가
                    if (existingDate == date && existingMenu == menu)
                    {
                        // 기존 라인 업데이트
                        lines[i] = $"{existingMenu},{existingQuantity + quantity},{price},{date}";
                        itemExists = true;
                        Debug.WriteLine("같은 품목 수량 추가 실행 됨");
                        break; // 해당 항목을 수정했으므로 더 이상 체크할 필요 없음
                    }
                }

                // 해당 항목이 없으면 새 항목으로 추가
                if (!itemExists)
                {
                    Debug.WriteLine("없는 품목 추가 실행 됨");
                    lines.Add($"{menu},{quantity},{price},{date}"); // 새로운 항목을 추가
                }
            }

            // 수정된 내용을 기존 파일에 덮어 씌우기
            File.WriteAllLines(filePath, lines, Encoding.UTF8);
            Debug.WriteLine("filewriteallline 실행됨");
        }
    }
}