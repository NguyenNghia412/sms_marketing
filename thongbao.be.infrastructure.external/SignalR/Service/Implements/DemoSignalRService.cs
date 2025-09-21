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
    /// <summary>
    /// Service để gửi SignalR messages từ bên ngoài Hub
    /// </summary>
    public class DemoSignalRService : IDemoSignalRService
    {
        private readonly IHubContext<DemoHub> _hubContext;

        public DemoSignalRService(IHubContext<DemoHub> hubContext)
        {
            _hubContext = hubContext;
        }

        /// <summary>
        /// Gửi thông báo tới tất cả clients
        /// </summary>
        public async Task SendNotificationToAll(string title, string message)
        {
            await _hubContext.Clients.All.SendAsync("ReceiveNotification", title, message, "info");
        }

        /// <summary>
        /// Gửi thông báo tới group cụ thể
        /// </summary>
        public async Task SendNotificationToGroup(string groupName, string title, string message)
        {
            await _hubContext.Clients.Group(groupName).SendAsync("ReceiveNotification", title, message, "group");
        }

        /// <summary>
        /// Gửi thông báo tới user cụ thể
        /// </summary>
        public async Task SendNotificationToUser(string userId, string title, string message)
        {
            await _hubContext.Clients.User(userId).SendAsync("ReceiveNotification", title, message, "personal");
        }

        /// <summary>
        /// Lấy số lượng connection hiện tại từ Hub
        /// </summary>
        public async Task<int> GetConnectionCount()
        {
            // Gọi method từ Hub để lấy connection count
            await _hubContext.Clients.All.SendAsync("GetConnectionCount");
            return 0; // Trả về 0 vì không thể trực tiếp lấy từ service
        }

        /// <summary>
        /// Broadcast connection count tới tất cả clients
        /// </summary>
        public async Task BroadcastConnectionCount(int count)
        {
            await _hubContext.Clients.All.SendAsync("UpdateConnectionCount", count);
        }
    }
}