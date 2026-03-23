using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace thongbao.be.application.GuiTinNhan.Dtos
{
    public class GuiTinNhanCoLichGuiDto
    {
        public int IdChienDich { get; set; }
        public int? IdDanhBa { get; set; }
        public List<ListSoDienThoaiCoLichGuiDto>? DanhSachSoDienThoai { get; set; } = new List<ListSoDienThoaiCoLichGuiDto>();
        public int? IdBrandName { get; set; }
        public bool IsFlashSms { get; set; }
        public bool IsAccented { get; set; }
        public string? NoiDung { get; set; } = string.Empty;
        public DateTime LichGui { get; set; }
    }

    public class ListSoDienThoaiCoLichGuiDto
    {
        public string SoDienThoai { get; set; } = string.Empty;
    }
}
