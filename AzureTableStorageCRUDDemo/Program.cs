using AzureTableStorageCRUDDemo.Models;
using Microsoft.Azure.Cosmos.Table;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace AzureTableStorageCRUDDemo
{
    // https://docs.microsoft.com/en-us/azure/cosmos-db/tutorial-develop-table-dotnet
    // https://docs.microsoft.com/en-us/visualstudio/azure/vs-azure-tools-table-designer-construct-filter-strings?view=vs-2019
    // https://microsoft.github.io/AzureTipsAndTricks/
    class Program
    {
        const string connectionString = "UseDevelopmentStorage=true";
        static async Task Main(string[] args)
        {
            var partitionKey = "8BB39AAA-8C00-43D6-8480-12123A41FA9E";
            var rowKey = "67bd7178-17ad-4b14-80cb-68ab802ab991";

            await GetAllMessages(partitionKey);
            await GetMessage(partitionKey, rowKey);

            Console.WriteLine("Azure Cosmos Table Samples");
            BasicSamples basicSamples = new BasicSamples();
            basicSamples.RunSamples().Wait();

            //AdvancedSamples advancedSamples = new AdvancedSamples();
            //advancedSamples.RunSamples().Wait();

            Console.WriteLine();
            Console.WriteLine("Press any key to exit");
            Console.Read();
        }


        static async Task GetAllMessages(string partitionKey)
        {
            var table = await CreateTableAsync();

            // option 1
            var query = new TableQuery<CustomerEntity>().Where(
                  TableQuery.GenerateFilterCondition(
                    "PartitionKey",
                    QueryComparisons.Equal,
                    partitionKey
                  ));

            //var queryResults = table.ExecuteQuery<CustomerEntity>(query).ToList();

            // option 2
            var queryResults = table.CreateQuery<CustomerEntity>()
                .Where(s => s.PartitionKey == partitionKey)
                .ToList();

            // option 3
            var tableQuery = new TableQuery<CustomerEntity>();
            tableQuery.FilterString = $"PartitionKey eq '{partitionKey}'";
            var tableQueryResults = table.ExecuteQuery<CustomerEntity>(tableQuery).ToList();

            Console.WriteLine("GetAllMessages begin");
            // Iterate the <see cref="Pageable"> to access all queried entities.
            foreach (var entity in queryResults)
            {
                Console.WriteLine(entity.Email);
                Console.WriteLine(entity.PhoneNumber);
            }

            Console.WriteLine("GetAllMessages ends");
        }


        static async Task GetMessage(string partitionKey, string rowKey)
        {
            var table = await CreateTableAsync();
            //Please refer to https://docs.microsoft.com/en-us/rest/api/storageservices/querying-tables-and-entities for more details about query syntax.
            var tableResult = await table.ExecuteAsync(TableOperation.Retrieve<CustomerEntity>(partitionKey, rowKey));
            Console.WriteLine(tableResult);
        }


        static async Task<CloudTable> CreateTableAsync(string tableName = "Customers")
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

            var table = tableClient.GetTableReference(tableName);
            await table.CreateIfNotExistsAsync();
            return table;
        }
    }
}
