using System;
using System.Linq;
using System.Web.Mvc;
using MiniLibrary.Models;
using MiniLibrary.Helpers; // keep if you have SecurityHelper
using System.Net;

namespace MiniLibrary.Controllers
{
    public class AdminController : Controller
    {
        private readonly LibraryContext db = new LibraryContext();

        // allow createadmin / saveadmin / login without session
        private bool IsAdminLogged => Session["AdminId"] != null;

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var actionName = filterContext.ActionDescriptor.ActionName.ToLower();
            var allowed = actionName == "createadmin" || actionName == "saveadmin" || actionName == "login";
            if (!allowed && !IsAdminLogged)
            {
                filterContext.Result = RedirectToAction("Login", "Account");
                return;
            }
            base.OnActionExecuting(filterContext);
        }

        // Index: show list of books (simple IEnumerable<Book>)
        public ActionResult Index()
        {
            var books = db.Books.OrderByDescending(b => b.CreatedAt).ToList();
            return View(books);
        }

        [HttpGet]
        public ActionResult CreateAdmin()
        {
            if (db.Admins.Any())
            {
                return RedirectToAction("Login", "Account");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveAdmin(string name, string email, string password)
        {
            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(password))
            {
                ModelState.AddModelError("", "Name and password required");
                return View("CreateAdmin");
            }

            // use your existing SecurityHelper if available
            var salt = SecurityHelper.GenerateSalt();
            var hash = SecurityHelper.HashPassword(password, salt);

            var admin = new Admin
            {
                Name = name.Trim(),
                Email = string.IsNullOrWhiteSpace(email) ? null : email.Trim(),
                PasswordSalt = salt,
                PasswordHash = hash,
                CreatedAt = DateTime.UtcNow
            };

            db.Admins.Add(admin);
            db.SaveChanges();

            Session["AdminId"] = admin.Id;
            Session["AdminName"] = admin.Name;

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddBook(Book model)
        {
            if (!ModelState.IsValid) return RedirectToAction("Index");

            if (model.CreatedAt == default(DateTime)) model.CreatedAt = DateTime.UtcNow;

            db.Books.Add(model);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteBook(int id)
        {
            var b = db.Books.Find(id);
            if (b != null)
            {
                db.Books.Remove(b);
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }
        // GET: Admin/Edit/5
        [HttpGet]
        public ActionResult Edit(int id)
        {
            var book = db.Books.Find(id);
            if (book == null) return HttpNotFound();
            return View(book); // expects Views/Admin/Edit.cshtml
        }

        // POST: Admin/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Book model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var existing = db.Books.Find(model.Id);
            if (existing == null) return HttpNotFound();

            // Update allowed fields only (avoid overwriting CreatedAt unintentionally)
            existing.Title = model.Title?.Trim();
            existing.Author = model.Author?.Trim();
            existing.Genre = model.Genre?.Trim();
            existing.Year = model.Year;
            existing.ISBN = model.ISBN?.Trim();
            existing.Rating = model.Rating;

            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}
