using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace thongbao.be.application.GuiTinNhan.Dtos
{
    public  class GetListBrandNameResponseDto
    {
        public int Id { get; set; }
        public string TenBrandName { get; set; } = String.Empty;
        public string? Mota { get; set; } = String.Empty;
        public DateTime? ThoiGianTao { get; set; }
        public DateTime? ThoiGianKetThuc { get; set; }
    }
}
