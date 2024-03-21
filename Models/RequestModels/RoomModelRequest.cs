using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using QuickQuiz.Models.Room;

namespace QuickQuiz.Models.RequestModels
{
    public class UpdateRoom
    {
        public int roomID { get; set; }
    }
    public class GetParticipantsByRoom
    {
        public int RoomID { get; set; }
        public int UserID { get; set; }
        public int pageSize { get; set; }
        public int pageNumber { get; set; }
    }
    public class DeleteRoomByRoom
    {
        public int UserID { get; set; }
        public int RoomID { get; set; }
    }
    public class GetPariticipantsAnswer
    {
        public int RoomID { get; set; }
        public int UserID { get; set; }
    }
    public class GetParticipantsResult
    {
        public int RoomID { get; set; }
    }
    public class GetParticipantsAnswerByID
    {
        public int RoomID { get; set; }
        public int UserID { get; set; }
        public int ParticipantID { get; set; }
    }

    public class GetParticipantsInfoByID
    {
        public int RoomID { get; set; }
        public int UserID { get; set; }
        public int ParticipantID { get; set; }
    }

    public class DeleteParticipantsByID
    {
        public int RoomID { get; set; }
        public int UserID { get; set; }
        public int ParticipantID { get; set; }
    }

    // public class AddParticipants
    // {
    //     public int RoomID { get; set; }
    //     public int UserID { get; set; }
    //     public int ParticipantID { get; set; }
    // }
    public class GetRoomResult
    {
        public int RoomID { get; set; }
        public int UserID { get; set; }
    }
    public class RoomStatus
    {
        public int RoomID { get; set; }
        public int UserID
        {
            get; set;
        }
    }

    public class AddParticipants
    {
        public int UserID { get; set; }
        public int RoomID { get; set; }
        public List<int> Participants { get; set; }
    }
    public class GetRoomListRequest
    {
        // public int RoomID { get; set; }
        public int UserID { get; set; }
    }

    public class AddQuestion
    {
        public int UserID { get; set; }
        public int RoomID { get; set; }
        public List<QuestionModel> Questions { get; set; }
    }

    public class RoomModel
    {
        public int RoomID { get; set; }
        public string RoomName { get; set; }
        public int UserID { get; set; }
        public List<int> Participants { get; set; }
        // public string StartDateTime { get; set; }
    }
    public class RoomUpdateModel
    {
        public int RoomID { get; set; }
        public string RoomName { get; set; }
        public int UserID { get; set; }
        public string StartTime { get; set; }
        public int RoomTypeID { get; set; }
        public int RoomStatus { get; set; }
    }

}