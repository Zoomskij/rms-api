using AutoMapper;
using RMSAutoAPI.App_Data;
using RMSAutoAPI.Helpers;
using RMSAutoAPI.Infrastructure;
using RMSAutoAPI.Models;
using RMSAutoAPI.Properties;
using RMSAutoAPI.Services;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;

namespace RMSAutoAPI.Controllers
{
    [RoutePrefix("api")]
    public class ArticlesController : ApiController
    {
        private ex_rmsauto_storeEntities db = new ex_rmsauto_storeEntities();
        private SettingsHelper _settingsHelper = new SettingsHelper();
        private ILoggerSerivce _log = new LoggerService();

        public Users CurrentUser { get; set; }
        public Settings CurrentSettings { get; set; }
        public int PerMinute { get; set; }
        public int PerHour { get; set; }
        public int PerDay { get; set; }
        public string DbName { get; set; }
        public string ServerName { get; set; }
        

        public string CurrentRole { get; set; } 

        public ArticlesController()
        {


        }

        public string Init()
        {
            var claims = (ClaimsIdentity)User.Identity;
            var region = claims.Claims.FirstOrDefault(x => x.Type.Equals("Region"))?.Value;

            if (!string.IsNullOrWhiteSpace(region) && !region.Equals("rmsauto"))
            {
                var currentFranch = db.spGetFranches().FirstOrDefault(x => x.InternalFranchName.ToUpper().Equals(region.ToUpper()));
                db.ChangeDatabase(initialCatalog: $"ex_{currentFranch.DbName}_store", dataSource: $"{currentFranch.ServerName}");
                DbName = currentFranch.DbName;
                ServerName = currentFranch.ServerName;
            }
            var userName = User.Identity.Name;
            CurrentUser = db.Users.FirstOrDefault(x => x.Username == userName || x.Email == userName);
            CurrentSettings = db.Settings.FirstOrDefault(x => x.UserId == CurrentUser.UserID);
            PerMinute = _settingsHelper.CountRequests(RequestDelimeter.Minute, CurrentUser.AcctgID);
            PerHour = _settingsHelper.CountRequests(RequestDelimeter.Hour, CurrentUser.AcctgID);
            PerDay = _settingsHelper.CountRequests(RequestDelimeter.Day, CurrentUser.AcctgID);
            return region;
        }

        /*/// <response code="500">Internal Server Error</response> - пока убрал, т.к. хорошо бы добавить обработку ошибок и тогда уже добавить описание всех возможных кодов ошибок*/
        /// <summary>
        /// Возвращает список брендов по артикулу
        /// </summary>
        /// <param name="article">Артикул (номер запчасти).</param>
        /// <param name="analogues">Искать аналоги. False - поиск без аналогов (значение по умолчанию). True - поиск с аналогами.</param>
        /// <remarks>Возвращает только те бренды по которым имеются     в наличие детали (на складе или у транзитных поставщиков). Для авторизации в HTTP-заголовок необходимо передавать пару key = "Authorization" value = "Bearer %ВАШ АВТОРИЗАЦИОННЫЙ ТОКЕН%"</remarks>
        /// <response code="200">OK result</response>
        /// <returns></returns>
        [HttpGet]
        [ResponseType(typeof(IEnumerable<Brand>))]
        [Authorize(Roles = "Client_SearchApi, NoAccess")]
        [Route("articles/{article:maxlength(50)}/brands")]
        public IHttpActionResult GetBrands(string article, bool analogues = false)
        {
            var region = Init();
            //Если токен еще не истек, а мы забрали права доступа то отсеиваем пользователя
            if (!CurrentUser.Permissions.Any(x => x.ID == 3 || x.ID == 4))
            {
                return Content(HttpStatusCode.Forbidden, Resources.ErrorAccessDenied);
            }

            if (CurrentSettings != null)
            {
                if (_settingsHelper.IsAccess(PerMinute, PerHour, PerDay, CurrentSettings.Rates.PerMinute, CurrentSettings.Rates.PerHour, CurrentSettings.Rates.PerDay) == false)
                    return Content(HttpStatusCode.Forbidden, Resources.ErrorMaxRequests);
            }
            try
            {
                var brands = db.spSearchBrands(article, analogues, CurrentUser.AcctgID, region);
                if (brands == null)
                {
                    return NotFound();
                }
                _log.Add(article, string.Empty, HttpContext.Current.Request.UserHostAddress, Resources.LogTypeBrand, CurrentUser.AcctgID, DbName, ServerName);

                var brandsMap = Mapper.Map<List<spSearchBrands_Result>, List<Brand>>(brands.ToList());

                return Ok(brandsMap);
            }
            catch (Exception ex)
            {
                var message = ex.InnerException.Message;
                return Content(HttpStatusCode.BadRequest, message);
            }
        }

        /*/// <response code="500">Internal Server Error</response> - пока убрал, т.к. хорошо бы добавить обработку ошибок и тогда уже добавить описание всех возможных кодов ошибок*/
        /// <summary>
        /// Возвращает список запчастей
        /// </summary>
        /// <param name="article">Артикул (номер запчасти).</param>
        /// <param name="brand">Бренд</param>
        /// <param name="analogues">Искать аналоги. False - поиск без аналогов (значение по умолчанию). True - поиск с аналогами.</param>
        /// <param name="franch">Internal Franch Name. rmsauto </param>
        /// <remarks>Для авторизации в HTTP-заголовок необходимо передавать пару key = "Authorization" value = "Bearer %ВАШ АВТОРИЗАЦИОННЫЙ ТОКЕН%"</remarks>
        /// <response code="200">OK result</response>
        /// <returns></returns>
        [HttpGet]
        [ResponseType(typeof(IEnumerable<SparePart>))]
        [Authorize(Roles = "Client_SearchApi, NoAccess")]
        [Route("articles/{article:maxlength(50)}/brand/{*brand:maxlength(50)}")]
        public IHttpActionResult GetSpareParts(string article, string brand, bool analogues = false)
        {
            var mainBrand = db.BrandEquivalents.FirstOrDefault(x => x.Equivalent.Equals(brand))?.Brand;
            mainBrand = string.IsNullOrWhiteSpace(mainBrand) ? brand : mainBrand;
            var region = Init();
            //Если токен еще не истек, а мы забрали права доступа то отсеиваем пользователя
            if (!CurrentUser.Permissions.Any(x => x.ID == 3 || x.ID == 4))
            {
                return Content(HttpStatusCode.Forbidden, Resources.ErrorAccessDenied);
            }

            if (CurrentSettings != null)
            {
                if (_settingsHelper.IsAccess(PerMinute, PerHour, PerDay, CurrentSettings.Rates.PerMinute, CurrentSettings.Rates.PerHour, CurrentSettings.Rates.PerDay) == false)
                    return Content(HttpStatusCode.Forbidden, Resources.ErrorMaxRequests);
            }
            // Подменяем введенный бренд на наш Main brand

            try
            {
                var crosses = db.spSearchCrossesWithPriceSVC(article, brand, analogues, string.Empty, CurrentUser.AcctgID, CurrentUser.ClientGroup, region);
                if (crosses == null)
                {   
                    return NotFound();
                }
                _log.Add(article, brand, HttpContext.Current.Request.UserHostAddress, Resources.LogTypePartNumber, CurrentUser.AcctgID, DbName, ServerName);

                // По быстрому отключил нашу уценку (склады 1212 и 1215)
                var crossesMap = Mapper.Map<List<spSearchCrossesWithPriceSVC_Result>, List<SparePart>>(crosses.Where(x => x.SupplierID != 1212 && x.SupplierID != 1215).ToList());

                return Ok(crossesMap);
            }
            catch (Exception ex)
            {
                var message = ex.InnerException.Message;
                return Content(HttpStatusCode.BadRequest, message);
            }
        }
    }
}
