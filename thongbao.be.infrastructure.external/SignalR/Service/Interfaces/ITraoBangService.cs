using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace thongbao.be.infrastructure.external.SignalR.Service.Interfaces
{
    public interface ITraoBangService
    {
        Task NotifySinhVienDangTrao(int idSubPlan, int id);
    }
}
