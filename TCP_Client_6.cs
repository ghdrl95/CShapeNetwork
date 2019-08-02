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
using System.Threading.Tasks;

/*
 * TCP 기반의 채팅 송수신 클라이언트 프로그램
 * 기능)
 * 메인스레드
 * 서버에 접속 및 사용자 입력을 서버에 전달
 * 서브스레드
 * 서버가 송신한 데이터를 화면에 출력
 */
namespace client
{
    class User
    {
        //접속한 클라이언트의 소켓, 스트림
        public TcpClient socket;
        public NetworkStream stream;
        //BinaryFormatter 객체
        public BinaryFormatter formatter;
        //ID값 저장 변수
        public string id;

        public User(TcpClient client)
        {
            socket = client;
            stream = socket.GetStream();
            formatter = new BinaryFormatter();
        }


        //서브스레드 - 서버가 송신한 데이터를 수신받아 화면에 출력
        public void RecvData()
        {
            for (; ; )
            {
                try
                {
                    string chat = (string)formatter.Deserialize(stream);

                    Console.WriteLine(chat);
                }
                catch //데이터 수신 대기중 서버가 연결을끊었을때의 예외처리
                {
                    Console.WriteLine("수신대기중 연결된 서버가 연결을 끊음");
                    break;
                }
            }
            Close();
        }

        //해당 서버에게 데이터를 송신하는 메소드
        public bool SendData(string chat)
        {
            bool isConnected = false;
            //조건문 - 서버의 다른스레드에서 이미 연결을 끊었는지 확인
            if (socket.Connected)
            {
                try
                {
                    //연결된 스트림으로 문자열을 전송
                    formatter.Serialize(stream, chat);
                    //Serialize에서 예외가 발생하지 않으면 연결이 유지되었음을 확인함
                    isConnected = true;
                }
                catch // 클라이언트가 연결을 끊은경우의 예외처리
                {
                    Console.WriteLine("서버가 연결을 끊음");
                    Close();
                }
            }
            return isConnected;
        }
        //클라이언트 연결종료 메소드
        public void Close()
        {
            socket.Close();
            stream.Close();
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            User user;
            while (true) //서버에 연결될때까지 연결시도
            {
                try
                {
                    TcpClient client = new TcpClient("127.0.0.1", 15000);
                    user = new User(client);
                    break;
                }
                catch //서버가 닫혀있는 경우의 예외처리
                {
                    Console.WriteLine("서버가 응답하지 않음...");
                    Thread.Sleep(5000);
                }
            }
            //114.70.60.88
            //ID 입력 및 송신
            Console.Write("ID 입력 : ");
            user.id = Console.ReadLine();
            user.SendData(user.id);
            Task recv_task = new Task(new Action(user.RecvData));
            recv_task.Start();
            for(; ; )
            {
                string chat = Console.ReadLine();
                bool isConnected = user.SendData(chat);
                if( !isConnected)
                {
                    Console.WriteLine("서버연결 끊김");
                    break;
                }
            }
            user.Close();
        }
    }
} 















