using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace thongbao.be.shared.Settings
{
    public class AuthServerSettings
    {
        public string Authority { get; set; } = String.Empty;
        public string AppUrl { get; set; } = String.Empty;
        public AuthGoogleSettings Google { get; set; }
        public AuthMsSettings MS { get; set; }
    }
}
