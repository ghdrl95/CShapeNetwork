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
 * 멀티캐스트 + 멀티쓰레드 기반의 채팅프로그램 구현
 * 기능)
 * 메인스레드
 * Udp 소켓 생성 및 특정 멀티캐스트 그룹 가입.
 * 가입된 소켓을 서브스레드에게 넘겨줌.
 * 사용자입력을 받아 멀티캐스트그룹에 송신
 * 서브스레드
 * 메인스레드가 준 Udp소켓을 바탕으로 수신되는 데이터를 출력
 */
namespace client
{
    class Program
    {

        static CancellationTokenSource source = new CancellationTokenSource();
        static void Main(string[] args)
        {
            Console.Write("ID 입력 : ");
            string ID = Console.ReadLine();
            //Udp소켓 생성 - 14000번 포트
            UdpClient socket = new UdpClient(14000);
            //멀티캐스트 그룹 가입 - 225.0.7.19
            IPAddress multicast_ip = IPAddress.Parse("225.0.7.19");
            socket.JoinMulticastGroup(multicast_ip, 20);
            //서브스레드 생성 - 매개변수로 Udp소켓 전달
            Task recv_task = new Task(new Action<object>(recv_thread), socket, source.Token);
            recv_task.Start();

            IPEndPoint group_ip = new IPEndPoint(IPAddress.Parse("225.0.7.19"), 14000);

            BinaryFormatter formatter = new BinaryFormatter();
            string data;
            byte[] send_data;
            //무한반복
            for (; ; )
            {
                data = Console.ReadLine();
                if(data.Equals("종료"))
                {
                    break;
                }
                //사용자입력
                data = string.Format("{0} : {1}", ID, data);
                //byte[]로 변환 및 멀티캐스트 그룹에 송신
                MemoryStream ms = new MemoryStream();
                formatter.Serialize(ms, data);
                send_data = ms.ToArray();
                socket.Send(send_data, send_data.Length, group_ip);
                ms.Close();
            }
            source.Cancel();//토큰을 받은 서브스레드를 종료하는 명령
            //Udp소켓 종료 
            socket.Close();
            recv_task.Wait();//서브스레드가 종료할때까지 대기
        }
        //데이터 수신을 받기 위한 서브스레드 생성용 함수 정의
        //socket : UdpClient 객체
        static void recv_thread(object socket)
        {
            //서브스레드를 생성한 곳에서 매개변수를 안줬을때에 대한 처리
            if( socket == null)
            {
                Console.WriteLine("소켓을 받지 못함");
                return;
            }
            //매개변수를 UdpClient객체로 변환
            UdpClient recv_socket = (UdpClient)socket;
            //byte[], string, BinaryFormatter, MemoryStream 선언
            byte[] recv_data = null;
            string result;
            BinaryFormatter formatter = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();
            IPEndPoint src_ip = new IPEndPoint(0, 0);
            //무한반복 - 쓰레드 종료 메시지가 들어오면 종료
            for (; !source.IsCancellationRequested ; )
            {
                //데이터 수신
                try
                {
                    recv_data = recv_socket.Receive(ref src_ip);
                }
                catch//UdpClient 객체가 Close되어 수신받지못하는 예외처리
                {
                    Console.WriteLine("메인스레드에서 소켓을 닫음");
                    break;
                }
                //조건문 - 수신된 바이트 배열 크기가 0초과인 경우 변환작업 수행
                if (recv_data.Length > 0)
                {
                    //byte[] -> string 변환 및 화면출력
                    ms.Write(recv_data, 0, recv_data.Length);
                    ms.Seek(0, SeekOrigin.Begin);
                    result = (string)formatter.Deserialize(ms);
                    Console.WriteLine(result);
                    ms.SetLength(0);
                }
                else
                {
                    Console.WriteLine("수신받은 데이터 크기가 0");
                }
            }
        }
    }
} 















