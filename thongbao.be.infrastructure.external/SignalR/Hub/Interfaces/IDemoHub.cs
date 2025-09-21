using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace thongbao.be.infrastructure.external.SignalR.Hub.Interfaces
{
    /// <summary>
    /// Interface cho DemoHub
    /// </summary>
    public interface IDemoHub
    {
        /// <summary>
        /// Gửi tin nhắn tới tất cả clients
        /// </summary>
        Task SendMessage(string user, string message);

        /// <summary>
        /// Gửi tin nhắn tới chính client gọi
        /// </summary>
        Task SendMessageToCaller(string user, string message);

        /// <summary>
        /// Join vào group
        /// </summary>
        Task JoinGroup(string groupName);

        /// <summary>
        /// Leave group
        /// </summary>
        Task LeaveGroup(string groupName);

        /// <summary>
        /// Lấy số lượng connection hiện tại
        /// </summary>
        Task GetConnectionCount();
    }
}