using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Owin;
using Owin;
using System.Web.Http;
using Microsoft.Owin.Security.OAuth;
using Microsoft.Owin.Security.Cookies;
using Microsoft.AspNet.Identity;
using UniversityRegistrationSystem.Providers;
using UniversityRegistrationSystem.OwinConfig;
using System.Configuration;
using Microsoft.Owin.Security.Facebook;
using Owin.Security.AesDataProtectorProvider;
using UniversityRegistrationSystem.OwinCustomMiddleware;

[assembly: OwinStartup(typeof(UniversityRegistrationSystem.Startup))]

namespace UniversityRegistrationSystem
{
    public partial class Startup
    {
        public static OAuthAuthorizationServerOptions OAuthOptions { get; private set; }

        public static string PublicClientId { get; private set; }

        public static FacebookAuthenticationOptions FacebookAuthOptions { get; private set; }

        // This code configures Web API. The Startup class is specified as a type
        // parameter in the WebApp.Start method.
        public void Configuration(IAppBuilder appBuilder)
        {
            // Configure Web API for self-host. 
            HttpConfiguration config = new HttpConfiguration();

			appBuilder.Use<GlobalExceptionMiddleware>();

            WebApiConfig.Register(config);

            ConfigureAuth(appBuilder);

            appBuilder.UseWebApi(config);
        }

        public void ConfigureAuth(IAppBuilder app)
        {
            app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);
            app.CreatePerOwinContext<ApplicationRoleManager>(ApplicationRoleManager.Create);

            // Enable the application to use a cookie to store information for the signed in user
            // and to use a cookie to temporarily store information about a user logging in with a third party login provider
            app.UseCookieAuthentication(new CookieAuthenticationOptions());
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            int expiryInDays = int.Parse(ConfigurationManager.AppSettings["TokenExpiryInDays"]);

            // Configure the application for OAuth based flow
            PublicClientId = "self";
            OAuthOptions = new OAuthAuthorizationServerOptions
            {
                TokenEndpointPath = new PathString("/Token"),
                Provider = new ApplicationOAuthProvider(PublicClientId),
                AuthorizeEndpointPath = new PathString("/api/Account/ExternalLogin"),
                AccessTokenExpireTimeSpan = TimeSpan.FromDays(expiryInDays),
                AllowInsecureHttp = true
            };

            // Enable the application to use bearer tokens to authenticate users
            app.UseOAuthBearerTokens(OAuthOptions);

            // Uncomment the following lines to enable logging in with third party login providers
            //app.UseMicrosoftAccountAuthentication(
            //    clientId: "",
            //    clientSecret: "");

            //app.UseTwitterAuthentication(
            //    consumerKey: "",
            //    consumerSecret: "");

            FacebookAuthOptions = new Microsoft.Owin.Security.Facebook.FacebookAuthenticationOptions
            {
                AppId = ConfigurationManager.AppSettings["FacebookAppId"].ToString(),
                AppSecret = ConfigurationManager.AppSettings["FacebookAppSecret"].ToString(),
                // NOTE: gseng - may need to use a backchannel handler to retrieve email, first_name and etc
                // BackchannelHttpHandler = new FacebookBackChannelHandler(),
                // UserInformationEndpoint = "https://graph.facebook.com/v2.4/me?fields=id,name,email,first_name,last_name,location",
                SignInAsAuthenticationType = DefaultAuthenticationTypes.ExternalCookie,
                Scope = { "email" },
                Provider = new FacebookAuthenticationProvider()
                {
                    OnAuthenticated = async context =>
                    {
                        await System.Threading.Tasks.Task.Run(() =>
                        {
                            context.Identity.AddClaim(new System.Security.Claims.Claim("FacebookAccessToken", context.AccessToken));
                            foreach (var claim in context.User)
                            {
                                var claimType = string.Format("urn:facebook:{0}", claim.Key);
                                string claimValue = claim.Value.ToString();
                                if (!context.Identity.HasClaim(claimType, claimValue))
                                    context.Identity.AddClaim(new System.Security.Claims.Claim(claimType, claimValue, "XmlSchemaString", "Facebook"));
                            }
                        });
                    }
                }
            };

            app.UseFacebookAuthentication(FacebookAuthOptions);

            //app.UseGoogleAuthentication(new GoogleOAuth2AuthenticationOptions()
            //{
            //    ClientId = "",
            //    ClientSecret = ""
            //});

			app.UseAesDataProtectorProvider();
        }
    }
}
