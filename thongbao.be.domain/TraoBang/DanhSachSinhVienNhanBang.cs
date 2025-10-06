using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using thongbao.be.shared.Constants.Db;
using thongbao.be.shared.Constants.TraoBang;
using thongbao.be.shared.Interfaces;

namespace thongbao.be.domain.TraoBang
{
    [Table(nameof(DanhSachSinhVienNhanBang), Schema = DbSchemas.TraoBang)]
    [Index(
      nameof(Id),
      IsUnique = false,
      Name = $"IX_{nameof(DanhSachSinhVienNhanBang)}"
  )]
    public  class DanhSachSinhVienNhanBang: ISoftDelted
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int IdSubPlan { get; set; }
        public string HoVaTen { get; set; } = String.Empty;
        public string Email { get; set; } = String.Empty ;
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
        public int Order { get; set; }
        public int TrangThai { get; set; } 
        public string LinkQR { get; set; } = String.Empty;
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? DeletedDate { get; set; }
        public bool Deleted { get; set; }
        public int? DeletedBy { get; set; }
    }
}
