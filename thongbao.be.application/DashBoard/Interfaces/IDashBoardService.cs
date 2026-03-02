using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using thongbao.be.application.DashBoard.Dto;

namespace thongbao.be.application.DashBoard.Interfaces
{
    public interface IDashBoardService
    {
        public GetStatisticsUserCreditsTheoNam GetStatisticsUserCreditsTheoNam(DateTime tuNgay, DateTime denNgay);
        public GetStatisticsTongSoTinNhanDaGuiTheoNam GetStatisticsTongSoTinNhanDaGuiTheoNam(DateTime tuNgay, DateTime denNgay);
        public GetStatisticsDashBoard GetStatisticsDashBoard();
        public GetStatisticsUserCreditsTheoThangByUser GetStatisticsUserCreditsTheoThangByUser(string userId, int nam);
    }
}
