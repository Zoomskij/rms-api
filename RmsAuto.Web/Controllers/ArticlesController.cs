﻿using AutoMapper;
using RMSAutoAPI.App_Data;
using RMSAutoAPI.Helpers;
using RMSAutoAPI.Models;
using RMSAutoAPI.Properties;
using RMSAutoAPI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
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

        public ArticlesController()
        {
            var userName = User.Identity.Name;
            CurrentUser = db.Users.FirstOrDefault(x => x.Username == userName);
            CurrentSettings = db.Settings.FirstOrDefault(x => x.UserId == CurrentUser.UserID);
            PerMinute = _settingsHelper.CountRequests(RequestDelimeter.Minute, CurrentUser.AcctgID);
            PerHour = _settingsHelper.CountRequests(RequestDelimeter.Hour, CurrentUser.AcctgID);
            PerDay = _settingsHelper.CountRequests(RequestDelimeter.Day, CurrentUser.AcctgID);
        }

		/*/// <response code="500">Internal Server Error</response> - пока убрал, т.к. хорошо бы добавить обработку ошибок и тогда уже добавить описание всех возможных кодов ошибок*/
		/// <summary>
		/// Возвращает список брендов по артикулу
		/// </summary>
		/// <param name="article">Артикул (номер запчасти).</param>
		/// <param name="analogues">Искать аналоги. False - поиск без аналогов (значение по умолчанию). True - поиск с аналогами.</param>
		/// <remarks>Возвращает только те бренды по которым имеются в наличие детали (на складе или у транзитных поставщиков). Для авторизации в HTTP-заголовок необходимо передавать пару key = "Authorization" value = "Bearer %ВАШ АВТОРИЗАЦИОННЫЙ ТОКЕН%"</remarks>
		/// <response code="200">OK result</response>
		/// <returns></returns>
		[HttpGet]
        [ResponseType(typeof(IEnumerable<Brand>))]
        [Authorize(Roles = "Client_SearchApi, NoAccess")]
        [Route("articles/{article:maxlength(50)}/brands")]
        public IHttpActionResult GetBrands(string article, bool analogues = false)
        {
            if (CurrentSettings != null)
            {
                if (_settingsHelper.IsAccess(PerMinute, PerHour, PerDay, CurrentSettings.Rates.PerMinute, CurrentSettings.Rates.PerHour, CurrentSettings.Rates.PerDay) == false)
                    return Content(HttpStatusCode.Forbidden, Resources.ErrorMaxRequests);
            }

            var brands = db.spSearchBrands(article, analogues);
            if (brands == null)
            {
                return NotFound();
            }
            _log.Add(article, string.Empty, HttpContext.Current.Request.UserHostAddress, Resources.LogTypeBrand, CurrentUser.AcctgID);

            var brandsMap = Mapper.Map<List<spSearchBrands_Result>, List<Brand>>(brands.ToList());

            return Ok(brandsMap);
        }

		/*/// <response code="500">Internal Server Error</response> - пока убрал, т.к. хорошо бы добавить обработку ошибок и тогда уже добавить описание всех возможных кодов ошибок*/
		/// <summary>
		/// Возвращает список запчастей
		/// </summary>
		/// <param name="article">Артикул (номер запчасти).</param>
		/// <param name="brand">Бренд</param>
		/// <param name="analogues">Искать аналоги. False - поиск без аналогов (значение по умолчанию). True - поиск с аналогами.</param>
		/// <remarks>Для авторизации в HTTP-заголовок необходимо передавать пару key = "Authorization" value = "Bearer %ВАШ АВТОРИЗАЦИОННЫЙ ТОКЕН%"</remarks>
		/// <response code="200">OK result</response>
		/// <returns></returns>
		[HttpGet]
        [ResponseType(typeof(IEnumerable<PartNumber>))]
        [Authorize(Roles = "Client_SearchApi, NoAccess")]
        [Route("articles/{article:maxlength(50)}/brand/{brand:maxlength(50)}")]
        public IHttpActionResult GetSpareParts(string article, string brand, bool analogues = false)
        {
            if (CurrentSettings != null)
            {
                if (_settingsHelper.IsAccess(PerMinute, PerHour, PerDay, CurrentSettings.Rates.PerMinute, CurrentSettings.Rates.PerHour, CurrentSettings.Rates.PerDay) == false)
                    return Content(HttpStatusCode.Forbidden, Resources.ErrorMaxRequests);
            }
            // Подменяем введенный бренд на наш Main brand
            var mainBrand = db.BrandEquivalents.FirstOrDefault(x => x.Equivalent.Equals(brand))?.Brand;
            mainBrand = string.IsNullOrWhiteSpace(mainBrand) ? brand : mainBrand;

            var crosses = db.spSearchCrossesWithPriceSVC(article, brand, analogues, string.Empty, CurrentUser.AcctgID, CurrentUser.ClientGroup);
            if (crosses == null)
            {
                return NotFound();
            }

            _log.Add(article, brand, HttpContext.Current.Request.UserHostAddress, Resources.LogTypePartNumber, CurrentUser.AcctgID);

            var crossesMap = Mapper.Map<List<spSearchCrossesWithPriceSVC_Result>, List<PartNumber>>(crosses.ToList());

            return Ok(crossesMap);
        }
    }
}
