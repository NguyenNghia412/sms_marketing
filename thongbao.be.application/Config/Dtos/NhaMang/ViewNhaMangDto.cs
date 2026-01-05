using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace thongbao.be.application.Config.Dtos.NhaMang
{
    public class ViewNhaMangDto
    {
        public int Id { get; set; }
        public string TenNhaMang { get; set; } = string.Empty;
        public string Prefix { get; set; } = string.Empty;
        public DonGiaDto? DonGia { get; set; }
        public BrandNameDto BrandName { get; set; } = new BrandNameDto();
    }
    public class DonGiaDto
    {
        public int Id { get; set; }
        //public int IdBrandName { get; set; }
        //public int IdNhaMang { get; set; }
        public int DonGia { get; set; }
        public DateTime? ThoiHan { get; set; }
    }

    public class BrandNameDto
    {
        public int Id { get; set; }
        public string TenBrandName { get; set; } = String.Empty;
        //public string? Mota { get; set; } = String.Empty;
    }
}
