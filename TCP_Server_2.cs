using data_struct;
using System;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

/*
 * TCP 서버 기본 예제 - 자료형,클래스 변수값을 송수신
 * TCP : 연결지향형 네트워크통신. TCP서버를 구현하려면
 * 클라이언트가 접속할 IP/PORT로 서버소켓을 생성하고 클라이언트 접속을 대기
 * 연결된 클라이언트와 Stream객체로 통신
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
            //클라이언트의 접속을 처리할 TcpListener 객체 생성
            TcpListener listener = new TcpListener(8000);//socket생성 및 bind
            //클라이언트의 접속요청 허용 
            listener.Start(); //listen()
            //BinaryFormatter : 변수값을 이진데이터로 변환하거나 (Serialize)
            //이진데이터를 변수값으로 변화하는 기능(Deserialize)이 있는 클래스
            BinaryFormatter formatter = new BinaryFormatter();
            //새로 정의한 클래스의 객체를 네트워크상에서 송수신할때,
            //다른 개발자가 만든 프로그램이 준 객체정보를 차단하는 기본설정이 되있음
            //어셈블리 정보 : 프로그램의 정보가 프로젝트단위로 들어있음
            //어셈블리 정보가 다르더라도 이진데이터->객체 변환할수있도록 허용하는 코드 
            formatter.Binder = new AllowAllAssemblyVersionsDeserializationBinder();
            while (true)//여러개의 클라이언트 접속을 처리하기 위한 반복문
            {
                //클라이언트 접속 대기
                TcpClient client = listener.AcceptTcpClient(); //accept()
                NetworkStream stream = client.GetStream();

                for (; ; )//한개의 클라이언트와 지속적인 데이터교환을위한 무한반복
                {
                    //클라이언트가 보낸 이진데이터를 자료형에 맞게 변환
                    try
                    {
                        //기본자료형
                        //string result = (string)formatter.Deserialize(stream);
                        //Console.WriteLine("수신된 문자열 : " + result);
                        //Console.WriteLine("수신된 데이터 크기 : {0}", result.Length);
                        //정의된 클래스 객체
                        Student student = (Student)formatter.Deserialize(stream);
                        Console.WriteLine("수신된 문자열 : " + student.name);
                        Console.WriteLine("수신된 문자열 : " + student.phone);
                    }
                    catch //클라이언트 데이터 수신 및 변환과정중 클라이언트가 연결을 끊은경우
                    {
                        Console.WriteLine("클라이언트가 연결을 끊음");
                        break;
                    }
                    
                }//for문 끝
                //연결된 클라이언트와 연결 끊기
                stream.Close();
                client.Close();
            }//while문 끝 
            listener.Stop(); //클라이언트 접속 허용을 해지
        }
    }
}
