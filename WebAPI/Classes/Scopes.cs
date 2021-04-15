using System;


namespace WebAPI.Classes
{
    public static class Scopes
    {
        public static String Basic { get { return "basic"; } }
        public static String PublicContent { get { return "public_content"; } }
        public static String FollowerList { get { return "follower_list"; } }
        public static String Comments { get { return "comments"; } }
        public static String Relationships { get { return "relationships"; } }
        public static String Likes { get { return "likes"; } }
    }
}

