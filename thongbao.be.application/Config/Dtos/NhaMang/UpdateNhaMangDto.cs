using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace thongbao.be.application.Config.Dtos.NhaMang
{
    public class UpdateNhaMangDto
    {
        public int Id { get; set; }
        public int IdBrandName { get; set; }
        public string TenNhaMang { get; set; } = string.Empty;
        public string Prefix { get; set; } = string.Empty;
        public int DonGia { get; set; }
        public DateTime? ThoiHan { get; set; }
    }
}
