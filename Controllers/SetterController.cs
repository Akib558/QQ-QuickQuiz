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

        [HttpGet("myrooms/{pg=1}")] //works
        public async Task<IActionResult> MyRooms(GetRoomListRequest getRoomListRequest, int pg)
        {
            var result = await _roomService.RoomList(getRoomListRequest, pg);
            return Ok(result);
        }

        [HttpGet("allparticipants/{pg=1}")] //works
        public async Task<IActionResult> AllParticipants(int pg)
        {
            var result = await _roomService.AllParticipants(pg);
            return Ok(result);
        }

        [HttpPost("room/create")] //works
        public async Task<IActionResult> RoomCreation(RoomModel roomModel)
        {
            var result = await _roomService.RoomCreation(roomModel);
            return Ok(result);
        }


        [HttpGet("room/participants/{pg=1}")] //create
        public async Task<IActionResult> RoomParticipants(GetParticipantsByRoom getParticipantsByRoom, int pg)
        {
            var result = await _roomService.GetParticipants(getParticipantsByRoom, pg);
            return Ok(result);
        }

        [HttpGet("room/questions/{pg=1}")] //works

        public async Task<IActionResult> RoomQuestions(GetQuestionsByRoom getQuestionsByRoom, int pg)
        {
            var result = await _roomService.GetQuestions(getQuestionsByRoom, pg);
            return Ok(result);
        }

        [HttpDelete("room/delete")] //works
        public async Task<IActionResult> RoomDelete(DeleteRoomByRoom deleteRoomByRoom)
        {
            var result = await _roomService.RoomDelete(deleteRoomByRoom);
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
            var result = await _roomService.QuestionDelete(deleteQuestion);
            return Ok(result);
        }

        [HttpPut("room/questions/update")] //works
        public async Task<IActionResult> QuestionUpdate(UpdateQuestionModel questionModel)
        {
            var result = await _roomService.QuestionUpdate(questionModel);
            return Ok(result);
        }

        [HttpGet("room/participants/answers/{pg=1}")] //works
        public async Task<IActionResult> RoomParticipantsAnswers(GetPariticipantsAnswer getPariticipantsAnswer, int pg)
        {
            var result = await _roomService.GetParticipantsAnswerByRoom(getPariticipantsAnswer, pg);
            return Ok(result);
        }

        [HttpGet("room/participants/results/{pg=1}")] //works
        public async Task<IActionResult> RoomParticipantsResults(GetRoomResult getRoomResult, int pg)
        {
            var result = await _roomService.GetRoomResult(getRoomResult, pg);
            return Ok(result);
        }

        [HttpGet("room/singleparticipant/answer")] //works
        public async Task<IActionResult> RoomParticipantsAnswersByID(GetParticipantsAnswerByID getParticipantsAnswerByID)
        {
            var result = await _roomService.GetParticipantsAnswer(
                getParticipantsAnswerByID
            );
            return Ok(result);
        }

        [HttpGet("room/singleparticipant/info")] //works
        public async Task<IActionResult> RoomParticipantsShowByID(GetParticipantsInfoByID getParticipantsInfoByID)
        {
            var result = await _roomService.GetParticipantInfoByID(
                getParticipantsInfoByID
            );
            return Ok(result);
        }

        [HttpGet("room/singleparticipant/delete")] //works
        public async Task<IActionResult> RoomParticipantsDelete(DeleteParticipantsByID deleteParticipantsByID)
        {
            var result = await _roomService.RoomParticipantDelete(
               deleteParticipantsByID
            );
            return Ok(result);
        }

        [HttpGet("room/result/{pg=1}")] //works
        public async Task<IActionResult> RoomResult(GetRoomResult getRoomResult, int pg)
        {
            var result = await _roomService.GetRoomResult(getRoomResult, pg);
            return Ok(result);
        }
        [HttpGet("room/start")]
        public async Task<IActionResult> StartQuiz(RoomStatus roomStatus)
        {
            var result = await _roomService.StartQuiz(roomStatus);
            return Ok(result);
        }
        [HttpGet("room/stop")]
        public async Task<IActionResult> StopQuiz(RoomStatus roomStatus)
        {
            var result = await _roomService.StopQuiz(roomStatus);
            return Ok(result);
        }
        [HttpGet("room/pause")]
        public async Task<IActionResult> PauseQuiz(RoomStatus roomStatus)
        {
            var result = await _roomService.PauseQuiz(roomStatus);
            return Ok(result);
        }

    }
}