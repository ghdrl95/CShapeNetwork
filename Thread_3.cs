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
 * naudio 라이브러리를 활용한 mp3플레이어 예제
 * 기능)
 * 특정한 mp3파일을 서브스레드로 재생
 * 사용자입력으로 음악재생이 되는 중에 종료
 */
namespace server
{
    
    class Program
    {
        //생성된 (또는 생성할) 서브스레드를 강제종료할 수있는 토큰을
        //생성할 수있는 클래스
        static CancellationTokenSource source;
        static void Main(string[] args)
        {
            source = new CancellationTokenSource();
            
            //재생할 파일명을 사용자입력으로 받기
            Console.Write("실행할 파일의 이름을 입력(확장자명포함): ");
            string filename = Console.ReadLine();
            //서브스레드를 생성하면서 파일명 전달
            //서브스레드 종료를 위한 토큰 전달
            //전달된 토큰이 이미 취소명령이 내려진상태면 서브스레드가
            //시작하지않음
            Task task = new Task(new Action<object>(play_music), 
                filename, source.Token);
            task.Start();
            Console.WriteLine("음악 재생 중. 종료하려면 엔터키입력");
            Console.ReadLine();//엔터키 입력 대기
            //source 객체의 토큰을 가진 모든 서브스레드 취소명령 호출
            source.Cancel(); 
            //무한반복
            /*
            for (; ; )
            {
                Console.WriteLine("메인 스레드 대기");
                Thread.Sleep(5000);
            }*/
            Console.WriteLine("음악 재생 종료");
        }
        //음악재생에 사용되는 서브스레드함수
        static void play_music(object filename)
        {
            //하드디스크에 저장된 음악파일을 로드하는 객체 생성
            AudioFileReader audioFile = new AudioFileReader((string)filename);
            //스피커에 음악파일을 출력할때 사용하는 객체 생성
            using (WaveOutEvent wave = new WaveOutEvent())
            {
                //재생할 음악파일 객체를 설정
                wave.Init(audioFile);
                wave.Play(); //설정된 음악파일 재생
                //음악파일을 모두 재생할때까지 대기
                while(source.IsCancellationRequested == false 
                    && wave.PlaybackState == PlaybackState.Playing)
                {
                    Thread.Sleep(500);
                }
            }
            audioFile.Close();
        }
    }
}






