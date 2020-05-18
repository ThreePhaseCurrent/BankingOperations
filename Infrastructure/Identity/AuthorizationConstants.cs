using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Identity
{
    public static class AuthorizationConstants
    {
        public const string DEFAULT_PASSWORD = "Pass@word1";

        public static class Roles
        {
            public const string ADMINISTRATOR = "Administrator";
            public const string CLIENT = "Client";
        }
    }
}
