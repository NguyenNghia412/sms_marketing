using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace thongbao.be.application.DiemDanh.Dtos
{
    public class UpdateDotDiemDanhDto
    {
        public string TenDotDiemDanh { get; set; } = String.Empty;
        public string TenMonHoc { get; set; } = String.Empty;

        public string MaMonHoc { get; set; } = String.Empty;
        public DateTime? ThoiGianBatDau { get; set; }
        public DateTime? ThoiGianKetThuc { get; set; }
        public string GhiChu { get; set; } = String.Empty;
    }
}
