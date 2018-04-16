using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security.Cryptography;
using System.Text;
using RMSAutoAPI.App_Data;
using RMSAutoAPI.Infrastructure;
using System.Threading.Tasks;

namespace RMSAutoAPI.Services
{
    //Логика по авторизации взята с основного сайта RMSAUTO.RU
    public class UserService : IUserService
    {
        private ex_rmsauto_storeEntities db = new ex_rmsauto_storeEntities();
        public Task<Users> GetUser(string login, string password, string region)
        {
            bool isRms = true;
            try
            {
                if (!string.IsNullOrEmpty(region))
                {
                    var franches = db.spGetFranches().ToList();
                    var currentFranch = franches.FirstOrDefault(x => x.InternalFranchName.ToUpper().Equals(region.ToUpper()));
                    db.ChangeDatabase(initialCatalog: $"ex_{currentFranch.DbName}_store", dataSource: $"{currentFranch.ServerName}");
                    isRms = false;
                }

                Users use = new Users();

                var md5Password = GetMD5Hash(password, isRms);
                return Task.Run(() => {
                    var user = db.Users.FirstOrDefault(x => x.Username == login && x.Password == md5Password);
                    return user;
                });

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public string GetMD5Hash(string input, bool isRms = true)
        {
            if (input == null) throw new ArgumentNullException("input");
            using (MD5 hasher = MD5.Create())
            {
                if (isRms)
                    return Convert.ToBase64String(hasher.ComputeHash(Encoding.Default.GetBytes(input)));
                else
                    return ComputeHash(hasher, ComputeHash(hasher, input));
            }
        }

        public string ComputeHash(MD5 hasher, string input)
        {
            var hash = hasher.ComputeHash(Encoding.Default.GetBytes(input));
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("x2"));
            }

            return sb.ToString();
        }
    }
}