using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace thongbao.be.application.Config.Dtos.UserCredits
{
    public class AddUserCreditsDto
    {
        public string UserId { get; set; } = String.Empty;
        public string HanMucCredit { get; set; } = String.Empty;
        public DateTime ThoiGianBatDauApDungHanMuc { get; set; }
        public DateTime? ThoiGianKetThucApDungHanMuc { get; set; }
    }
}
