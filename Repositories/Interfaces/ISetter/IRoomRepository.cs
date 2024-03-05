using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using QuickQuiz.Models.Room;

namespace QuickQuiz.Repositories.Interfaces.ISetter
{
    public interface IRoomRepository
    {
        Task<bool> createRoom(RoomModel roomModel);
        Task<List<int>> RoomParticipants(int roomID);
        Task<List<QuestionModel>> GetQuetions(int roomID);
        Task<List<ParticipantsModel>> AllParticipants();
        Task<int> AddQuestions(AddQuestion addQuestion);
        Task<List<GetParticipantsAnswerByIDModel>> GetParticipantsAnswerByRoom(int roomID);
        Task<int> AddParticipants(AddParticipants addParticipants);
        Task<List<RoomResultModel>> GetRoomResult(int roomID);
        Task<List<RoomModel>> RoomList(GetRoomListRequest getRoomListRequest);
        Task<int> StartQuiz(int roomID);
        Task<int> StopQuiz(int roomID);
        Task<int> PauseQuiz(int roomID);

    }
}