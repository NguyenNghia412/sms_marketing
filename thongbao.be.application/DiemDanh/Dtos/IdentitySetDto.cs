using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace thongbao.be.application.DiemDanh.Dtos
{
    public class IdentitySetDto
    {
        public string? Upn { get; set; }
        public string? Role { get; set; }
        public IdentityDto? Identity { get; set; }

    }
}
