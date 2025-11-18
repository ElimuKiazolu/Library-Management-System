using System.Linq;
using System.Web.Mvc;
using MiniLibrary.Models;
using MiniLibrary.Helpers;

namespace MiniLibrary.Controllers
{
    public class AccountController : Controller
    {
        private readonly LibraryContext db = new LibraryContext();

        [HttpGet]
        public ActionResult Login(string returnUrl = null)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string nameOrEmail, string password, string returnUrl = null)
        {
            if (string.IsNullOrWhiteSpace(nameOrEmail) || string.IsNullOrEmpty(password))
            {
                ModelState.AddModelError("", "Enter name/email and password.");
                return View();
            }

            var admin = db.Admins.FirstOrDefault(a => a.Name == nameOrEmail || a.Email == nameOrEmail);
            if (admin != null)
            {
                var hash = SecurityHelper.HashPassword(password, admin.PasswordSalt);
                if (hash == admin.PasswordHash)
                {
                    Session["AdminId"] = admin.Id;
                    Session["AdminName"] = admin.Name;
                    if (!string.IsNullOrWhiteSpace(returnUrl)) return Redirect(returnUrl);
                    return RedirectToAction("Index", "Admin");
                }
            }

            ModelState.AddModelError("", "Invalid credentials.");
            return View();
        }

        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}
