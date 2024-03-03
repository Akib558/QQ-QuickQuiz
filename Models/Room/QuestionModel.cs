using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuickQuiz.Models.Room
{
    public class QuestionModel
    {
        public int QuestionID { get; set; }
        public string Question { get; set; }
        public List<string> Options { get; set; }
        public int Answer { get; set; }
        public int RoomID { get; set; }
    }
}