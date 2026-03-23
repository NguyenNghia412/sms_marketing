using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace thongbao.be.application.Config.Dtos.NhaCungCapDichVu
{
    public class ViewNhaCungCapByIdDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = String.Empty;
        public string ApiKey { get; set; } = String.Empty;
        public string ApiSecret { get; set; } = String.Empty;
        public string BaseUrl { get; set; } = String.Empty;
        public bool IsConfigAuthReq { get; set; }

        public List<BrandNameDto> BrandNames { get; set; } = new List<BrandNameDto>();
    }
    public class BrandNameDto
    {
        public int Id { get; set; }
        public string TenBrandName { get; set; } = String.Empty;
    }
}
