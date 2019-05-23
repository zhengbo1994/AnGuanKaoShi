using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Model
{
    public class Biz_ExamRoomNVR
    {
        [Key]
        public int Id { get; set; }
        public string IPAddress { get; set; }
        public int Protocol { get; set; }
        public int Port { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public int ExamRoomId { get; set; }
    }
}
