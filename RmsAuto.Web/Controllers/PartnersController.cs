using AutoMapper;
using RMSAutoAPI.App_Data;
using RMSAutoAPI.Helpers;
using RMSAutoAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;

namespace RMSAutoAPI.Controllers
{
    public class PartnersController : ApiController
    {
        private ex_rmsauto_storeEntities db = new ex_rmsauto_storeEntities();
        private SettingsHelper _settingsHelper = new SettingsHelper();

        /// <summary>
		/// Возвращает список партнеров 
		/// </summary>
		/// <response code="200">OK result</response>
		/// <returns></returns>
        [HttpGet]
        [ResponseType(typeof(IEnumerable<Partner>))]
        //[Authorize(Roles = "Client_SearchApi, NoAccess")]
        public IHttpActionResult GetPartners()
        {
			var user = User.Identity;
            var partners = db.spGetFranches();

            if (partners == null)
            {
                return NotFound();
            }

            var partnersMap = Mapper.Map<List<spGetFranches_Result>, List<Partner>>(partners.ToList());


            return Ok(partnersMap);
        }

    }
}
