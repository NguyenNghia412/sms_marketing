using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace thongbao.be.application.DiemDanh.Dtos
{
    public class PKCEDto
    {
        public string CodeVerifier { get; set; } = String.Empty;
        public string CodeChallenge { get; set; } = String.Empty;
        public string State { get; set; } = String.Empty;
    }
}
