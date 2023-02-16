using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace ATM_DATA
{
    public class ATMDBContext : IDisposable
    {
        private readonly string _connectionString;

        bool _disposed = false;

        private SqlConnection _dbConnection = null;

        public ATMDBContext()
            : this(
                @"Data Source=LAPTOP-3L0EUQLC\SQLEXPRESS;Initial Catalog=ATM;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False"
            )
        { }

        public ATMDBContext(string connectionString)
        {
            _connectionString = connectionString;
        }


        public SqlConnection OpenConnection()
        {
            _dbConnection = new SqlConnection(_connectionString);
            _dbConnection.Open();
            return _dbConnection;
        }


        public void CloseConnection()
        {
            if (_dbConnection?.State != System.Data.ConnectionState.Closed)
            {
                _dbConnection?.Close();
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
                _dbConnection.Dispose();
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







