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
        // GET: Account
        public ActionResult Index()
        {
            List<SelectListItem> items = new List<SelectListItem>();
            var franches = db.spGetFranches().ToList();
            foreach (var franch in franches)
            {
                items.Add(new SelectListItem() { Text = $"{franch.City} {franch.Franch}", Value = franch.InternalFranchName });
            }
            ViewBag.Partners = items;
            return View();
        }

        [HttpGet]
        public PartialViewResult Index3()
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Index(LoginModel model, string returnUrl)
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
                //var token = response.Content
                var bearerToken = $"{token.TokenType} {token.AccessToken}";
            
            
                ViewBag.returnUrl = returnUrl;
                TempData["bearerToken"] = bearerToken;
                TempData["Email"] = user.Email;
                return RedirectToAction("Index2", "Home");
                //return View(model);
            }

            ModelState.AddModelError("", "Неверный логин или пароль.");


            //_userService = new UserService();
            //if (ModelState.IsValid)
            //{
            //    var user = _userService.GetUser(model.Email, model.Password, model.Region);
            //    if (user == null)
            //    {
            //        ModelState.AddModelError("", "Неверный логин или пароль.");
            //    }
            //    else
            //    {
            //        FormsAuthentication.SetAuthCookie(user.Email, true);

            //        ClaimsIdentity identity = new ClaimsIdentity("ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
            //        //var identity = new ClaimsIdentity("Bearer");

            //        identity.AddClaim(new Claim(ClaimTypes.Name, user.Username));
            //        identity.AddClaim(new Claim(ClaimTypes.Email, user.Email));
            //        identity.AddClaim(new Claim("Region", model.Region));

            //        var rolesTechnicalNamesUser = new List<string>();

            //        switch (user.UserRole)
            //        {
            //            case 0:
            //                identity.AddClaim(new Claim(ClaimTypes.Role, "Client"));
            //                break;
            //            case 1:
            //                identity.AddClaim(new Claim(ClaimTypes.Role, "Manager"));
            //                break;
            //            case 2:
            //                identity.AddClaim(new Claim(ClaimTypes.Role, "LimitedManager"));
            //                break;
            //            case 3:
            //                identity.AddClaim(new Claim(ClaimTypes.Role, "NoAccess"));
            //                break;
            //            case 4:
            //                identity.AddClaim(new Claim(ClaimTypes.Role, "Client_SearchApi"));
            //                break;
            //        }

            //        var principal = new GenericPrincipal(identity, rolesTechnicalNamesUser.ToArray());

            //        AuthenticationManager.SignOut();
            //        AuthenticationManager.SignIn(new AuthenticationProperties
            //        {
            //            IsPersistent = true
            //        }, identity);

            //        //var _client = new RestClient("http://localhost:52682");

            //        //var request = new RestRequest("/api/auth/token", Method.POST);

            //        //request.AddQueryParameter("format", "json");
            //        //request.RequestFormat = DataFormat.Json;
            //        //request.AddParameter("username", "api");
            //        //request.AddParameter("password", "123");
            //        //request.AddParameter("grant_type", "password");

            //        //var response = _client.Execute<JObject>(request);


            //        Thread.CurrentPrincipal = principal;
            //        return RedirectToAction("Index2", "Home");
            //    }
            //}


            //  return response;

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