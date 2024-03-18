using QuickQuiz.Models.RequestModels;

namespace QuickQuiz.Services.Interfaces.ISetter;

public interface IQuestionService
{
    public Task<object> QuestionDelete(DeleteQuestion deleteQuestion);
    public Task<object> QuestionUpdate(UpdateQuestionModel questionModel);
    public Task<object> DeleteOption(DeleteOption deleteOption);
    public Task<object> AddQuestions(AddQuestion addQuestion);
    public Task<object> GetQuestions(GetQuestionsByRoom getQuestionsByRoom, int pg);
}