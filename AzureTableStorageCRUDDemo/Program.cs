using System;
using System.Threading.Tasks;

namespace AzureTableStorageCRUDDemo
{
    // https://docs.microsoft.com/en-us/azure/cosmos-db/tutorial-develop-table-dotnet
    // https://docs.microsoft.com/en-us/visualstudio/azure/vs-azure-tools-table-designer-construct-filter-strings?view=vs-2019
    // https://microsoft.github.io/AzureTipsAndTricks/
    class Program
    {
        static async Task Main(string[] args)
        {
            //Console.WriteLine("Azure Cosmos Table Samples");
            //BasicSamples basicSamples = new BasicSamples();
            //basicSamples.RunSamples().Wait();

            var partitionKey = "Armen";
            var rowKey = "Hovsepian";
            var customerRepository = new CustomerRepository();
            //await customerRepository.GetAllMessages(partitionKey);
            //await customerRepository.GetMessage(partitionKey, rowKey);
            await customerRepository.ThrowingExceptionIfRowKeysAreSameInBatchAsync();

            Console.WriteLine("Press any key to exit");
            Console.Read();
        }
    }
}
