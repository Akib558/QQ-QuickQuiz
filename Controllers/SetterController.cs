using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
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

        [HttpPost("room/addquestions")]
        public async Task<IActionResult> AddQuestions(AddQuestion addQuestion)
        {
            var result = await _roomService.AddQuestions(addQuestion);
            return Ok(result);
        }

        [HttpPost("room/addparticipants")]
        public async Task<IActionResult> AddParticipants(AddParticipants addParticipants)
        {
            var result = await _roomService.AddParticipants(addParticipants);
            return Ok(result);
        }

        [HttpGet("myrooms")]
        public async Task<IActionResult> MyRooms(GetRoomListRequest getRoomListRequest)
        {
            var result = await _roomService.RoomList(getRoomListRequest);
            return Ok(result);
        }

        [HttpGet("allparticipants")]
        public async Task<IActionResult> AllParticipants()
        {
            var result = await _roomService.AllParticipants();
            return Ok(result);
        }

        [HttpPost("room/create")]
        public async Task<IActionResult> RoomCreation(RoomModel roomModel)
        {
            var result = await _roomService.RoomCreation(roomModel);
            return Ok(result);
        }

        [HttpDelete("room/delete")]
        public IActionResult RoomDeletion()
        {
            var result = _roomService.RoomDeletion();
            return Ok(result);
        }


        [HttpPut("room/update")]
        public IActionResult RoomUpdate()
        {
            var result = _roomService.RoomUpdate();
            return Ok(result);
        }

        [HttpGet("room/data")]
        public IActionResult RoomData(int setterID)
        {
            var result = _roomService.RoomData(setterID);
            return Ok(result);
        }

        [HttpGet("room/{roomID}/participants")]
        public async Task<IActionResult> RoomParticipants(int roomID)
        {
            // return Ok("Hello");
            var result = await _roomService.GetParticipants(roomID);
            return Ok(result);
        }

        [HttpGet("room/{roomID}/questions")]

        public async Task<IActionResult> RoomQuestions(int roomID)
        {
            var result = await _roomService.GetQuestions(roomID);
            return Ok(result);
        }

        [HttpGet("room/{roomID}/participants/answers")]
        public async Task<IActionResult> RoomParticipantsAnswers(int roomID)
        {
            var result = await _roomService.GetParticipantsAnswerByRoom(roomID);
            return Ok(result);
        }

        [HttpGet("room/{roomID}/result")]
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