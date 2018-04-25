using RMSAutoAPI.App_Data;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace RMSAutoAPI.Services
{
    interface IUserService
    {
        Task<Users> GetUser(string login, string password, string region);
        Task<string> GetMD5Hash(string input, bool isRms = true);

        string ComputeHash(MD5 hasher, string input);

    }
}
