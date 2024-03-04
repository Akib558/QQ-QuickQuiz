using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using QuickQuiz.Models.Room;

namespace QuickQuiz.Repositories.Interfaces.IParticipants
{
    public interface IRoomPartRepository
    {
        Task<List<QuestionModelParticipant>> GetQuestions(int roomID);
        Task<RoomAnswerSubmitModel> SubmitAnswer(RoomAnswerSubmitModel roomAnswerSubmitModel);
    }
}