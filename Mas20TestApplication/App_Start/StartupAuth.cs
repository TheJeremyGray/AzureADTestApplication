using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Web;
using Owin;
using Microsoft.Owin.Extensions;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;
using Microsoft.IdentityModel.Protocols;
using Microsoft.Owin.Security.Notifications;
using System.Threading.Tasks;
using System.Security.Claims;

namespace Mas20TestApplication
{
    public partial class Startup
    {
        private static string clientId = ConfigurationManager.AppSettings["ida:ClientId"];
        private static string aadInstance = ConfigurationManager.AppSettings["ida:AADInstance"];
        private static string tenantId = ConfigurationManager.AppSettings["ida:TenantId"];
        private static string postLogoutRedirectUri = ConfigurationManager.AppSettings["ida:PostLogoutRedirectUri"];

        string authority = aadInstance + tenantId;

        public void ConfigureAuth(IAppBuilder app)
        {
            app.SetDefaultSignInAsAuthenticationType(CookieAuthenticationDefaults.AuthenticationType);

            app.UseCookieAuthentication(new CookieAuthenticationOptions());

            // Definition of options
            //https://www.microsoftpressstore.com/articles/article.aspx?p=2473126&seqNum=2
            app.UseOpenIdConnectAuthentication(
                new OpenIdConnectAuthenticationOptions
                {
                    ClientId = clientId,
                    Authority = authority,
                    PostLogoutRedirectUri = postLogoutRedirectUri,                   
                   TokenValidationParameters =  {
                        NameClaimType = "name"
                    },
                    UseTokenLifetime = false, // tells the cookie middleware that the session it creates should have the same duration and validity window as the id_token received from the authority. If you want to decouple the session validity window from the token (which, by the way, Azure AD sets to one hour), you must set this property to false
                    
                    Notifications = new OpenIdConnectAuthenticationNotifications()
                    {
                        AuthenticationFailed = (context) =>
                        {                            
                            //context.OwinContext.Response.Redirect("/Home/Error");
                            //context.HandleResponse();
                            return Task.FromResult(0);
                        },
                        SecurityTokenValidated = OnSecurityTokenValidated,
                        RedirectToIdentityProvider = OnRedirectToIdentityProvider,
                        SecurityTokenReceived = (arg) => 
                        {
                            // if you want to test whether the id token has been signed, grab the value from the "arg.ProtocolMessage.IdToken", paste it into https://www.jsonwebtoken.io/, 
                            // tamper with the Payload, copy the new JWT string back into the "arg.ProtocolMessage.IdToken".  The AUthenticationFailed Event should fire and give an appropriate error. 
                            return Task.FromResult(0);
                        }

                    }              
                }
                );

            // This makes any middleware defined above this line run before the Authorization rule is applied in web.config
            app.UseStageMarker(PipelineStage.Authenticate);
        }

        private Task OnRedirectToIdentityProvider(RedirectToIdentityProviderNotification<OpenIdConnectMessage, OpenIdConnectAuthenticationOptions> arg)
        {
            /*
             * This is likely the notification you’ll work with most often. It is executed right after the OpenID Connect middleware 
             * creates a protocol message, and it gives you the opportunity to override the option values the middleware uses to build 
             * the message, augment them with extra parameters, and so on. 
             */
            return Task.FromResult(0);
        }

        private Task OnSecurityTokenValidated(SecurityTokenValidatedNotification<OpenIdConnectMessage, OpenIdConnectAuthenticationOptions> arg)
        {
            // called from this method
            // http://sourcebrowser.io/Browse/jchannon/katanaproject/src/Microsoft.Owin.Security.OpenIdConnect/OpenidConnectAuthenticationHandler.cs#183

            // token validation
            // http://sourcebrowser.io/Browse/jchannon/katanaproject/src/Microsoft.Owin.Security.OpenIdConnect/OpenidConnectAuthenticationHandler.cs#291

            IList<Claim> claims = new List<Claim>() { new Claim("foo","bar") };   
            arg.AuthenticationTicket.Identity.AddClaims(claims);
            return Task.FromResult(0);
        }
    }
}
