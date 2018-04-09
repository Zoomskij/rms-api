using RMSAutoAPI.App_Data;
using RMSAutoAPI.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RMSAutoAPI.Services
{
    public class LoggerService : ILoggerSerivce
    {
        /// <summary>
        /// Логгируем все запросы в общую таблицу логов
        /// </summary>
        /// <param name="partNumber"></param>
        /// <param name="brand"></param>
        /// <param name="ip"></param>
        /// <param name="sourcePage"></param>
        /// <param name="acctgId"></param>
        /// <param name="DbName"></param>
        /// <param name="ServerName"></param>
        /// <returns></returns>
        public bool Add(string partNumber, string brand, string ip, string sourcePage, string acctgId, string DbName, string ServerName)
        {
            using (var db = new ex_rmsauto_logEntities())
            {
                if (!string.IsNullOrEmpty(DbName) && !string.IsNullOrEmpty(ServerName))
                {
                    db.ChangeDatabase(initialCatalog: $"ex_{DbName}_log", dataSource: $"{ServerName}");
                }

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
}