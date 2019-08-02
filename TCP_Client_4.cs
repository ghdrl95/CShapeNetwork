using data_struct;
using System;
using System.IO;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

/*
 * TCP 클라이언트 - 파일데이터를 서버에 송신
 */
namespace client
{
    class Program
    {
        static void Main(string[] args)
        {
            //서버연결
            TcpClient client = new TcpClient("127.0.0.1", 10000);
            //NetworkStream stream = client.GetStream();
            using (var stream = client.GetStream())
            {
                //업로드할 파일을 열기
                FileStream file_stream = File.OpenRead("./upload.txt");
                //파일을 읽은 만큼 서버에 송신
                byte[] data = new byte[1024];
                int data_size;
                //파일을 끝까지 읽기위한 반복문
                while((data_size = file_stream.Read(data,0,data.Length))>0)
                {
                    //읽은 데이터만큼 서버에 송신
                    stream.Write(data, 0, data_size);
                }
                //파일 닫기
                file_stream.Close();
            }
            //서버연결끊기
            //stream.Close();
            client.Close();
        }
    }
}
