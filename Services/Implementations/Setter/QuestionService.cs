﻿using QuickQuiz.Models.RequestModels;
using QuickQuiz.Models.Response;
using QuickQuiz.Repositories.Interfaces.ISetter;
using QuickQuiz.Services.Interfaces.ISetter;

namespace QuickQuiz.Services.Implementations.Setter;

public class QuestionService : IQuestionService
{
    private IQuestionRepository _roomRepository;
    private ICustomLogger _logger;
    public QuestionService(IQuestionRepository roomRepository, ICustomLogger customLogger)
    {
        _roomRepository = roomRepository;
        _logger = customLogger;
    }
        
    public List<T> GetPage<T>(List<T> list, int page, int pageSize = 5)
    {
        return list.Skip((page - 1) * pageSize).Take(pageSize).ToList();
    }
    
    public async Task<object> GetQuestions(GetQuestionsByRoom getQuestionsByRoom, int pg)
    {
        int UserID = getQuestionsByRoom.UserID;
        int RoomID = getQuestionsByRoom.RoomID;
        var response = new ResponseModel();

        var status = await _roomRepository.isSetter(UserID);
        if (!status)
        {
            _logger.Log(LogLevel.Warning, "Room unauthorized access " + RoomID + " :" + UserID);
            response.Status = false;
            response.Message = "You are not authorized to view questions";
            return await Task.FromResult(response);
        }
        var questions = await _roomRepository.GetQuetions(RoomID);
        if (questions.Count > 0)
        {
            _logger.Log(LogLevel.Information, "Room Questions Fetched " + RoomID + " :" + UserID);
            response.Status = true;
            response.Message = "Questions Fetched Successfully";
            response.Data = GetPage(questions, pg);
            return await Task.FromResult(response);
        }
        _logger.Log(LogLevel.Warning, "Room Questions Not Found " + RoomID + " :" + UserID);
        response.Status = false;
        response.Message = "No Questions Found";
        return await Task.FromResult(response);
    }
    
    public async Task<object> AddQuestions(AddQuestion addQuestion)
    {
        int roomID = addQuestion.RoomID;
        int SetterID = addQuestion.SetterID;
        var response = new ResponseModel();
        var status = await _roomRepository.isRoomSetter(roomID, SetterID);
        if (!status)
        {
            _logger.Log(LogLevel.Warning, "Room unauthorized access " + roomID + " :" + SetterID);
            response.Status = false;
            response.Message = "You are not authorized to add questions";
            return await Task.FromResult(response);
        }
        _logger.Log(LogLevel.Information, "Questions Added " + roomID + " :" + SetterID);
        response.Status = true;
        response.Message = "Questions Added Successfully";
        response.Data = await _roomRepository.AddQuestions(addQuestion);
        return await Task.FromResult(response);

    }
    
    public async Task<object> QuestionDelete(DeleteQuestion deleteQuestion)
    {
        int roomID = deleteQuestion.RoomID;
        int SetterID = deleteQuestion.SetterID;
        int questionID = deleteQuestion.QuestionID;
        var response = new ResponseModel();
        var status = await _roomRepository.isRoomSetter(roomID, SetterID);
        if (!status)
        {
            _logger.Log(LogLevel.Warning, "Room unauthorized access " + roomID + " :" + SetterID);
            response.Status = false;
            response.Message = "You are not authorized to delete question";
            return await Task.FromResult(response);
        }
            
        if (await _roomRepository.QuestionDelete(questionID))
        {
            _logger.Log(LogLevel.Information, "Question Deleted " + questionID);
            response.Status = true;
            response.Message = "Question Deleted Successfully";
            return await Task.FromResult(response);
        }
        _logger.Log(LogLevel.Warning, "Question Deletion Failed " + questionID);
        response.Status = false;
        response.Message = "Question Deletion Failed";
        return await Task.FromResult(response);
    }
    
    public async Task<object> DeleteOption(DeleteOption deleteOption)
    {
        int roomID = deleteOption.RoomID;
        int SetterID = deleteOption.SetterID;
        int optionID = deleteOption.OptionID;
        int questionID = deleteOption.QuestionID;
        var response = new ResponseModel();
        var status = await _roomRepository.isRoomSetter(roomID, SetterID);
        if (!status)
        {
            _logger.Log(LogLevel.Warning, "Room unauthorized access " + roomID + " :" + SetterID);
            response.Status = false;
            response.Message = "You are not authorized to delete option";
            return await Task.FromResult(response);
        }
        if (await _roomRepository.DeleteOption(questionID, optionID))
        {
            _logger.Log(LogLevel.Information, "Option Deleted " + optionID);
            response.Status = true;
            response.Message = "Option Deleted Successfully";
            return await Task.FromResult(response);
        }
        _logger.Log(LogLevel.Warning, "Option Deletion Failed " + optionID);
        response.Status = false;
        response.Message = "Option Deletion Failed";
        return await Task.FromResult(response);
            
    }
    
    
    public async Task<object> QuestionUpdate(UpdateQuestionModel questionModel)
    {
        int roomID = questionModel.RoomID;
        int SetterID = questionModel.SetterID;
        var response = new ResponseModel();
        var status = await _roomRepository.isRoomSetter(roomID, SetterID);
        if (!status)
        {
            _logger.Log(LogLevel.Warning, "Room unauthorized access " + roomID + " :" + SetterID);
            response.Status = false;
            response.Message = "You are not authorized to update question";
            return await Task.FromResult(response);
        }
        var status2 = await _roomRepository.QuestionUpdate(questionModel);
        if (status2)
        {
            _logger.Log(LogLevel.Information, "Question Updated " + roomID + " :" + SetterID);
            response.Status = true;
            response.Message = "Question Updated Successfully";
            return await Task.FromResult(response);
        }
        _logger.Log(LogLevel.Warning, "Question Updation Failed " + roomID + " :" + SetterID);
        response.Status = false;
        response.Message = "Question Updation Failed";
        return await Task.FromResult(response);

    }
}