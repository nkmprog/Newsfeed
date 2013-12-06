using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Web;

namespace Newsfeed.Services
{
    [ServiceContract]
    public interface INewsfeedServiceCallback
    {
        [OperationContract(IsOneWay = true, Action = "*")]
        void Send(Message message);
    }
}