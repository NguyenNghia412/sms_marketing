using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace thongbao.be.application.Config.Dtos.UserCredits
{
    public class ViewUserCreditsByUserDto
    {
        public ViewUserCreditDto UserCredit { get; set; } = new ViewUserCreditDto();
        public string HanMucCredit { get; set; } = String.Empty;
        
        public string? CreditDaSuDung { get; set; } = String.Empty;
        public string? CreditChuaSuDung { get; set; } = String.Empty;
        public string DonVi { get; set; } = "VND";
    }

    public class ViewUserCreditDto
    {
        public string UserId { get; set; } = String.Empty;
        public string UserName { get; set; } = String.Empty;
        public string FullName { get; set; } = String.Empty;
        public string Email { get; set; } = String.Empty;


    }

}
