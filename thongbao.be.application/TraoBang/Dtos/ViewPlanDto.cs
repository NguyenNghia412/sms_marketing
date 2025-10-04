using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace thongbao.be.application.TraoBang.Dtos
{
    public class ViewPlanDto
    {
        public int Id { get; set; }
        public string Ten { get; set; } = String.Empty;
        public string MoTa { get; set; } = String.Empty;
        public DateTime ThoiGianBatDau { get; set; }
        public DateTime ThoiGianKetThuc { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
