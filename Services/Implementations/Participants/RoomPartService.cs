using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using QuickQuiz.Models.Room;
using QuickQuiz.Repositories.Interfaces.IParticipants;
using QuickQuiz.Services.Interfaces.IParticipants;

namespace QuickQuiz.Services.Implementations.Participants
{
    public class RoomPartService : IRoomPartService
    {
        private IRoomPartRepository _roomPartRepository;
        public RoomPartService(IRoomPartRepository roomPartRepository)
        {
            _roomPartRepository = roomPartRepository;
        }
        public async Task<List<QuestionModelParticipant>> GetQuestions(int roomID)
        {
            var res = await _roomPartRepository.GetQuestions(roomID);
            return res;
        }

        public async Task<RoomAnswerSubmitModel> SubmitAnswer(RoomAnswerSubmitModel roomAnswerSubmitModel)
        {
            var res = await _roomPartRepository.SubmitAnswer(roomAnswerSubmitModel);
            return res;
        }
    }
}