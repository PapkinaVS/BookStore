using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using BookStore.Models;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using System.Configuration;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace BookStore.Controllers
{
    public class SearchController : Controller
    {
        // GET: Search
        public ActionResult Index(string query)
        {
            SearchViewModel searchViewModel;

            var lazyConnection = new Lazy<ConnectionMultiplexer>(() =>
            {
                string cacheConnection = ConfigurationManager.AppSettings["CacheConnection"].ToString();
                return ConnectionMultiplexer.Connect(cacheConnection);
            });
            IDatabase cache = lazyConnection.Value.GetDatabase();
            var cachedValue = cache.StringGet(query);

            if (cachedValue.HasValue)
            {
                searchViewModel = JsonConvert.DeserializeObject<SearchViewModel>(cachedValue.ToString());
            }
            else
            {
                var credentials = new SearchCredentials("2BDBEA23B682B7C4EE0784C74FE839A9");

                var bookIndexClient = new SearchIndexClient("bookstore", "book", credentials);
                var soccerIndexClient = new SearchIndexClient("bookstore", "soccer", credentials);
                var studentsIndexClient = new SearchIndexClient("bookstore", "students", credentials);

                var book = bookIndexClient.Documents.Search<Book>(query).Results;
                //var purchase = bookIndexClient.Documents.Search<Purchase>(query).Results;

                var player = soccerIndexClient.Documents.Search<Player>(query).Results;
                //var team = soccerIndexClient.Documents.Search<Team>(query).Results;

                var student = studentsIndexClient.Documents.Search<Student>(query).Results;

                searchViewModel = new SearchViewModel
                {
                    Books = GetDocuments(book),
                    Players = GetDocuments(player),
                    Students = GetDocuments(student)
                };

                var serialized = JsonConvert.SerializeObject(searchViewModel);
                cache.StringSet(query, serialized, TimeSpan.FromSeconds(120));
                
            }
            return View(searchViewModel);
        }

        public IEnumerable<TModel> GetDocuments<TModel>(IList<SearchResult<TModel>> results) where TModel : class
        {
            return results.Take(5).Select((result) => result.Document);
        }
    }
}