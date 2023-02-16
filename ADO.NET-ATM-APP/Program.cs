using System;
using Microsoft.Data.SqlClient;
using System.ComponentModel.DataAnnotations.Schema;
using ATM_DATA.Database;
using ATM_DATA.Domain;

using ATM_DATA;

namespace ADO.NET_ATM_APP
{
    public class Program
    {
        static void Main(string[] args)
        {


            bool showMenu = true;
            while (showMenu)
            {
                ATMservices pro = new ATMservices(new ATMDBContext());
                pro.UserLogin();
                showMenu = pro.MainMenu();
            }
        }

    }
}




