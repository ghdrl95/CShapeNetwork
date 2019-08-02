using data_struct;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

/*
 * UDP 클라이언트 통신 기본 예제
 * 기능
 * UDP 서버에 문자열을 송신
 * ->MemoryStream, BinaryFormatter를 활용한 변수/객체 전달
 */
namespace client
{
    class Program
    {
        static void Main(string[] args)
        {
            //UdpClient 객체 생성 - 데이터를 먼저 보내는 송신자측은 포트번호 임시발급
            UdpClient client = new UdpClient(10000);
            //서버의 IP/PORT를 저장
            IPEndPoint des_ip = new IPEndPoint(IPAddress.Parse("114.70.60.88"), 11000);
            //데이터 송신
            string data = "안녕하세요!";
            //UDP 통신으로 이진데이터를 송신하려면 변수값을 MemoryStream에 저장
            //-> MemoryStream에 저장된 값을 byte[]로 변환한 뒤 Send메소드로 송신
            MemoryStream stream = new MemoryStream();
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(stream, data);//변수/객체 -> 메모리스트림에 저장
            //Stream객체에 저장된 데이터를 byte[]로 추출
            byte[] byte_data = stream.ToArray();
            //stream.Close();
            //데이터를 수신받는 프로그램이 없더라도 비연결지향 프로토콜인 UDP는
            //예외가 발생하지 않음

            client.Send(byte_data, byte_data.Length, des_ip);

            //기존의 변수를 활용해 데이터 수신 코드 구현

            //수신자측에서 보내는 데이터를 수신
            //수신된 데이터를 byte[] 배열에 저장
            byte_data = client.Receive(ref des_ip);
            //MemoryStream.SetLength(숫자) : 입력한 숫자만큼의 데이터를 저장할수있도록
            //저장공간을 수정하는 메소드. 0을 입력하면 저장된 모든 데이터를 지움
            stream.SetLength(0); //stream객체에 저장된 데이터를 지움
            stream.Write(byte_data, 0, byte_data.Length);
            //커서위치를 데이터 맨앞으로 이동
            stream.Seek(0, SeekOrigin.Begin);
            //이진데이터 -> 문자열 변환 및 출력
            data = (string)formatter.Deserialize(stream);
            Console.WriteLine(data);
            //UdpClient 객체 종료
            client.Close();
        }
    }
}//2, 
