using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace thongbao.be.infrastructure.external.SignalR.Service.Interfaces
{
    public interface IDemoSignalRService
    {
        /// <summary>
        /// Gửi thông báo tới tất cả clients
        /// </summary>
        Task SendNotificationToAll(string title, string message);

        /// <summary>
        /// Gửi thông báo tới group cụ thể
        /// </summary>
        Task SendNotificationToGroup(string groupName, string title, string message);

        /// <summary>
        /// Gửi thông báo tới user cụ thể
        /// </summary>
        Task SendNotificationToUser(string userId, string title, string message);

        /// <summary>
        /// Lấy số lượng connection hiện tại từ Hub
        /// </summary>
        Task<int> GetConnectionCount();

        /// <summary>
        /// Broadcast connection count tới tất cả clients
        /// </summary>
        Task BroadcastConnectionCount(int count);
    }
}