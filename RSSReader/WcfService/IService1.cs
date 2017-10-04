using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace WcfService
{
    [ServiceContract]
    public interface IService1
    {
        [OperationContract]
        string[] GetFilenames(string url);
    }

    [DataContract]
    public class Article
    {
        [DataMember]
        public string Title { get; set; }

        [DataMember]
        public string Link { get; set; }

        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public string PubDate { get; set; }
    }

    [DataContract]
    public class Channel
    {
        [DataMember]
        public string Title { get; set; }

        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public string Link { get; set; }
    }
}
