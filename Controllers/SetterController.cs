using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using QuickQuiz.Models;
using QuickQuiz.Models.RequestModels;
using QuickQuiz.Models.Room;
using QuickQuiz.Services.Interfaces.ISetter;

namespace QuickQuiz.Controllers
{
    [ApiController]
    [Route("api/setter")]
    public class SetterController : ControllerBase
    {
        private readonly IRoomService _roomService;
        public SetterController(IRoomService roomService)
        {
            _roomService = roomService;
        }

        [HttpPost("room/addquestions")] //works
        public async Task<IActionResult> AddQuestions(AddQuestion addQuestion)
        {
            var result = await _roomService.AddQuestions(addQuestion);
            return Ok(result);
        }

        [HttpPost("room/addparticipants")] //works
        public async Task<IActionResult> AddParticipants(AddParticipants addParticipants)
        {
            var result = await _roomService.AddParticipants(addParticipants);
            return Ok(result);
        }

        [HttpGet("myrooms")] //works
        public async Task<IActionResult> MyRooms(GetRoomListRequest getRoomListRequest)
        {
            var result = await _roomService.RoomList(getRoomListRequest);
            return Ok(result);
        }

        [HttpGet("allparticipants")] //works
        public async Task<IActionResult> AllParticipants()
        {
            var result = await _roomService.AllParticipants();
            return Ok(result);
        }

        [HttpPost("room/create")] //works
        public async Task<IActionResult> RoomCreation(RoomModel roomModel)
        {
            var result = await _roomService.RoomCreation(roomModel);
            return Ok(result);
        }


        [HttpGet("room/participants")] //create
        public async Task<IActionResult> RoomParticipants(GetParticipantsByRoom getParticipantsByRoom)
        {
            var result = await _roomService.GetParticipants(getParticipantsByRoom.RoomID);
            return Ok(result);
        }

        [HttpGet("room/questions")] //works

        public async Task<IActionResult> RoomQuestions(GetQuestionsByRoom getQuestionsByRoom)
        {
            var result = await _roomService.GetQuestions(getQuestionsByRoom.RoomID);
            return Ok(result);
        }

        [HttpDelete("room/delete")] //works
        public async Task<IActionResult> RoomDelete(DeleteRoomByRoom deleteRoomByRoom)
        {
            var result = await _roomService.RoomDelete(deleteRoomByRoom.RoomID);
            return Ok(result);
        }

        [HttpPut("room/update")] //works
        public async Task<IActionResult> RoomUpdate(RoomUpdateModel roomModel)
        {
            var result = await _roomService.RoomUpdate(roomModel);
            return Ok(result);
        }

        [HttpDelete("room/questions/delete")] // works
        public async Task<IActionResult> QuestionDelete(DeleteQuestion deleteQuestion)
        {
            var result = await _roomService.QuestionDelete(deleteQuestion.QuestionID);
            return Ok(result);
        }

        [HttpPut("room/questions/update")] //works
        public async Task<IActionResult> QuestionUpdate(UpdateQuestionModel questionModel)
        {
            var result = await _roomService.QuestionUpdate(questionModel);
            return Ok(result);
        }

        [HttpGet("room/participants/answers")] //works
        public async Task<IActionResult> RoomParticipantsAnswers(GetPariticipantsAnswer getPariticipantsAnswer)
        {
            var result = await _roomService.GetParticipantsAnswerByRoom(getPariticipantsAnswer.RoomID);
            return Ok(result);
        }

        [HttpGet("room/participants/results")] //works
        public async Task<IActionResult> RoomParticipantsResults(GetParticipantsResult getParticipantsResult)
        {
            var result = await _roomService.GetParticipantsAnswerByRoom(getParticipantsResult.RoomID);
            return Ok(result);
        }

        [HttpGet("room/singleparticipant/answer")] //works
        public async Task<IActionResult> RoomParticipantsAnswersByID(GetParticipantsAnswerByID getParticipantsAnswerByID)
        {
            var result = await _roomService.GetParticipantsAnswer(
                getParticipantsAnswerByID.RoomID,
                getParticipantsAnswerByID.ParticipantID
            );
            return Ok(result);
        }

        // [HttpGet("room/{roomID}/participants/{participantID}/result")]
        // public async Task<IActionResult> RoomParticipantsAnswersResult(int roomID, int participantID)
        // {
        //     var result = await _roomService.GetParticipantsAnswerByRoom(roomID);
        //     return Ok(result);
        // }

        [HttpGet("room/singleparticipant/info")] //works
        public async Task<Participant> RoomParticipantsShowByID(GetParticipantsInfoByID getParticipantsInfoByID)
        {
            var result = await _roomService.GetParticipantInfoByID(
                getParticipantsInfoByID.RoomID,
                getParticipantsInfoByID.ParticipantID
            );
            return result;
        }

        [HttpGet("room/singleparticipant/delete")] //works
        public async Task<IActionResult> RoomParticipantsDelete(DeleteParticipantsByID deleteParticipantsByID)
        {
            var result = await _roomService.RoomParticipantDelete(
                deleteParticipantsByID.RoomID,
                deleteParticipantsByID.ParticipantID
            );
            return Ok(result);
        }
        // [HttpGet("room/{roomID}/participants/{participantID}/update")]
        // public async Task<IActionResult> RoomParticipantsUpdate(int roomID, int participantID)
        // {
        //     var result = await _roomService.GetParticipantsAnswerByRoom(roomID);
        //     return Ok(result);
        // }

        [HttpGet("room/result")] //works
        public async Task<IActionResult> RoomResult(RoomStatus roomStatus)
        {
            var result = await _roomService.GetRoomResult(roomStatus.RoomID);
            return Ok(result);
        }
        [HttpGet("room/start")]
        public async Task<IActionResult> StartQuiz(RoomStatus roomStatus)
        {
            var result = await _roomService.StartQuiz(roomStatus.RoomID);
            return Ok(result);
        }
        [HttpGet("room/stop")]
        public async Task<IActionResult> StopQuiz(RoomStatus roomStatus)
        {
            var result = await _roomService.StopQuiz(roomStatus.RoomID);
            return Ok(result);
        }
        [HttpGet("room/pause")]
        public async Task<IActionResult> PauseQuiz(RoomStatus roomStatus)
        {
            var result = await _roomService.PauseQuiz(roomStatus.RoomID);
            return Ok(result);
        }

    }
}