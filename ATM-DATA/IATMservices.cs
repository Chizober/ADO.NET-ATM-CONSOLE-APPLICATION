

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ATM_DATA.Domain;

namespace ATM_DATA
{
    public interface IATMservices : IDisposable
    {
        void UserLogin();
        void Transfer();
        void Withdraw();
        void CheckBalance();
        void Deposit();
        bool MainMenu();

    }
}



