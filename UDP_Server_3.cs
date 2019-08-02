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
 * UDP 클라이언트의 IP/PORT로 데이터 송신
 */
namespace server
{
    class Program
    {
        static void Main(string[] args)
        {
            //UDP클라이언트의 IP/PORT를 얻기위한 소켓
            UdpClient udp = new UdpClient(11000);
            //UDP클라이언트에게 데이털 송신할 소켓
            UdpClient sender = new UdpClient();
            IPEndPoint src_ip = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 0);

            for(; ; )
            {
                byte[] recv_data = udp.Receive(ref src_ip);
                Console.WriteLine("{0} byte 수신 됨", recv_data.Length);
                Console.WriteLine("송신자의 IP : {0} Port : {1}", src_ip.Address.ToString(),
                    src_ip.Port);
                //DateTime : c#에서 시간데이터를 처리할때 사용하는 클래스
                //DateTime.Now 정적변수 : 컴퓨터의 현재시간을 저장하는 DateTime 객체
                DateTime datetime = DateTime.Now;
                //객체나 변수값을 전송하기 위해 MemoryStream, BinaryFormatter 사용
                BinaryFormatter formatter = new BinaryFormatter();
                MemoryStream stream = new MemoryStream();
                //메모리공간에 객체 데이터를 누적
                formatter.Serialize(stream, datetime);
                //메모리공간에 데이터를 byte[]로 추출
                byte[] send_data = stream.ToArray();
                //UDP통신으로 송신 - UDP클라이언트의 IP/PORT로 전송
                sender.Send(send_data, send_data.Length, src_ip);
            }
            sender.Close();
            udp.Close();
        }
    }
}






