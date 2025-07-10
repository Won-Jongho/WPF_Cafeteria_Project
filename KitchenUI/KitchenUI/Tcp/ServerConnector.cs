using Sharedlib.Models; // 주문 모델
using System;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading;




namespace KitchenApp.Tcp
{
    public class KitchenClientConnector
    {
        private TcpClient _client;
        private NetworkStream _stream;
        private StreamReader _reader;
        private StreamWriter _writer;

        public event Action<Order> OrderReceived;


        public void Connect(string host, int port)
        {
            _client = new TcpClient();
            _client.Connect(host, port);

            _stream = _client.GetStream();
            _reader = new StreamReader(_client.GetStream(), Encoding.UTF8);
            _writer = new StreamWriter(_client.GetStream(), new UTF8Encoding(false)) { AutoFlush = true };

            SendRegist(); // 연결 후 type role 전송

            // 수신 스레드 시작
            Thread receiveThread = new Thread(ReceiveLoop);
            receiveThread.IsBackground = true;
            receiveThread.Start();
        }

        // 👋 서버에 역할 전송
        private void SendRegist()
        {
            string registMsg = "{\"type\":\"register\", \"role\":\"kitchen\"}\n";
            _writer.Write(registMsg);

        }

        // 📥 서버로부터 주문 수신
        private void ReceiveLoop()
        {
            try
            {
                while (true)
                {
                    string msg = _reader.ReadLine();
                    Debug.WriteLine("수신된 원본: " + msg);

                    if (string.IsNullOrWhiteSpace(msg))
                        continue; // 빈 줄 무시

                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true};
                    var wrapper = JsonSerializer.Deserialize<Order>(msg, options);
                    OrderReceived?.Invoke(wrapper);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("수신 중 오류 발생: " + ex.Message);
            }
        }

        // 📤 주문 수락 메시지 전송
        public void SendAcceptOrder(Order order)
        {
            try
            {
                var options = new JsonSerializerOptions
                {
                    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                    PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower // 직렬화 시 camelCase로 변환
                };
                var acceptMessage = new
                {
                    type = "complete",
                    order_id = order.Order_Id,
                    items = order.Items
                    
                };

                string json = JsonSerializer.Serialize(acceptMessage, options) + "\n";
                _writer.Write(json);

                Debug.WriteLine($"주문 수락 전송: {order.Order_Id}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"주문 수락 전송 실패: {ex.Message}");
            }
        }



        public void Close()
        {
            _writer?.Close();
            _reader?.Close();
            _stream?.Close();
            _client?.Close();
        }
    }
}
