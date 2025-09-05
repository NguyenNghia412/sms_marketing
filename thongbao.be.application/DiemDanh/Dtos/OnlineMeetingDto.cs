using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace thongbao.be.application.DiemDanh.Dtos
{
    public class OnlineMeetingDto
    {
        public string Id { get; set; } = string.Empty;
        public string? Subject { get; set; }
        public DateTime? CreationDateTime { get; set; }
        public DateTime? StartDateTime { get; set; }
        public DateTime? EndDateTime { get; set; }
        public string JoinWebUrl { get; set; } = string.Empty;
        public ChatInforDto? ChatInfo { get; set; }
        public MeetingParticipantsDto? Participants { get; set; }
        //[JsonPropertyName("joinMeetingIdSettings")]
        //public JoinMeetingIdSettings? JoinMeetingIdSettings { get; set; }
    }
}
