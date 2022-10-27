using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net.Http;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;

namespace Teknikfördjupning
{
    public static class CallApiFunction
    {
        [FunctionName("CallApiFunction")]
        public static async Task<string> Run([ActivityTrigger] string website)
        {
            HttpClient client = new HttpClient();
            var res = await client.GetStringAsync(website);
            client.Dispose();

            return res; 
        }
    }
}
