using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security.Cryptography;
using System.Text;
using RMSAutoAPI.App_Data;
using RMSAutoAPI.Infrastructure;


namespace RMSAutoAPI.Services
{
    //Логика по авторизации взята с основного сайта RMSAUTO.RU
    public class UserService : IUserService
    {
        private ex_rmsauto_storeEntities db = new ex_rmsauto_storeEntities();
        public Users GetUser(string login, string password, string region = "rmsauto")
        {
            try
            {
                if (!string.IsNullOrEmpty(region) && !region.Equals("rmsauto"))
                {
                    var franches = db.Franch.ToList();
                    var currentFranch = db.Franch.FirstOrDefault(x => x.InternalFranchName.ToUpper().Equals(region.ToUpper()));
                    db.ChangeDatabase(initialCatalog: $"ex_{currentFranch.DbName}_store", dataSource: $"{currentFranch.ServerName}");
                }

                Users use = new Users();

                var md5Password = GetMD5Hash(password);
                var user = db.Users.FirstOrDefault(x => x.Username == login && x.Password == md5Password);
                return user;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public string GetMD5Hash(string input)
        {
            if (input == null) throw new ArgumentNullException("input");
            using (MD5 hasher = MD5.Create())
            {
                return Convert.ToBase64String(hasher.ComputeHash(Encoding.Default.GetBytes(input)));
            }
        }
    }
}