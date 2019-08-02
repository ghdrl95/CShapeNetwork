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
 * UDP 수신 프로그램 - 브로드캐스트된 데이터를 수신
 */
namespace server
{
    class Program
    {
        static void Main(string[] args)
        {
            UdpClient recever = new UdpClient(12000);

            IPEndPoint src_ip = new IPEndPoint(0, 0);

            byte[] recv_data;
            BinaryFormatter formatter = new BinaryFormatter();
            MemoryStream stream = new MemoryStream();
            string result;
            for(; ; )
            {
                //UDP 소켓으로 데이터 수신 
                //- 소켓으로 직접적으로 오는데이터 or 브로드캐스트된 데이터가 수신됨
                recv_data = recever.Receive(ref src_ip);
                //메모리스트림에 수신받은 데이터 삽입
                stream.Write(recv_data, 0, recv_data.Length);
                //커서위치를 데이터 처음으로 이동
                stream.Seek(0, SeekOrigin.Begin);
                //메모리스트림의 데이터 전체를 문자열로 변환
                result = (string)formatter.Deserialize(stream);
                //결과 출력
                Console.WriteLine("수신된 데이터 : {0}", result);
                //메모리 스트림의 데이터 지우기
                stream.SetLength(0);
            }
            stream.Close();
            recever.Close();
        }
    }
}






