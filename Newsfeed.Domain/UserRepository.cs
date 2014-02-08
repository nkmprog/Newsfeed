﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

namespace Newsfeed.Domain
{
    public class UserRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserRepository" /> class.
        /// </summary>
        /// <param name="users">The users.</param>
        public UserRepository(MongoCollection<User> users)
        {
            this.users = users;
        }

        /// <summary>
        /// Inserts the specified user.
        /// </summary>
        /// <param name="user">The user.</param>
        public void Insert(User user)
        {
            this.users.Insert(user);
        }

        /// <summary>
        /// Saves the specified user.
        /// </summary>
        /// <param name="user">The user.</param>
        public void Save(User user)
        {
            this.users.Save(user);
        }

        /// <summary>
        /// Gets the specified id.
        /// </summary>
        /// <param name="id">The id.</param>
        public void Get(ObjectId id)
        {
            this.users.FindOne(Query.EQ("Id", id));
        }

        /// <summary>
        /// Adds the blockedUserId and blockedUsername to the collection of blocked users of the user with id - userId.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="blockedUserId">The blocked user id.</param>
        /// <param name="blockedUsername">The blocked username.</param>
        public void Block(ObjectId userId, ObjectId blockedUserId, string blockedUsername)
        {
            this.users.Update(Query.EQ("Id", userId),
                Update.AddToSet("BlockedUsers",
                    new BsonDocument{
                        {"Id", blockedUserId},
                        {"Username", blockedUsername}
            }));
        }

        private readonly MongoCollection<User> users;
    }
}