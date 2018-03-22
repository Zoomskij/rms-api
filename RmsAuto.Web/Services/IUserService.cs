using RMSAutoAPI.App_Data;

namespace RMSAutoAPI.Services
{
    interface IUserService
    {
        Users GetUser(string login, string password, string region = "rmsauto");
        string GetMD5Hash(string input);
    }
}
