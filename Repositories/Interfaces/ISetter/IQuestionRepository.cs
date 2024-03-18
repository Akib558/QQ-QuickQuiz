using QuickQuiz.Models.RequestModels;

namespace QuickQuiz.Repositories.Interfaces.ISetter;

public interface IQuestionRepository
{
    Task<List<GetQuestionModel>> GetQuetions(int roomID);
    Task<int> AddQuestions(AddQuestion addQuestion);
    Task<bool> QuestionDelete(int questionID);
    Task<bool> QuestionUpdate(UpdateQuestionModel questionModel);
    Task<bool> DeleteOption(int questionID, int optionId);
    Task<bool> isRoomAuthorized(int roomID, int userID);
    Task<bool> isSetter(int userID);
    Task<bool> isRoomSetter(int roomID, int setterID);
    // Task<bool> roomStatus(int roomID);
    Task<bool> isRoomActive(int roomID);
    Task<bool> isMember(int roomID, int userID);
}