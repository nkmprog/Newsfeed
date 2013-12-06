using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Web;

namespace Newsfeed.Services
{
    [ServiceContract(CallbackContract=typeof(INewsfeedServiceCallback))]
    public interface INewsfeedService
    {
        [OperationContract(IsOneWay = true, Action = "*")]
        void Recieve(Message message);
    }
}