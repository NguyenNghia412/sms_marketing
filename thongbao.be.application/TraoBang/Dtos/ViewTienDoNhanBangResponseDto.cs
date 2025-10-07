using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace thongbao.be.application.TraoBang.Dtos
{
    public class ViewTienDoNhanBangResponseDto
    {
        public int Id { get; set; }
        public string HoVaTen { get; set; } = String.Empty;
        public string MaSoSinhVien { get; set; } = String.Empty;
        public string CapBang { get; set; } = String.Empty;
        public string TenNganhDaoTao { get; set; } = String.Empty;
        public string ? Note { get; set; }
        public int TrangThai { get; set; }
        public int Order { get; set; }
        public bool IsShow { get; set; }

    }
}
