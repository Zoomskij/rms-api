using RMSAutoAPI.App_Data;
using System.Security.Cryptography;

namespace RMSAutoAPI.Services
{
    interface IUserService
    {
        Users GetUser(string login, string password, string region);
        string GetMD5Hash(string input, bool isRms = true);

        string ComputeHash(MD5 hasher, string input);

    }
}
