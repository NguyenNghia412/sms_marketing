using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace thongbao.be.application.DashBoard.Dto
{
    public class GetStatisticsUserCreditsTheoNam
    {
        public  List<GetStatisticsUserCreditsTheoNamByUser> ListUserCredits { get; set; } = new List<GetStatisticsUserCreditsTheoNamByUser>();
    }

    public class GetStatisticsUserCreditsTheoNamByUser
    {
        public User User { get; set; } = new User();
        public string CreditDaSuDung { get; set; } = String.Empty;
        public string DonVi { get; set; } = "VND";
    }

    public class User
    {
        public string UserId { get; set; } = String.Empty;
        public string FullName { get; set; } = String.Empty;
    }
}
