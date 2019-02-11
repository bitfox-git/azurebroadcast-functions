using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;


namespace Bitfox.AzureBroadcast.Functions
{

    public static class FunctionsContainer
    {
       
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
           
            return signalRMessages.AddAsync(
                new SignalRMessage
                {
                    //Only broadcasting to all connected clients. No direct user or group based communication. 
                    //UserId = ,
                    //GroupName = ,

                    //Target is the method name on the clientside.
                    Target = "newMessage",
                    Arguments = new[] { message }
                });

        }

    }
}
