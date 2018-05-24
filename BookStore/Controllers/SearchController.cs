using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using BookStore.Models;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;

namespace BookStore.Controllers
{
    public class SearchController : Controller
    {
        // GET: Search
        public ActionResult Index(string query)
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

            return View(new SearchViewModel
            {
                Books = GetDocuments(book),
                Players = GetDocuments(player),
                Students = GetDocuments(student)
            });
        }

        public IEnumerable<TModel> GetDocuments<TModel>(IList<SearchResult<TModel>> results) where TModel : class
        {
            return results.Take(5).Select((result) => result.Document);
        }
    }
}