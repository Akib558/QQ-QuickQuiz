using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuickQuiz.Models.RequestModels
{
    public class GetQuestionsByRoom
    {
        public int RoomID { get; set; }
    }

    public class DeleteQuestion
    {
        public int SetterID { get; set; }
        public int QuestionID { get; set; }
        public int RoomID { get; set; }
    }
}