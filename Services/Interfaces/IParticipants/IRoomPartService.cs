using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using QuickQuiz.Models.Room;

namespace QuickQuiz.Services.Interfaces.IParticipants
{
    public interface IRoomPartService
    {
        Task<List<QuestionModelParticipant>> GetQuestions(int roomID);
        Task<RoomAnswerSubmitModel> SubmitAnswer(RoomAnswerSubmitModel roomAnswerSubmitModel);
    }
}