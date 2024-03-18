using QuickQuiz.Models;
using QuickQuiz.Models.RequestModels;
using QuickQuiz.Models.Room;

namespace QuickQuiz.Repositories.Interfaces.ISetter;

public interface IParticipantRepository
{
    Task<List<int>> RoomParticipants(int roomID);

    Task<List<ParticipantsModel>> AllParticipants();

    Task<List<GetParticipantsAnswerByIDModel>> GetParticipantsAnswerByRoom(int roomID);
    Task<GetParticipantsAnswerByIDModel> GetParticipantsAnswerByID(int participantID, int roomID);
    Task<Participant> GetParticipantInfoByID(int roomID, int participantID);
    Task<bool> RoomParticipantDelete(int roomID, int ParticiapntsID);
    Task<int> AddParticipants(AddParticipants addParticipants);
    Task<bool> isRoomAuthorized(int roomID, int userID);
    Task<bool> isSetter(int userID);
    Task<bool> isRoomSetter(int roomID, int setterID);
    // Task<bool> roomStatus(int roomID);
    Task<bool> isRoomActive(int roomID);
    Task<bool> isMember(int roomID, int userID);
}