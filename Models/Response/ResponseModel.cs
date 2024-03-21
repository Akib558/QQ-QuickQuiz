using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuickQuiz.Models.Response
{
    public class ResponseModel
    {
        public bool Status { get; set; }
        public string Message { get; set; }
        public object pages { get; set; }
        public object Data { get; set; }
    }
}