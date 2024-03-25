using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using QuickQuiz.Models;
using QuickQuiz.Models.RequestModels;
using QuickQuiz.Models.Room;

namespace QuickQuiz.Repositories.Interfaces.ISetter
{
    public interface IRoomRepository
    {
        Task<bool> createRoom(RoomModel roomModel);

        Task<List<RoomResultModel>> GetRoomResult(int roomID);
        Task<List<RoomModel>> RoomList(GetRoomListRequest getRoomListRequest);
        Task<bool> StartQuiz(int roomID);
        Task<bool> StopQuiz(int roomID);
        Task<bool> PauseQuiz(int roomID);
        Task<bool> RoomDelete(int roomID);
        Task<bool> RoomUpdate(RoomUpdateModel roomModel);

        Task<RoomModel> GetRoom(int roomID);
        
        
        Task<bool> isRoomAuthorized(int roomID, int userID);
        Task<bool> isSetter(int userID);
        Task<bool> isRoomSetter(int roomID, int setterID);
        // Task<bool> roomStatus(int roomID);
        Task<bool> isRoomActive(int roomID);
        Task<bool> isMember(int roomID, int userID);
    }
}