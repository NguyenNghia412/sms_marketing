using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace thongbao.be.application.GuiTinNhan.Dtos
{

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
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public List<ChienDichDanhBaDto> DanhBas { get; set; } = new List<ChienDichDanhBaDto> { };

        
    }
}