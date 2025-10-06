using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using thongbao.be.infrastructure.external.SignalR.Hub.Implements;
using thongbao.be.infrastructure.external.SignalR.Service.Interfaces;

namespace thongbao.be.infrastructure.external.SignalR.Service.Implements
{
    internal class TraoBangService: ITraoBangService
    {
        private readonly IHubContext<TraoBangHub> _hubContext;

        public TraoBangService(IHubContext<TraoBangHub> hubContext)
        {
            _hubContext = hubContext;
        }
    }
}
