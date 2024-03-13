using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
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
        public RoomService(IRoomRepository roomRepository)
        {
            _roomRepository = roomRepository;
        }

        public async Task<object> RoomCreation(RoomModel roomModel)
        {
            var response = new ResponseModel();
            var room = await _roomRepository.isSetter(roomModel.RoomID);
            if (!room)
            {
                response.Status = false;
                response.Message = "You are not authorized to create room";
                return await Task.FromResult(response);
            }
            if (await _roomRepository.createRoom(roomModel))
            {
                response.Status = true;
                response.Message = "Room Created Successfully";
                return await Task.FromResult(response);
            }
            response.Status = false;
            response.Message = "Room Creation Failed";
            return await Task.FromResult(response);
        }

        public async Task<object> GetParticipants(GetParticipantsByRoom getParticipantsByRoom)
        {
            var response = new ResponseModel();
            var room = await _roomRepository.isRoomAuthorized(
                getParticipantsByRoom.RoomID,
                getParticipantsByRoom.SetterID
                );
            if (!room)
            {
                response.Status = false;
                response.Message = "You are not authorized to view participants";
                return await Task.FromResult(response);
            }
            if (await _roomRepository.RoomParticipants(getParticipantsByRoom.RoomID) != null)
            {
                response.Status = true;
                response.Message = "Participants Fetched Successfully";
                response.Data = await _roomRepository.RoomParticipants(getParticipantsByRoom.RoomID);
                return await Task.FromResult(response);
            }
            response.Status = false;
            response.Message = "No Participants Found";
            return await Task.FromResult(response);
            // return await _roomRepository.RoomParticipants(roomID);
        }

        public async Task<object> GetQuestions(GetQuestionsByRoom getQuestionsByRoom)
        {
            int UserID = getQuestionsByRoom.UserID;
            int RoomID = getQuestionsByRoom.RoomID;
            var response = new ResponseModel();

            var status = await _roomRepository.isSetter(UserID);
            if (!status)
            {
                response.Status = false;
                response.Message = "You are not authorized to view questions";
                return await Task.FromResult(response);
            }
            var questions = await _roomRepository.GetQuetions(RoomID);
            if (questions.Count > 0)
            {
                response.Status = true;
                response.Message = "Questions Fetched Successfully";
                response.Data = questions;
                return await Task.FromResult(response);
            }

            response.Status = false;
            response.Message = "No Questions Found";
            return await Task.FromResult(response);
        }


        public async Task<object> GetParticipantInfoByID(GetParticipantsInfoByID getParticipantsInfoByID)
        {
            int roomID = getParticipantsInfoByID.RoomID;
            int participantID = getParticipantsInfoByID.ParticipantID;
            int SetterID = getParticipantsInfoByID.SetterID;
            var response = new ResponseModel();

            var status = await _roomRepository.isRoomSetter(roomID, SetterID);

            if (!status)
            {
                response.Status = false;
                response.Message = "You are not authorized to view participants";
                return await Task.FromResult(response);
            }

            var ans = await _roomRepository.GetParticipantInfoByID(roomID, participantID);
            if (ans == null)
            {
                response.Status = false;
                response.Message = "No Participants Found";
                return await Task.FromResult(response);
            }
            response.Status = true;
            response.Message = "Participants Fetched Successfully";
            response.Data = ans;
            return await Task.FromResult(response);
        }

        public async Task<object> RoomParticipantDelete(DeleteParticipantsByID deleteParticipantsByID)
        {
            int roomID = deleteParticipantsByID.RoomID;
            int ParticiapntsID = deleteParticipantsByID.ParticipantID;
            int SetterID = deleteParticipantsByID.SetterID;
            var response = new ResponseModel();

            var status = await _roomRepository.isRoomSetter(roomID, SetterID);
            if (!status)
            {
                response.Status = false;
                response.Message = "You are not authorized to delete participants";
                return await Task.FromResult(response);
            }

            if (await _roomRepository.RoomParticipantDelete(roomID, ParticiapntsID))
            {
                response.Status = true;
                response.Message = "Participants Deleted Successfully";
                return await Task.FromResult(response);
            }
            response.Status = false;
            response.Message = "Participants Deletion Failed";
            return await Task.FromResult(response);

        }

        public async Task<List<GetParticipantsAnswerByIDModel>> GetParticipantsAnswerByRoom(int roomID)
        {
            var ans = await _roomRepository.GetParticipantsAnswerByRoom(roomID);
            return ans;
        }

        public async Task<object> GetParticipantsAnswer(GetParticipantsAnswerByID getParticipantsAnswerByID)
        {
            int roomID = getParticipantsAnswerByID.RoomID;
            int SetterID = getParticipantsAnswerByID.SetterID;
            int participantID = getParticipantsAnswerByID.ParticipantID;
            var response = new ResponseModel();

            var status = await _roomRepository.isRoomSetter(roomID, SetterID);
            if (!status)
            {
                response.Status = false;
                response.Message = "You are not authorized to view participants answer";
                return await Task.FromResult(response);
            }
            var ans = await _roomRepository.GetParticipantsAnswerByID(participantID, roomID);
            if (ans == null)
            {
                response.Status = false;
                response.Message = "No Participants Found";
                return await Task.FromResult(response);
            }
            response.Status = true;
            response.Message = "Participant answer Fetched Successfully";
            response.Data = ans;

            return await Task.FromResult(response);
        }

        public async Task<object> GetRoomResult(GetRoomResult getRoomResult)
        {
            int roomID = getRoomResult.RoomID;
            int UserID = getRoomResult.UserID;
            var response = new ResponseModel();

            var status = await _roomRepository.isRoomSetter(roomID, UserID);
            if (status)
            {
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
                    response.Status = false;
                    response.Message = "Cannot Get Result When Room is active";
                    return await Task.FromResult(response);
                }
                var status2 = await _roomRepository.isMember(roomID, UserID);
                if (!status2)
                {
                    response.Status = false;
                    response.Message = "You are not authorized to view room result";
                    return await Task.FromResult(response);
                }

                response.Status = true;
                response.Message = "Room Result Fetched Successfully";
                response.Data = await _roomRepository.GetRoomResult(roomID);
                return await Task.FromResult(response);
            }
        }

        public async Task<object> AllParticipants()
        {
            var response = new ResponseModel();
            var ans = await _roomRepository.AllParticipants();
            if (ans.Count > 0)
            {
                response.Status = true;
                response.Message = "Participants Fetched Successfully";
                response.Data = ans;
            }
            else
            {
                response.Status = false;
                response.Message = "No Participants Found";
            }
            return await Task.FromResult(response);
        }

        public async Task<object> AddParticipants(AddParticipants addParticipants)
        {
            int roomID = addParticipants.RoomID;
            int SetterID = addParticipants.SetterID;
            var response = new ResponseModel();
            var status = await _roomRepository.isRoomSetter(roomID, SetterID);

            if (!status)
            {
                response.Status = false;
                response.Message = "You are not authorized to add participants";
                return await Task.FromResult(response);
            }
            response.Status = true;
            response.Message = "Participants Added Successfully";
            response.Data = await _roomRepository.AddParticipants(addParticipants);
            return await Task.FromResult(response);
        }

        public async Task<object> RoomList(GetRoomListRequest getRoomListRequest)
        {
            int setterID = getRoomListRequest.SetterID;
            var response = new ResponseModel();
            var status = await _roomRepository.isSetter(setterID);
            if (!status)
            {
                response.Status = false;
                response.Message = "You are not authorized to view rooms";
                return await Task.FromResult(response);
            }
            var ans = await _roomRepository.RoomList(getRoomListRequest);
            if (ans.Count > 0)
            {
                response.Status = true;
                response.Message = "Rooms Fetched Successfully";
                response.Data = ans;
            }
            else
            {
                response.Status = false;
                response.Message = "No Rooms Found";
            }
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
                response.Status = false;
                response.Message = "You are not authorized to add questions";
                return await Task.FromResult(response);
            }
            response.Status = true;
            response.Message = "Questions Added Successfully";
            response.Data = await _roomRepository.AddQuestions(addQuestion);
            return await Task.FromResult(response);

        }


        public async Task<object> StartQuiz(RoomStatus roomStatus)
        {
            int roomID = roomStatus.RoomID;
            int SetterID = roomStatus.SetterID;
            var response = new ResponseModel();
            var status = await _roomRepository.isRoomSetter(roomID, SetterID);
            if (!status)
            {
                response.Status = false;
                response.Message = "You are not authorized to start quiz";
                return await Task.FromResult(response);
            }
            var status2 = await _roomRepository.isRoomActive(roomID);
            if (status2)
            {
                response.Status = false;
                response.Message = "Quiz is already active";
                return await Task.FromResult(response);
            }
            var status3 = await _roomRepository.StartQuiz(roomID);
            if (status3)
            {
                response.Status = true;
                response.Message = "Quiz Started Successfully";
            }
            return await Task.FromResult(response);

        }
        public async Task<object> StopQuiz(RoomStatus roomStatus)
        {
            int roomID = roomStatus.RoomID;
            int SetterID = roomStatus.SetterID;
            var response = new ResponseModel();
            var status = await _roomRepository.isRoomSetter(roomID, SetterID);
            if (!status)
            {
                response.Status = false;
                response.Message = "You are not authorized to start quiz";
                return await Task.FromResult(response);
            }
            var status2 = await _roomRepository.isRoomActive(roomID);
            if (!status2)
            {
                response.Status = false;
                response.Message = "Quiz is not active";
                return await Task.FromResult(response);
            }
            var status3 = await _roomRepository.StopQuiz(roomID);
            if (status3)
            {
                response.Status = true;
                response.Message = "Quiz Stopped Successfully";
            }
            return await Task.FromResult(response);
        }
        public async Task<object> PauseQuiz(RoomStatus roomStatus)
        {
            int roomID = roomStatus.RoomID;
            int SetterID = roomStatus.SetterID;
            var response = new ResponseModel();
            var status = await _roomRepository.isRoomSetter(roomID, SetterID);
            if (!status)
            {
                response.Status = false;
                response.Message = "You are not authorized to start quiz";
                return await Task.FromResult(response);
            }
            var status2 = await _roomRepository.isRoomActive(roomID);
            if (!status2)
            {
                response.Status = false;
                response.Message = "Quiz is not active";
                return await Task.FromResult(response);
            }
            var status3 = await _roomRepository.PauseQuiz(roomID);
            if (status3)
            {
                response.Status = true;
                response.Message = "Quiz Paused Successfully";
            }
            return await Task.FromResult(response);
        }



        public async Task<object> RoomDelete(DeleteRoomByRoom deleteRoomByRoom)
        {
            int roomID = deleteRoomByRoom.RoomID;
            int SetterID = deleteRoomByRoom.SetterID;

            var response = new ResponseModel();
            var status = await _roomRepository.isRoomSetter(roomID, SetterID);
            if (!status)
            {
                response.Status = false;
                response.Message = "You are not authorized to delete room";
                return await Task.FromResult(response);
            }
            if (await _roomRepository.RoomDelete(roomID))
            {
                response.Status = true;
                response.Message = "Room Deleted Successfully";
                return await Task.FromResult(response);
            }
            response.Status = false;
            response.Message = "Room Deletion Failed";
            return await Task.FromResult(response);

        }

        public async Task<object> RoomUpdate(RoomUpdateModel roomModel)
        {
            int roomID = roomModel.RoomID;
            int SetterID = roomModel.SetterID;
            var response = new ResponseModel();
            var status = await _roomRepository.isRoomSetter(roomID, SetterID);
            if (!status)
            {
                response.Status = false;
                response.Message = "You are not authorized to update room";
                return await Task.FromResult(response);
            }
            if (await _roomRepository.RoomUpdate(roomModel))
            {
                response.Status = true;
                response.Message = "Room Updated Successfully";
                return await Task.FromResult(response);
            }
            response.Status = false;
            response.Message = "Room Updation Failed";
            return await Task.FromResult(response);
        }


        public async Task<object> QuestionDelete(DeleteQuestion deleteQuestion)
        {
            int questionID = deleteQuestion.QuestionID;
            var response = new ResponseModel();
            if (await _roomRepository.QuestionDelete(questionID))
            {
                response.Status = true;
                response.Message = "Question Deleted Successfully";
                return await Task.FromResult(response);
            }
            response.Status = false;
            response.Message = "Question Deletion Failed";
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
                response.Status = false;
                response.Message = "You are not authorized to update question";
                return await Task.FromResult(response);
            }
            var status2 = await _roomRepository.QuestionUpdate(questionModel);
            if (status2)
            {
                response.Status = true;
                response.Message = "Question Updated Successfully";
                return await Task.FromResult(response);
            }
            response.Status = false;
            response.Message = "Question Updation Failed";
            return await Task.FromResult(response);

        }

    }
}