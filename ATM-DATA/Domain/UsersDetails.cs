
using System;
using System.Collections.Generic;
using System.Text;

namespace ATM_DATA.Domain
{
    public abstract class UsersDetails
    {
        protected UsersDetails()
        {

        }
        public static string? Pin { get; set; }
        public static string? CardNumber { get; set; }
    }
}


