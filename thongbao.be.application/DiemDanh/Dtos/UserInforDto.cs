using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace thongbao.be.application.DiemDanh.Dtos
{
    public class UserInforDto
    {
        public string Id { get; set; } = string.Empty;
        public string? DisplayName { get; set; }
        //public DateTime? JoinIn { get; set; }
        //public DateTime? JoinOut { get; set; }
        //[JsonPropertyName("tenantId")]
        //public string? TenantId { get; set; }
        public string? SurName { get; set; } = string.Empty;
        public string? GivenName { get; set; } = string.Empty;
        public string? MobilePhone { get; set; } = string.Empty;
        public string? JobTitle { get; set; } = string.Empty;
        public string? OfficeLocation { get; set; } = string.Empty;
        //public DateTime? Birthday { get; set; }

        public List<ChatMessageDto> ChatMessages { get; set; } = new();
        public AttendanceDetailDto AttendanceDetail { get; set; } = new AttendanceDetailDto();
    }
}
