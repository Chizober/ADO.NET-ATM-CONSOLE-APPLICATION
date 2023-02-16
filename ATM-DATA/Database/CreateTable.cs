
using ATM_DATA;
using Microsoft.Data.SqlClient;
using System;
namespace ATM_DATA.Database
{
    public class CreateTable : IDisposable
    {
        private readonly ATMDBContext _dbContext;
        private bool _disposed;

        public CreateTable(ATMDBContext dbContext)
        {
            _dbContext = dbContext;
        }
        public CreateTable()
        {
        }
        public void CreateUsersTable()
        {
            SqlConnection sqlConnection = _dbContext.OpenConnection();



            string query2 = @"Create table Users(
                                UserID int Primary Key IDENTITY(1,1) NOT NULL,
                                FirstName VARCHAR(50) NOT NULL,
                                LastName VARCHAR(50) NOT NULL,
                                Email  VARCHAR(50) NULL,
                                Phone VARCHAR(50)  NULL,
                                AccountName VARCHAR(50) NOT NULL,
                                AccountNo VARCHAR(50) NOT NULL,
                                AccountType VARCHAR(50) NULL,
                                CardNumber VARCHAR(50) NOT NULL,
                                Pin VARCHAR(30) NOT NULL,
                                Balance DECIMAL(15, 2) NULL
)";




            using (SqlCommand command = new SqlCommand(query2, sqlConnection))
            {
                try
                {
                    command.ExecuteNonQuery();
                    Console.WriteLine("table created  successfully.");

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


