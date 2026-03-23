using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace thongbao.be.application.DashBoard.Dto
{
    public class GetStatisticsUserCreditsTheoThangByUser
    {
        
         public List<GetStatisticsUserCreditsByUser> UserCreditsTheoThangByUsers { get; set; } = new List<GetStatisticsUserCreditsByUser>();
    }

    public class UserTheoThang
    {
        public string UserId { get; set; } = String.Empty;
        public string FullName { get; set; } = String.Empty;
    }

    public class GetStatisticsUserCreditsByUser
    {
        public DateTime TuNgay { get; set; }
        public DateTime DenNgay { get; set; }
        public UserTheoThang User { get; set; } = new UserTheoThang();
        public string CreditDaSuDung { get; set; } = String.Empty;
        public string HanMucCredit { get; set; } = String.Empty;
        public string DonVi { get; set; } = "VND";
    }
}
