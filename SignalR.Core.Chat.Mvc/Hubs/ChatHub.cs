using System;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using SignalR.Core.Chat.Mvc.Data;
using SignalR.Core.Chat.Mvc.Models;

namespace SignalR.Core.Chat.Mvc.Hubs
{
    public class ChatHub : Hub
    {

        #region Constructor

        private readonly UserManager<ChatUser> _userManager;
        private readonly ApplicationDbContext _context;

        public ChatHub(UserManager<ChatUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        #endregion

        #region BroadCastFromClient

        public async Task BroadCastFromClient(string message)
        {
            try
            {
                //get current user
                var currentUser = await _userManager.GetUserAsync(Context.User);
                //create message model
                var m = new Message()
                {
                    MessageBody = message,
                    MessageDateTime = DateTime.Now,
                    FromUser = currentUser
                };
                //save in db
                await _context.AddAsync(m);
                await _context.SaveChangesAsync();
                //inform all client 
                await Clients.All.SendAsync(
                    "Broadcast",
                    new
                    {
                        messageBody = m.MessageBody,
                        messageDateTime = m.MessageDateTime.ToString("hh:mm tt MMM dd", CultureInfo.InvariantCulture),
                        fromUser = currentUser.Email
                    });

            }
            catch (Exception e)
            {
                await Clients.Caller.SendAsync("HubError", new { error = e.Message });
            }
        }


        #endregion

        #region OnConnectedAsync

        public override async Task OnConnectedAsync()
        {


            await Clients.All.SendAsync(
                "UserConnected",
                new
                {
                    connectedId = Context.ConnectionId,
                    connectionDateTime = DateTime.Now,
                    messageDateTime = DateTime.Now.ToString("hh:mm tt MMM dd", CultureInfo.InvariantCulture),
                });

        }

        #endregion

        #region OnDisconnectedAsync

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await Clients.All.SendAsync(
                "UserDisconnected", $"User disconnected,connectionId : {Context.ConnectionId}");
        }


        #endregion


    }
}
