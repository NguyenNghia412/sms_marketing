using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace thongbao.be.application.DiemDanh.Dtos
{
    public class GraphApiUserInforResponseDto
    {
        public string Id { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string GivenName { get; set; } = string.Empty;
        public string Surname { get; set; } = string.Empty;
        public string Mail { get; set; } = string.Empty;
        public string UserPrincipalName { get; set; } = string.Empty;
        public string JobTitle { get; set; } = string.Empty;
        public string OfficeLocation { get; set; } = string.Empty;
        public string MobilePhone { get; set; } = string.Empty;


    }
}
