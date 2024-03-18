using QuickQuiz.Models.RequestModels;

namespace QuickQuiz.Services.Interfaces.ISetter;

public interface IParticipantService
{
    public Task<object> AllParticipants(int pg);
    public Task<object> AddParticipants(AddParticipants addParticipants);
    public Task<object> GetParticipants(GetParticipantsByRoom getParticipantsByRoom, int pg);
    public Task<object> GetParticipantsAnswerByRoom(GetPariticipantsAnswer getPariticipantsAnswer, int pg);
    public Task<object> GetParticipantsAnswer(GetParticipantsAnswerByID getParticipantsAnswerByID);
    public Task<object> GetParticipantInfoByID(GetParticipantsInfoByID getParticipantsInfoByID);
    public Task<object> RoomParticipantDelete(DeleteParticipantsByID deleteParticipantsByID);
    
}