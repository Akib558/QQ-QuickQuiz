using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using QuickQuiz.Models.Room;

namespace QuickQuiz.Services.Interfaces.ISetter
{
    public interface IRoomService
    {
        public Task<List<ParticipantsModel>> AllParticipants();
        public Task<bool> RoomCreation(RoomModel roomModel);
        public bool RoomDeletion();
        public bool RoomUpdate();

        public Task<int> AddParticipants(AddParticipants addParticipants);
        public Task<int> AddQuestions(AddQuestion addQuestion);
        public Task<List<QuestionModel>> GetQuestions(int roomID);
        public Task<List<int>> GetParticipants(int roomID);
        public Task<List<GetParticipantsAnswerByIDModel>> GetParticipantsAnswerByRoom(int roomID);
        public Task<List<RoomResultModel>> GetRoomResult(int roomID);
        public IActionResult RoomData(int setterID);
        public Task<List<RoomModel>> RoomList(GetRoomListRequest getRoomListRequest);
        public Task<int> StartQuiz(int roomID);
        public Task<int> StopQuiz(int roomID);
        public Task<int> PauseQuiz(int roomID);

    }
}