using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuickQuiz.Models.Room
{
    public class QuestionAnswer
    {
        public int QuestionID { get; set; }
        public string Question { get; set; }
        public List<string> OptionsList { get; set; }
        public int CorrectOption { get; set; }
        public int UserAnswer { get; set; }


    }
    public class GetParticipantsAnswerByIDModel
    {
        public int UseID { get; set; }
        public int RoomID { get; set; }
        public List<QuestionAnswer> Questions { get; set; }

    }
}