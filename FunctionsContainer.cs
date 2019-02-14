using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Description;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;


namespace Bitfox.AzureBroadcast.Functions
{

    public static class FunctionsContainer
    {
        [AppSetting(Default = "AzureSignalRConnectionString")]
        private static string azure { get; set; }

        [FunctionName("info")]
        public static string Info(
        [HttpTrigger(AuthorizationLevel.Anonymous)] HttpRequest req)           
        {
            var version = Assembly.GetEntryAssembly().GetName().Version.ToString();
            var signalrsettingfound = azure!="";
            return $"Version  : {version}" + 
                   $"AzureSignalRConnectionString found : {signalrsettingfound}";
        }

        [FunctionName("negotiate")]
        public static SignalRConnectionInfo Negotiate(
            [HttpTrigger(AuthorizationLevel.Anonymous)] HttpRequest req,
            [SignalRConnectionInfo(HubName = "general", UserId = "{headers.x-ms-signalr-userid}")] SignalRConnectionInfo connectionInfo)
        {
            return connectionInfo;
        }


        [FunctionName("broadcast")]
        public static Task Broadcast(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post")]HttpRequest req,
            [SignalR(HubName = "general")]IAsyncCollector<SignalRMessage> signalRMessages)
        {
           
            string message;
            using (StreamReader stream = new StreamReader(req.Body))
            {
                message = stream.ReadToEnd();
            }
            var wrapped = JsonConvert.DeserializeObject<BroadcastMessage>(message);

            return signalRMessages.AddAsync(
                new SignalRMessage
                {
                    UserId = wrapped.toUser,
                    GroupName = wrapped.toGroupName,
                    Target = "newMessage",
                    Arguments = new[] { message }
                });

        }

        [FunctionName("groupaction")]
        public static Task GroupActions(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post")]HttpRequest req,
            [SignalR(HubName = "general")]IAsyncCollector<SignalRGroupAction> signalRGroupAction)
        {
            string gamstring;
            using (StreamReader stream = new StreamReader(req.Body))
            {
                gamstring = stream.ReadToEnd();
            }
            var gam = JsonConvert.DeserializeObject<GroupActionMessage>(gamstring);
            string userId = req.Headers["x-ms-signalr-userid"];
           
            return signalRGroupAction.AddAsync(
                new SignalRGroupAction
                {
                    Action = gam.groupAction,
                    GroupName = gam.groupName,
                    UserId = userId
                });

        }

    }
}
