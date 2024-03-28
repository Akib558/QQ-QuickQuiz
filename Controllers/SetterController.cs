using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using QuickQuiz.Models;
using QuickQuiz.Models.RequestModels;
using QuickQuiz.Models.Room;
using QuickQuiz.Services.Interfaces.ISetter;
using Microsoft.AspNetCore.Authorization;

namespace QuickQuiz.Controllers
{
    [ApiController]
    [Route("api/setter")]
    public class SetterController : ControllerBase
    {
        private readonly IRoomService _roomService;
        private readonly IQuestionService _questionService;
        private readonly IParticipantService _participantService;
        public SetterController(IRoomService roomService, IQuestionService questionService, IParticipantService participantService)
        {
            _roomService = roomService;
            _questionService = questionService;
            _participantService = participantService;
        }
        
        [Authorize]
        [HttpPost("room/addquestions")] //works
        public async Task<IActionResult> AddQuestions(AddQuestion addQuestion)
        {
            var result = await _questionService.AddQuestions(addQuestion);
            return Ok(result);
        }
        
        [Authorize]
        [HttpPost("room/addparticipants")] //works
        public async Task<IActionResult> AddParticipants(AddParticipants addParticipants)
        {
            var result = await _participantService.AddParticipants(addParticipants);
            return Ok(result);
        }
        
        [Authorize]
        [HttpPost("myroom")]
        public async Task<IActionResult> MyRoom(GetRoom getRoom)
        {
            var result = await _roomService.GetRoom(getRoom);
            return Ok(result);
        }
        
        [Authorize]
        [HttpPost("myrooms/{pg=1}")] //works
        public async Task<IActionResult> MyRooms(GetRoomListRequest getRoomListRequest, int pg)
        {
            var result = await _roomService.RoomList(getRoomListRequest, pg);
            return Ok(result);
        }

        [Authorize]
        [HttpGet("allparticipants/{pg=1}")] //works
        public async Task<IActionResult> AllParticipants(int pg)
        {
            var result = await _participantService.AllParticipants(pg);
            return Ok(result);
        }

        [Authorize]
        [HttpPost("room/create")] //works
        public async Task<IActionResult> RoomCreation(RoomModel roomModel)
        {
            Console.WriteLine("room creation router");
            var result = await _roomService.RoomCreation(roomModel);
            return Ok(result);
        }

        [Authorize]
        [HttpGet("room/participants/{pg=1}")] //create
        public async Task<IActionResult> RoomParticipants(GetParticipantsByRoom getParticipantsByRoom, int pg)
        {
            var result = await _participantService.GetParticipants(getParticipantsByRoom, pg);
            return Ok(result);
        }
        
        [Authorize]
        [HttpPost("room/questions/{pg=1}")] //works

        public async Task<IActionResult> RoomQuestions(GetQuestionsByRoom getQuestionsByRoom, int pg)
        {
            var result = await _questionService.GetQuestions(getQuestionsByRoom, pg);
            return Ok(result);
        }
        
        [Authorize]
        [HttpGet]
        [Route("room/questions/deleteoption")] //works
        public async Task<IActionResult> DeleteOption(DeleteOption deleteOption)
        {
            var result = await _questionService.DeleteOption(deleteOption);
            return Ok(result);
        }

        [Authorize]
        [HttpDelete("room/delete")] //works
        public async Task<IActionResult> RoomDelete(DeleteRoomByRoom deleteRoomByRoom)
        {
            var result = await _roomService.RoomDelete(deleteRoomByRoom);
            return Ok(result);
        }

        [Authorize]
        [HttpPut("room/update")] //works
        public async Task<IActionResult> RoomUpdate(RoomUpdateModel roomModel)
        {
            var result = await _roomService.RoomUpdate(roomModel);
            return Ok(result);
        }
        
        [Authorize]
        [HttpDelete("room/questions/delete")] // works
        public async Task<IActionResult> QuestionDelete(DeleteQuestion deleteQuestion)
        {
            var result = await _questionService.QuestionDelete(deleteQuestion);
            return Ok(result);
        }

        [Authorize]
        [HttpPut("room/questions/update")] //works
        public async Task<IActionResult> QuestionUpdate(UpdateQuestionModel questionModel)
        {
            var result = await _questionService.QuestionUpdate(questionModel);
            return Ok(result);
        }

        [Authorize]
        [HttpGet("room/participants/answers/{pg=1}")] //works
        public async Task<IActionResult> RoomParticipantsAnswers(GetPariticipantsAnswer getPariticipantsAnswer, int pg)
        {
            var result = await _participantService.GetParticipantsAnswerByRoom(getPariticipantsAnswer, pg);
            return Ok(result);
        }
        
        [Authorize]
        [HttpGet("room/participants/results/{pg=1}")] //works
        public async Task<IActionResult> RoomParticipantsResults(GetRoomResult getRoomResult, int pg)
        {
            var result = await _roomService.GetRoomResult(getRoomResult, pg);
            return Ok(result);
        }
        
        [Authorize]
        [HttpGet("room/singleparticipant/answer")] //works
        public async Task<IActionResult> RoomParticipantsAnswersByID(GetParticipantsAnswerByID getParticipantsAnswerByID)
        {
            var result = await _participantService.GetParticipantsAnswer(
                getParticipantsAnswerByID
            );
            return Ok(result);
        }
        
        [Authorize]
        [HttpGet("room/singleparticipant/info")] //works
        public async Task<IActionResult> RoomParticipantsShowByID(GetParticipantsInfoByID getParticipantsInfoByID)
        {
            var result = await _participantService.GetParticipantInfoByID(
                getParticipantsInfoByID
            );
            return Ok(result);
        }
        
        [Authorize]
        [HttpGet("room/singleparticipant/delete")] //works
        public async Task<IActionResult> RoomParticipantsDelete(DeleteParticipantsByID deleteParticipantsByID)
        {
            var result = await _participantService.RoomParticipantDelete(
               deleteParticipantsByID
            );
            return Ok(result);
        }
        
        [Authorize]
        [HttpGet("room/result/{pg=1}")] //works
        public async Task<IActionResult> RoomResult(GetRoomResult getRoomResult, int pg)
        {
            var result = await _roomService.GetRoomResult(getRoomResult, pg);
            return Ok(result);
        }
        
        [Authorize]
        [HttpPost("room/start")]
        public async Task<IActionResult> StartQuiz(RoomStatus roomStatus)
        {
            Console.WriteLine("start quiz");
            var result = await _roomService.StartQuiz(roomStatus);
            return Ok(result);
        }
        
        [Authorize]
        [HttpPost("room/stop")]
        public async Task<IActionResult> StopQuiz(RoomStatus roomStatus)
        {
            var result = await _roomService.StopQuiz(roomStatus);
            return Ok(result);
        }
        
        [Authorize]
        [HttpPost("room/pause")]
        public async Task<IActionResult> PauseQuiz(RoomStatus roomStatus)
        {
            var result = await _roomService.PauseQuiz(roomStatus);
            return Ok(result);
        }

    }
}