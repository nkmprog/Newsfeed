using MongoDB.Bson;
using Newsfeed.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Newsfeed.Managers
{
    public class UserManager
    {
        private UserRepository userRepository;

        public UserManager()
        {
            userRepository = new UserRepository();
        }

        public Boolean IsValid(string username, string password)
        {
            var user = userRepository.Get(username);
            if (user == null || (!user.Password.Equals(password)))
            {
                return false;
            }
            return true;
        }

        public void RegisterUser(Models.User user)
        {
            userRepository.Insert(new Domain.User()
            {
                Username = user.Username,
                Password = user.Password,
                BlockedUsers = new List<BsonDocument>(),
                Avatar = user.AvatarId
            });
        }

        // TODO: Map all properties.
        private Domain.User MapClientUserToDomain(Models.User userCTO)
        {
            var blockedUsers = new List<BsonDocument>();
            // TODO: Ugh!
            foreach(String username in userCTO.BlockedUsers)
            {
                var blockedUser = new BsonDocument();
                var user = userRepository.Get(username);
                blockedUser.Add(new BsonElement("_id", user.Id));
                blockedUser.Add(new BsonElement("Username", user.Username));
                blockedUsers.Add(blockedUser);
            }
            var userDTO = new Domain.User()
            {
                Id = new ObjectId(userCTO.Id),
                Username = userCTO.Username,
                Password = userCTO.Password,
                BlockedUsers = blockedUsers,
                Avatar = userCTO.AvatarId
            };

            return userDTO;
        }

        private Models.User MapDomainUserToClient(Domain.User userDTO)
        {
            // TODO: Make a call to the GridFSRepository to retrieve the Avatar

            // Possibly use an aggregation to retrieve only the blocked user names
            // rather than retrieving the whole array of blocked users.
            IList<string> blockedUsers = new List<string>();
            foreach (MongoDB.Bson.BsonDocument user in userDTO.BlockedUsers)
            {
                var username = user.GetElement("Username");
                blockedUsers.Add(username.Value.AsString);
            }

            var userCTO = new Models.User()
            {
                Id = userDTO.Id.ToString(),
                Username = userDTO.Username,
                Password = userDTO.Password,
                BlockedUsers = blockedUsers
            };

            return userCTO;
        }

        public bool IsRegistered(string username)
        {
            var user = userRepository.Get(username);
            if (user == null) return false;
            return true;
        }

        internal Models.User GetUserByUserName(string username)
        {
            var userDTO = userRepository.Get(username);
            if (userDTO == null)
            {
                throw new Exception();
            }
            var userCTO = MapDomainUserToClient(userDTO);
            return userCTO;
        }

        public void SaveChanges(Models.User user)
        {
            var domainUser = MapClientUserToDomain(user);
            userRepository.Save(domainUser);
        }
    }
}