using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Owin;
using RestSharp;
using RMSAutoAPI.App_Data;
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
using System.Web.Configuration;
using System.Web.Mvc;
using System.Web.Security;

namespace RMSAutoAPI.Controllers
{
    // В данный момент контроллер не работает, необходимо переписывать авторизацию 
    // либо выносить в отдельную сборку
    public class AccountController : Controller
    {

        // public UserManager<ApplicationUser> UserManager { get; private set; }
        private ex_rmsauto_storeEntities db = new ex_rmsauto_storeEntities();

        private IUserService _userService;

        [HttpGet]
        public ActionResult Login()
        {
            List<SelectListItem> items = new List<SelectListItem>();
            var franches = db.spGetFranches().ToList();
            foreach (var franch in franches)
            {
                items.Add(new SelectListItem() { Text = $"{franch.City} {franch.Franch}", Value = franch.InternalFranchName });
            }
            ViewBag.Partners = items;
            return PartialView();
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

        public RedirectResult Logout()
        {
            TempData["logout"] = 1;
            AuthenticationManager.SignOut();
            return RedirectPermanent("/");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginModel model, string returnUrl)
        {
            _userService = new UserService();
            var _client = new RestClient(WebConfigurationManager.AppSettings["UrlApi"]);

            var request = new RestRequest("/api/auth/token", Method.POST);

            request.AddQueryParameter("format", "json");
            request.RequestFormat = DataFormat.Json;
            request.AddParameter("username", model.Email);
            request.AddParameter("password", model.Password);
            request.AddParameter("region", model.Region);
            request.AddParameter("grant_type", "password");

            var response = _client.Execute<JObject>(request);

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var user = _userService.GetUser(model.Email, model.Password, model.Region);
                var token = JsonConvert.DeserializeObject<Token>(response.Content);
                var bearerToken = $"{token.TokenType} {token.AccessToken}";
            
            
                ViewBag.returnUrl = returnUrl;
                TempData["bearerToken"] = bearerToken;
                TempData["Email"] = user.Email;
                return RedirectToAction("Index2", "Home");
            }

            ModelState.AddModelError("", "Неверный логин или пароль.");

            ViewBag.returnUrl = returnUrl;
            return View(model);
        }
    }
}