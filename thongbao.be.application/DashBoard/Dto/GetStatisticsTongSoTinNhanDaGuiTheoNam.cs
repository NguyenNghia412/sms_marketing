using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace thongbao.be.application.DashBoard.Dto
{
    public class GetStatisticsTongSoTinNhanDaGuiTheoNam
    {
        public List<GetStatisticsTongSoTinNhanDaGuiTheoNamTheoUser> ListThongKeTongSoTinNhanDaGui { get; set; } = new List<GetStatisticsTongSoTinNhanDaGuiTheoNamTheoUser>();
    }
    public class GetStatisticsTongSoTinNhanDaGuiTheoNamTheoUser
    {
        public User User { get; set; } = new User();
        public int TongSoTinNhanDaGuiThanhCong { get; set; }
        public int TongSoTinNhanDaGuiThatBai { get; set; }
        public int TongSoTinNhanDaGui { get; set; }
    }
    public class ViewUser
    {
        public string UserId { get; set; } = String.Empty;
        public string FullName { get; set; } = String.Empty;
    }
}
