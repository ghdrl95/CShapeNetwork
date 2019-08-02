using System;
using System.Net.Sockets;
using System.Text;

/*
 * TCP 클라이언트 기본 예제
 */
namespace client
{
    class Program
    {
        static void Main(string[] args)
        {
            //접속할 서버의 Ip,Port를 인자값으로 TCPClient객체 생성
            //127.0.0.1: 컴퓨터에 다른프로그램을 접근할때 사용하는 IPv4주소
            //내컴퓨터에서 8000번포트로 실행하는 서버에 접속요청
            TcpClient client = new TcpClient("127.0.0.1",8000);//socket생성 및 connect
            //서버에게 데이터 송신
            //using : 영역을 벗어나면 소괄호안에 있는 객체의 Close메소드를 자동호출
            using(NetworkStream stream = client.GetStream())
            {
                for (; ; )//여러 데이터를 송신할수있도록 무한반복문
                {
                    Console.Write("송신할 데이터 입력 (quit:종료): ");
                    string data = Console.ReadLine();//"Hello Network!";
                    //사용자가 종료문자열을 입력했는지 파악
                    if(data.Equals("quit"))
                    {
                        Console.WriteLine("서버연결 종료");
                        break;
                    }
                    byte[] byte_data = Encoding.Default.GetBytes(data);
                    stream.Write(byte_data, 0, byte_data.Length);
                }
            }
            //연결 종료
            client.Close();//서버와 연결 끊기
        }
    }
}
