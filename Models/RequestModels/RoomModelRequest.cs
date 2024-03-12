using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuickQuiz.Models.RequestModels
{
    public class UpdateRoom
    {
        public int roomID { get; set; }
    }
    public class GetParticipantsByRoom
    {
        public int RoomID { get; set; }
    }
    public class DeleteRoomByRoom
    {
        public int RoomID { get; set; }
    }
    public class GetPariticipantsAnswer
    {
        public int RoomID { get; set; }
    }
    public class GetParticipantsResult
    {
        public int RoomID { get; set; }
    }
    public class GetParticipantsAnswerByID
    {
        public int RoomID { get; set; }
        public int ParticipantID { get; set; }
    }

    public class GetParticipantsInfoByID
    {
        public int RoomID { get; set; }
        public int ParticipantID { get; set; }
    }

    public class DeleteParticipantsByID
    {
        public int RoomID { get; set; }
        public int ParticipantID { get; set; }
    }


    public class RoomStatus
    {
        public int RoomID { get; set; }
        public int SetterID
        {
            get; set;
        }
    }
}