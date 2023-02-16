
using System;
using ATM_DATA;
using Microsoft.Data.SqlClient;

namespace ATM_DATA.Database
{
    public class InsertUsers  : IDisposable
    {
        private readonly ATMDBContext _dbContext;
        private bool _disposed;

        public InsertUsers(ATMDBContext dbContext)
        {
            _dbContext = dbContext;
        }
        public InsertUsers()
        {

        }
        public void InsertUsersQuery()
        {
            SqlConnection connection = _dbContext.OpenConnection();

            string insert =
                @"INSERT INTO Users(FirstName,LastName,Email,Phone,AccountName,AccountNo,AccountType,CardNumber,Pin,Balance)
                      VALUES
  ('Charlse','Okoro','charlse.okoro@gmail.com','070-234-123-20','Charlse Okoro','1223345601','Current','1010101010','1111','200000.56'),
  ('Chizoba','Egbujie','chizoba.egbujie@gmail.com','091-114-155-20','Egbujie Chizoba','3445670910','Current','5050505050','5555','900000000.00'),
  ('Kendrick','Chibueze','kendrick.chibueze@gmail.com','070-234-111-20','Kendrick Chibueze','2233448610','Savings','2020202020','2222','4000000.00'),
  ('Chinenye','Okeke','chinenye.okeke@gmail.com','090-134-199-20','Chinenye Okeke','6756432190','Savings','3030303030','3333','90000000.10'),
  ('Ugochukwu','Agu','ugochukwu.agu@gmail.com','090-114-193-00','Ugochukwu Agu','7656789041','Current','4040404040','4444','78400000.25')
                    ";

            using (SqlCommand command = new SqlCommand(insert, connection))
            {
                try
                {
                    command.ExecuteNonQuery();
                    Console.WriteLine("Users inserted  successfully.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: {0}", ex.Message);
                }
                finally
                {
                    _dbContext.CloseConnection();
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
