using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace thongbao.be.application.DiemDanh.Dtos
{

    public class ViewCuocHopDto
    {
        public int Id { get; set; }
        public string TenCuocHop { get; set; } = String.Empty;
        public string MoTa { get; set; } = String.Empty;
        public DateTime? ThoiGianBatDau { get; set; }
        public DateTime? ThoiGianKetThuc { get; set; }
        public DateTime? ThoiGianDiemDanh { get; set; }
        public string? IdCuocHop { get; set; } = String.Empty;
        public string? LinkCuocHop { get; set; } = String.Empty;
        public string IdTinNhanChung { get; set; }  = String.Empty;
        public DateTime? ThoiGianTao { get; set; }




    }
}
