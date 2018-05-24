using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BookStore.Models
{
    public class SearchViewModel
    {
        public IEnumerable<Book> Books { get; set; }
        public IEnumerable<Player> Players { get; set; }
        public IEnumerable<Student> Students { get; set; }
    }
}