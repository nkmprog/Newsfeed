using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Newsfeed.Domain
{
    public class GridFSRepository
    {
        public GridFSRepository()
        {
            this.database = Database.GetDB();
        }

        public ObjectId UploadFile(string fileName)
        {
            using (var fs = new FileStream(fileName, FileMode.Open))
            {
                var gridFsInfo = database.GridFS.Upload(fs, fileName);
                var fileId = gridFsInfo.Id;

                return  (ObjectId)fileId;
            }
        }

        private readonly MongoDatabase database;
    }
}
