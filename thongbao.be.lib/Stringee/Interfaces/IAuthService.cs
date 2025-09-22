using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace thongbao.be.lib.Stringee.Interfaces
{
    public interface IAuthService
    {
        Task<string> GenerateJwtTokenAsync(int expirationInMinutes = 60);
    }
}
