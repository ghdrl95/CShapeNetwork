using System;
using System.Collections.Generic;
using System.Text;

namespace data_struct
{
    [Serializable]
    class Student
    {
        public string name;
        public string phone;
    }

    //파일정보를 저장하는 클래스 정의
    [Serializable]
    class File_Info
    {
        public string filename;
        public long filesize;
    }
}
