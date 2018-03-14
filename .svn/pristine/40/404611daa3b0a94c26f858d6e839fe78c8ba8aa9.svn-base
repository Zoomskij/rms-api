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

        public Users CurrentUser { get; set; }
        public Settings CurrentSettings { get; set; }


        //В будущем этот контроллер предполагалось сделать для отображения статистики по пользователям
        //Текущая авторизация которая используется для WEB API не подходит (смотреть: ApiController и Controller)
        //TO DO: Create model for view and config credentials for Controller
        //Authorize(Roles = "admin,manager")]
        public ActionResult Index()
        {
            var userName = User.Identity.Name;
            CurrentUser = db.Users.FirstOrDefault(x => x.Username == userName);
            CurrentUser.AcctgID = "000042078";
            CurrentUser = db.Users.FirstOrDefault(x => x.AcctgID == "000042078");

            CurrentSettings = db.Settings.FirstOrDefault(x => x.UserId == CurrentUser.UserID);

            ViewBag.PerMinute = _settingsHelper.CountRequests(RequestDelimeter.Minute, CurrentUser.AcctgID);
            ViewBag.PerHour = _settingsHelper.CountRequests(RequestDelimeter.Hour, CurrentUser.AcctgID);
            ViewBag.PerDay = _settingsHelper.CountRequests(RequestDelimeter.Day, CurrentUser.AcctgID);

            ViewBag.AllowInMinute = db.Settings.FirstOrDefault(x => x.UserId == CurrentUser.UserID).Rates.PerMinute;  
            ViewBag.AllowInHour = db.Settings.FirstOrDefault(x => x.UserId == CurrentUser.UserID).Rates.PerHour;
            ViewBag.AllowInDay = db.Settings.FirstOrDefault(x => x.UserId == CurrentUser.UserID).Rates.PerDay;

            return View();
        }
    }
}