
using Microsoft.Data.SqlClient;
using System;

namespace ATM_DATA.Database
{
    public class CreateDatabase
    {
        public static void CreateATMDatabase()
        {

            string dbconnectionString =
               (@"data source = LAPTOP-3L0EUQLC\\SQLEXPRESS;Connect Timeout=30;Encrypt=False; Integrated security=true");


            string createQuery = $"CREATE DATABASE ATM";

            using (SqlConnection sqlConnection = new SqlConnection(dbconnectionString))
            {
                try
                {
                    sqlConnection.Open();

                    using (SqlCommand command = new SqlCommand(createQuery, sqlConnection))
                    {
                        command.ExecuteNonQuery();
                        Console.WriteLine("Database successfully created.");
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            Console.ReadLine();

        }

    }
}
