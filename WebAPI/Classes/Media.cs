using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InstaMatch.WebAPI.Classes
{
    public class Media
    {
        public string Id { get; set; }

        public Media(string userId, string accessToken)
        {
            JObject jsResult = IGUtil.GetUserMedia(userId, accessToken);
            Id = jsResult["data"]["id"].ToString();
        }
    }
}