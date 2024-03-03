using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuickQuiz.Models.Room
{
    public class RoomModel
    {
        public int RoomID { get; set; }
        public string RoomName { get; set; }
        public int SetterID { get; set; }
        public List<int> Participants { get; set; }
        // public string StartDateTime { get; set; }
    }
}       