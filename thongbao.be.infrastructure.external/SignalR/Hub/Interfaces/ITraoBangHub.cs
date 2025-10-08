using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace thongbao.be.infrastructure.external.SignalR.Hub.Interfaces
{
    public interface ITraoBangHub
    {
        public Task ReceiveSinhVienDangTrao();
        public Task ReceiveChonKhoa(int idSubPlan);
        public Task ReceiveChuyenKhoa();
        public Task ReceiveCheckIn(string mssv);
     

    }
}
