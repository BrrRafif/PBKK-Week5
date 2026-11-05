using System;
using System.Collections.Generic;
using System.Text;

namespace StudentManager.Models
{
    public class Student
    {
        public int id { get; set; }
        public string NRP { get; set; } = "";
        public string Nama { get; set; } = "";
        public string Jurusan { get; set; } = "";
        public string Gender { get; set; } = "";
        public string Email { get; set; } = "";
    }



}
