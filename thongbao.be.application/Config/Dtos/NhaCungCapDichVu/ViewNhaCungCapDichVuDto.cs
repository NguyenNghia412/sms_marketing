using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace thongbao.be.application.Config.Dtos.NhaCungCapDichVu
{
    public class ViewNhaCungCapDichVuDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = String.Empty;
        public string ApiKey { get; set; } = String.Empty;
        public string ApiSecret { get; set; } = String.Empty;
        public string BaseUrl { get; set; } = String.Empty;
        public bool IsConfigAuthReq { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public DateTime? DeletedDate { get; set; }
        public bool Deleted { get; set; }
        public string? DeletedBy { get; set; }
        public List<BrandName> BrandNames { get; set; } = new List<BrandName>();
    }

    public class BrandName
    {
        public int Id { get; set; }
        public string TenBrandName { get; set; } = String.Empty;
    }
}
