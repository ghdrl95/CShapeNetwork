using data_struct;
using System;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

/*
 * TCP 클라이언트 기본 예제 - 자료형,객체를 네트워크상에 전달
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
                BinaryFormatter formatter = new BinaryFormatter();
                for (; ; )//여러 데이터를 송신할수있도록 무한반복문
                {
                    //Console.Write("송신할 데이터 입력 (quit:종료): ");
                    //string data = Console.ReadLine();//"Hello Network!";
                    Student data = new Student();
                    Console.Write("서버에 전송할 이름 입력 : ");
                    data.name = Console.ReadLine();
                    Console.Write("서버에 전송할 전화번호 입력 : ");
                    data.phone = Console.ReadLine();
                    //사용자가 종료문자열을 입력했는지 파악
                    if(data.name.Equals("quit"))
                    //if(data.Equals("quit"))
                    {
                        Console.WriteLine("서버연결 종료");
                        break;
                    }
                    formatter.Serialize(stream, data);
                }
            }
            //연결 종료
            client.Close();//서버와 연결 끊기
        }
    }
}
