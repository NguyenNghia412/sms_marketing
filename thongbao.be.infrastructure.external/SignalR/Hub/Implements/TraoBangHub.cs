using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using thongbao.be.infrastructure.external.SignalR.Hub.Interfaces;

namespace thongbao.be.infrastructure.external.SignalR.Hub.Implements
{
    public class TraoBangHub : Microsoft.AspNetCore.SignalR.Hub<ITraoBangHub>
    {
        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await base.OnDisconnectedAsync(exception);
        }
    }
}