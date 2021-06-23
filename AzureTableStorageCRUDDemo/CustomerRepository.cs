using AzureTableStorageCRUDDemo.Models;
using Microsoft.Azure.Cosmos.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AzureTableStorageCRUDDemo
{
    public class CustomerRepository
    {
        public async Task GetAllMessages(string partitionKey)
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


        public async Task GetMessage(string partitionKey, string rowKey)
        {
            var table = await CreateTableAsync();
            //Please refer to https://docs.microsoft.com/en-us/rest/api/storageservices/querying-tables-and-entities for more details about query syntax.
            var tableResult = await table.ExecuteAsync(TableOperation.Retrieve<CustomerEntity>(partitionKey, rowKey));
            Console.WriteLine(tableResult);
        }


        private async Task<CloudTable> CreateTableAsync(string tableName = "Customers")
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(AppSettings.ConnectionString);
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

            var table = tableClient.GetTableReference(tableName);
            await table.CreateIfNotExistsAsync();
            return table;
        }


        public async Task ThrowingExceptionIfRowKeysAreSameInBatchAsync()
        {
            try
            {
                var table = await CreateTableAsync();

                var lastname = Guid.NewGuid().ToString();
                var firstname = Guid.NewGuid().ToString();

                var batches = new List<CustomerEntity>
            {
                new CustomerEntity(lastname, firstname),
                new CustomerEntity(lastname, firstname),
                new CustomerEntity(lastname, firstname)
            };

                TableBatchOperation batchOperationObj = new TableBatchOperation();
                foreach (var batch in batches)
                {
                    batchOperationObj.InsertOrReplace(batch);
                }

                await table.ExecuteBatchAsync(batchOperationObj);
            }
            catch (StorageException e)
            {

                throw;
            }

        }
    }
}
