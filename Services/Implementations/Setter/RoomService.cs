using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using QuickQuiz.Models.Room;
using QuickQuiz.Repositories.Interfaces.ISetter;
using QuickQuiz.Services.Interfaces.ISetter;

namespace QuickQuiz.Services.Implementations.Setter
{
    public class RoomService : IRoomService
    {
        private IRoomRepository _roomRepository;
        public RoomService(IRoomRepository roomRepository)
        {
            _roomRepository = roomRepository;
        }

        public async Task<bool> RoomCreation(RoomModel roomModel)
        {


            if (await _roomRepository.createRoom(roomModel))
            {
                return await Task.FromResult(true);
            }
            return await Task.FromResult(false);

        }

        public async Task<List<int>> GetParticipants(int roomID)
        {

            if (await _roomRepository.RoomParticipants(roomID) != null)
            {
                return await _roomRepository.RoomParticipants(roomID);
            }
            return await _roomRepository.RoomParticipants(roomID);
        }

        public async Task<List<QuestionModel>> GetQuestions(int roomID)
        {
            if (await _roomRepository.RoomParticipants(roomID) != null)
            {
                return await _roomRepository.GetQuetions(roomID);
            }
            return await _roomRepository.GetQuetions(roomID);
        }


        public async Task<List<GetParticipantsAnswerByIDModel>> GetParticipantsAnswerByRoom(int roomID)
        {
            var ans = await _roomRepository.GetParticipantsAnswerByRoom(roomID);
            return ans;
        }

        public async Task<List<RoomResultModel>> GetRoomResult(int roomID)
        {
            var ans = await _roomRepository.GetRoomResult(roomID);
            return ans;
        }

        public async Task<List<ParticipantsModel>> AllParticipants()
        {
            var ans = await _roomRepository.AllParticipants();
            return ans;
        }

        public async Task<int> AddParticipants(AddParticipants addParticipants)
        {
            var ans = await _roomRepository.AddParticipants(addParticipants);
            return ans;
        }

        public async Task<List<RoomModel>> RoomList(GetRoomListRequest getRoomListRequest)
        {
            return await _roomRepository.RoomList(getRoomListRequest);
        }


        public async Task<int> AddQuestions(AddQuestion addQuestion)
        {
            return await _roomRepository.AddQuestions(addQuestion);
        }


        public async Task<int> StartQuiz(int roomID)
        {
            return await _roomRepository.StartQuiz(roomID);
        }
        public async Task<int> StopQuiz(int roomID)
        {
            return await _roomRepository.StopQuiz(roomID);
        }
        public async Task<int> PauseQuiz(int roomID)
        {
            return await _roomRepository.PauseQuiz(roomID);
        }



        public bool RoomDeletion()
        {
            return true;
        }

        public bool RoomUpdate()
        {
            return true;
        }

        public IActionResult RoomData(int setterID)
        {
            return null;
        }

    }
}