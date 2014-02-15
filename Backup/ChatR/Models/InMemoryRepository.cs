using System;
using System.Collections.Generic;
using System.Linq;

namespace ChatR.Models
{
    public class InMemoryRepository
    {
        private static ICollection<ChatUser> _connectedUsers;
        private static Dictionary<string, string> _mappings;
        private static InMemoryRepository _instance = null;
        private const int MaxRandom = 3;

        public static InMemoryRepository GetInstance()
        {
            return _instance ?? (_instance = new InMemoryRepository());
        }

        #region Private methods

        private InMemoryRepository()
        {
            _connectedUsers = new List<ChatUser>();
            _mappings = new Dictionary<string, string>();
        }

        #endregion

        #region Repository methods

        public IQueryable<ChatUser> Users { get { return _connectedUsers.AsQueryable(); } }

        public void Add(ChatUser user)
        {
            _connectedUsers.Add(user);
        }

        public void Remove(ChatUser user)
        {
            _connectedUsers.Remove(user);
        }

        public string GetRandomizedUsername(string username)
        {
            var tempUsername = username;
            var newRandom = MaxRandom;
            var oldRandom = 0;
            var loops = 0;
            var random = new Random();
            do
            {
                if (loops > newRandom)
                {
                    oldRandom = newRandom;
                    newRandom *= 2;
                }
                username = tempUsername + "_" + random.Next(oldRandom, newRandom).ToString();
                loops++;
            } while (GetInstance().Users.Where(u => u.Username.Equals(username)).ToList().Count > 0);

            return username;
        }

        public void AddMapping(string connectionId, string userId)
        {
            if (!string.IsNullOrEmpty(connectionId) && !string.IsNullOrEmpty(userId))
            {
                _mappings.Add(connectionId, userId);
            }
        }

        public string GetUserByConnectionId(string connectionId)
        {
            string userId = null;
            _mappings.TryGetValue(connectionId, out userId);            
            return userId;
        }

        #endregion
    }
}