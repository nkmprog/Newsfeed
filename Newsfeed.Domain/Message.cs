using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Newsfeed.Domain
{
    public class Message
    {
        public ObjectId Id { get; set; }

        public string Text { get; set; }

        /// <summary>
        /// Gets or sets the likes.
        /// </summary>
        /// <value>The likes.</value>
        public int Likes { get; set; }

        /// <summary>
        /// Gets or sets the sent date.
        /// </summary>
        /// <value>The sent date.</value>
        public DateTime SentDate { get; set; }

        public BsonDocument Author { get; set; }
    }
}
