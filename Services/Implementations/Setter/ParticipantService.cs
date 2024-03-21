using QuickQuiz.Helpers;
using QuickQuiz.Models.RequestModels;
using QuickQuiz.Models.Response;
using QuickQuiz.Repositories.Interfaces.ISetter;
using QuickQuiz.Services.Interfaces.ISetter;

namespace QuickQuiz.Services.Implementations.Setter;

public class ParticipantService : IParticipantService
{
    
    private IParticipantRepository _roomRepository;
    private ICustomLogger _logger;
    public ParticipantService(IParticipantRepository roomRepository, ICustomLogger customLogger)
    {
        _roomRepository = roomRepository;
        _logger = customLogger;
    }
    
        
    public List<T> GetPage<T>(List<T> list, int page, int pageSize = 5)
    {
        return list.Skip((page - 1) * pageSize).Take(pageSize).ToList();
    }
    
    public async Task<object> GetParticipants(GetParticipantsByRoom getParticipantsByRoom, int pg)
    {
        var response = new ResponseModel();
        var room = await _roomRepository.isRoomAuthorized(
            getParticipantsByRoom.RoomID,
            getParticipantsByRoom.UserID
        );
        if (!room)
        {
            _logger.Log(LogLevel.Warning, "Room unauthorized access " + getParticipantsByRoom.RoomID + " :" + getParticipantsByRoom.UserID);
            response.Status = false;
            response.Message = "You are not authorized to view participants";
            return await Task.FromResult(response);
        }
        var ans = await _roomRepository.RoomParticipants(getParticipantsByRoom.RoomID);
        if (ans != null)
        {
            _logger.Log(LogLevel.Information, "Room Participants Fetched " + getParticipantsByRoom.RoomID + " :" + getParticipantsByRoom.UserID);
            response.Status = true;
            response.Message = "Participants Fetched Successfully";
            response.Data = GetPage(ans, pg);
            return await Task.FromResult(response);
        }
        _logger.Log(LogLevel.Warning, "Room Participants Not Found " + getParticipantsByRoom.RoomID + " :" + getParticipantsByRoom.UserID);
        response.Status = false;
        response.Message = "No Participants Found";
        return await Task.FromResult(response);

    }
    
    public async Task<object> AllParticipants(int pg)
    {
        var response = new ResponseModel();
        var ans = await _roomRepository.AllParticipants();
        if (ans.Count > 0)
        {
            _logger.Log(LogLevel.Information, "All Participants Fetched");
            response.Status = true;
            response.Message = "Participants Fetched Successfully";
            response.Data = GetPage(ans, pg);
            Pager pager = new Pager(ans.Count, pg);
            response.pages = pager.GetPagingInfo();
                

        }
        else
        {
            _logger.Log(LogLevel.Warning, "No Participants Found");
            response.Status = false;
            response.Message = "No Participants Found";
        }
        return await Task.FromResult(response);
    }

    public async Task<object> AddParticipants(AddParticipants addParticipants)
    {
        int roomID = addParticipants.RoomID;
        int UserID = addParticipants.UserID;
        var response = new ResponseModel();
        var status = await _roomRepository.isRoomSetter(roomID, UserID);

        if (!status)
        {
            _logger.Log(LogLevel.Warning, "Room unauthorized access " + roomID + " :" + UserID);
            response.Status = false;
            response.Message = "You are not authorized to add participants";
            return await Task.FromResult(response);
        }
        _logger.Log(LogLevel.Information, "Participants Added " + roomID + " :" + UserID);
        response.Status = true;
        response.Message = "Participants Added Successfully";
        response.Data = await _roomRepository.AddParticipants(addParticipants);
        return await Task.FromResult(response);
    }

    
            public async Task<object> GetParticipantInfoByID(GetParticipantsInfoByID getParticipantsInfoByID)
        {
            int roomID = getParticipantsInfoByID.RoomID;
            int participantID = getParticipantsInfoByID.ParticipantID;
            int UserID = getParticipantsInfoByID.UserID;
            var response = new ResponseModel();

            var status = await _roomRepository.isRoomSetter(roomID, UserID);

            if (!status)
            {

                _logger.Log(LogLevel.Warning, "Room unauthorized access " + roomID + " :" + UserID);
                response.Status = false;
                response.Message = "You are not authorized to view participants";
                return await Task.FromResult(response);
            }

            var ans = await _roomRepository.GetParticipantInfoByID(roomID, participantID);
            if (ans == null)
            {
                _logger.Log(LogLevel.Warning, "Room Participants Not Found " + roomID + " :" + UserID);
                response.Status = false;
                response.Message = "No Participants Found";
                return await Task.FromResult(response);
            }
            _logger.Log(LogLevel.Information, "Room Participants Fetched " + roomID + " :" + UserID);
            response.Status = true;
            response.Message = "Participants Fetched Successfully";
            response.Data = ans;
            return await Task.FromResult(response);
        }

        public async Task<object> RoomParticipantDelete(DeleteParticipantsByID deleteParticipantsByID)
        {
            int roomID = deleteParticipantsByID.RoomID;
            int ParticiapntsID = deleteParticipantsByID.ParticipantID;
            int UserID = deleteParticipantsByID.UserID;
            var response = new ResponseModel();

            var status = await _roomRepository.isRoomSetter(roomID, UserID);
            if (!status)
            {
                _logger.Log(LogLevel.Warning, "Room unauthorized access " + roomID + " :" + UserID);
                response.Status = false;
                response.Message = "You are not authorized to delete participants";
                return await Task.FromResult(response);
            }

            if (await _roomRepository.RoomParticipantDelete(roomID, ParticiapntsID))
            {
                _logger.Log(LogLevel.Information, "Room Participants Deleted " + roomID + " :" + UserID);
                response.Status = true;
                response.Message = "Participants Deleted Successfully";
                return await Task.FromResult(response);
            }
            _logger.Log(LogLevel.Warning, "Room Participants Deletion Failed " + roomID + " :" + UserID);
            response.Status = false;
            response.Message = "Participants Deletion Failed";
            return await Task.FromResult(response);

        }

        public async Task<object> GetParticipantsAnswerByRoom(GetPariticipantsAnswer getPariticipantsAnswer, int pg)
        {
            int roomID = getPariticipantsAnswer.RoomID;
            var response = new ResponseModel();
            var status = await _roomRepository.isRoomSetter(roomID, getPariticipantsAnswer.UserID);
            if (!status)
            {
                _logger.Log(LogLevel.Warning, "Room unauthorized access " + roomID + " :" + getPariticipantsAnswer.UserID);
                response.Status = false;
                response.Message = "You are not authorized to view participants answer";
                return await Task.FromResult(response);
            }
            var ans = await _roomRepository.GetParticipantsAnswerByRoom(roomID);
            if (ans.Count > 0)
            {
                _logger.Log(LogLevel.Information, "Room Participants Answer Fetched " + roomID + " :" + getPariticipantsAnswer.UserID);
                response.Status = true;
                response.Message = "Participants Answer Fetched Successfully";
                response.Data = GetPage(ans, pg);
            }
            return await Task.FromResult(response);

        }

        public async Task<object> GetParticipantsAnswer(GetParticipantsAnswerByID getParticipantsAnswerByID)
        {
            int roomID = getParticipantsAnswerByID.RoomID;
            int UserID = getParticipantsAnswerByID.UserID;
            int participantID = getParticipantsAnswerByID.ParticipantID;
            var response = new ResponseModel();

            var status = await _roomRepository.isRoomSetter(roomID, UserID);
            if (!status)
            {
                _logger.Log(LogLevel.Warning, "Room unauthorized access " + roomID + " :" + UserID);
                response.Status = false;
                response.Message = "You are not authorized to view participants answer";
                return await Task.FromResult(response);
            }
            var ans = await _roomRepository.GetParticipantsAnswerByID(participantID, roomID);
            if (ans == null)
            {
                _logger.Log(LogLevel.Warning, "Room Participants Answer Not Found " + roomID + " :" + UserID);
                response.Status = false;
                response.Message = "No Participants Found";
                return await Task.FromResult(response);
            }
            _logger.Log(LogLevel.Information, "Room Participants Answer Fetched " + roomID + " :" + UserID);
            response.Status = true;
            response.Message = "Participant answer Fetched Successfully";
            response.Data = ans;

            return await Task.FromResult(response);
        }
}