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

        public async Task<bool> RoomCreation(RoomModel roomModel){


            if(await _roomRepository.createRoom(roomModel)){
                return await Task.FromResult(true);
            }
            return await Task.FromResult(false);

        }

        public async  Task<List<int>> GetParticipants(int roomID){
         
            if(await _roomRepository.RoomParticpants(roomID) != null){
                return await _roomRepository.RoomParticpants(roomID);
            }
            return await _roomRepository.RoomParticpants(roomID);
        }

        public async Task<List<QuestionModel>> GetQuestions(int roomID){
            if(await _roomRepository.RoomParticpants(roomID) != null){
                return await _roomRepository.GetQuetions(roomID);
            }
            return await _roomRepository.GetQuetions(roomID);
        }

        public bool RoomDeletion(){
            return true;
        }

        public bool RoomUpdate(){
            return true;
        }

        public IActionResult RoomData(int setterID){
            return null;
        }
        
    }
}