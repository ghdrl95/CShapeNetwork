using data_struct;
using NAudio.Wave;
using System;
using System.Collections.Generic;
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
 * TCP통신 기반의 1Room 멀티채팅 서버 프로그램
 * 기능
 * 메인스레드
 * 서버 설정, 클라이언트 접속 대기, 접속한 클라이언트를 매개변수로 서브스레드 생성
 * 서브스레드1
 * 클라이언트가 송신한 채팅내용을 다른클라이언트들에게 송신
 * 서브스레드2...n
 * 클라이언트가 송신 데이터를 수신 및 채팅내용 저장변수에 전달
 */
namespace server
{
    //접속한 클라이언트를 관리하는 클래스
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


        //서브스레드 - 클라이언트가 송신한 데이터를 수신받아 메시지저장변수에 누적
        public void RecvData()
        {
            //ID값 수신
            try
            {
                id = (string)formatter.Deserialize(stream);
            }
            catch //ID값 수신중 클라이언트가 연결 종료
            {
                Console.WriteLine("ID값 수신대기중 연결 종료 확인");
                Close();
                return;
            }
            for(; ; )
            {
                try
                {
                    string chat = (string)formatter.Deserialize(stream);
                    //chat 변수와 ID값을 합침
                    chat = string.Format("{0} : {1}", id, chat);
                    //문자열을 메세지저장변수에 전달
                    //여러 서브스레드가 요소를 동시에 추가하는 것을 막기위해 lock 사용
                    lock (Manager.messages)
                    {
                        Manager.messages.Add(chat);
                    }
                }
                catch //데이터 수신 대기중 클라이언트가 연결을끊었을때의 예외처리
                {
                    Console.WriteLine("수신대기중 연결된 클라이언트가 연결을 끊음");
                    break;
                }
            }
            Close();
        }

        //해당 클라이언트에게 데이터를 송신하는 메소드
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
                    Console.WriteLine("{0} 유저가 연결을 끊음", id);
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
    //서버를 관리하는 클래스
    class Manager
    {
        //상수변수로 욕 등록
        public const string FILTER = "바보";
        //서버소켓
        public TcpListener listener;
        //List<User> 
        public List<User> users;
        //메세지 저장용 변수 List<string>
        public static List<string> messages;
        //서브스레드 관리용 List<Task>
        public List<Task> user_tasks;

        //메인스레드용
        //서버 생성
        public void init_server()
        {
            listener = new TcpListener(15000);
            users = new List<User>();
            messages = new List<string>();
            user_tasks = new List<Task>();
        }
        //클라이언트 접속 허용 및 접속 대기
        public void connect_client()
        {
            listener.Start();
            //서브스레드1번 객체 생성 및 실행
            Task send_task = new Task(new Action(send_users));
            send_task.Start();
            for(; ; )
            {
                TcpClient client = listener.AcceptTcpClient();
                User new_user = new User(client);
                Task new_task = new Task(new Action(new_user.RecvData));
                users.Add(new_user);
                user_tasks.Add(new_task);
                new_task.Start();
            }

        }

        //서버 닫기
        public void close_server()
        {
            listener.Stop();
            //서브스레드로 동작하는 클라이언트 task 멈추는 코드

        }
        //서브스레드1용
        //메세지 저장 변수에서 문자열 추출 및 해당 문자열을 연결된 클라이언트에게 송신
        public void send_users()
        {
            string chat;
            for(; ;)
            {
                //조건문 - messages 에 보낼 채팅이 저장되있는지 확인
                if(messages.Count > 0)
                {
                    lock (messages)//messages 변수 접근을 하나의 스레드만 할수있도록 설정
                    {
                        chat = messages[0];
                        messages.RemoveAt(0);
                    }
                    //조건식 - 채팅에 욕이있는지 확인
                    if (chat.Contains(FILTER))
                    {
                        //*로 대체
                        chat = chat.Replace(FILTER, "**");
                    }
                    //연결된 모든 클라이언트에게 메세지 전송
                    for(int i=0;i<users.Count;i++)
                    {
                        bool isConnected = users[i].SendData(chat);
                        //연결이 끊긴 클라이언트를 리스트객체에서 제거
                        if( !isConnected)
                        {
                            Console.WriteLine("{0}번째 클라이언트 리스트 제거", i);
                            user_tasks.RemoveAt(i);
                            users.RemoveAt(i);
                            //지워진 요소 만큼 i값을 뺌. 삭제된 인덱스다음 요소가 데이터를 전송받도록 처리
                            i--; 

                        }
                    }
                }
            }
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            Manager manager = new Manager();
            manager.init_server();
            manager.connect_client();
            //connect_client 메소드에 무한반복이 있기때문에 서버 닫는 메소드가 호출되지않음
            manager.close_server();
        }
    }
}






