using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Google;
using Microsoft.Owin.Security.OAuth;
using Owin;
using WebAPI.Providers;
using WebAPI.Models;
using Microsoft.Owin.Security;
using System.Configuration;
using Microsoft.Owin.Security.Facebook;
using WebAPI.Classes;

namespace WebAPI
{
    public partial class Startup
    {
        public static OAuthAuthorizationServerOptions OAuthOptions { get; private set; }

        public static string PublicClientId { get; private set; }

        // For more information on configuring authentication, please visit https://go.microsoft.com/fwlink/?LinkId=301864
        public void ConfigureAuth(IAppBuilder app)
        {
            //Enable CORS
            app.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);
            

            // Configure the db context and user manager to use a single instance per request
            app.CreatePerOwinContext(ApplicationDbContext.Create);
            app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);

            // Enable the application to use a cookie to store information for the signed in user
            // and to use a cookie to temporarily store information about a user logging in with a third party login provider
            
            app.UseCookieAuthentication(new CookieAuthenticationOptions()); //default

            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);
            

            // Configure the application for OAuth based flow
            PublicClientId = "self";
            OAuthOptions = new OAuthAuthorizationServerOptions
            {
                TokenEndpointPath = new PathString("/Token"),
                Provider = new ApplicationOAuthProvider(PublicClientId),
                AuthorizeEndpointPath = new PathString("/api/Account/ExternalLogin"),
                AccessTokenExpireTimeSpan = TimeSpan.FromDays(14),
                // In production mode set AllowInsecureHttp = false
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

            //app.UseFacebookAuthentication(
            //    appId: ConfigurationManager.AppSettings["FacebookClientId"],
            //    appSecret: ConfigurationManager.AppSettings["FacebookClientSecret"]);


            //Facebook Auth
            var facebookOptions = new FacebookAuthenticationOptions()
            {
                AppId = ConfigurationManager.AppSettings["FacebookClientId"],
                AppSecret = ConfigurationManager.AppSettings["FacebookClientSecret"],

            };
            facebookOptions.Scope.Add("email");
            facebookOptions.Scope.Add("public_profile");

            facebookOptions.Fields.Add("email");
            facebookOptions.Fields.Add("id");
            facebookOptions.Fields.Add("first_name");
            facebookOptions.Fields.Add("last_name");
           // facebookOptions.UserInformationEndpoint = "https://graph.facebook.com/v2.5/me?fields=id,name,email,first_name,last_name";
                                                            // facebookOptions.Fields.Add("user_birthday");
            app.UseFacebookAuthentication(facebookOptions);



            //Google Auth
            app.UseGoogleAuthentication(new GoogleOAuth2AuthenticationOptions()
            {
                
                ClientId = ConfigurationManager.AppSettings["GoogleClientId"],
                ClientSecret = ConfigurationManager.AppSettings["GoogleClientSecret"],
                //SignInAsAuthenticationType = DefaultAuthenticationTypes.ExternalBearer
            });





           // var googlePlusOptions = new GoogleOAuth2AuthenticationOptions
           // {
           //     ClientId = "7559469544-ueuq20vq1u74k148g6vnp5ohthsa956r.apps.googleusercontent.com",
           //     ClientSecret = "i8rVQE50ncfi4lsFNT8GvYdR",
           // };

           // // default scopes
           //// googlePlusOptions.Scope.Add("openid");
           // //googlePlusOptions.Scope.Add("profile");
           // googlePlusOptions.Scope.Add("email");
           // googlePlusOptions.SignInAsAuthenticationType = DefaultAuthenticationTypes.ExternalBearer;
           // googlePlusOptions.CallbackPath = PathString.FromUriComponent(new Uri("http://localhost:52737/signin-google"));

           // //// additional scope(s)
           // // googlePlusOptions.Scope.Add("https://www.googleapis.com/auth/youtube.readonly");
           // app.UseGoogleAuthentication(googlePlusOptions);


        }
    }
}
