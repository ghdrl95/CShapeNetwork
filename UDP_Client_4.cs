using data_struct;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;

/*
 * 브로드캐스트 : 해당 컴퓨터가 속한 네트워크 영역에 연결된 모든 컴퓨터/스마트폰/인터넷장비
 * 에게 동일한 데이터를 동일한 포트로 송신하는 기능
 * 이 데이터를 받는 장치들은 포트번호를 고정한 UDP 소켓을 생성해 수신을 대기
 * 단점 : 해당 데이터를 받는 장치나 안받는 장치에 상관없이 모든 장치에 데이터를 송신하기
 * 때문에 해당 네트워크 영역에 부하가 발생
 * UDP 송신 프로그램 - 브로드캐스트를 통해 데이터를 송신
 */
namespace client
{
    class Program
    {
        static void Main(string[] args)
        {
            //UdpClient 객체 생성
            UdpClient sender = new UdpClient();
            //sender.EnableBroadcast : 해당 UDP소켓이 브로드캐스트를 사용하는지 설정하는 변수
            // true (기본값) : 브로드캐스트 허용.
            // false : 브로드캐스트로 송신하거나 수신하지 못하도록 설정

            //IPEndPoint 객체 생성 - 브로드캐스트설정 및 12000번포트로 설정
            //전송할 대상의 IP를 255.255.255.255로 설정하면 브로드캐스트로 송신하게됨
            IPEndPoint des_ip = new IPEndPoint(IPAddress.Parse("255.255.255.255"), 12000);

            Console.Write("ID 입력 : ");
            string id = Console.ReadLine();
            for (; ; )
            {
                //보낼데이터를 저장한 문자열변수 선언 및 초기화
                Console.Write("채팅 입력 : ");
                string data = string.Format("{0} : {1}", id, Console.ReadLine()) ;
                BinaryFormatter formatter = new BinaryFormatter();
                MemoryStream stream = new MemoryStream();
                //네트워크에 UDP소켓으로 송신
                formatter.Serialize(stream, data);
                byte[] send_data = stream.ToArray();
                sender.Send(send_data, send_data.Length, des_ip);
                stream.Close();
            }
            //UdpClient 객체 종료
            sender.Close();
        }
    }
} 



