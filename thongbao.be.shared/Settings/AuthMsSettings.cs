using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace thongbao.be.shared.Settings
{
    public class AuthMsSettings
    {
        public string ClientId { get; set; } = String.Empty;
        public string ClientSecret { get; set; } = String.Empty;
        public string RedirectUri { get; set; } = String.Empty;
        public string AppCallbackRoute { get; set; } = String.Empty;
    }
}
