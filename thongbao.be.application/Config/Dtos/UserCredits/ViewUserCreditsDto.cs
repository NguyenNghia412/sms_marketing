using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace thongbao.be.application.Config.Dtos.UserCredits
{
    public class ViewUserCreditsDto
    {
        public int Id { get; set; }
        public ViewUserDto User { get; set; } = new ViewUserDto();
        public string HanMucCredit { get; set; } = String.Empty;
        public DateTime ThoiGianBatDauApDungHanMuc { get; set; }
        public DateTime ThoiGianKetThucApDungHanMuc { get; set; }
        public int LoaiApiCredit { get; set; }
        public string? CreditDaSuDung { get; set; } = String.Empty;
        public string? CreditChuaSuDung { get; set; } = String.Empty;
        public string? CreditConSauKhiKetThucThoiGianApDungHanMuc { get; set; } = String.Empty;
        public string DonVi { get; set; } = "VND";
    }

    public class ViewUserDto
    {
        public string UserId { get; set; } = String.Empty;
        public string UserName { get; set; } = String.Empty;
        public string FullName { get; set; } = String.Empty;
        public string Email { get; set; } = String.Empty;


    }
}
