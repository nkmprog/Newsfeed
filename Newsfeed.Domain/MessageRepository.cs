using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MongoDB.Driver.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Newsfeed.Domain
{
    public class MessageRepository
    {
        private MongoCollection<Message> messageCollection;

        public MessageRepository()
        {
            var db = Database.GetDB();
            this.messageCollection = db.GetCollection<Message>("messages");
        }

        // Inserts a single message into the collection. If the messages is
        // already present in the collection an exception will be thrown.
        public void InsertMessage(Message message)
        {
            messageCollection.Insert(message);
        }

        public void InsertMessageBatch(IEnumerable<Message> batch)
        {
            messageCollection.InsertBatch(batch);
        }

        // Conveniance method to find a single message
        // based on it's id as a primary key 
        public Message FindOneMessage(ObjectId id)
        {
            var query = Query<Message>.EQ(message => message.Id, id);
            return messageCollection.FindOne(query);
        }

        public void RemoveMessage(ObjectId id)
        {
            // Consider using FindAndRemove
            var query = Query<Message>.EQ(message => message.Id, id);
            messageCollection.Remove(query);
        }

        public void UpdateMessageContent(ObjectId id, string content)
        {
            var query = Query<Message>.EQ(message => message.Id, id);
            var update = Update<Message>.Set(message => message.Text, content);
            messageCollection.Update(query, update);
        }

        public void UpdateMessageLikes(ObjectId id, int likes)
        {
            var query = Query<Message>.EQ(message => message.Id, id);
            var update = Update<Message>.Inc(message => message.Likes, likes);
            messageCollection.Update(query, update);
        }

        public IEnumerable<Message> GetLatestMessages(int skip, int take, IList<BsonDocument> blockedUsers)
        {
            var blocked = blockedUsers.Select(u => { return u.GetValue("Username"); });

            var messages = this.messageCollection.Find(Query.NotIn("Author.Username", blocked));

            return messages.AsQueryable<Message>()
                .OrderByDescending(m => m.SentDate)
                .Skip(skip)
                .Take(take);
        }
    }
}
