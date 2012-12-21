using System;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace WcfWithDI.Interfaces
{
    [ServiceContract]
    public interface IService1
    {

        [OperationContract]
        [WebGet(BodyStyle = WebMessageBodyStyle.Wrapped,
            ResponseFormat = WebMessageFormat.Json,
            RequestFormat=WebMessageFormat.Json,
            UriTemplate = "GetData")]
        string GetData();
    }
}

