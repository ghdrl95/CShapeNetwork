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
 * UDP : 비연결형 통신 방식. 서버와 클라이언트가 연결과정을 수행하지 않고 통신하는 방식.
 * 따라서 네트워크 상에서 데이터가 손실되더라도 재전송 요청을 할수없음.
 * UDP 서버 통신 기본 예제
 * 기능
 * UDP 클라이언트가 보낸 바이트배열을 문자열로 변환 및 출력
 */
namespace server
{
    class Program
    {
        static void Main(string[] args)
        {
            //UDP 서버 생성 - socket 생성 및 bind
            /* UDP 통신은 TCP통신과 달리 연결과정이 없다.(listen, accept)
             * 따라서 서버와 클라이언트의 개념이 약함.
             * c#에서는 UDP통신을 사용할때 UdpClient클래스만 활용함
             * UdpClient 객체를 생성할때 포트를 고정하면 bind 작업을 수행 - 서버
             * 포트를 고정하지않으면 운영체제가 비어있는 포트번호를 임시발급 - 클라이언트
             */
            UdpClient udp = new UdpClient(11000);
            //데이터를 수신받을 때, 어떤 프로그램이 데이터를 송신했는지 알기위해서
            //송신자의 IP와 PORT를 저장할 IPEndPoint객체를 수신할때마다 사용해야함
            IPEndPoint src_ip = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 0);
            //데이터 수신 - TCP와 다르게 송신자의정보와 수신자의 정보가 데이터에 동봉되있기
            //때문에 Stream객체를 사용할수없음.
            //송신자의 IP/PORT정보를 src_ip에 저장하고, 수신된 데이터는 recv_data변수에 저장
            byte[] recv_data = udp.Receive(ref src_ip);
            //바이트 데이터를 문자열로 변환 및 출력
            string result = Encoding.UTF8.GetString(recv_data);
            Console.WriteLine(result);
            //UDP 서버 닫기
            udp.Close();
        }
    }
}






