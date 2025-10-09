using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using thongbao.be.infrastructure.external.SignalR.Hub.Implements;
using thongbao.be.infrastructure.external.SignalR.Hub.Interfaces;
using thongbao.be.infrastructure.external.SignalR.Service.Interfaces;

namespace thongbao.be.infrastructure.external.SignalR.Service.Implements
{
    public class TraoBangService : ITraoBangService
    {
        private readonly IHubContext<TraoBangHub, ITraoBangHub> _hubContext;

        public TraoBangService(IHubContext<TraoBangHub, ITraoBangHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task NotifySinhVienDangTrao()
        {
            await _hubContext.Clients.All.ReceiveSinhVienDangTrao();
        }
        public async Task NotifyChonKhoa()
        {
            await _hubContext.Clients.All.ReceiveChonKhoa();
        }
        /*public async Task NotifyChuyenKhoa()
        {
            await _hubContext.Clients.All.ReceiveChuyenKhoa();
        }*/
        public async Task NotifyCheckIn()
        {
            await _hubContext.Clients.All.ReceiveCheckIn();
        }
       
    }
}