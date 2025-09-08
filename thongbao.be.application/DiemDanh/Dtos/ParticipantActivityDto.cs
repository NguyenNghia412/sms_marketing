
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace thongbao.be.application.DiemDanh.Dtos
{
    public class ParticipantActivityDto
    {
        public DateTime? JoinDateTime { get; set; }    
        public DateTime? LeaveDateTime { get; set; }  
        public int DurationInSeconds { get; set; }     
        public TimeSpan Duration => TimeSpan.FromSeconds(DurationInSeconds);
    }
}
