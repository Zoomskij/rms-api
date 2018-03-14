using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Configuration;
using System.Web.Http;
using System.Web.Http.Description;

namespace RMSAutoAPI.Controllers
{
    public class User
    {
        [Required]
        [MaxLength(50)]
        public string Username { get; set; }
        [Required]
        [MaxLength(50)]
        public string Password { get; set; }
    }

    public class Token
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }

        [JsonProperty("token_type")]
        public string TokenType { get; set; }

        [JsonProperty("expires_in")]
        public long ExpiresIn { get; set; }
    }

    [RoutePrefix("api")]
    public class AuthController : ApiController
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        //[HttpGet]
        //[ResponseType(typeof(Token))]
        //[AllowAnonymous]
        //[Route("auth")]
        //public IHttpActionResult Auth(string username, string password)
        //{
        //    var client = new RestClient(WebConfigurationManager.AppSettings["UrlApi"]);
        //    var request = new RestRequest("/api/auth/token", Method.POST);

        //    request.AddParameter("username", username);
        //    request.AddParameter("password", password);
        //    request.AddParameter("grant_type", "password");

        //    var response = client.Execute<JObject>(request);
        //    var token = JsonConvert.DeserializeObject<Token>(response.Content);
        //    return Ok(token);
        //}
    }
}
