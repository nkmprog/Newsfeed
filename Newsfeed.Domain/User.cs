using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;

namespace Newsfeed.Domain
{
    public class User
    {
        public ObjectId Id { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public IList<BsonDocument> BlockedUsers { get; set; }

        public ObjectId Avatar { get; set; }
    }
}
