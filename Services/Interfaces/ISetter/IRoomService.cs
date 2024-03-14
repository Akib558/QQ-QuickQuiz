using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using QuickQuiz.Models;
using QuickQuiz.Models.RequestModels;
using QuickQuiz.Models.Room;

namespace QuickQuiz.Services.Interfaces.ISetter
{
    public interface IRoomService
    {
        public Task<object> AllParticipants(int pg);
        public Task<object> RoomCreation(RoomModel roomModel);


        public Task<object> AddParticipants(AddParticipants addParticipants);
        public Task<object> AddQuestions(AddQuestion addQuestion);
        public Task<object> GetQuestions(GetQuestionsByRoom getQuestionsByRoom, int pg);
        public Task<object> GetParticipants(GetParticipantsByRoom getParticipantsByRoom, int pg);
        public Task<object> GetParticipantsAnswerByRoom(GetPariticipantsAnswer getPariticipantsAnswer, int pg);
        public Task<object> GetParticipantsAnswer(GetParticipantsAnswerByID getParticipantsAnswerByID);
        public Task<object> GetParticipantInfoByID(GetParticipantsInfoByID getParticipantsInfoByID);
        public Task<object> RoomParticipantDelete(DeleteParticipantsByID deleteParticipantsByID);
        public Task<object> GetRoomResult(GetRoomResult getRoomResult, int pg);
        public Task<object> RoomList(GetRoomListRequest getRoomListRequest, int pg);
        public Task<object> StartQuiz(RoomStatus roomStatus);
        public Task<object> StopQuiz(RoomStatus roomStatus);
        public Task<object> PauseQuiz(RoomStatus roomStatus);
        public Task<object> RoomDelete(DeleteRoomByRoom deleteRoomByRoom);
        public Task<object> RoomUpdate(RoomUpdateModel roomModel);
        public Task<object> QuestionDelete(DeleteQuestion deleteQuestion);
        public Task<object> QuestionUpdate(UpdateQuestionModel questionModel);

        /*
            Todo: Excel Import and Export
        */

    }
}