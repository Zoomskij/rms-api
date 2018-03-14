using Microsoft.Owin;
using Microsoft.Owin.Security.OAuth;
using Owin;
using RMSAutoAPI.App_Data;
using RMSAutoAPI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

[assembly: OwinStartup(typeof(RMSAutoAPI.Startup))]
namespace RMSAutoAPI
{
    public class Startup
    {
        
        public void Configuration(IAppBuilder app)
        {
            var config = new HttpConfiguration();

            ConfigureOAuth(app);
            app.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);
            app.UseWebApi(config);
        }

        // Настройка времени жиз
        public void ConfigureOAuth(IAppBuilder app)
        {
            var oAuthServerOptions = new OAuthAuthorizationServerOptions()
            {
                AllowInsecureHttp = true,
                TokenEndpointPath = new PathString("/api/auth/token"),
                AccessTokenExpireTimeSpan = TimeSpan.FromDays(365),
                Provider = new AuthorizationServerProvider() 
            };

            app.UseOAuthAuthorizationServer(oAuthServerOptions);
            app.UseOAuthBearerAuthentication(new OAuthBearerAuthenticationOptions());
        }
    }

    public class AuthorizationServerProvider : OAuthAuthorizationServerProvider
    {
        private IUserService _userService;
        private ex_rmsauto_storeEntities db = new ex_rmsauto_storeEntities();
        public override async Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            context.Validated();
        }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            _userService = new UserService();
            context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { "*" });

            try
            {
                var user = _userService.GetUser(context.UserName, context.Password);

                var identity = new ClaimsIdentity(context.Options.AuthenticationType);

                identity.AddClaim(new Claim(ClaimTypes.Name, user.Username));
                identity.AddClaim(new Claim(ClaimTypes.Email, user.Email));

                var rolesTechnicalNamesUser = new List<string>();

                switch (user.UserRole)
                {
                    case 0:
                        identity.AddClaim(new Claim(ClaimTypes.Role, "Client"));
                        break;
                    case 1:
                        identity.AddClaim(new Claim(ClaimTypes.Role, "Manager"));
                        break;
					case 2:
						identity.AddClaim(new Claim(ClaimTypes.Role, "LimitedManager"));
						break;
					case 3:
						identity.AddClaim(new Claim(ClaimTypes.Role, "NoAccess"));
						break;
					case 4:
						identity.AddClaim(new Claim(ClaimTypes.Role, "Client_SearchApi"));
						break;
				}
               
                var principal = new GenericPrincipal(identity, rolesTechnicalNamesUser.ToArray());

                Thread.CurrentPrincipal = principal;

                context.Validated(identity);
            }
            catch (Exception ex)
            {
                context.SetError("invalid_grant", "message");
            }
        }
    }
}