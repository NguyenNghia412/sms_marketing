using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace thongbao.be.infrastructure.external.SignalR.Hub.Interfaces
{
    public interface ITraoBangHub
    {
        Task ReceiveSinhVienDangTrao(int idSubPlan, int id);
    }
}
