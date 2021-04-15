using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace WebAPI.Classes
{
    public class Constants
    {
        //EMAIL CONSTANTS
        private static string influencerInviteUrl = null;
        public static string InfluencerInviteUrl
        {
            get
            {
                if (influencerInviteUrl == null)
                    influencerInviteUrl = ConfigurationManager.AppSettings["InfluencerInviteUrl"];
                return influencerInviteUrl;
            }
        }

        private static string defaultEmailFromAddress = null;
        public static string DefaultEmailFromAddress
        {
            get
            {
                if (defaultEmailFromAddress == null)
                    defaultEmailFromAddress = ConfigurationManager.AppSettings["DefaultEmailFromAddress"];
                return defaultEmailFromAddress;
            }
        }

        //Google constants
        private static string googleClientID = null;
        public static string GoogleClientId
        {
            get
            {
                if (googleClientID == null)
                    googleClientID = ConfigurationManager.AppSettings["GoogleClientId"];
                return googleClientID;
            }
        }

        private static string googleClientSecret = null;
        public static string GoogleClientSecret
        {
            get
            {
                if (googleClientSecret == null)
                    googleClientSecret = ConfigurationManager.AppSettings["GoogleClientSecret"];
                return googleClientSecret;
            }
        }

        //IG STUFF BELOW
        //TODO: prefix constants below with IG
        public const string AuthorizationCode = "authorization_code";

        private static string clientID = null;
        public static string ClientId
        {
            get
            {
                if (clientID == null)
                    clientID = ConfigurationManager.AppSettings["ClientID"];
                return clientID;
            }
        }

        private static string clientSecret = null;
        public static string ClientSecret
        {
            get
            {
                if (clientSecret == null)
                    clientSecret = ConfigurationManager.AppSettings["ClientSecret"];
                return clientSecret;
            }
        }

        private static string authenticateUrl = null;
        public static string AuthenticateUrl
        {
            get
            {
                if (authenticateUrl == null)
                    authenticateUrl = ConfigurationManager.AppSettings["AuthenticateUrl"];
                return authenticateUrl;
            }
        }

        private static string iGAccessTokenUrl = null;
        public static string IGAccessTokenUrl
        {
            get
            {
                if (iGAccessTokenUrl == null)
                    iGAccessTokenUrl = ConfigurationManager.AppSettings["IGAccessTokenUrl"];
                return iGAccessTokenUrl;
            }
        }

        private static string apiUriDefault = null;
        public static string ApiUriDefault
        {
            get
            {
                if (apiUriDefault == null)
                    apiUriDefault = ConfigurationManager.AppSettings["ApiUriDefault"];
                return apiUriDefault;
            }
        }

        private static string oAuthUriDefault = null;
        public static string OAuthUriDefault
        {
            get
            {
                if (oAuthUriDefault == null)
                    oAuthUriDefault = ConfigurationManager.AppSettings["OAuthUriDefault"];
                return oAuthUriDefault;
            }
        }

        private static string oAuthUriAuthorize = null;
        public static string OAuthUriAuthorize
        {
            get
            {
                if (oAuthUriAuthorize == null)
                    oAuthUriAuthorize = ConfigurationManager.AppSettings["OAuthUriAuthorize"];
                return oAuthUriAuthorize;
            }
        }


        private static string realTimeApiDefault = null;
        public static string RealTimeApiDefault
        {
            get
            {
                if (realTimeApiDefault == null)
                    realTimeApiDefault = ConfigurationManager.AppSettings["RealTimeApiDefault"];
                return realTimeApiDefault;
            }
        }

    }
}