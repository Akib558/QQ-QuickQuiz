using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using QuickQuiz.Models.Room;

namespace QuickQuiz.Models
{
    public class Participant
    {
        public int UserID { get; set; }
        public int RoomID { get; set; }
        public int Marks { get; set; }
        // public int Performed { get; set; }
        public List<QuestionAnswer> Questions { get; set; }
        // public int RoomRank { get; set; }

    }
    public class ParticipantQuestionRequest
    {
        public int UserID { get; set; }
        public int RoomID { get; set; }
    }
}