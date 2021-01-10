using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatingApplicationBackEnd.SignalR
{
    public class PresenceTracker
    {
        /*key => username ; value => list of connection id for that user.*/
        private static readonly Dictionary<string, List<string>> OnlineUsers =
            new Dictionary<string, List<string>>();

        public Task UserConnected(string username, string connectionId)
        {
            lock (OnlineUsers)
            {
                if (OnlineUsers.ContainsKey(username))
                {
                    OnlineUsers[username].Add(connectionId);
                }
                else
                {
                    OnlineUsers.Add(username, new List<string> { connectionId });
                }
            }

            return Task.CompletedTask;
        }

        public Task UserDisconnected(string username, string connectionId)
        {
            lock (OnlineUsers)
            {
                if (!OnlineUsers.ContainsKey(username)) return Task.CompletedTask;

                OnlineUsers[username].Remove(connectionId);
                if (OnlineUsers[username].Count == 0)
                {
                    OnlineUsers.Remove(username);
                }
            }

            return Task.CompletedTask;
        }

        public Task<string[]> GetOnlineUsers()
        {
            string[] _onlineUsers;

            lock (OnlineUsers)
            {
                _onlineUsers = OnlineUsers.OrderBy(k => k.Key).Select(k => k.Key).ToArray();
            }
            return Task.FromResult(_onlineUsers);
        }

    }
}
