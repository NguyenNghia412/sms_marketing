using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace thongbao.be.application.TraoBang.Dtos
{
    public class GetSinhVienDangTraoBangInforDto
    {
        public string TenSubPlan { get; set; } = String.Empty;
        public int Id { get; set; }
        public string HoVaTen { get; set; } = String.Empty;
        public string MaSoSinhVien { get; set; } = String.Empty ;
        public string TenNganhDaoTao { get; set; } = String.Empty;
        public string XepHang { get; set; } = String.Empty;
        public string ThanhTich { get; set; } = String.Empty;
        public string CapBang { get; set; } = String.Empty;
        public string Note { get; set; } = String.Empty;
        public string? Text { get; set; }
    }
}
