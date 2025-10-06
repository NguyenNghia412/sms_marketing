using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace thongbao.be.application.TraoBang.Dtos
{
    public class ViewSinhVienNhanBangDto
    {
        public int Id { get; set; }
        public int IdSubPlan { get; set; }
        public string TenSubPlan { get; set; } = String.Empty;
        public string HoVaTen { get; set; } = String.Empty;
        public string Email { get; set; } = String.Empty;
        public string EmailSinhVien { get; set; } = String.Empty;
        public string MaSoSinhVien { get; set; } = String.Empty;
        public string Lop { get; set; } = String.Empty;
        public DateTime NgaySinh { get; set; }
        public string CapBang { get; set; } = String.Empty;
        public string TenNganhDaoTao { get; set; } = String.Empty;
        public string XepHang { get; set; } = String.Empty;
        public string ThanhTich { get; set; } = String.Empty;
        public string KhoaQuanLy { get; set; } = String.Empty;

        public string SoQuyetDinhTotNghiep { get; set; } = String.Empty;
        public DateTime NgayQuyetDinh { get; set; }
        public string? Note { get; set; }
        public bool IsShow { get; set; }
        public string Order { get; set; } = String.Empty;
        public int TrangThai { get; set; }
        public string LinkQR { get; set; } = String.Empty;
    }
}
