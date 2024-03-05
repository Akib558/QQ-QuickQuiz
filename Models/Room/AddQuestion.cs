using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuickQuiz.Models.Room
{
    public class AddQuestion
    {
        public int SetterID { get; set; }
        public int RoomID { get; set; }
        public List<QuestionModel> Questions { get; set; }
    }
}