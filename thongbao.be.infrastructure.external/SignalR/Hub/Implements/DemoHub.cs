using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace thongbao.be.infrastructure.external.SignalR.Hub.Implements
{
    /// <summary>
    /// Demo Hub để test SignalR functionality
    /// </summary>
    public class DemoHub : Microsoft.AspNetCore.SignalR.Hub
    {
        // Static dictionary để track connections
        private static readonly ConcurrentDictionary<string, bool> _connections = new ConcurrentDictionary<string, bool>();

        /// <summary>
        /// Gửi tin nhắn tới tất cả clients
        /// </summary>
        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }

        /// <summary>
        /// Gửi tin nhắn tới chính client gọi
        /// </summary>
        public async Task SendMessageToCaller(string user, string message)
        {
            await Clients.Caller.SendAsync("ReceiveMessage", user, message);
        }

        /// <summary>
        /// Join vào group
        /// </summary>
        public async Task JoinGroup(string groupName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            await Clients.Group(groupName).SendAsync("ReceiveMessage", "System", $"{Context.ConnectionId} joined {groupName}");
        }

        /// <summary>
        /// Leave group
        /// </summary>
        public async Task LeaveGroup(string groupName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
            await Clients.Group(groupName).SendAsync("ReceiveMessage", "System", $"{Context.ConnectionId} left {groupName}");
        }

        /// <summary>
        /// Lấy số lượng connection hiện tại
        /// </summary>
        public async Task GetConnectionCount()
        {
            var count = _connections.Count;
            await Clients.Caller.SendAsync("UpdateConnectionCount", count);
        }

        public override async Task OnConnectedAsync()
        {
            _connections.TryAdd(Context.ConnectionId, true);
            var connectionCount = _connections.Count;

            await Clients.All.SendAsync("ReceiveMessage", "System", $"{Context.ConnectionId} connected");
            await Clients.All.SendAsync("UpdateConnectionCount", connectionCount);

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            _connections.TryRemove(Context.ConnectionId, out _);
            var connectionCount = _connections.Count;

            await Clients.All.SendAsync("ReceiveMessage", "System", $"{Context.ConnectionId} disconnected");
            await Clients.All.SendAsync("UpdateConnectionCount", connectionCount);

            await base.OnDisconnectedAsync(exception);
        }
    }
}