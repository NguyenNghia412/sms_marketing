using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace thongbao.be.application.DiemDanh.Dtos
{
    public class AttendanceDetailDto
    {
        public List<ParticipantActivityDto> AttendanceIntervals { get; set; } = new List<ParticipantActivityDto>();
        public int TotalAttendanceInSeconds { get; set; }  
        public TimeSpan TotalAttendanceTime => TimeSpan.FromSeconds(TotalAttendanceInSeconds);
        public int TotalJoinCount => AttendanceIntervals.Count; 
        public string EmailAddress { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public DateTime? FirstJoinTime { get; set; }
        public DateTime? LastLeaveTime { get; set; }
    }
}
