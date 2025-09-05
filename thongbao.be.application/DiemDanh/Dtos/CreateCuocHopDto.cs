using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace thongbao.be.application.DiemDanh.Dtos
{
    public class CreateCuocHopDto
    {
        public string TenCuocHop { get; set; } = String.Empty;
        public string MoTa { get; set; } = String.Empty;
        public DateTime? ThoiGianBatDau { get; set; }
        public DateTime? ThoiGianKetThuc { get; set; }
        public DateTime? ThoiGianDiemDanh { get; set; }
        //public string? LinkCuocHop { get; set; } = String.Empty;
        public int? ThoiHanDiemDanh { get; set; }
    }
}
