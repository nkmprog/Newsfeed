using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Newsfeed.Domain
{
    class MessageRepository
    {
        private MongoCollection<Message> messageCollection;

        public MessageRepository(MongoCollection<Message> messageCollection)
        {
            this.messageCollection = messageCollection;
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

        public void updateMessageContent(ObjectId id, string content)
        {
            var query = Query<Message>.EQ(message => message.Id, id);
            var update = Update<Message>.Set(message => message.Text, content);
            messageCollection.Update(query, update);
        }

        public void updateMessageLikes(ObjectId id, int likes)
        {
            var query = Query<Message>.EQ(message => message.Id, id);
            var update = Update<Message>.Inc(message => message.Likes, likes);
            messageCollection.Update(query, update);
        }
    }
}
