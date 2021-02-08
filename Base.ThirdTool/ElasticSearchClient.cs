using Microsoft.Extensions.Configuration;
using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Base.ThirdTool
{
    public class ElasticSearchClient
    {
        private  ElasticClient _client;
        private readonly IConfiguration _configuration;

        public ElasticSearchClient(IConfiguration configuration)
        {
            _configuration = configuration;
            var settings = new ConnectionSettings(new Uri(_configuration["ElasticSearch:Url"]))
                .DefaultIndex(_configuration["ElasticSearch:DefaultIndex"]);
            _client = new ElasticClient(settings);
        }
        public ElasticClient GetEsClient()
        {
            return _client;
        }
        public void Send<T>(List<T> model) where T : class
        {
            _client.IndexMany(model);
        }

        public void InsertOrUpdata<T>(T model) where T : class
        {
            var a= _client.IndexDocument(model);
        }

        public bool Delete<T>(string id) where T : class
        {

            var response = _client.Delete<T>(id);
            return response.IsValid;
        }
        public bool DropIndex(string indexName)
        {
            return _client.Indices.Delete(Indices.Parse(indexName)).IsValid;
        }
        public void CreateIndex(string indexName)
        {
            var settings = new ConnectionSettings(new Uri(_configuration["ElasticSearch:Url"]))
                .DefaultIndex(indexName);
            _client = new ElasticClient(settings);
        }
    }
}
