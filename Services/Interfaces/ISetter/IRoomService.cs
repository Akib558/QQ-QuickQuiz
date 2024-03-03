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
        public Task<bool> RoomCreation(RoomModel roomModel);
        public bool RoomDeletion();
        public bool RoomUpdate();

        public Task<List<QuestionModel>> GetQuestions(int roomID);
        public Task<List<int>> GetParticipants(int roomID);
        public IActionResult RoomData(int setterID);
    }
}