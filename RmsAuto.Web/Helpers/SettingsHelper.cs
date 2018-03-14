using RMSAutoAPI.App_Data;
using System;
using System.Data.Entity.Core.Objects;
using System.Linq;

namespace RMSAutoAPI.Helpers
{
    public class SettingsHelper
    {
        private ex_rmsauto_storeEntities db = new ex_rmsauto_storeEntities();
        private ex_rmsauto_logEntities dbLog = new ex_rmsauto_logEntities();

        // Логируем все наши запросы в основную таблицу логов 
        public int CountRequests(RequestDelimeter delimeter, string ip)
        {
            var sec = (int)delimeter * -1; // Чтобы получить отрицательное значение и найти разницу в кол-ве запросов
            return dbLog.SearchSparePartsLog.Count(x => x.AcctgID.Equals(ip) && EntityFunctions.AddSeconds(DateTime.Now, sec) < x.SearchDate && x.Source == 1);
        }

        //Если превышен лимит запросов в минуту/в час/в день то не даем выполнить запрос снова
        public bool IsAccess(int reqPerMinute, int reqPerHour, int reqPerDay, int allowedInMinute, int allowedInHour, int allowedInDay)
        {
            if (reqPerMinute >= allowedInMinute || reqPerHour >= allowedInHour || reqPerDay >= allowedInDay) return false;
            return true;
        }
    }
}