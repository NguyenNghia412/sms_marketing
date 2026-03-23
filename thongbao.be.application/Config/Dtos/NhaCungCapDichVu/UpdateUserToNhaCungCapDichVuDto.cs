using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace thongbao.be.application.Config.Dtos.NhaCungCapDichVu
{
    public class UpdateUserToNhaCungCapDichVuDto
    {
        public int IdUserNhaCungCapDichVu { get; set; }
        public DateTime? ThoiGianBatDauSuDungDichVu { get; set; }
        public DateTime? ThoiGianKetThucSuDungDichVu { get; set; }
    }
}
