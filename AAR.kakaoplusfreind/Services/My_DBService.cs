using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using OhIlSeokBot.KakaoPlusFriend.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace AAR.kakaoplusfreind.Services
{
    public class My_DBService
    {
        private static readonly string DatabaseId = ConfigurationManager.AppSettings["database"];
        private static readonly string CollectionId = ConfigurationManager.AppSettings["collection"];
        DocumentClient documentClient;

        public My_DBService() {

            documentClient = new DocumentClient(new Uri(ConfigurationManager.AppSettings["endpoint"]), ConfigurationManager.AppSettings["authKey"]);
        }


        public async Task<IEnumerable<ConversationInfo>> readDataFromDocument(ConversationInfo info)
        {

            //await client.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionId), info);
            IDocumentQuery<ConversationInfo> query = documentClient.CreateDocumentQuery<ConversationInfo>(
                UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionId),
                new FeedOptions { MaxItemCount = -1 })
                .Where(d => d.id == info.id)
                .AsDocumentQuery();

            List<ConversationInfo> results = new List<ConversationInfo>();
            while (query.HasMoreResults)
            {
                results.AddRange(await query.ExecuteNextAsync<ConversationInfo>());
            }

            return results;
        }
        public async Task SetInfoAsync(ConversationInfo info)
        {
            await documentClient.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionId), info);            
        }



    }
}