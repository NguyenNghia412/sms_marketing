using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace thongbao.be.application.GuiTinNhan.Dtos
{
    public class DuplicateChienDichDto
    {
        public string TenChienDich { get; set; } = String.Empty;
        public string MoTa { get; set; } = String.Empty;
        public DateTime NgayBatDau { get; set; }
        public DateTime NgayKetThuc { get; set; }
        public bool IsFlash { get; set; }
        public int IdBrandName { get; set; }
        public int IdMauNoiDung { get; set; }
        public string NoiDung { get; set; } = String.Empty;
        public bool IsAccented { get; set; }
        public bool TrangThai { get; set; }
    }
}
