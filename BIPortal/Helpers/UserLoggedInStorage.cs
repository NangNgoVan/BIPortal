using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BIPortal.Helpers
{
    public static class UserLoggedInStorage
    {
        private static Dictionary<string, string> UserLoggedInList = new Dictionary<string, string>();

        public static bool checkIsUserLoggedIn(string username, string sessionId)
        {
            if (UserLoggedInList.ContainsKey(username))
            {
                if (UserLoggedInList[username] == sessionId)
                    return true;
                return false;
            }
            return false;
        }

        public static void updateUserLoggedIn(string username, string sessionId)
        {
            if (UserLoggedInList.ContainsKey(username))
            {
                UserLoggedInList[username] = sessionId;
            }
            else UserLoggedInList.Add(username, sessionId);
        }
    }
}