using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace thongbao.be.application.DiemDanh.Dtos
{
    public class ViewThongKeDiemDanhRequestDto
    {
        public int IdCuocHop { get; set; }
        public string? Filter { get; set; } = String.Empty;
    }
}
