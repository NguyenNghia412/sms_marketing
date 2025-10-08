using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace thongbao.be.application.TraoBang.Dtos
{
    public  class GetInforSubPlanDangTraoResponseDto
    {
        public int Id { get; set; }
        public string Ten { get; set; } = String.Empty;
        public string TienDo { get; set; } = String.Empty;
        public List<ListSinhVienDto> Items { get; set; } = new List<ListSinhVienDto>();
    }
    public class ListSinhVienDto
    {
        public string TenSubPlan { get; set; } = String.Empty;
        public int Id { get; set; }
        public string HoVaTen { get; set; } = String.Empty;
        public string MaSoSinhVien { get; set; } = String.Empty;
        public string TenNganhDaoTao { get; set; } = String.Empty;
        public int TrangThai { get; set; }
    }
}
