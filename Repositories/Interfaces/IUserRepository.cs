using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using QuickQuiz.Models;

namespace QuickQuiz.Repositories.Interfaces
{
    public interface IUserRepository
    {
        public bool Authenticate(LoginRequestModel loginRequest);
        public bool Register(RegistrationRequest request); 
        public string HashPassword(string password);
    }
}