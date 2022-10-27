using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using System.Linq;
using System.Dynamic;
using Microsoft.Azure.WebJobs.Extensions.Storage;

namespace Teknikfördjupning
{
    public static class ProcessorFunction
    {
        public class Data
        {
            public string PartitionKey { get; set; }
            public string RowKey { get; set; }
            public string Text { get; set; }
        }

        [FunctionName("ProcessorFunction")]
        public static async Task<string> Run([ActivityTrigger] (string cocktail, string meal) input,
            [Table("Output"), StorageAccount("AzureWebJobsStorage")] ICollector<ProcessorFunction.Data> msg)
            //StorageAccount skrivas om till connectionstring till azure storage account. den ligger i local.settings.json
            //Table vill skrivas om också, det ska vara tabellen som vi vill skriva till
        {
            string res = "";
            try
            {
                var jsonCocktail = JsonConvert.DeserializeObject<dynamic>(input.cocktail);
                var jsonMeal = JsonConvert.DeserializeObject<dynamic>(input.meal);

                string drinkName = jsonCocktail.drinks[0]["strDrink"];
                string drinkImg = jsonCocktail.drinks[0]["strDrinkThumb"];

                string mealName = jsonMeal.meals[0]["strMeal"];
                string mealImg = jsonMeal.meals[0]["strMealThumb"];

                string data = drinkName + "|" + drinkImg + "|" + mealName + "|" + mealImg;
                string json = JsonConvert.SerializeObject(data);

                msg.Add(new ProcessorFunction.Data { PartitionKey = "http", RowKey = Guid.NewGuid().ToString(), Text = json});

                res = "done!";
            }
            catch (Exception x)
            {
                res = "Oops! something went wrong! " + x.Message;
            }
            return res;
        }
    }
}
