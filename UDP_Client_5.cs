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
 * 멀티캐스트 : 224.0.1.0 ~ 239.255.255.255 중 특정 네트워크에 가입해 1대다 통신을 사용하는
 * 기법. 224.0.0.0~244.0.0.255 는 네트워크 장치가 사용하는 영역으로 멀티캐스트에 가입할수없음
 * 멀티캐스트에 가입할 수 있는 소켓은 UDP 소켓만 가입가능. UDP 소켓은 여러개의 멀티캐스트를
 * 가입할 수 있고, 탈퇴가 자유로움. 가입된 멀티캐스트로 들어오는 데이터를 수신할수있으나
 * 포트번호가 다르면 수신받을 수 없음.
 * 멀티캐스트 그룹으로 데이터 송신시 IP설정을 멀티캐스트의 IP로 설정해 데이터 송신
 * 멀티캐스트는 다른 네트워크 영역에 있는 UDP소켓에게 전달할 수 있음.
 * TTL (Time To Live) : 네트워크 장비(라우터)를 거칠때마다 TTL값이 1씩 감소해
 * TTL이 0이되면 데이터가 소멸됨. TTL을 높힐수록 물리적으로 멀리떨어진 UDP소켓에게 데이터
 * 전송이 가능함.
 * 
 * UDP 송신 프로그램 - 멀티캐스트를 통해 데이터를 송신
 */
namespace client
{
    class Program
    {
        static void Main(string[] args)
        {
            //UdpClient 객체 생성
            UdpClient sender = new UdpClient();
            //송신할 멀티캐스트의 IP/PORT 저장할 IPEndPoint 객체 생성
            //멀티캐스트 IP : 224.0.1.0  PORT : 13000
            IPEndPoint destination_ip = new IPEndPoint(IPAddress.Parse("224.0.1.0"), 13000);
            //문자열, byte[], BinaryFormatter, MemoryStream 변수 생성
            string data;
            byte[] send_data;
            BinaryFormatter formatter = new BinaryFormatter();
            MemoryStream stream = new MemoryStream();
            for (; ; )
            {
                //문자열 데이터를 MemoryStream 객체에 Serialize
                data = Console.ReadLine();
                formatter.Serialize(stream, data);
                //MemoryStream객체에 저장된 데이터를 byte[]로 변환
                send_data = stream.ToArray();
                //byte[]와 IPEndPoint 객체를 UdpClient객체로 송신
                sender.Send(send_data, send_data.Length, destination_ip);

                //버퍼 초기화
                stream.SetLength(0);

            }
            stream.Close();
            //UdpClient 객체 연결 종료
            sender.Close();
        }
    }
} 



