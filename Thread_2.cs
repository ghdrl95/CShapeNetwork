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
using System.Threading;
using System.Threading.Tasks;

/*
 * 서브스레드를 기반으로한 경주프로그램
 * 기능
 * 메인스레드는 서브스레드가 모두 종료할때까지 도달지점을 화면에출력
 * 서브스레드는 골인지점에 도착하면 등수를 저장하는 변수에 등번호 입력
 * 모든 서브스레드가 종료하면 등수를 출력
 */
namespace server
{
    //선수 정의
    class Player
    {
        //정적변수로 순위를 저장하는 배열과 인덱스저장 변수 선언
        public static int[] rank;
        public static int rank_idx;
        //현재 지점, 등번호, 종료여부 변수 선언
        public int distance, number;
        public bool isFinish;
        //서브스레드로 동작할 달리기 함수 정의
        public void run(object seed)
        {
            isFinish = false;
            //달리기
            //Random : 난수를 생성할 때 사용하는클래스
            //seed : 난수생성 규칙
            Random random = new Random((int)DateTime.Now.Ticks + ((int) seed));
            //Random.Next(최대값) : 0~최대값 사이의 임의의 숫자를 반환하는 함수
            for(distance = 0;distance < 1000 ; distance += random.Next(10))
            {
                Thread.Sleep(50);
            }
            isFinish = true;
            //도착 후 순위 등록
            //lock(변수) : 특정 변수를 접근하는 다양한 스레드들의 데이터
            //입력/변경을 순차적으로 진행하도록 처리하는 문법
            //ex) 1,5번 스레드가 거의 동시에 rank변수 접근하는 경우
            //가장먼저 lock문법에 도착한 스레드가 점유하는동안
            //다른 스레드가 대기상태로 정지함
            lock (rank)
            {
                rank[rank_idx] = number;
                rank_idx++;
            }
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            int num = 20;
            //랭크변수 초기화
            Player.rank = new int[num];
            Player.rank_idx = 0;

            Player[] players = new Player[num];
            //선수 수만큼 객체 생성
            for(int i=0;i<num;i++)
            {
                players[i] = new Player();
                players[i].number = i;
            }
            Task[] tasks = new Task[num];
            //서브스레드 생성 - Player객체의 run 메소드
            for(int i=0;i <num;i++)
            {
                tasks[i] = new Task(new Action<object>(players[i].run),i);
            }
            Console.WriteLine("시작하려면 엔터키를 누르세요");
            Console.ReadLine();
            //서브스레드 시작
            for (int i = 0; i < num; i++)
                tasks[i].Start();
            //반복문 - 모든선수가 도착할때까지 반복
            for (; ; )
            {
                //선수들이 경기를 마쳤는지 확인
                bool isAllFinish = true;
                for (int i = 0; i < num; i++)
                {
                    if((isAllFinish=players[i].isFinish) == false)
                    {
                        break;
                    }
                }
                if(isAllFinish)
                {
                    Console.WriteLine("경기종료");
                    break;
                }
                Console.Clear();
                //Player객체의 distance값을 화면에 출력
                for(int i=0;i<num;i++)
                {
                    Console.WriteLine("{0} : {1}",
                        players[i].number,players[i].distance);
                }
                Thread.Sleep(100);
            }
            //순위 출력
            Console.WriteLine("순위\t번호");
            Console.WriteLine("------------");
            for(int i=0;i<num;i++)
            {
                Console.WriteLine("{0}\t{1}", i + 1, Player.rank[i]);
            }
        }
    }
}






