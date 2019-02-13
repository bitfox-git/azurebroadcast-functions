using Microsoft.Azure.WebJobs.Extensions.SignalRService;

namespace Bitfox.AzureBroadcast
{

    public class GroupActionMessage {
        public GroupAction groupAction { get;set;}
        public string groupName {get;set;}
    }
}

