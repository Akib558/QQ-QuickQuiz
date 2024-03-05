using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using QuickQuiz.Models;
using QuickQuiz.Models.Room;
using QuickQuiz.Services.Interfaces.IParticipants;

namespace QuickQuiz.Controllers
{
    [ApiController]
    [Route("api/participants")]
    public class ParticipantsController : ControllerBase
    {
        private readonly IRoomPartService _roomPartService;
        public ParticipantsController(IRoomPartService roomPartService)
        {
            _roomPartService = roomPartService;
        }

        [HttpGet("room/questions")]
        public async Task<IActionResult> GetQuestions(ParticipantQuestionRequest participantQuestionRequest)
        {
            if (CheckParticipantAccess(participantQuestionRequest.UserID, participantQuestionRequest.RoomID))
            {
                return Unauthorized();
            }
            var result = await _roomPartService.GetQuestions(participantQuestionRequest.RoomID);
            return Ok(result);
        }

        [HttpPost("room/answer")]
        public async Task<IActionResult> AnswerQuestion(RoomAnswerSubmitModel roomAnswerSubmitModel)
        {

            var result = await _roomPartService.SubmitAnswer(roomAnswerSubmitModel);
            return Ok(result);
        }

        private bool CheckParticipantAccess(int userID, int roomID)
        {
            string _connectionString = "Server=(localdb)\\QuickQuiz; Database=QuickQuiz; Trusted_Connection=True;Encrypt=false;";

            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("SELECT * FROM RoomParticipant WHERE UserID = @userID and RoomID = @roomID", connection))
                {
                    command.Parameters.AddWithValue("@userID", userID);
                    command.Parameters.AddWithValue("@roomID", roomID);

                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    return !reader.HasRows;
                }
            }
        }


    }


}