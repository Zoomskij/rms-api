using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using RMSAutoAPI.Models;
using RMSAutoAPI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace RMSAutoAPI.Controllers
{
    // В данный момент контроллер не работает, необходимо переписывать авторизацию 
    // либо выносить в отдельную сборку
    public class AccountController : Controller
    {

       // public UserManager<ApplicationUser> UserManager { get; private set; }

        private IUserService _userService;
        // GET: Account
        public ActionResult Index()
        {
            return View();
        }

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        public ActionResult Login(string returnUrl)
        {
            ViewBag.returnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Index(LoginModel model, string returnUrl)
        {
            _userService = new UserService();
            if (ModelState.IsValid)
            {
                var user = _userService.GetUser(model.Email, model.Password);
                if (user == null)
                {
                    ModelState.AddModelError("", "Неверный логин или пароль.");
                }
                else
                {
                    var identity = new ClaimsIdentity("Bearer");

                    identity.AddClaim(new Claim(ClaimTypes.Name, user.Username));
                    identity.AddClaim(new Claim(ClaimTypes.Email, user.Email));

                    var rolesTechnicalNamesUser = new List<string>();

                    switch (user.UserRole)
                    {
                        case 0:
                            identity.AddClaim(new Claim(ClaimTypes.Role, "admin"));
                            identity.AddClaim(new Claim(ClaimTypes.Role, "manager"));
                            break;

                    }

                    var principal = new GenericPrincipal(identity, rolesTechnicalNamesUser.ToArray());

                    Thread.CurrentPrincipal = principal;
                    return RedirectToAction("Index2", "Home");
                }
            }
            ViewBag.returnUrl = returnUrl;
            return View(model);
        }
        public ActionResult Logout()
        {
            AuthenticationManager.SignOut();
            return RedirectToAction("Login");
        }
    }
}