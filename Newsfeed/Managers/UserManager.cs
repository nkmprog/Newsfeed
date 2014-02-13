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
            var userDTO = MapClientUserToDomain(user);
            userRepository.Insert(userDTO);
        }

        // TODO: Map all properties.
        private Domain.User MapClientUserToDomain(Models.User userCTO)
        {
            var userDTO = new Domain.User()
            {
                Username = userCTO.Username,
                Password = userCTO.Password,
                Avatar = userCTO.AvatarId
            };

            return userDTO;
        }

        public bool IsRegistered(string username)
        {
            var user = userRepository.Get(username);
            if (user == null) return false;
            return true;
        }
    }
}