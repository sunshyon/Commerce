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
        private readonly ElasticClient _client;

        public ElasticSearchClient(IConfiguration configuration)
        {
            var settings = new ConnectionSettings(new Uri(configuration.GetConnectionString("ElasticSearch"))).DefaultIndex("TestIndex");
            _client = new ElasticClient(settings);
        }
    }
}
