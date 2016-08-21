using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using UniversityRegistrationSystem.Models;
using UniversityRegistrationSystem.Providers;
using UniversityRegistrationSystem.Results;
using UniversityRegistrationSystem.Identity;
using UniversityRegistrationSystem.OwinConfig;
using Newtonsoft.Json.Linq;
using System.Globalization;
using System.Configuration;
using UniversityRegistrationSystem;

namespace UniversityRegistrationSystem.Controllers
{
    [Authorize(Roles= "Admin")]
    [RoutePrefix("api/Account")]
    public class AccountController : ApiController
    {
        private const string LocalLoginProvider = "Local";
        private ApplicationUserManager _userManager;

        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager,
            ISecureDataFormat<AuthenticationTicket> accessTokenFormat)
        {
            UserManager = userManager;
            AccessTokenFormat = accessTokenFormat;
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        public ISecureDataFormat<AuthenticationTicket> AccessTokenFormat { get; private set; }

        // GET api/Account/UserInfo
        [HostAuthentication(DefaultAuthenticationTypes.ExternalBearer)]
        [Route("UserInfo")]
        public UserInfoViewModel GetUserInfo()
        {
            ExternalLoginData externalLogin = ExternalLoginData.FromIdentity(User.Identity as ClaimsIdentity);

            // We wouldn't normally be likely to do this:
            var user = UserManager.FindByName(User.Identity.Name);
            return new UserInfoViewModel
            {
                Email = user.Email,
                HasRegistered = externalLogin == null,
                LoginProvider = externalLogin != null ? externalLogin.LoginProvider : null,
            };
        }

        // POST api/Account/Logout
        [Route("Logout")]
        public IHttpActionResult Logout()
        {
            Authentication.SignOut(CookieAuthenticationDefaults.AuthenticationType);
            return Ok();
        }
        
        // POST api/Account/ChangePassword
        [Route("ChangePassword")]
        public async Task<IHttpActionResult> ChangePassword(ChangePasswordBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            IdentityResult result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword,
                model.NewPassword);
            
            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            return Ok();
        }

        // POST api/Account/SetPassword
        [Route("SetPassword")]
        public async Task<IHttpActionResult> SetPassword(SetPasswordBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            IdentityResult result = await UserManager.AddPasswordAsync(User.Identity.GetUserId(), model.NewPassword);

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            return Ok();
        }

        // POST api/Account/AddExternalLogin
        [Route("AddExternalLogin")]
        public async Task<IHttpActionResult> AddExternalLogin(AddExternalLoginBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie);

            AuthenticationTicket ticket = AccessTokenFormat.Unprotect(model.ExternalAccessToken);

            if (ticket == null || ticket.Identity == null || (ticket.Properties != null
                && ticket.Properties.ExpiresUtc.HasValue
                && ticket.Properties.ExpiresUtc.Value < DateTimeOffset.UtcNow))
            {
                return BadRequest("External login failure.");
            }

            ExternalLoginData externalData = ExternalLoginData.FromIdentity(ticket.Identity);

            if (externalData == null)
            {
                return BadRequest("The external login is already associated with an account.");
            }

            IdentityResult result = await UserManager.AddLoginAsync(User.Identity.GetUserId(),
                new UserLoginInfo(externalData.LoginProvider, externalData.ProviderKey));

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            return Ok();
        }

        // POST api/Account/RemoveLogin
        [Route("RemoveLogin")]
        public async Task<IHttpActionResult> RemoveLogin(RemoveLoginBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            IdentityResult result;

            if (model.LoginProvider == LocalLoginProvider)
            {
                result = await UserManager.RemovePasswordAsync(User.Identity.GetUserId());
            }
            else
            {
                result = await UserManager.RemoveLoginAsync(User.Identity.GetUserId(),
                    new UserLoginInfo(model.LoginProvider, model.ProviderKey));
            }

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            return Ok();
        }

        // GET api/Account/ExternalLogin
        [OverrideAuthentication]
        [HostAuthentication(DefaultAuthenticationTypes.ExternalCookie)]
        [AllowAnonymous]
        [Route("ExternalLogin", Name = "ExternalLogin")]
        public async Task<IHttpActionResult> GetExternalLogin(string provider, string error = null)
        {
            if (error != null)
            {
                return Redirect(Url.Content("~/") + "#error=" + Uri.EscapeDataString(error));
            }

            if (!User.Identity.IsAuthenticated)
            {
                return new ChallengeResult(provider, this);
            }

            ExternalLoginData externalLogin = ExternalLoginData.FromIdentity(User.Identity as ClaimsIdentity);

            if (externalLogin == null)
            {
                return InternalServerError();
            }

            if (externalLogin.LoginProvider != provider)
            {
                Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie);
                return new ChallengeResult(provider, this);
            }

            CustomUser user = await UserManager.FindAsync(new UserLoginInfo(externalLogin.LoginProvider,
                externalLogin.ProviderKey));

            bool hasRegistered = user != null;

            if (hasRegistered)
            {
                Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie);

                ClaimsIdentity oAuthIdentity = await user.GenerateUserIdentityAsync(UserManager,
                    OAuthDefaults.AuthenticationType);
                ClaimsIdentity cookieIdentity = await user.GenerateUserIdentityAsync(UserManager,
                    CookieAuthenticationDefaults.AuthenticationType);

                AuthenticationProperties properties = ApplicationOAuthProvider.CreateProperties(user.UserName);
                Authentication.SignIn(properties, oAuthIdentity, cookieIdentity);
            }
            else
            {
                IEnumerable<Claim> claims = externalLogin.GetClaims();
                ClaimsIdentity identity = new ClaimsIdentity(claims, OAuthDefaults.AuthenticationType);
                Authentication.SignIn(identity);
            }

            return Ok();
        }

        // POST api/Account/LoginWithExternalToken
        [HttpPost]
        [OverrideAuthentication]
        [HostAuthentication(DefaultAuthenticationTypes.ExternalCookie)]
        [AllowAnonymous]
        [Route("LoginWithExternalToken")]
        public async Task<IHttpActionResult> LoginWithExternalToken(LoginWithExternalTokenBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // validate token
            ExternalLoginData externalLogin = await FromToken(model.Provider, model.ExternalToken);

            if (externalLogin == null)
            {
                return BadRequest("External login could not be found");
            }

            if (externalLogin.LoginProvider != model.Provider)
            {
                Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie);

                return BadRequest("Login provider does not match");
            }

            var passedLoginInfo = new UserLoginInfo(externalLogin.LoginProvider, externalLogin.ProviderKey);

            // if we reached this point then token is valid, so query the user
            var user = await UserManager.FindAsync(passedLoginInfo);

            bool hasRegistered = user != null;

            if (!hasRegistered)
            {
                return BadRequest("User has not been registered yet, must be a business error.");
            }
            else
            {
                ClaimsIdentity oAuthIdentity = await user.GenerateUserIdentityAsync(UserManager,
                    OAuthDefaults.AuthenticationType);
                ClaimsIdentity cookieIdentity = await user.GenerateUserIdentityAsync(UserManager,
                    CookieAuthenticationDefaults.AuthenticationType);

                AuthenticationProperties properties = ApplicationOAuthProvider.CreateProperties(user.UserName);
                Authentication.SignIn(properties, oAuthIdentity, cookieIdentity);

                oAuthIdentity.AddClaim(new Claim(ClaimTypes.Name, user.UserName));
                oAuthIdentity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id));

                AuthenticationTicket ticket = new AuthenticationTicket(oAuthIdentity, new AuthenticationProperties());

                DateTime currentUtc = DateTime.UtcNow;
                ticket.Properties.IssuedUtc = currentUtc;
                ticket.Properties.ExpiresUtc = currentUtc.Add(TimeSpan.FromDays(365));

                string accessToken = Startup.OAuthOptions.AccessTokenFormat.Protect(ticket);
                Request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

                int expiryInDays = int.Parse(ConfigurationManager.AppSettings["TokenExpiryInDays"]);

                // Create the response building a JSON object that mimics exactly the one issued by the default /Token endpoint
                JObject token = new JObject(
                    new JProperty("userName", user.UserName),
                    new JProperty("access_token", accessToken),
                    new JProperty("token_type", "bearer"),
                    new JProperty("expires_in", TimeSpan.FromDays(expiryInDays).TotalSeconds.ToString()),
                    new JProperty("issued", currentUtc.ToString("ddd, dd MMM yyyy HH':'mm':'ss 'GMT'", CultureInfo.InvariantCulture)),
                    new JProperty("expires", currentUtc.Add(TimeSpan.FromDays(expiryInDays)).ToString("ddd, dd MMM yyyy HH:mm:ss 'GMT'", CultureInfo.InvariantCulture))
                );

                return Ok(token);
            }
        }

        // GET api/Account/ExternalLogins?returnUrl=%2F&generateState=true
        [AllowAnonymous]
        [Route("ExternalLogins")]
        public IEnumerable<ExternalLoginViewModel> GetExternalLogins(string returnUrl, bool generateState = false)
        {
            IEnumerable<AuthenticationDescription> descriptions = Authentication.GetExternalAuthenticationTypes();
            List<ExternalLoginViewModel> logins = new List<ExternalLoginViewModel>();

            string state;

            if (generateState)
            {
                const int strengthInBits = 256;
                state = RandomOAuthStateGenerator.Generate(strengthInBits);
            }
            else
            {
                state = null;
            }

            foreach (AuthenticationDescription description in descriptions)
            {
                ExternalLoginViewModel login = new ExternalLoginViewModel
                {
                    Name = description.Caption,
                    Url = Url.Route("ExternalLogin", new
                    {
                        provider = description.AuthenticationType,
                        response_type = "token",
                        client_id = Startup.PublicClientId,
                        redirect_uri = new Uri(Request.RequestUri, returnUrl).AbsoluteUri,
                        state = state
                    }),
                    State = state
                };
                logins.Add(login);
            }

            return logins;
        }

        // POST api/Account/Register
        [AllowAnonymous]
        [Route("Register")]
        public async Task<IHttpActionResult> Register(RegisterBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = new CustomUser() { UserName = model.Email, Email = model.Email };

            IdentityResult result = await UserManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            return Ok();
        }

        // POST api/Account/RegisterExternal
        [OverrideAuthentication]
        [HostAuthentication(DefaultAuthenticationTypes.ExternalBearer)]
        [Route("RegisterExternal")]
        public async Task<IHttpActionResult> RegisterExternal(RegisterExternalBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var info = await Authentication.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return InternalServerError();
            }

            var user = new CustomUser() { UserName = model.Email, Email = model.Email };

            IdentityResult result = await UserManager.CreateAsync(user);
            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            result = await UserManager.AddLoginAsync(user.Id, info.Login);
            if (!result.Succeeded)
            {
                return GetErrorResult(result); 
            }
            return Ok();
        }

        // POST api/Account/RegisterWithExternalToken
        [HttpPost]
        [OverrideAuthentication]
        [AllowAnonymous]
        [Route("RegisterWithExternalToken")]
        public async Task<IHttpActionResult> RegisterWithExternalToken(LoginWithExternalTokenBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // validate token
            ExternalLoginData externalLogin = await FromToken(model.Provider, model.ExternalToken);

            if (externalLogin == null)
            {
                return BadRequest("External login could not be found");
            }

            if (externalLogin.LoginProvider != model.Provider)
            {
                Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie);

                return BadRequest("Login provider does not match");
            }

            var passedLoginInfo = new UserLoginInfo(externalLogin.LoginProvider, externalLogin.ProviderKey);

            // if we reached this point then token is valid, so query the user
            var user = await UserManager.FindAsync(passedLoginInfo);

            bool hasRegistered = user != null;

            if (!hasRegistered)
            {
                // the user has not been registered into the database yet
                // first we need to retrieve info for the user and register him/her

                user = await RetrieveUserDetailsWithProvider(model.Provider, model.ExternalToken);

                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    var userLoginInfo = new UserLoginInfo(externalLogin.LoginProvider, externalLogin.ProviderKey);

                    result = await UserManager.AddLoginAsync(user.Id, userLoginInfo);
                    if (result.Succeeded)
                    {
                        // TODO: gseng - add this user to the "asp_net_user_roles" table

                        AuthenticationProperties properties = ApplicationOAuthProvider.CreateProperties(user.UserName);
                        Authentication.SignIn(properties);
                    }
                }
            }
            else
            {
                AuthenticationProperties properties = ApplicationOAuthProvider.CreateProperties(user.UserName);
                Authentication.SignIn(properties);
            }

            return Ok();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                UserManager.Dispose();
            }

            base.Dispose(disposing);
        }

        #region Helpers

        private IAuthenticationManager Authentication
        {
            get { return Request.GetOwinContext().Authentication; }
        }

        private IHttpActionResult GetErrorResult(IdentityResult result)
        {
            if (result == null)
            {
                return InternalServerError();
            }

            if (!result.Succeeded)
            {
                if (result.Errors != null)
                {
                    foreach (string error in result.Errors)
                    {
                        ModelState.AddModelError("", error);
                    }
                }

                if (ModelState.IsValid)
                {
                    // No ModelState errors are available to send, so just return an empty BadRequest.
                    return BadRequest();
                }

                return BadRequest(ModelState);
            }

            return null;
        }

        private class ExternalLoginData
        {
            public string LoginProvider { get; set; }
            public string ProviderKey { get; set; }
            public string UserName { get; set; }

            public IList<Claim> GetClaims()
            {
                IList<Claim> claims = new List<Claim>();
                claims.Add(new Claim(ClaimTypes.NameIdentifier, ProviderKey, null, LoginProvider));

                if (UserName != null)
                {
                    claims.Add(new Claim(ClaimTypes.Name, UserName, null, LoginProvider));
                }

                return claims;
            }

            public static ExternalLoginData FromIdentity(ClaimsIdentity identity)
            {
                if (identity == null)
                {
                    return null;
                }

                Claim providerKeyClaim = identity.FindFirst(ClaimTypes.NameIdentifier);

                if (providerKeyClaim == null || String.IsNullOrEmpty(providerKeyClaim.Issuer)
                    || String.IsNullOrEmpty(providerKeyClaim.Value))
                {
                    return null;
                }

                if (providerKeyClaim.Issuer == ClaimsIdentity.DefaultIssuer)
                {
                    return null;
                }

                return new ExternalLoginData
                {
                    LoginProvider = providerKeyClaim.Issuer,
                    ProviderKey = providerKeyClaim.Value,
                    UserName = identity.FindFirstValue(ClaimTypes.Name)
                };
            }
        }

        private static class RandomOAuthStateGenerator
        {
            private static RandomNumberGenerator _random = new RNGCryptoServiceProvider();

            public static string Generate(int strengthInBits)
            {
                const int bitsPerByte = 8;

                if (strengthInBits % bitsPerByte != 0)
                {
                    throw new ArgumentException("strengthInBits must be evenly divisible by 8.", "strengthInBits");
                }

                int strengthInBytes = strengthInBits / bitsPerByte;

                byte[] data = new byte[strengthInBytes];
                _random.GetBytes(data);

				return System.Uri.EscapeDataString(data.ToString());

                // return HttpServerUtility.UrlTokenEncode(data);
            }
        }

        private async Task<CustomUser> RetrieveUserDetailsWithProvider(string provider, string accessToken)
        {
            // write a helper function to retrieve user info from Facebook
            string detailRetrivalEndPoint = string.Empty;

            if (provider == "Facebook")
            {
                detailRetrivalEndPoint = string.Format("https://graph.facebook.com/v2.4/me?fields=id,name,email,first_name,last_name,location&access_token={0}", accessToken);
            }
            else
            {
                throw new NotImplementedException(string.Format("Unkown provider: {0} passed for validation but not implemented yet"));
            }

            Uri uri = new Uri(detailRetrivalEndPoint);
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync(uri);

            if (response.IsSuccessStatusCode)
            {
                string content = await response.Content.ReadAsStringAsync();
                dynamic details = (Newtonsoft.Json.Linq.JObject)Newtonsoft.Json.JsonConvert.DeserializeObject(content);

                var user = new CustomUser
                {
                    // TODO: gseng - does that really all we need?????
                    UserName = details["email"],
                    Email = details["email"],
                    FirstName = details["first_name"],
                    LastName = details["last_name"],
                    EmailConfirmed = true // since this is auth through Facebook, we know the email is confirmed
                };

                return await Task.FromResult<CustomUser>(user);
            }
            else
            {
                return await Task.FromResult<CustomUser>(default(CustomUser));
            }
        }

        private async Task<ExternalLoginData> FromToken(string provider, string accessToken)
        {
            string verifyTokenEndPoint = string.Empty;
            string verifyAppEndpoint = string.Empty;

            if (provider == "Facebook")
            {
                verifyTokenEndPoint = string.Format("https://graph.facebook.com/me?access_token={0}", accessToken);
                verifyAppEndpoint = string.Format("https://graph.facebook.com/app?access_token={0}", accessToken);
            }
            else
            {
                throw new NotImplementedException(string.Format("Unkown provider: {0} passed for validation but not implemented yet"));
            }

            Uri uri = new Uri(verifyTokenEndPoint);
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync(uri);
            ClaimsIdentity identity = null;

            if (response.IsSuccessStatusCode)
            {
                string content = await response.Content.ReadAsStringAsync();
                dynamic iObj = (Newtonsoft.Json.Linq.JObject)Newtonsoft.Json.JsonConvert.DeserializeObject(content);

                identity = new ClaimsIdentity(OAuthDefaults.AuthenticationType);

                if (provider == "Facebook")
                {
                    uri = new Uri(verifyAppEndpoint);
                    response = await client.GetAsync(uri);
                    content = await response.Content.ReadAsStringAsync();

                    dynamic appObj = (Newtonsoft.Json.Linq.JObject)Newtonsoft.Json.JsonConvert.DeserializeObject(content);

                    if (appObj["id"] != Startup.FacebookAuthOptions.AppId)
                    {
                        throw new Exception("ids does not match");
                    }

                    identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, iObj["id"].ToString(), ClaimValueTypes.String, "Facebook", "Facebook"));

                }
                else if (provider == "Google")
                {
                    throw new NotImplementedException();
                }
                else if (provider == "Twitter")
                {
                    identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, iObj["id"].ToString(), ClaimValueTypes.String, "Twitter", "Twitter"));
                }
            }

            if (identity == null)
            {
                throw new Exception("Could not resolve identity inspite of all efforts");
            }

            Claim providerKeyClaim = identity.FindFirst(ClaimTypes.NameIdentifier);

            if (providerKeyClaim == null || String.IsNullOrEmpty(providerKeyClaim.Issuer) || String.IsNullOrEmpty(providerKeyClaim.Value))
            {
                throw new Exception("Provider key claim could not be found");
            }

            if (providerKeyClaim.Issuer == ClaimsIdentity.DefaultIssuer)
            {
                throw new Exception("Provider key claim issuer is not as expected");
            }

            ExternalLoginData extLoginData = new ExternalLoginData
            {
                LoginProvider = providerKeyClaim.Issuer,
                ProviderKey = providerKeyClaim.Value,
                UserName = identity.FindFirstValue(ClaimTypes.Name)
            };

            return await Task.FromResult<ExternalLoginData>(extLoginData);
        }

        #endregion
    }
}
