using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Web.Http;
using DurableTask.Core.Stats;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace Teknikfördjupning
{
    public static class OrchestrationFunction
    {
        [FunctionName("OrchestrationFunction")]
        public static async Task<string> RunOrchestrator(
            [OrchestrationTrigger] IDurableOrchestrationContext context)
        {
            string res = "";
            try
            {
                string cocktail = await context.CallActivityAsync<string>("CallApiFunction", "https://www.thecocktaildb.com/api/json/v1/1/random.php");
                string meal = await context.CallActivityAsync<string>("CallApiFunction", "https://www.themealdb.com/api/json/v1/1/random.php");
                await context.CallActivityAsync<string>("ProcessorFunction", (cocktail, meal));
                res = "Done!";
            }
            catch (System.Exception ex)
            {
               res = "Woops! Something went wrong! " + ex.Message;
            }
            return res;
        }

        [FunctionName("GetData")]
        public static async Task ChronStart([TimerTrigger("*/30 * * * * *", RunOnStartup = false)] TimerInfo myTimer,
            [DurableClient] IDurableOrchestrationClient starter,
            ILogger log)
        {
            string instanceId = await starter.StartNewAsync("OrchestrationFunction", null);

            log.LogInformation($"Started orchestration with ID = '{instanceId}'.");
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
        }
    }
}
