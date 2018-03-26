using RMSAutoAPI.App_Data;
using RMSAutoAPI.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RMSAutoAPI.Controllers
{
    public class StatsController : Controller
    {
        private ex_rmsauto_storeEntities db = new ex_rmsauto_storeEntities();
        private SettingsHelper _settingsHelper = new SettingsHelper();

        public static Users CurrentUser { get; set; }
        public Settings CurrentSettings { get; set; }


        //В будущем этот контроллер предполагалось сделать для отображения статистики по пользователям
        //Текущая авторизация которая используется для WEB API не подходит (смотреть: ApiController и Controller)
        //TO DO: Create model for view and config credentials for Controller
        //Authorize(Roles = "admin,manager")]
        public ActionResult Index(string currentUser)
        {
            //var userName = User.Identity.Name;
            if (!string.IsNullOrWhiteSpace(currentUser))
            {
                CurrentUser = db.Users.FirstOrDefault(x => x.Email == currentUser);
            }
            if (CurrentUser != null)
            {
                CurrentSettings = db.Settings.FirstOrDefault(x => x.UserId == CurrentUser.UserID);

                ViewBag.PerMinute = _settingsHelper.CountRequests(RequestDelimeter.Minute, CurrentUser.AcctgID);
                ViewBag.PerHour = _settingsHelper.CountRequests(RequestDelimeter.Hour, CurrentUser.AcctgID);
                ViewBag.PerDay = _settingsHelper.CountRequests(RequestDelimeter.Day, CurrentUser.AcctgID);

                ViewBag.AllowInMinute = db.Settings.FirstOrDefault(x => x.UserId == CurrentUser.UserID)?.Rates?.PerMinute.ToString() ?? "∞";
                ViewBag.AllowInHour = db.Settings.FirstOrDefault(x => x.UserId == CurrentUser.UserID)?.Rates?.PerHour.ToString() ?? "∞";
                ViewBag.AllowInDay = db.Settings.FirstOrDefault(x => x.UserId == CurrentUser.UserID)?.Rates?.PerDay.ToString() ?? "∞";
            }
            ViewBag.CurrentUser = currentUser;
            return View();
        }
    }
}