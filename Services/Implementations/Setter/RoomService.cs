using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using QuickQuiz.Helpers;
using QuickQuiz.Models;
using QuickQuiz.Models.RequestModels;
using QuickQuiz.Models.Response;
using QuickQuiz.Models.Room;
using QuickQuiz.Repositories.Interfaces.ISetter;
using QuickQuiz.Services.Interfaces.ISetter;

namespace QuickQuiz.Services.Implementations.Setter
{
    public class RoomService : IRoomService
    {
        private IRoomRepository _roomRepository;
        private ICustomLogger _logger;
        public RoomService(IRoomRepository roomRepository, ICustomLogger customLogger)
        {
            _roomRepository = roomRepository;
            _logger = customLogger;
        }


        public List<T> GetPage<T>(List<T> list, int page, int pageSize = 5)
        {
            return list.Skip((page - 1) * pageSize).Take(pageSize).ToList();
        }

        public async Task<object> RoomCreation(RoomModel roomModel)
        {
            Console.WriteLine("Room Creation service called");
            var response = new ResponseModel();
            var room = await _roomRepository.isSetter(roomModel.UserID);
            if (!room)
            {
                _logger.Log(LogLevel.Warning, "Room unauthorized access " + roomModel.RoomID + " :" + roomModel.UserID);
                response.Status = false;
                response.Message = "You are not authorized to create room";
                return await Task.FromResult(response);
            }
            
            // var ans = await _roomRepository.createRoom(roomModel);
            if (await _roomRepository.createRoom(roomModel))
            {
                _logger.Log(LogLevel.Information, "Room Created " + roomModel.RoomID + " :" + roomModel.UserID);
                response.Status = true;
                response.Message = "Room Created Successfully";
                return await Task.FromResult(response);
            }
            _logger.Log(LogLevel.Error, "Room Creation Failed " + roomModel.RoomID + " :" + roomModel.UserID);
            response.Status = false;
            response.Message = "Room Creation Failed";
            return await Task.FromResult(response);
        }









        public async Task<object> GetRoomResult(GetRoomResult getRoomResult, int pg)
        {
            int roomID = getRoomResult.RoomID;
            int UserID = getRoomResult.UserID;
            var response = new ResponseModel();

            var status = await _roomRepository.isRoomSetter(roomID, UserID);
            if (status)
            {
                _logger.Log(LogLevel.Warning, "Room unauthorized access " + roomID + " :" + UserID);
                response.Status = true;
                response.Message = "Room Result Fetched Successfully";
                response.Data = await _roomRepository.GetRoomResult(roomID);
                return await Task.FromResult(response);
            }
            else
            {

                var status3 = await _roomRepository.isRoomActive(roomID);
                if (status3)
                {
                    _logger.Log(LogLevel.Warning, "Room access when room is active" + roomID + " :" + UserID);
                    response.Status = false;
                    response.Message = "Cannot Get Result When Room is active";
                    return await Task.FromResult(response);
                }
                var status2 = await _roomRepository.isMember(roomID, UserID);
                if (!status2)
                {
                    _logger.Log(LogLevel.Warning, "Room unauthorized access " + roomID + " :" + UserID);
                    response.Status = false;
                    response.Message = "You are not authorized to view room result";
                    return await Task.FromResult(response);
                }
                var ans = await _roomRepository.GetRoomResult(roomID);
                _logger.Log(LogLevel.Information, "Room Result Fetched " + roomID + " :" + UserID);
                response.Status = true;
                response.Message = "Room Result Fetched Successfully";
                response.Data = GetPage(ans, pg);
                return await Task.FromResult(response);
            }
        }


        public async Task<object> RoomList(GetRoomListRequest getRoomListRequest, int pg)
        {
            int UserID = getRoomListRequest.UserID;
            var response = new ResponseModel();
            var status = await _roomRepository.isSetter(UserID);
            if (!status)
            {
                _logger.Log(LogLevel.Warning, "Room unauthorized access " + UserID);
                response.Status = false;
                response.Message = "You are not authorized to view rooms";
                return await Task.FromResult(response);
            }
            var ans = await _roomRepository.RoomList(getRoomListRequest);
            if (ans.Count > 0)
            {
                _logger.Log(LogLevel.Information, "Rooms Fetched " + UserID);
                response.Status = true;
                response.Message = "Rooms Fetched Successfully";
                response.Data = GetPage(ans, pg);
                Pager pager = new Pager(ans.Count, pg);
                response.pages = pager.GetPagingInfo();
            }
            else
            {
                _logger.Log(LogLevel.Warning, "No Rooms Found " + UserID);
                response.Status = false;
                response.Message = "No Rooms Found";
            }
            return await Task.FromResult(response);
        }

        public async Task<object> GetRoom(GetRoom getRoom)
        {
            int roomID = getRoom.RoomID;
            int UserID = getRoom.UserID;
            var response = new ResponseModel();
            var status = await _roomRepository.isRoomSetter(roomID, UserID);
            if (!status)
            {
                _logger.Log(LogLevel.Warning, "Room unauthorized access " + roomID + " :" + UserID);
                response.Status = false;
                response.Message = "You are not authorized to view room";
                return await Task.FromResult(response);
            }
            var ans = await _roomRepository.GetRoom(getRoom.RoomID);
            if (ans != null)
            {
                _logger.Log(LogLevel.Information, "Room Fetched " + roomID + " :" + UserID);
                response.Status = true;
                response.Message = "Room Fetched Successfully";
                response.Data = ans;
            }
            else
            {
                _logger.Log(LogLevel.Warning, "Room Not Found " + roomID + " :" + UserID);
                response.Status = false;
                response.Message = "Room Not Found";
            }
            return await Task.FromResult(response);
        }



        public async Task<object> StartQuiz(RoomStatus roomStatus)
        {
            int roomID = roomStatus.RoomID;
            int UserID = roomStatus.UserID;
            var response = new ResponseModel();
            var status = await _roomRepository.isRoomSetter(roomID, UserID);
            if (!status)
            {
                _logger.Log(LogLevel.Warning, "Room unauthorized access " + roomID + " :" + UserID);
                response.Status = false;
                response.Message = "You are not authorized to start quiz";
                return await Task.FromResult(response);
            }
            var status2 = await _roomRepository.isRoomActive(roomID);
            if (status2)
            {
                _logger.Log(LogLevel.Warning, "Room unauthorized access " + roomID + " :" + UserID);
                response.Status = false;
                response.Message = "Quiz is already active";
                return await Task.FromResult(response);
            }
            var status3 = await _roomRepository.StartQuiz(roomID);
            if (status3)
            {
                _logger.Log(LogLevel.Information, "Quiz Started " + roomID + " :" + UserID);
                response.Status = true;
                response.Message = "Quiz Started Successfully";
            }
            return await Task.FromResult(response);

        }
        public async Task<object> StopQuiz(RoomStatus roomStatus)
        {
            int roomID = roomStatus.RoomID;
            int UserID = roomStatus.UserID;
            var response = new ResponseModel();
            var status = await _roomRepository.isRoomSetter(roomID, UserID);
            if (!status)
            {
                _logger.Log(LogLevel.Warning, "Room unauthorized access " + roomID + " :" + UserID);
                response.Status = false;
                response.Message = "You are not authorized to start quiz";
                return await Task.FromResult(response);
            }
            var status2 = await _roomRepository.isRoomActive(roomID);
            if (!status2)
            {
                _logger.Log(LogLevel.Warning, "Room unauthorized access " + roomID + " :" + UserID);
                response.Status = false;
                response.Message = "Quiz is not active";
                return await Task.FromResult(response);
            }
            var status3 = await _roomRepository.StopQuiz(roomID);
            if (status3)
            {
                _logger.Log(LogLevel.Information, "Quiz Stopped " + roomID + " :" + UserID);
                response.Status = true;
                response.Message = "Quiz Stopped Successfully";
            }
            return await Task.FromResult(response);
        }
        public async Task<object> PauseQuiz(RoomStatus roomStatus)
        {
            int roomID = roomStatus.RoomID;
            int UserID = roomStatus.UserID;
            var response = new ResponseModel();
            var status = await _roomRepository.isRoomSetter(roomID, UserID);
            if (!status)
            {
                _logger.Log(LogLevel.Warning, "Room unauthorized access " + roomID + " :" + UserID);
                response.Status = false;
                response.Message = "You are not authorized to start quiz";
                return await Task.FromResult(response);
            }
            var status2 = await _roomRepository.isRoomActive(roomID);
            if (!status2)
            {
                _logger.Log(LogLevel.Warning, "Quiz is not active but want to pause " + roomID + " :" + UserID);
                response.Status = false;
                response.Message = "Quiz is not active";
                return await Task.FromResult(response);
            }
            var status3 = await _roomRepository.PauseQuiz(roomID);
            if (status3)
            {
                _logger.Log(LogLevel.Information, "Quiz Paused " + roomID + " :" + UserID);
                response.Status = true;
                response.Message = "Quiz Paused Successfully";
            }
            return await Task.FromResult(response);
        }



        public async Task<object> RoomDelete(DeleteRoomByRoom deleteRoomByRoom)
        {
            int roomID = deleteRoomByRoom.RoomID;
            int UserID = deleteRoomByRoom.UserID;

            var response = new ResponseModel();
            var status = await _roomRepository.isRoomSetter(roomID, UserID);
            if (!status)
            {
                _logger.Log(LogLevel.Warning, "Room unauthorized access " + roomID + " :" + UserID);
                response.Status = false;
                response.Message = "You are not authorized to delete room";
                return await Task.FromResult(response);
            }
            if (await _roomRepository.RoomDelete(roomID))
            {
                _logger.Log(LogLevel.Information, "Room Deleted " + roomID + " :" + UserID);
                response.Status = true;
                response.Message = "Room Deleted Successfully";
                return await Task.FromResult(response);
            }
            _logger.Log(LogLevel.Warning, "Room Deletion Failed " + roomID + " :" + UserID);
            response.Status = false;
            response.Message = "Room Deletion Failed";
            return await Task.FromResult(response);

        }

        public async Task<object> RoomUpdate(RoomUpdateModel roomModel)
        {
            int roomID = roomModel.RoomID;
            int UserID = roomModel.UserID;
            var response = new ResponseModel();
            var status = await _roomRepository.isRoomSetter(roomID, UserID);
            if (!status)
            {
                _logger.Log(LogLevel.Warning, "Room unauthorized access " + roomID + " :" + UserID);
                response.Status = false;
                response.Message = "You are not authorized to update room";
                return await Task.FromResult(response);
            }
            if (await _roomRepository.RoomUpdate(roomModel))
            {
                _logger.Log(LogLevel.Information, "Room Updated " + roomID + " :" + UserID);
                response.Status = true;
                response.Message = "Room Updated Successfully";
                return await Task.FromResult(response);
            }
            _logger.Log(LogLevel.Warning, "Room Updation Failed " + roomID + " :" + UserID);
            response.Status = false;
            response.Message = "Room Updation Failed";
            return await Task.FromResult(response);
        }

    }
}