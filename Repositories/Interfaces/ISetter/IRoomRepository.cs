using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using QuickQuiz.Models;
using QuickQuiz.Models.RequestModels;
using QuickQuiz.Models.Room;

namespace QuickQuiz.Repositories.Interfaces.ISetter
{
    public interface IRoomRepository
    {
        Task<bool> createRoom(RoomModel roomModel);
        Task<List<int>> RoomParticipants(int roomID);
        Task<List<GetQuestionModel>> GetQuetions(int roomID);
        Task<List<ParticipantsModel>> AllParticipants();
        Task<int> AddQuestions(AddQuestion addQuestion);
        Task<List<GetParticipantsAnswerByIDModel>> GetParticipantsAnswerByRoom(int roomID);
        Task<GetParticipantsAnswerByIDModel> GetParticipantsAnswerByID(int participantID, int roomID);
        Task<Participant> GetParticipantInfoByID(int roomID, int participantID);
        Task<bool> RoomParticipantDelete(int roomID, int ParticiapntsID);
        Task<int> AddParticipants(AddParticipants addParticipants);
        Task<List<RoomResultModel>> GetRoomResult(int roomID);
        Task<List<RoomModel>> RoomList(GetRoomListRequest getRoomListRequest);
        Task<bool> StartQuiz(int roomID);
        Task<bool> StopQuiz(int roomID);
        Task<bool> PauseQuiz(int roomID);
        Task<bool> RoomDelete(int roomID);
        Task<bool> RoomUpdate(RoomUpdateModel roomModel);
        Task<bool> QuestionDelete(int questionID);
        Task<bool> QuestionUpdate(UpdateQuestionModel questionModel);
        Task<bool> isRoomAuthorized(int roomID, int userID);
        Task<bool> isSetter(int userID);
        Task<bool> isRoomSetter(int roomID, int setterID);
        // Task<bool> roomStatus(int roomID);
        Task<bool> isRoomActive(int roomID);
        Task<bool> isMember(int roomID, int userID);
    }
}