using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace thongbao.be.lib.Stringee.Interfaces
{
    public interface IAuthService
    {
        public  Task<string> GenerateJwtTokenAsync(int expirationInMinutes = 60);
        public  Task<string> GenerateAccountJwtTokenAsync(int expirationInMinutes = 60);
    }
}
