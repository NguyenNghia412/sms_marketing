using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace thongbao.be.application.DiemDanh.Dtos
{
    public class GraphApiCallbackDto
    {
        public string Code { get; set; } = String.Empty;
        public string State { get; set; } = String.Empty;
        public string? Error { get; set; }
        public string? ErrorDescription { get; set; }
        public string CodeVerifier { get; set; } = String.Empty;
    }
}
