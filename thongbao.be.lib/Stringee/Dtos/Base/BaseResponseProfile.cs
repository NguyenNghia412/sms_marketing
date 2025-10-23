using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace thongbao.be.lib.Stringee.Dtos.Base
{
    public class Account
    {
        public int Id { get; set; }
        //public string AccountSid { get; set; } = String.Empty;
        //public string AccountKey { get; set; } = String.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string CountryNumber { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;

        public decimal Amount { get; set; } 
    }
    public class BaseResponseProfile
    {
        public int? Code { get; set; }
        //public string Message { get; set; } = string.Empty;
        public Account? Data { get; set; }

    }
}
