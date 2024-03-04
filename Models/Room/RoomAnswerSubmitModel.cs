using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuickQuiz.Models.Room
{
    public class RoomAnswerSubmitModel
    {
        public int UserID { get; set; }
        public int RoomID { get; set; }
        public List<List<int>> Answers { get; set; }
    }
}