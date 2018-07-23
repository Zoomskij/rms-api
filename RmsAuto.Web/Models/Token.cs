using Newtonsoft.Json;
using System.ComponentModel;

namespace RMSAutoAPI.Models
{
	public partial class Token
    {
        [JsonProperty("access_token")]
		[Description("Авторизационный токен")]
        public string AccessToken { get; set; }

		[JsonProperty("token_type")]
		[Description("Тип токена")]
		public string TokenType { get; set; }

        [JsonProperty("expires_in")]
		[Description("Время жизни токена (сек)")]
		public long ExpiresIn { get; set; }
    }
}