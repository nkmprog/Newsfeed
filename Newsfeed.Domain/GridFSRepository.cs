using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;

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

        public ObjectId UploadFile(Stream file, string fileName)
        {
            var gridFsInfo = database.GridFS.Upload(file, fileName);
            var fileId = gridFsInfo.Id;

            file.Close();

            return (ObjectId)fileId;
        }

        public Stream GetFile(ObjectId fileId)
        {
            var fileInfo = database.GridFS.FindOneById(fileId);
            var fileStream = fileInfo.OpenRead();
            return fileStream;
        }

        public void RemoveFile(ObjectId fileId)
        {
            if (database.GridFS.ExistsById(fileId))
            {
                database.GridFS.DeleteById(fileId);
            }
        }

        private readonly MongoDatabase database;
    }
}
