using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using thongbao.be.lib.CdsConnect.Dtos.Base;
using thongbao.be.lib.CdsConnect.Dtos.SvValidate;

namespace thongbao.be.lib.Stringee.Interfaces
{
    public interface IProfileService
    {
        public  Task<BaseResponseProfile?> GetProfileStringeeInfor();

        public  Task<ResponseGetExchangeApiDto?> GetExchangeRate();
    }
}
