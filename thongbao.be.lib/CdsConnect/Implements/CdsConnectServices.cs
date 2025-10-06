using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using thongbao.be.lib.CdsConnect.Dtos.SvValidate;
using thongbao.be.lib.CdsConnect.Interfaces;
using thongbao.be.shared.Settings;

namespace thongbao.be.lib.CdsConnect.Implements
{
    public class CdsConnectServices : ICdsConnectServices
    {
        private readonly CdsConnectSettings _config;
        public CdsConnectServices(IOptions<CdsConnectSettings> options)
        {
            _config = options.Value;
        }

        //public ViewSvValidateDto ValidateSv(reque)
    }
}
