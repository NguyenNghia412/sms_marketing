using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace thongbao.be.application.Config.Dtos.UserCredits
{
    public class UpdateUserCreditsDto
    {
        public int Id { get; set; }
        //public int IdNhaCungCapDichVu { get; set; }
        //public string UserId { get; set; } = String.Empty;
        public string HanMucCredit { get; set; } = String.Empty;
        public string? ToiDaHanMucCreditGiaHan { get; set; } = String.Empty;
        public DateTime ThoiGianBatDauApDungHanMuc { get; set; }
        public DateTime? ThoiGianKetThucApDungHanMuc { get; set; }
    }
}
