using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace thongbao.be.application.Config.Dtos.NhaCungCapDichVu
{
    public class CreateNhaCungCapDichVuDto
    {
        public string Name { get; set; } = String.Empty;
        public string ApiKey { get; set; } = String.Empty;
        public string ApiSecret { get; set; } = String.Empty;
        public string BaseUrl { get; set; } = String.Empty;
        public bool IsConfigAuthReq { get; set; }

        //public string TenBrandName { get; set; } = String.Empty;
        public List<CreateBrandNameDto>? BrandNames { get; set; } = new List<CreateBrandNameDto>();
    }
    public class CreateBrandNameDto
    {
        public string TenBrandName { get; set; } = String.Empty;
        public DateTime? ThoiGianBatDauHoatDong { get; set; }
        public DateTime? ThoiGianKetThucHoatDong { get; set; }
    }
}
