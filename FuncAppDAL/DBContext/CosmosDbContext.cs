using FuncAppDAL.Models;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;

namespace FuncAppDAL.DBContext
{
    public class CosmosDbContext
    {
        private readonly string EndpointUri = Environment.GetEnvironmentVariable("CosmosDbEndpointUri")
            .ToString();
        // The primary key for the Azure Cosmos account.
        private readonly string PrimaryKey = Environment.GetEnvironmentVariable("CosmosDbPrimaryKey")
            .ToString();
        // The name of the database and container we will create
        string databaseName = Environment.GetEnvironmentVariable("CosmosDatabaseName").ToString();
        string CustomerCollectionName = "Customers";
        MongoClient client;
        private readonly IMongoClient _client;
        private readonly IMongoDatabase _database;

        public CosmosDbContext()
        {
            MongoClientSettings settings = new MongoClientSettings();
            //settings.Server = new MongoServerAddress(EndpointUri, 10255);
            //settings.UseTls = true;
            //settings.SslSettings = new SslSettings();
            //settings.SslSettings.EnabledSslProtocols = SslProtocols.Tls12;

            MongoIdentity identity = new MongoInternalIdentity(databaseName, Environment.GetEnvironmentVariable("CosmosDbUserName").ToString());
            MongoIdentityEvidence evidence = new PasswordEvidence(PrimaryKey);
            settings.Credential = new MongoCredential("SCRAM-SHA-1", identity, evidence);
            client = new MongoClient(Environment.GetEnvironmentVariable("CosmosDbConnectionString"));
            _database = client.GetDatabase(databaseName);
            _client = client;
        }
        public IMongoCollection<Customer> GetCustomers
        {
            get { return _database.GetCollection<Customer>(CustomerCollectionName); }
        }

    }
}
