using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuickQuiz.Models
{
    public class Participant
    {

    }
    public class ParticipantQuestionRequest
    {
        public int UserID { get; set; }
        public int RoomID { get; set; }
    }
}