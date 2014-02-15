using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using ChatR.Models;
using ChatR.Utilities;
using Microsoft.AspNet.SignalR.Hubs;

namespace ChatR.Hubs
{
    public class ChatHub : Hub
    {
        private readonly InMemoryRepository _repository;

        public ChatHub()
        {
            _repository = InMemoryRepository.GetInstance();
        }

        #region IDisconnect and IConnected event handlers implementation

        /// <summary>
        /// Fired when a client disconnects from the system. The user associated with the client ID gets deleted from the list of currently connected users.
        /// </summary>
        /// <returns></returns>
        public override Task OnDisconnected()
        {
            string userId = _repository.GetUserByConnectionId(Context.ConnectionId);
            if (userId != null)
            {
                var user = _repository.Users.FirstOrDefault(u => u.Id == userId);
                var roomname = user.Roomname;
                if (user != null)
                {
                    _repository.Remove(user);
                    return Clients.Group(roomname).leaves(user.Id, user.Username, DateTime.Now);
                }
            }

            return base.OnDisconnected();
        }

        #endregion

        #region Chat event handlers

        /// <summary>
        /// Fired when a client pushes a message to the server.
        /// </summary>
        /// <param name="message"></param>
        public void Send(ChatMessage message)
        {
            if (!string.IsNullOrEmpty(message.Content))
            {
                // Sanitize input
                message.Content = HttpUtility.HtmlEncode(message.Content);
                // Process URLs: Extract any URL and process rich content (e.g. Youtube links)
                HashSet<string> extractedUrLs;
                message.Content = TextParser.TransformAndExtractUrls(message.Content, out extractedUrLs);
                message.Timestamp = DateTime.Now;

                string userId = _repository.GetUserByConnectionId(Context.ConnectionId);
                var user      = _repository.Users.FirstOrDefault(u => u.Id == userId);
                if (user != null)
                {
                    var roomname  = user.Roomname;
                    Clients.Group(roomname).onMessageReceived(message);
                }
            }
        }

        /// <summary>
        /// Fired when a client joins the chat. Here round trip state is available and we can register the user in the list
        /// </summary>
        public void Joined()
        {
            var roomname = Clients.Caller.roomname;
            var user = new ChatUser()
            {
                //Id = Context.ConnectionId, 
                Id = Guid.NewGuid().ToString(),
                Username = Clients.Caller.username,
                Roomname = roomname
            };
            _repository.Add(user);
            _repository.AddMapping(Context.ConnectionId, user.Id);

            Groups.Add(Context.ConnectionId, roomname);

            Clients.Group(roomname).joins(user.Id, Clients.Caller.username, DateTime.Now);
        }

        /// <summary>
        /// Invoked when a client connects. Retrieves the list of all currently connected users to room
        /// </summary>
        /// <returns></returns>
        public ICollection<ChatUser> GetConnectedUsers(string roomname)
        {
            return _repository.Users.Where(x => x.Roomname == roomname).ToList<ChatUser>();
        }

        #endregion
    }
}