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

        Task<List<GetParticipantsAnswerByIDModel>> GetParticipantsAnswerByRoom(int roomID);

        Task<List<RoomResultModel>> GetRoomResult(int roomID);
    }
}