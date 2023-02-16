

using System;
using ATM_DATA.Database;
using Microsoft.Data.SqlClient;
using ATM_DATA.Domain;
using static System.TimeZoneInfo;

namespace ATM_DATA
{
    public class ATMservices : IATMservices
    {


        private readonly ATMDBContext _dbContext;
        private bool _disposed;

        public ATMservices(ATMDBContext dbContext)
        {
            _dbContext = dbContext;
        }
        public ATMservices() { }

        public void UserLogin()
        {
            Console.WriteLine("\n\t\t\t Please Login to access the ATM\n");
            Console.WriteLine("\n\tEnter CardNumber: ");
            UsersDetails.CardNumber = Console.ReadLine();

            Console.WriteLine("Enter Pin: ");
            UsersDetails.Pin = Console.ReadLine();

            bool IsValid = false;

            string VerifyUser = "SELECT *  FROM Users WHERE CardNumber = @CardNumber AND Pin = @Pin";

            SqlConnection sqlConn = _dbContext.OpenConnection();

            using (SqlCommand command = new SqlCommand(VerifyUser, sqlConn))
            {
                try
                {
                    command.Parameters.AddWithValue("@CardNumber", UsersDetails.CardNumber);
                    command.Parameters.AddWithValue("@Pin", UsersDetails.Pin);


                    int result = (int)command.ExecuteScalar();


                    if (result > 0)
                    {
                        IsValid = true;
                    }


                }
                catch (Exception)
                {
                    Console.WriteLine("Login failed. Incorrect Card Number or Pin.\nPlease Enter Correct Card Number and Pin. ");
                }
                finally
                {

                    _dbContext.CloseConnection();
                }

                if (IsValid)
                {

                    Console.WriteLine("Login successful!\n\nPress ENTER to continue!");
                    Console.ReadKey();
                    Console.Clear();
                    MainMenu();
                }
                else
                {
                    Console.WriteLine();
                    UserLogin();
                }

            }

        }
        public bool MainMenu()
        {

            Console.WriteLine("\n\n........WELCOME TO MANIZO ATM");
            Console.WriteLine("Choose an option:");
            Console.WriteLine("1) Deposit");
            Console.WriteLine("2) Withdraw");
            Console.WriteLine("3) Transfer");
            Console.WriteLine("4) CheckBalance");
            Console.WriteLine("5) Exit");
            Console.Write("\r\nSelect an option: ");

            switch (Console.ReadLine())
            {

                case "1":
                    Deposit();
                    return true;
                case "2":
                    Withdraw();
                    return true;
                case "3":
                    Transfer();
                    return true;
                case "4":
                    CheckBalance();
                    return true;

                case "5":
                    return false;
                default:
                    return true;
            }
        }

        public void Deposit()
        {

            Console.WriteLine(" Please Enter  amount to deposit: ");
            string input = Console.ReadLine();
            while (string.IsNullOrEmpty(input) || input == "0" || !decimal.TryParse(input, out _))
            {
                Console.WriteLine("Invalid input. Please enter a valid deposit amount: ");
                input = Console.ReadLine();
            }
            decimal depositAmount = Convert.ToDecimal(input);



            SqlConnection sqlConn = _dbContext.OpenConnection();

            string BalanceQuery = "SELECT Balance FROM Users WHERE CardNumber = @cardNumber AND PIN = @pin";

            using (SqlCommand command = new SqlCommand(BalanceQuery, sqlConn))
            {
                try
                {
                    command.Parameters.AddWithValue("@cardNumber", UsersDetails.CardNumber);
                    command.Parameters.AddWithValue("@pin", UsersDetails.Pin);

                    decimal Balance = (decimal)command.ExecuteScalar();


                    Balance += depositAmount;

                    string TransactionType = "Deposit";
                    DateTime TransactionTime = DateTime.Now;

                    string updateQuery = "UPDATE Users SET Balance = @balance WHERE CardNumber = @cardNumber AND Pin = @pin";
                    /*using (SqlCommand updateCommand = new SqlCommand(updateQuery, sqlConn))
                    {
                        updateCommand.Parameters.AddWithValue("@balance", Balance);
                        updateCommand.Parameters.AddWithValue("@cardNumber", UsersDetails.CardNumber);
                        updateCommand.Parameters.AddWithValue("@pin", UsersDetails.Pin);
                        updateCommand.ExecuteNonQuery();
                    }


                    string InsertHistoryQuery = @"INSERT INTO Transaction_History (UserCardNumber, TransactionType, Amount, Date)
                                                   VALUES (@UserCardNumber, @TransactionType, @Amount, @Date)";

                    using (SqlCommand historyCommand = new SqlCommand(InsertHistoryQuery, sqlConn))
                    {
                        historyCommand.Parameters.AddWithValue("@UserCardNumber", UsersDetails.CardNumber);
                        historyCommand.Parameters.AddWithValue("@TransactionType", TransactionType);
                        historyCommand.Parameters.AddWithValue("@Amount", depositAmount);
                        historyCommand.Parameters.AddWithValue("@Date", TransactionTime);

                        historyCommand.ExecuteNonQuery();
                    }*/


                    Console.WriteLine($"Deposit Successful! Your new balance is: #{Balance}\n\nPress ENTER to go to Main Menu");

                    Console.ReadKey();
                    Console.Clear();


                }
                catch (Exception e)
                {
                    Console.WriteLine("Error: {0}", e);
                }
                finally
                {
                    sqlConn.Close();
                }
            }

        }
        public void Withdraw()
        {
            Console.WriteLine("Enter amount you want to withdraw: ");
            string input = Console.ReadLine();
            while (string.IsNullOrEmpty(input) || input == "0" || !decimal.TryParse(input, out _))
            {
                Console.WriteLine("Invalid input. Please enter a valid amount: ");
                input = Console.ReadLine();
            }
            decimal WithdrawalAmount = Convert.ToDecimal(input);

            SqlConnection sqlConn = _dbContext.OpenConnection();

            string BalanceQuery = "SELECT Balance FROM Users WHERE CardNumber = @cardNumber AND PIN = @pin";


            using (SqlTransaction sqlTransaction = sqlConn.BeginTransaction())
            {
                try
                {
                    using (SqlCommand command = new SqlCommand(BalanceQuery, sqlConn, sqlTransaction))
                    {
                        try
                        {
                            command.Parameters.AddWithValue("@cardNumber", UsersDetails.CardNumber);
                            command.Parameters.AddWithValue("@pin", UsersDetails.Pin);

                            decimal Balance = (decimal)command.ExecuteScalar();


                            if (Balance < WithdrawalAmount)
                            {
                                Console.WriteLine("Insufficient balance. Your current balance is: " + Balance);
                                Withdraw();
                            }
                            else
                            {
                                Balance -= WithdrawalAmount;


                                string TransactionType = "Withdrawal";
                                DateTime TransactionTime = DateTime.Now;

                                string updateQuery = "UPDATE Users SET Balance = @balance WHERE CardNumber = @cardNumber AND Pin = @pin";
                                using (SqlCommand updateCommand = new SqlCommand(updateQuery, sqlConn, sqlTransaction))
                                {
                                    updateCommand.Parameters.AddWithValue("@balance", Balance);
                                    updateCommand.Parameters.AddWithValue("@cardNumber", UsersDetails.CardNumber);
                                    updateCommand.Parameters.AddWithValue("@pin", UsersDetails.Pin);
                                    updateCommand.ExecuteNonQuery();
                                }


                                /*string InsertHistoryQuery = @"INSERT INTO Transaction_History (UserCardNumber, TransactionType, Amount, Date)
                                                   VALUES (@UserCardNumber, @TransactionType, @Amount, @Date)";

                                using (SqlCommand historyCommand = new SqlCommand(InsertHistoryQuery, sqlConn, sqlTransaction))
                                {
                                    historyCommand.Parameters.AddWithValue("@UserCardNumber", UsersDetails.CardNumber);
                                    historyCommand.Parameters.AddWithValue("@TransactionType", TransactionType);
                                    historyCommand.Parameters.AddWithValue("@Amount", WithdrawalAmount);
                                    historyCommand.Parameters.AddWithValue("@Date", TransactionTime);

                                    historyCommand.ExecuteNonQuery();
                                }*/

                                Console.WriteLine($"Withdrawal successful. Your new balance is: #{Balance}\n\nPress ENTER to go to Main Menu");

                                Console.ReadKey();
                                Console.Clear();


                            }

                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("Error: {0}", e);
                        }
                        finally
                        {
                            sqlConn.Close();
                        }
                    }
                }
                catch (Exception e)
                {
                    sqlTransaction.Rollback();
                }
            }
        }

        public void Transfer()
        {

            Console.WriteLine("Enter Reciever Account Number: ");
            string input = Console.ReadLine();
            while (string.IsNullOrEmpty(input) || input == "0" || !decimal.TryParse(input, out _))
            {
                Console.WriteLine("Invalid input. Please enter a valid input: ");
                input = Console.ReadLine();
            }
            string RecieverAccountNo = input.ToString();


            bool isRecieverAccountNoValid = false;

            string VerifyRecieverQuery = "SELECT * FROM Users WHERE AccountNo = @recieverAccountNo";

            SqlConnection sqlConn = _dbContext.OpenConnection();

            using (SqlTransaction sqlTransaction = sqlConn.BeginTransaction())
            {
                try
                {
                    using (SqlCommand verifyCommand = new SqlCommand(VerifyRecieverQuery, sqlConn, sqlTransaction))
                    {
                        try
                        {
                            verifyCommand.Parameters.AddWithValue("@recieverAccountNo", RecieverAccountNo);

                            int result = (int)verifyCommand.ExecuteScalar();

                            if (result > 0)
                            {
                                isRecieverAccountNoValid = true;
                            }
                        }
                        catch (Exception)
                        {
                            Console.WriteLine("Transfer failed. Incorrect Reciever's Account Number.\nPlease crosscheck the account number and try again!");
                        }
                    }

                    if (isRecieverAccountNoValid)
                    {

                        Console.WriteLine("Enter amount you want to Transfer: ");
                        string input2 = Console.ReadLine();
                        while (string.IsNullOrEmpty(input2) || input2 == "0" || !decimal.TryParse(input2, out _))
                        {
                            Console.WriteLine("Invalid input. Please enter a valid amount: ");
                            input2 = Console.ReadLine();
                        }
                        decimal TransferAmount = Convert.ToDecimal(input2);


                        string BalanceQuery = "SELECT Balance FROM Users WHERE CardNumber = @cardNumber AND PIN = @pin";

                        using (SqlCommand command = new SqlCommand(BalanceQuery, sqlConn, sqlTransaction))
                        {
                            try
                            {
                                command.Parameters.AddWithValue("@cardNumber", UsersDetails.CardNumber);
                                command.Parameters.AddWithValue("@pin", UsersDetails.Pin);

                                decimal Balance = (decimal)command.ExecuteScalar();

                                if (Balance < TransferAmount)
                                {
                                    Console.WriteLine("You can't continue this tansaction because of Insufficient balance. Your current balance is: " + Balance);
                                    Transfer();
                                }
                                else
                                {
                                    Balance -= TransferAmount;

                                    string TransactionType = "Transfer";
                                    DateTime TransactionTime = DateTime.Now;

                                    string SenderQuery = "UPDATE Users SET Balance = @balance WHERE CardNumber = @cardNumber AND PIN = @pin";
                                    using (SqlCommand updateCommand = new SqlCommand(SenderQuery, sqlConn, sqlTransaction))
                                    {
                                        updateCommand.Parameters.AddWithValue("@balance", Balance);
                                        updateCommand.Parameters.AddWithValue("@cardNumber", UsersDetails.CardNumber);
                                        updateCommand.Parameters.AddWithValue("@pin", UsersDetails.Pin);
                                        updateCommand.ExecuteNonQuery();


                                        string RecieverQuery = "SELECT Balance FROM Users WHERE AccountNo = @recieverAccountNo";
                                        using (SqlCommand RecieverCommand = new SqlCommand(RecieverQuery, sqlConn, sqlTransaction))
                                        {
                                            RecieverCommand.Parameters.AddWithValue("@recieverAccountNo", RecieverAccountNo);
                                            decimal recieverBalance = (decimal)RecieverCommand.ExecuteScalar();
                                            recieverBalance += TransferAmount;

                                            string recieverUpdateQuery = "UPDATE Users SET Balance = @recieverBalance WHERE AccountNo = @recieverAccountNo";
                                            using (SqlCommand recieverUpdateCommand = new SqlCommand(recieverUpdateQuery, sqlConn, sqlTransaction))
                                            {
                                                recieverUpdateCommand.Parameters.AddWithValue("@recieverBalance", recieverBalance);
                                                recieverUpdateCommand.Parameters.AddWithValue("@recieverAccountNo", RecieverAccountNo);

                                                recieverUpdateCommand.ExecuteNonQuery();
                                            }

                                            string InsertHistoryQuery = @"INSERT INTO Transaction_History (UserCardNumber, TransactionType, Amount, Date)
                                                   VALUES (@UserCardNumber, @TransactionType, @Amount, @Date)";

                                            using (SqlCommand historyCommand = new SqlCommand(InsertHistoryQuery, sqlConn, sqlTransaction))
                                            {
                                                historyCommand.Parameters.AddWithValue("@UserCardNumber", UsersDetails.CardNumber);
                                                historyCommand.Parameters.AddWithValue("@TransactionType", TransactionType);
                                                historyCommand.Parameters.AddWithValue("@Amount", TransferAmount);
                                                historyCommand.Parameters.AddWithValue("@Date", TransactionTime);

                                                historyCommand.ExecuteNonQuery();
                                            }

                                            Console.WriteLine($"Transfer successful. Your new balance is: #{Balance}\n\nPress Enter to go to the Main Menu");

                                            Console.ReadKey();




                                        }

                                    }

                                }

                            }
                            catch (Exception e)
                            {
                                Console.WriteLine("Error: {0}", e);
                            }
                            finally
                            {
                                sqlConn.Close();
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine();
                        Transfer();
                    }
                }
                catch (Exception e)
                {
                    sqlTransaction.Rollback();
                }
            }


        }

        public void CheckBalance()
        {
            SqlConnection sqlConn = _dbContext.OpenConnection();

            string BalanceQuery = "SELECT Balance FROM Users WHERE CardNumber = @cardNumber AND PIN = @pin";

            using (SqlCommand command = new SqlCommand(BalanceQuery, sqlConn))
            {
                try
                {
                    command.Parameters.AddWithValue("@cardNumber", UsersDetails.CardNumber);
                    command.Parameters.AddWithValue("@pin", UsersDetails.Pin);

                    decimal Balance = (decimal)command.ExecuteScalar();
                    Console.WriteLine($"Your Account Balance is: #{Balance}");

                    Console.ReadKey();

                    Console.ReadLine();



                }
                catch (Exception e)
                {
                    Console.WriteLine("Error: {0}", e);
                }
                finally
                {
                    sqlConn.Close();
                }

            }
        }


        protected virtual void Dispose(bool disposing)
        {

            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                _dbContext.Dispose();
            }

            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }


    }

}

