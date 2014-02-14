using System;
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
        public UserRepository()
        {
            var db = Database.GetDB();
            this.ratings = db.GetCollection("ratings");
            this.users = db.GetCollection<User>("users");
            this.messages = db.GetCollection<Message>("messages");
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
        public User Get(ObjectId id)
        {
           return this.users.FindOne(Query.EQ("Id", id));
        }

        /// <summary>
        /// Adds the blockedUserId and blockedUsername to the collection of blocked users of the user with id - userId.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="blockedUserId">The blocked user id.</param>
        /// <param name="blockedUsername">The blocked username.</param>
        public void Block(ObjectId userId, ObjectId blockedUserId, string blockedUsername)
        {
            this.users.Update(Query.EQ("_id", userId),
                Update.AddToSet("BlockedUsers",
                    new BsonDocument{
                        {"Id", blockedUserId},
                        {"Username", blockedUsername}
            }));
        }

        public User Get(string username)
        {
            return this.users.FindOne(Query.EQ("Username", username));
        }

        public void UpdateUserName(string username, string newUserName)
        {
            var query = Query<User>.EQ(user => user.Username, username);
            var update = Update<User>.Set(user => user.Username, newUserName);
            users.Update(query, update);
        }

        private readonly MongoCollection<User> users;
        private readonly MongoCollection<Message> messages;
        private readonly MongoCollection ratings;

        // This should be a service action.
        public Dictionary<string, double> generateRatings()
        {
            var finalResults = new Dictionary<string, double>();
            // TODO: perform incremental mapReduce
            // Join the messsages and users tables on the username
            string mapMessages = @"
                function() {
                    emit(this.Author.username, { blockedBy : 0, Likes : this.Likes});
            }";
            string mapUsers = @"
                function() {
                    emit(this.Username, { blockedBy : 0, Likes : 0});
                    for (var i = 0; i < this.BlockedUsers.length; i++){
                        var blockedUser = this.BlockedUsers[i];
                        emit(blockedUser.Username, { blockedBy : 1, Likes : 0 });
                    }
            }";
            string reduce = @"
                function(key, values){
                    var result = { blockedBy : 0, Likes : 0 };
                    values.forEach(function(value) {
                        result.blockedBy += value.blockedBy;
                        result.Likes += value.Likes;  
                    });
                    return result;
            }";
            string finalize = @"
                function(key, reducedValue){
                    var blocked = 1;
                    if(reducedValue.blockedBy != 0){
                        blocked = parseInt(reducedValue.blockedBy);
                    }
                    var rating = parseInt(reducedValue.Likes) / (blocked * blocked);
                    reducedValue.rating = parseInt(rating);
                    return reducedValue;
            }";
            var options = new MapReduceOptionsBuilder();
            options.SetOutput(new MapReduceOutput { 
                CollectionName = "ratings",
                Mode = MapReduceOutputMode.Reduce
            });
            messages.MapReduce(mapMessages, reduce, options);
            options.SetFinalize(finalize);
            users.MapReduce(mapUsers, reduce, options);
            
            // Since sorting requires a pre build index 
            // when using mapReduce we issue another query.
            var results = ratings.FindAllAs<BsonDocument>().SetSortOrder(SortBy.Descending("value.rating"));

            foreach ( var result in results ) 
            {
                var values = result["value"];
                var rating = values["rating"].AsDouble;
                var username = result["_id"].AsString;
                finalResults.Add(username, rating);
            }

            return finalResults;
        }
    }
}
