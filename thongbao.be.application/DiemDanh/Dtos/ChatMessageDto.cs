using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace thongbao.be.application.DiemDanh.Dtos
{
    public class ChatMessageDto
    {
        public string Id { get; set; } = string.Empty;
        public string? MessageType { get; set; }
        public DateTime? CreatedDateTime { get; set; }
        public string? Subject { get; set; }
        public ChatMessageBodyDto? Body { get; set; }
        //public ChatMessageFromDto? From { get; set; }
    }
}
