using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using FuncAppDAL.Models;
using FuncAppDAL.Services;
using System.Net;
using System.Net.Http;
using FuncAppDAL.Interfaces;
using Microsoft.CSharp.RuntimeBinder;
using Newtonsoft.Json.Linq;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Queue;

namespace DemoFuncApp
{
    public class SignUpIntoPortal
    {
        public ICosmosService _cosmosSvc;
        public SignUpIntoPortal(ICosmosService cosmosSvc)
        {
            _cosmosSvc = cosmosSvc;
        }
        [FunctionName("SignUpIntoPortal")]
        public async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            try
            {
                log.LogInformation("C# HTTP trigger function SignUpIntoPortal processed a request.");
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                if (requestBody == null)
                {
                    log.LogInformation("SignUpIntoPortal function failed");
                    return new HttpResponseMessage(HttpStatusCode.BadRequest);
                }
                JObject requestData = (JObject)JsonConvert.DeserializeObject(requestBody.Trim());

                await AddCustomerDetailsIntoAzureQueue(log, requestData);
                var obj = JsonConvert.DeserializeObject<Customer>(requestBody);

                var result = _cosmosSvc.AddOrUpdateCustomerDetailsIntoDb(obj);
                if (result == true)
                {
                    log.LogInformation("Customer Details has been Updated for" + obj.FirstName);

                }
                else
                {
                    log.LogInformation("Details not added");
                }

            }
            catch (Exception ex)
            {
                log.LogError("Error while inserting object for Customer" + ex.Message);
                return new HttpResponseMessage(HttpStatusCode.InternalServerError);
            }
            return new HttpResponseMessage(HttpStatusCode.Created);
        }
        public async Task<bool> AddCustomerDetailsIntoAzureQueue(ILogger log, JObject requestData)
        {
            try
            {
                string FirstName = requestData.GetValue("FirstName")?.ToString();
                string EmailAddress = requestData.GetValue("EmailAddress")?.ToString();

                string storageAccountName = Environment.GetEnvironmentVariable("StorageAccountName");
                string storageAccountKey = Environment.GetEnvironmentVariable("StorageAccountKey");
                var storageAccount = new CloudStorageAccount(new StorageCredentials(storageAccountName, storageAccountKey), true);

                CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();
                CloudQueue queue = queueClient.GetQueueReference("userdata");
                // If not exist then Create Queue.  
                await queue.CreateIfNotExistsAsync();
                var queuemessage = FirstName + " "+EmailAddress;
                var encodedqueuemessagebyte = System.Text.Encoding.UTF8.GetBytes(queuemessage);
                var encodedmessage = System.Convert.ToBase64String(encodedqueuemessagebyte);
                CloudQueueMessage message = new CloudQueueMessage(encodedmessage);
                await queue.AddMessageAsync(message);
                log.LogInformation("Message has been inserted into Queue");
                return true;
            }
            catch (Exception ex)
            {
                log.LogError("Messgae insertion Failed" + ex.Message);
                return false;
            }
        }

    }
}
