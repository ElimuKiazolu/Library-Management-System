using System.Collections.Generic;

namespace MiniLibrary.Models.ViewModels
{
    public class BookSearchVm
    {
        public string Query { get; set; }
        public string Genre { get; set; }
        public string SortBy { get; set; }
        public List<string> Genres { get; set; }
        public List<MiniLibrary.Models.Book> Results { get; set; }
    }
}
