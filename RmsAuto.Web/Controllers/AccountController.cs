﻿using Microsoft.Owin.Security;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using RMSAutoAPI.App_Data;
using RMSAutoAPI.Models;
using RMSAutoAPI.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;

namespace RMSAutoAPI.Controllers
{
    // В данный момент контроллер не работает, необходимо переписывать авторизацию 
    // либо выносить в отдельную сборку
    public class AccountController : Controller
    {
        private ex_rmsauto_storeEntities db = new ex_rmsauto_storeEntities();
        private static List<SelectListItem> franchList = new List<SelectListItem>();

        private IUserService _userService;

        [HttpGet]
        public ActionResult Login()
        {
            if (!franchList.Any())
            {
                var franches = db.spGetFranches().ToList();
                if (!franchList.Any())
                foreach (var franch in franches)
                {
                    franchList.Add(new SelectListItem() { Text = $"{franch.City} {franch.Franch}", Value = franch.InternalFranchName });
                }
            }
            ViewBag.Partners = franchList;
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
        public async Task<ActionResult> LoginAuth(string username, string password, string code)
        {
            _userService = new UserService();
            var client = new RestClient(WebConfigurationManager.AppSettings["UrlApi"]);

            var request = new RestRequest("/api/auth/token", Method.POST);

            request.AddQueryParameter("format", "json");
            request.RequestFormat = DataFormat.Json;
            request.AddParameter("username", username);
            request.AddParameter("password", password);
            request.AddParameter("code", code);
            request.AddParameter("grant_type", "password");

            var response = client.Execute<JObject>(request);

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var token = JsonConvert.DeserializeObject<Token>(response.Content);
                var bearerToken = $"{token.TokenType} {token.AccessToken}";

                TempData["Token"] = bearerToken;
                TempData["UserName"] = username;
                TempData["Code"] = code;
                
                return RedirectToAction("Index", "Home");
            }
            ModelState.AddModelError("", "Неверный логин или пароль.");
            return View();
        }
    }
}