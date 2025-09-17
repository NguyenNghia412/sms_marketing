using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace thongbao.be.application.DanhBa.Dtos
{
    public  class ViewDanhBaChiTietDto
    {
        public int Id { get; set; }
        public int IdDanhBa { get; set; }
        public string HoVaTen { get; set; } = String.Empty;
        public string MaSoNguoiDung { get; set; } = String.Empty;
        public string SoDienThoai { get; set; } = String.Empty;
        public string EmailHuce { get; set; } = string.Empty;
        //public string Khoa { get; set; } = string.Empty;
        //public string MaSoKhoa { get; set; } = string.Empty;
        //public int LaNguoiDung { get; set; }
        //public string? Lop { get; set; }
        //public string? KhoaSinhVien { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? DeletedDate { get; set; }
        public bool Deleted { get; set; }
        public int? DeletedBy { get; set; }
    }
}
