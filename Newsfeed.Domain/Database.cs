using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Newsfeed.Domain
{
    public class Database
    {
        private static Database db = null;
        private static Object mutex = new Object();
        private MongoServer mongoServer;
        private string databaseName;

        private Database(string connectionString, string databaseName)
        {
            var mongoClient = new MongoClient(connectionString);
            this.mongoServer = mongoClient.GetServer();
            this.databaseName = databaseName;
        }

        public static Database Connection(string connectionString, string databaseName)
        {
            if (db == null)
            {
                lock (mutex)
                {
                    if (db == null)
                    {
                        db = new Database(connectionString, databaseName);
                    }
                }
            }
            return db;
        }

        public static MongoDatabase GetDB()
        {
            return db.mongoServer.GetDatabase(db.databaseName);
        }

    }
}
