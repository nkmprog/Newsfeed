using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.IO;
using MongoDB.Bson;
using Newsfeed.Domain;
using System.Net.Http.Headers;

namespace Newsfeed.Controllers
{
    public class ImageController : ApiController
    {
        // GET api/image/5
        public HttpResponseMessage Get(string id)
        {
            var gridFS = new GridFSRepository();
            var fileId = new ObjectId(id);

            var fileStream = gridFS.GetFile(fileId);

            var response = new HttpResponseMessage(HttpStatusCode.OK);

            response.Content = new StreamContent(fileStream);
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");

            return response;
        }
    }
}
