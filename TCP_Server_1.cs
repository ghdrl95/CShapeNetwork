using System;
using System.Net.Sockets;
using System.Text;

/*
 * TCP 서버 기본 예제
 * TCP : 연결지향형 네트워크통신. TCP서버를 구현하려면
 * 클라이언트가 접속할 IP/PORT로 서버소켓을 생성하고 클라이언트 접속을 대기
 * 연결된 클라이언트와 Stream객체로 통신
 */
namespace server
{
    class Program
    {
        static void Main(string[] args)
        {
            //클라이언트의 접속을 처리할 TcpListener 객체 생성
            TcpListener listener = new TcpListener(8000);//socket생성 및 bind
            //클라이언트의 접속요청 허용 
            listener.Start(); //listen()
            while (true)//여러개의 클라이언트 접속을 처리하기 위한 반복문
            {
                //클라이언트 접속 대기
                TcpClient client = listener.AcceptTcpClient(); //accept()
                NetworkStream stream = client.GetStream();

                for (; ; )//한개의 클라이언트와 지속적인 데이터교환을위한 무한반복
                {
                    //클라이언트가 보낸 데이터 수신 - byte배열
                    byte[] data = new byte[1024];
                    //파일입출력에서 사용한 Stream과 동일한 메서드를 사용해 
                    //데이터 송수신 구현
                    int size = stream.Read(data, 0, data.Length);
                    //클라이언트가 먼저 연결을 끊은경우, Read메서드 호출시
                    //수신된 데이터의 사이즈가 0으로 반환됨
                    if(size <= 0)
                    {
                        Console.WriteLine("클라이언트 연결 끊음");
                        break;
                    }
                    //콘솔창에 결과 출력
                    //byte배열에 저장된 값을 문자열로 변환
                    //Encoding : 문자열을 메모리공간에 저장할때 여러 방법들을 제공하는 클래스
                    //Encoding.Default : .Net에서 사용하는 기본 인코딩 기법
                    //Encoding.Default.GetString : byte배열을 문자열로 바꿔주는 정적함수
                    // <-> GetBytes : 문자열을 byte배열로 바꿔주는 정적함수
                    string result = Encoding.Default.GetString(data);
                    Console.WriteLine("수신된 문자열 : " + result);
                    Console.WriteLine("수신된 데이터 크기 : {0}", size);
                }//for문 끝
                //연결된 클라이언트와 연결 끊기
                stream.Close();
                client.Close();
            }//while문 끝 
            listener.Stop(); //클라이언트 접속 허용을 해지
        }
    }
}
