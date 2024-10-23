using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace BCCM1.Pages.Users
{
    public class HomepageModel : PageModel
    {
        public UsersInfo userInfo = new UsersInfo();
        public TransCate? transCate = new();

        public decimal inCome = 0;
        public decimal outCome = 0;

        public List<decimal> monthlyIncomes = new List<decimal>();
        public List<decimal> monthlyExpenses = new List<decimal>();

        public void OnGet()
            {
            string id = Request.Query["id"];
            try
                {
                    string connectionString = "Data Source = THANHHOA\\MSSQLSERVER01;Initial Catalog=HFINANCE;" + "Integrated Security=True;Pooling=False;TrustServerCertificate=True";
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        string sql = "Select * from Users where userId=@id";
                        using (SqlCommand command = new SqlCommand(sql, connection))
                        {
                        command.Parameters.AddWithValue("id", id);
                        using (SqlDataReader reader = command.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    userInfo.id = "" + reader.GetInt32(0);
                                    userInfo.email = reader.GetString(1);
                                }
                            }
                        }

                    string sqlIncome = "SELECT SUM(amount) AS TotalIncome FROM Transactions INNER JOIN Categories ON Transactions.categoryId=Categories.categoryId WHERE transType = 'Income' AND Transactions.userId=@id";
                    using (SqlCommand command = new SqlCommand(sqlIncome, connection))
                    {
                        command.Parameters.AddWithValue("id", id);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                inCome = reader.IsDBNull(0) ? 0 : reader.GetDecimal(0);
                                //if (reader.IsDBNull(0))
                                //{
                                //    transCate = null;
                                //}
                                //else
                                //{
                                //    transCate = new()
                                //    {
                                //        totalIncome = reader.GetDecimal(0).ToString()
                                //    };
                                //}      
                            }
                        }
                    }

                    string sqlExp = "SELECT SUM(amount) AS TotalExpense FROM Transactions INNER JOIN Categories ON Transactions.categoryId=Categories.categoryId WHERE transType = 'Expense' AND Transactions.userId=@id";
                    using (SqlCommand command = new SqlCommand(sqlExp, connection))
                    {
                        command.Parameters.AddWithValue("id", id);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                outCome = reader.IsDBNull(0) ? 0 : reader.GetDecimal(0);
                                //transCate.totalExpense = reader.GetDecimal(0).ToString();
                            }
                        }
                    }

                    for (int month = 1; month <= 12; month++)
                    {
                        string sqlIncomeDate = "SELECT SUM(amount) AS TotalIncome FROM Transactions INNER JOIN Categories ON Transactions.categoryId=Categories.categoryId WHERE transType = 'Income' AND Transactions.userId=@id AND MONTH(transDate) = @month";
                        using (SqlCommand command = new SqlCommand(sqlIncomeDate, connection))
                        {
                            command.Parameters.AddWithValue("id", id);
                            command.Parameters.AddWithValue("month", month);
                            using (SqlDataReader reader = command.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    decimal income = reader.IsDBNull(0) ? 0 : reader.GetDecimal(0);
                                    monthlyIncomes.Add(income);
                                }
                            }
                        }

                        string sqlExpDate = "SELECT SUM(amount) AS TotalExpense FROM Transactions INNER JOIN Categories ON Transactions.categoryId=Categories.categoryId WHERE transType = 'Expense' AND Transactions.userId=@id AND MONTH(transDate) = @month";
                        using (SqlCommand command = new SqlCommand(sqlExpDate, connection))
                        {
                            command.Parameters.AddWithValue("id", id);
                            command.Parameters.AddWithValue("month", month);
                            using (SqlDataReader reader = command.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    decimal expense = reader.IsDBNull(0) ? 0 : reader.GetDecimal(0);
                                    monthlyExpenses.Add(expense);
                                }
                            }
                        }
                    }
                }

            }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    throw;
                }
            }
    }
}
