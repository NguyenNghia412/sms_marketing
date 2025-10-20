using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace thongbao.be.application.GuiTinNhan.Dtos
{

    public class ChienDichCreatedByDto{
        public string Id { get; set; } = String.Empty;
        //public string UserName { get; set; } = String.Empty;
        public string FullName { get; set; } = String.Empty;
        //public string SoDienThoai { get; set; } = String.Empty;
        //public string Email { get; set; } = String.Empty;
    }
    public class ChienDichDanhBaDto
    {
        public int IdDanhBa { get; set; }
        public string TenDanhBa { get; set; } = String.Empty;
        public DateTime? CreatedDate { get; set; }
    }
    public class ViewChienDichDto
    {
        public int Id { get; set; }
        public string TenChienDich { get; set; } = String.Empty;
        public string MoTa { get; set; } = String.Empty;
        public string? NoiDung {  get; set; } = String.Empty;
        public DateTime? NgayBatDau { get; set; }
        public DateTime? NgayKetThuc { get; set; }
        public int? IdMauNoiDung { get; set; }
        public string? TenMauNoiDung { get; set; }
        public string? NoiDungMauNoiDung { get; set; }
        public int IdBrandName { get; set; }
        public string TenBrandName { get; set; } = String.Empty;
        public bool IsFlashSms { get; set; }
        public bool TrangThai { get; set; }
        public int? SoLuongThueBao { get; set; }
        public int? SoLuongSmsDaGuiThanhCong { get; set; }
        public int? SoLuongSmsGuiThatBai { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public List<ChienDichDanhBaDto> DanhBas { get; set; } = new List<ChienDichDanhBaDto> { };
        public ChienDichCreatedByDto Users { get ; set; } = new ChienDichCreatedByDto();
       


    }
}