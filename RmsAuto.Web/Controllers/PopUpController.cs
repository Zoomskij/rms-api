using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RMSAutoAPI.Controllers
{
    public class PopUpController : Controller
    {
        // GET: PopUp
        public ActionResult GetPopUp()
        {
            return View("UserControl/PopupView"); // The defined View in the project
        }
    }
}