using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Server
{
    internal class Program
    {
        static TcpListener listener;
        static Dictionary<string, TcpClient> roleClients = new Dictionary<string, TcpClient>(); // role → client

        static void Main(string[] args)
        {
            try
            {
                IPAddress localIP = GetLocalIPAddress();
                int port = 3333;

                Console.WriteLine($"[서버 IP 주소]: {localIP}");
                Console.WriteLine($"[서버 포트]: {port}");

                listener = new TcpListener(localIP, port);
                listener.Start();
                Console.WriteLine("서버가 시작되었습니다.");

                while (true)
                {
                    TcpClient client = listener.AcceptTcpClient();
                    Console.WriteLine("클라이언트가 연결되었습니다.");

                    Thread clientThread = new Thread(HandleClient);
                    clientThread.Start(client);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("에러: " + ex.Message);
            }
        }

        public static IPAddress GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip;
                }
            }
            throw new Exception("로컬 IPv4 주소를 찾을 수 없습니다.");
        }


        static void HandleClient(object obj)
        {
            TcpClient client = (TcpClient)obj;
            StreamReader reader = new StreamReader(client.GetStream(), Encoding.UTF8);
            StreamWriter writer = new StreamWriter(client.GetStream(), new UTF8Encoding(false)) { AutoFlush = true };

            string role = null;

            try
            {
                while (true)
                {
                    string msg = reader.ReadLine();
                    if (msg == null) break;

                    JObject json = JObject.Parse(msg);
                    string type = json["type"]?.ToString();

                    if (type == "register")
                    {
                        role = json["role"]?.ToString();
                        if (role != null)
                        {
                            roleClients[role] = client;
                            Console.WriteLine($"[{role}] 클라이언트 등록됨.");
                        }
                        continue;
                    }

                    if (type == "order")
                    {
                        Console.WriteLine("[주문 수신] " + json.ToString());
                        SendTo("kitchen", msg);
                    }
                    else if (type == "complete")
                    {
                        Console.WriteLine("[조리 완료 수신] " + json.ToString());
                        SendTo("stock", msg);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[{role ?? "unknown"} 클라이언트 오류] {ex.Message}");
            }
            finally
            {
                if (role != null && roleClients.ContainsKey(role))
                {
                    roleClients.Remove(role);
                }

                Console.WriteLine($"[{role ?? "unknown"}] 클라이언트 연결 종료됨.");
                reader.Close();
                writer.Close();
                client.Close();
            }
        }

        static void SendTo(string role, string msg)
        {
            if (roleClients.ContainsKey(role))
            {
                StreamWriter writer = new StreamWriter(roleClients[role].GetStream(), new UTF8Encoding(false)) { AutoFlush = true };
                writer.WriteLine(msg);
                Console.WriteLine($"→ {role}에게 메시지 전송됨:\n{msg}");
            }
            else
            {
                Console.WriteLine($"[경고] {role} 클라이언트가 연결되어 있지 않음");
            }
        }
    }
}
