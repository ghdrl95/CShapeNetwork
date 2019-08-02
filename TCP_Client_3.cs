using data_struct;
using System;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

/*
 * TCP 클라이언트 - 가위바위보 게임
 * 기능
 * 서버에 접속
 * 게임시작 값이 오기전까지 대기
 * 가위바위보 값을 사용자 입력으로 받기
 * 사용자 입력을 서버 송신
 * 결과 수신
 * 다시하기 여부를 사용자입력으로 받기
 * 
 */
namespace client
{
    class Program
    {
        static void Main(string[] args)
        {
            //게임에 사용되는 송수신값을 상수로 정의
            //const : 자료형/클래스명 앞에 붙일 수 있음. const로 변수를 만든경우
            //변수에 값을 대입하지 못함. 클래스의 멤버변수의 get만 지정한것과 동일
            const int GAME_START = 1,
                DISCONNET = -1,
                SCISSORS = 1,
                ROCK = 2,
                PAPER = 3,
                WIN = 1,
                LOSE = 2,
                DRAW = 3;
            //접속할 서버의 IP,PORT를 상수로 선언
            const string SERVER_IP = "127.0.0.1";
            const int SERVER_PORT = 9000;

            //socket 객체 생성
            TcpClient client = null;
            //반복문에서 사용되는 변수들 정의
            NetworkStream stream = null;
            int data = 0;
            BinaryFormatter formatter = new BinaryFormatter();
            //반복문 - 다시하기 입력이 들어온경우 반복 
            while (true)
            {
                client = new TcpClient();
                //서버에 접속
                client.Connect(SERVER_IP, SERVER_PORT);
                stream = client.GetStream();
                //게임시작 신호올때까지 대기
                Console.WriteLine("다른 플레이어 접속을 대기중...");
                data = (int)formatter.Deserialize(stream);
                //가위바위보값을 입력받고 서버에 송신
                do {
                    try
                    {
                        Console.Write("1.가위 2.바위 3.보 중 하나 입력 : ");
                        data = int.Parse(Console.ReadLine());
                    }
                    catch//사용자가 숫자로 변환할수없는 문자열을 입력했을때 발생하는 예외를 처리
                    {
                        Console.WriteLine("숫자값을 입력하세요");
                        continue;
                    }
                } while ( !(1 <= data && data <= 3) );//사용자가 1,2,3 중 하나를 입력했는지 확인
                formatter.Serialize(stream, data);
                //결과 수신 및 화면에 출력
                data = (int)formatter.Deserialize(stream);
                string result = "";
                if(data == WIN)
                {
                    result = "이겼습니다!";
                }else if(data == LOSE)
                {
                    result = "졌습니다!";
                }
                else if(data == DRAW)
                {
                    result = "비겼습니다!";
                }
                else if (data == DISCONNET)
                {
                    result = "상대방이 연결을 끊었습니다!";
                }
                Console.WriteLine(result);
                //다시하기 입력 받기
                Console.Write("다시하겠습니까? (y : 다시하기, 그외 : 그만하기) : ");
                result = Console.ReadLine();
                if(result.Equals("y") || result.Equals("Y"))
                {
                    Console.Clear();
                }
                else
                {
                    break;
                }
                stream.Close();
                client.Close();
            }
            Console.WriteLine("프로그램 종료");
        }
    }
}
