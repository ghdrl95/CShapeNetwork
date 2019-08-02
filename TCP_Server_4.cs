using data_struct;
using System;
using System.IO;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

/*
 * TCP 서버 - 파일 데이터 수신 및 저장
 */
namespace server
{
    sealed class AllowAllAssemblyVersionsDeserializationBinder : SerializationBinder
    {
        public override Type BindToType(string assemblyName, string typeName)
        {
            Type typeToDeserialize = null;

            String currentAssembly = Assembly.GetExecutingAssembly().FullName;

            // In this case we are always using the current assembly
            assemblyName = currentAssembly;

            // Get the type using the typeName and assemblyName
            typeToDeserialize = Type.GetType(String.Format("{0}, {1}",
                typeName, assemblyName));

            return typeToDeserialize;
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            //서버 생성 - 10000번 포트 사용
            TcpListener listener = new TcpListener(10000);
            //클라이언트 연결 허용
            listener.Start();
            //클라이언트 연결
            TcpClient client = listener.AcceptTcpClient();
            NetworkStream stream = client.GetStream();
            //파일 데이터를 저장할 파일스트림 객체 생성
            FileStream file_stream = File.Create("./download.txt");
            //수신받은 데이터를 파일스트림에 저장
            byte[] recv_data = new byte[1024];
            int recv_size = 0;
            //클라이언트가 연결을 끊을때까지 수신할수있도록 반복
            while( ( recv_size = stream.Read(recv_data,0,recv_data.Length))> 0)
            {
                file_stream.Write(recv_data, 0, recv_size);
            }
            //파일 닫기
            file_stream.Close();
            //클라이언트 연결 종료
            stream.Close();
            client.Close();
            //서버 종료
            listener.Stop();
        }
    }
}






