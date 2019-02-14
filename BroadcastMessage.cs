
using Newtonsoft.Json;

namespace Bitfox.AzureBroadcast.Functions
{
     public class BroadcastMessage {
            
            [JsonProperty(PropertyName = "jm")]
            public string jsonmessage {get;set;}
            [JsonProperty(PropertyName = "fu")]
            public string fromUser {get;set;}
            [JsonProperty(PropertyName = "tu")]
            public string toUser {get;set;}
            [JsonProperty(PropertyName = "gn")]
            public string toGroupName {get;set;}

        }
}