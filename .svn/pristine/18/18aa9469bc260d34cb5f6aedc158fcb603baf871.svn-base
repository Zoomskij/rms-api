using RMSAutoAPI.App_Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RMSAutoAPI.Services
{
    public class LoggerService : ILoggerSerivce
    {
        // Логгируем все запросы в общую таблицу логов
        private ex_rmsauto_logEntities db = new ex_rmsauto_logEntities();
        public bool Add(string partNumber, string brand, string ip, string sourcePage, string acctgId)
        {
            SearchSparePartsLog logLine = new SearchSparePartsLog
            {
                SearchDate = DateTime.Now,
                PartNumber = partNumber,
                ClientIP = ip,
                Manufacturer = brand,
                SourcePage = sourcePage,
                Source = 1,
                AcctgID = acctgId
            };
            db.SearchSparePartsLog.Add(logLine);
            db.SaveChanges();
            return true;
        }
    }
}