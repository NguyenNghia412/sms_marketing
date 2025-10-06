using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using thongbao.be.lib.CdsConnect.Dtos.Base;

namespace thongbao.be.lib.CdsConnect.Dtos.SvValidate
{
    public class RequestSvValidateDto
    {
        [JsonPropertyName("code")]
        public string? Code { get; set; }

        [JsonPropertyName("password")]
        public string? Password { get; set; }
    }
}
