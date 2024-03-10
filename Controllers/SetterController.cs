using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using QuickQuiz.Models;
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


        [HttpGet("room/{roomID}/participants/show")] //create
        public async Task<IActionResult> RoomParticipants(int roomID)
        {
            // return Ok("Hello");
            var result = await _roomService.GetParticipants(roomID);
            return Ok(result);
        }

        [HttpGet("room/{roomID}/questions")] //works

        public async Task<IActionResult> RoomQuestions(int roomID)
        {
            var result = await _roomService.GetQuestions(roomID);
            return Ok(result);
        }

        [HttpDelete("room/{roomID}/delete")] //works
        public async Task<IActionResult> RoomDelete(int roomID)
        {
            var result = await _roomService.RoomDelete(roomID);
            return Ok(result);
        }

        [HttpPut("room/{roomID}/update")] //works
        public async Task<IActionResult> RoomUpdate(int roomID, RoomUpdateModel roomModel)
        {
            var result = await _roomService.RoomUpdate(roomModel);
            return Ok(result);
        }

        [HttpDelete("room/{roomID}/questions/{questionID}/delete")] // works
        public async Task<IActionResult> QuestionDelete(int questionID)
        {
            var result = await _roomService.QuestionDelete(questionID);
            return Ok(result);
        }

        [HttpPut("room/{roomID}/questions/update")] //works
        public async Task<IActionResult> QuestionUpdate(UpdateQuestionModel questionModel)
        {
            var result = await _roomService.QuestionUpdate(questionModel);
            return Ok(result);
        }

        [HttpGet("room/{roomID}/participants/answers")] //works
        public async Task<IActionResult> RoomParticipantsAnswers(int roomID)
        {
            var result = await _roomService.GetParticipantsAnswerByRoom(roomID);
            return Ok(result);
        }

        [HttpGet("room/{roomID}/participants/results")] //works
        public async Task<IActionResult> RoomParticipantsResults(int roomID, int participantID)
        {
            var result = await _roomService.GetParticipantsAnswerByRoom(roomID);
            return Ok(result);
        }

        [HttpGet("room/{roomID}/participants/{participantID}/answers")] //works
        public async Task<IActionResult> RoomParticipantsAnswersByID(int roomID, int participantID)
        {
            var result = await _roomService.GetParticipantsAnswer(roomID, participantID);
            return Ok(result);
        }

        // [HttpGet("room/{roomID}/participants/{participantID}/result")]
        // public async Task<IActionResult> RoomParticipantsAnswersResult(int roomID, int participantID)
        // {
        //     var result = await _roomService.GetParticipantsAnswerByRoom(roomID);
        //     return Ok(result);
        // }

        [HttpGet("room/{roomID}/participants/{participantID}/show")] //works
        public async Task<Participant> RoomParticipantsShowByID(int roomID, int participantID)
        {
            var result = await _roomService.GetParticipantInfoByID(roomID, participantID);
            return result;
        }

        [HttpGet("room/{roomID}/participants/{participantID}/delete")] //works
        public async Task<IActionResult> RoomParticipantsDelete(int roomID, int participantID)
        {
            var result = await _roomService.RoomParticipantDelete(roomID, participantID);
            return Ok(result);
        }
        // [HttpGet("room/{roomID}/participants/{participantID}/update")]
        // public async Task<IActionResult> RoomParticipantsUpdate(int roomID, int participantID)
        // {
        //     var result = await _roomService.GetParticipantsAnswerByRoom(roomID);
        //     return Ok(result);
        // }

        [HttpGet("room/{roomID}/result")] //works
        public async Task<IActionResult> RoomResult(int roomID)
        {
            var result = await _roomService.GetRoomResult(roomID);
            return Ok(result);
        }
        [HttpGet("room/{roomID}/start")]
        public async Task<IActionResult> StartQuiz(int roomID)
        {
            var result = await _roomService.StartQuiz(roomID);
            return Ok(result);
        }
        [HttpGet("room/{roomID}/stop")]
        public async Task<IActionResult> StopQuiz(int roomID)
        {
            var result = await _roomService.StopQuiz(roomID);
            return Ok(result);
        }
        [HttpGet("room/{roomID}/pause")]
        public async Task<IActionResult> PauseQuiz(int roomID)
        {
            var result = await _roomService.PauseQuiz(roomID);
            return Ok(result);
        }
    }
}