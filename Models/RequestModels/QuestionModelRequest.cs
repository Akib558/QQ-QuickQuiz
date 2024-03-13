using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuickQuiz.Models.RequestModels
{
    public class GetQuestionsByRoom
    {
        public int RoomID { get; set; }
        public int UserID { get; set; }
    }

    public class DeleteQuestion
    {
        public int SetterID { get; set; }
        public int QuestionID { get; set; }
        public int RoomID { get; set; }
    }
    public class QuestionModel
    {
        public int QuestionID { get; set; }
        public string Question { get; set; }
        public List<string> Options { get; set; }
        public int Answer { get; set; }
        public int RoomID { get; set; }
    }


    public class OptionModel
    {
        public int OptionID { get; set; }
        public string Option { get; set; }
    }

    public class GetQuestionModel
    {
        public int QuestionID { get; set; }
        public string Question { get; set; }
        public List<OptionModel> Options { get; set; }
        public int Answer { get; set; }
        public int RoomID { get; set; }
    }

    public class UpdateQuestionModel
    {
        public int SetterID { get; set; }
        public int QuestionID { get; set; }
        public string Question { get; set; }
        public List<OptionModel> Options { get; set; }
        public int Answer { get; set; }
        public int RoomID { get; set; }
    }

    public class QuestionModelParticipant
    {
        public int QuestionID { get; set; }
        public string Question { get; set; }
        public List<string> Options { get; set; }
        // public int Answer { get; set; }
        public int RoomID { get; set; }
    }
}