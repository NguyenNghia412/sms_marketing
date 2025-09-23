using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace thongbao.be.application.DanhBa.Dtos
{
    public  class GetRefreshTokenDto
    {
        public string RefreshToken { get; set; }= String.Empty;
        public string AccessToken { get; set; } = String.Empty;
        public DateTime ExpiresAt { get; set; }
        public string TokenType { get; set; } = String.Empty;
    }
}
