using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
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

        [HttpPost("{userID}/room/{roomID}/questions")]
        public async Task<IActionResult> GetQuestions(int roomID)
        {
            if (CheckParticipantAccess(int.Parse(Request.RouteValues["userID"].ToString()), Request.RouteValues["roomID"].ToString()))
            {
                return Unauthorized();
            }
            var result = await _roomPartService.GetQuestions(roomID);
            return Ok(result);
        }

        [HttpPost("room/{roomID}/answer")]
        public async Task<IActionResult> AnswerQuestion(RoomAnswerSubmitModel roomAnswerSubmitModel)
        {

            var result = await _roomPartService.SubmitAnswer(roomAnswerSubmitModel);
            return Ok(result);
        }

        private bool CheckParticipantAccess(int userID, string roomID)
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