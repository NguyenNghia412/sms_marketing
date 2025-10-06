using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using thongbao.be.lib.CdsConnect.Dtos.Base;

namespace thongbao.be.lib.CdsConnect.Dtos.SvValidate
{
    public class ResponseSvValidateDto : BaseResponseCdsConnect<ViewSvValidateDto>
    {
    }

    public class ViewSvValidateDto
    {
        [JsonPropertyName("maSinhVien")]
        public string? MaSinhVien { get; set; }

        [JsonPropertyName("hoDem")]
        public string? HoDem { get; set; }

        [JsonPropertyName("ten")]
        public string? Ten { get; set; }

        [JsonPropertyName("gioiTinh")]
        public bool? GioiTinh { get; set; }

        [JsonPropertyName("ngaySinh")]
        public DateTime? NgaySinh { get; set; }

        [JsonPropertyName("ngaySinh2")]
        public string? NgaySinh2 { get; set; }

        [JsonPropertyName("noiSinh")]
        public string? NoiSinh { get; set; }

        [JsonPropertyName("soDienThoai")]
        public string? SoDienThoai { get; set; }

        [JsonPropertyName("email")]
        public string? Email { get; set; }

        [JsonPropertyName("trangThai")]
        public int? TrangThai { get; set; }
    }
}
