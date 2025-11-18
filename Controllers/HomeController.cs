using System.Linq;
using System.Web.Mvc;
using MiniLibrary.Models;
using MiniLibrary.Models.ViewModels;

namespace MiniLibrary.Controllers
{
    public class HomeController : Controller
    {
        private readonly LibraryContext db = new LibraryContext();

        public ActionResult Index(string q = "", string genre = "", string sort = "alpha")
        {
            var vm = new BookSearchVm
            {
                Query = q,
                Genre = genre,
                SortBy = sort
            };

            var books = db.Books.AsQueryable();

            if (!string.IsNullOrEmpty(q))
            {
                var qq = q.Trim();
                books = books.Where(b => b.Title.Contains(qq) || b.Author.Contains(qq) || b.ISBN.Contains(qq));
            }
            if (!string.IsNullOrEmpty(genre))
            {
                books = books.Where(b => b.Genre == genre);
            }

            switch (sort)
            {
                case "year_desc": books = books.OrderByDescending(b => b.Year); break;
                case "year_asc": books = books.OrderBy(b => b.Year); break;
                default: books = books.OrderBy(b => b.Title); break;
            }

            vm.Results = books.ToList();
            vm.Genres = db.Books.Select(b => b.Genre).Where(g => g != null).Distinct().ToList();
            return View(vm);
        }
    }
}
