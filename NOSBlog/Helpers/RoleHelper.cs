using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NOSBlog.Helpers
{
    public static class RoleHelper
    {
        private const int _user = 1;
        private const int _employee = 5;
        private const int _admin = 10;

        public static string RoleString(int role)
        {
            switch(role)
            {
                case _user: return "User";
                case _employee: return "Employee";
                case _admin: return "Admin";
                default:
                    return "Unknown";
            }
        }
    }
}