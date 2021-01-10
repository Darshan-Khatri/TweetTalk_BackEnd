using AutoMapper;
using DatingApplicationBackEnd.Core.Models;
using DatingApplicationBackEnd.DTOs;
using DatingApplicationBackEnd.Extensions;
using DatingApplicationBackEnd.Interfaces;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatingApplicationBackEnd.SignalR
{
    public class MessageHub : Hub
    {
        private readonly IMessageRepository messageRepository;
        private readonly IMapper mapper;
        private readonly IUserRepository userRepository;

        public MessageHub(IMessageRepository messageRepository, IMapper mapper, IUserRepository userRepository)
        {
            this.messageRepository = messageRepository;
            this.mapper = mapper;
            this.userRepository = userRepository;
        }

        public override async Task OnConnectedAsync()
        {
            var httpContext = Context.GetHttpContext();
            var otherUser = httpContext.Request.Query["user"].ToString();
            var groupName = GetGroupName(Context.User.GetUsername(), otherUser);
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

            var message = await messageRepository.GetMessageThread(Context.User.GetUsername(), otherUser);

            await Clients.Group(groupName).SendAsync("ReceiveMessageThread", message); 
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessage(CreateMessageDto createMessageDto)
        {
            //here you get username of loggedIn user by using Authorization token claims.
            var username = Context.User.GetUsername();

            if (username == createMessageDto.RecipientUsername.ToLower())
                throw new HubException("You can't send message to yourself."); 

            //gets loggedIn user info
            var sender = await userRepository.GetUserByUsernameAsync(username);

            //gets recipient's info
            var recipient = await userRepository.GetUserByUsernameAsync(createMessageDto.RecipientUsername);

            if (recipient == null) throw new HubException("User not found"); 

            var message = new Message
            {
                Sender = sender,
                Recipient = recipient,
                SenderUsername = sender.UserName,
                RecipientUsername = recipient.UserName,
                Content = createMessageDto.Content
            };

            messageRepository.AddMessage(message);

            if (await messageRepository.SaveAllAsync())
            {
                var group = GetGroupName(sender.UserName, recipient.UserName);
                //We are receiving new message from our hub, Then what we need to do in our client is to update message thread observable to show this new message when it is received.
                await Clients.Group(group).SendAsync("NewMessage", mapper.Map<MessageDto>(message));
            }
        }

        private string GetGroupName(string caller, string other)
        {
            var stringCompare = string.CompareOrdinal(caller, other) < 0;
            return stringCompare ? $"{caller}-{other}" : $"{other}-{caller}";
        }
    }
}
