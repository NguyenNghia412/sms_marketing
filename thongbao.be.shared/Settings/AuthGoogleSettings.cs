using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace thongbao.be.shared.Settings
{
    public class AuthGoogleSettings
    {
        public string ClientId { get; set; } = String.Empty;
        public string ClientSecret { get; set; } = String.Empty;
        public string RedirectUri { get; set; } = String.Empty;
    }
}
