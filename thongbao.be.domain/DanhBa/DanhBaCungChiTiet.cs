using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using thongbao.be.domain.DiemDanh;
using thongbao.be.shared.Constants.DanhBa;
using thongbao.be.shared.Constants.Db;
using thongbao.be.shared.Interfaces;

namespace thongbao.be.domain.DanhBa
{
    [Table(nameof(DanhBaCungChiTiet), Schema = DbSchemas.Core)]
    [Index(
    nameof(Id),
    IsUnique = false,
    Name = $"IX_{nameof(DanhBaCungChiTiet)}"
    )]
    public class DanhBaCungChiTiet: ISoftDelted
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string HoVaTen { get; set; } = string.Empty;
        public string HoDem { get; set; } = string.Empty;
        public string Ten { get; set; } = string.Empty;
        public string SoDienThoai { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime? NgaySinh { get; set; }
        public GioiTinhEnum? GioiTinh { get; set; } = GioiTinhEnum.ChuaXacDinh;
        public string? DiaChi { get; set; } 
        public int LaNguoiDung { get; set; }
        public string MaSoNguoiDung { get; set; } = string.Empty;
        //public string Khoa { get; set; } = string.Empty;
        //public string MaSoKhoa { get; set; } = string.Empty;
        //public string? Lop { get; set; } 
        // public string? KhoaSinhVien { get; set; }
        public TrangThaiHoatDongEnum? TrangThaiHoatDong { get; set; } = TrangThaiHoatDongEnum.DangHoatDong;
        public string? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? DeletedDate { get; set; }
        public bool Deleted { get; set; }
        public string? DeletedBy { get; set; }

    }
}
