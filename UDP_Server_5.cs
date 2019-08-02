using data_struct;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

/*
 * UDP 수신 프로그램 - 멀티캐스트된 데이터를 수신
 */
namespace server
{
    class Program
    {
        static void Main(string[] args)
        {
            //UdpClient 객체 생성 - 13000 포트
            UdpClient rev = new UdpClient(13000);
            //IPAddess 객체 생성 - 가입할 멀티캐스트 주소를 저장할 객체 224.0.1.0
            IPAddress multicast_ip = IPAddress.Parse("224.0.1.0");
            //멀티캐스트 가입
            //JoinMulticastGroup(IPAddress 객체) : IPAddress객체가 저장한 IP주소(멀티캐스트주소)
            //로 해당 UDP소켓이 가입하는 기능이 있는 메소드
            rev.JoinMulticastGroup(multicast_ip);
            //IPEndPoint객체 생성 - 0,0 인자로 사용
            IPEndPoint ip = new IPEndPoint(0, 0);
            BinaryFormatter formatter = new BinaryFormatter();
            for (; ; ) {
                //데이터 수신 - byte[]
                byte[] recv_data = rev.Receive(ref ip);
                //byte[]을 MemoryStream 에 객체생성시 인자값으로 사용
                MemoryStream stream = new MemoryStream(recv_data);
                //MemoryStream에 커서위치를 데이터 맨앞으로 이동
                stream.Seek(0, SeekOrigin.Begin);
                //BinaryFormatter로 Deserialize 메소드 호출로 string 변환
                string str =(string) formatter.Deserialize(stream);
                //결과출력
                Console.WriteLine("{0}/{1} : {2}", ip.Address.ToString(),ip.Port,str);
                stream.Close();
            }
            //가입한 멀티캐스트 그룹을 탈퇴
            rev.DropMulticastGroup(multicast_ip);
            //UdpClient 객체 연결 종료
            rev.Close();
        }
    }
}






