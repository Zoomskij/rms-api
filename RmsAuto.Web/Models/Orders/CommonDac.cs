using RMSAutoAPI.App_Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RMSAutoAPI.Models.Orders
{
    public static class CommonDac
    {
        public static Dictionary<int, int> GetPermutations()
        {
            Dictionary<int, int> res = new Dictionary<int, int>();
            using (var dc = new ex_rmsauto_storeEntities())
            {
                try
                {
                    res = dc.Permutation1C.Select(x => x).ToDictionary(x => x.OldSupplierId, x => x.NewSupplierId);
                }
                catch (Exception ex)
                {
                   // Logger.WriteError("Произошла ошибка при загрузке словаря перестановок SupplierID", EventLogerID.UnknownError, EventLogerCategory.UnknownCategory, ex);
                }
            }
            return res;
        }
    }
}