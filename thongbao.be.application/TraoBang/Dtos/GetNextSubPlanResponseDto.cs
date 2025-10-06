using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace thongbao.be.application.TraoBang.Dtos
{
    public class GetNextSubPlanResponseDto
    {
        public int Id { get; set; }
        public string TenSubPlan { get; set; } = string.Empty;
        public string TruongKhoa { get; set; } = string.Empty;
        public int Order { get; set; }
        public int TrangThai { get; set; }
    }
}
