using DatingApplicationBackEnd.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatingApplicationBackEnd.SignalR
{
    /*This class acts as a SignalR hub and it does the some task when user connects to hub and disconnect from hub.*/
    [Authorize]
    public class PresenceHub : Hub
    {
        private readonly PresenceTracker tracker;

        public PresenceHub(PresenceTracker tracker)
        {
            this.tracker = tracker;
        }
        public override async Task OnConnectedAsync()
        {
            /*These are clients who is connected with Hub.
                - UserIsOnline is the method that we need to use client side 
                - Every time user connects to application or they can also connect from different browser every time they get new connection id.
             */
            await tracker.UserConnected(Context.User.GetUsername(), Context.ConnectionId); 
            await Clients.Others.SendAsync("UserIsOnline", Context.User.GetUsername());

            var currentUsers = await tracker.GetOnlineUsers();
            await Clients.All.SendAsync("GetOnlineUsers", currentUsers);
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await tracker.UserDisconnected(Context.User.GetUsername(), Context.ConnectionId);

            await Clients.Others.SendAsync("UserIsOffline", Context.User.GetUsername());

            var currentUsers = await tracker.GetOnlineUsers();
            await Clients.All.SendAsync("GetOnlineUsers", currentUsers);

            await base.OnDisconnectedAsync(exception);
        }
    }
} 
// Now we are adding SignalR service to our application => Go to startUp.cs class, will add signalR service their.
