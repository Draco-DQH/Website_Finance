using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Transactions;

namespace BCCM1.Pages.Users
{
    public class TransactionsModel : PageModel
    {
        public UsersInfo userInfo = new UsersInfo();
        public string errorMessage = "";
        public decimal inCome = 0;
        public decimal outCome = 0;

        public List<Categories> listCate = new List<Categories>();
        public List<TransCate> transCates = new List<TransCate>();
        public void OnGet()
        {
            String id = Request.Query["id"];
            if (TempData["ErrorMessage"] != null)
            {
                errorMessage = TempData["ErrorMessage"].ToString();
            }
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

                    string sqlCate = "  Select categoryId,cateName from Categories WHERE userId IS NULL OR userId=@id ";
                    using (SqlCommand command = new SqlCommand(sqlCate, connection))
                    {
                        command.Parameters.AddWithValue("id", id);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Categories cate = new Categories();
                                cate.cateId = "" + reader.GetInt32(0);
                                cate.cateName= reader.GetString(1);
                                listCate.Add(cate);
                            }
                        }
                    }

                    string sqlTrans = "Select transactionId,amount,cateName,transType,transDate,description from Transactions INNER JOIN Categories ON Transactions.categoryId=Categories.categoryId where Transactions.userId=@id";
                    using (SqlCommand command = new SqlCommand(sqlTrans, connection))
                    {
                        command.Parameters.AddWithValue("id", id);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                TransCate transCate = new TransCate();

                                transCate.transId = "" + reader.GetInt32(0);
                                transCate.amount = reader.GetDecimal(1).ToString();
                                transCate.cateName = reader.GetString(2);
                                transCate.transType = reader.GetString(3);
                                transCate.transDate = reader.GetString(4);
                                transCate.description = reader.GetString(5);

                                transCates.Add(transCate);
                            }
                        }
                    }
                    string sqlIncome = "SELECT SUM(amount) AS TotalIncome FROM Transactions INNER JOIN Categories ON Transactions.categoryId=Categories.categoryId WHERE transType = 'Income' AND Transactions.userId=@id";
                    using (SqlCommand command = new SqlCommand(sqlIncome, connection))
                    {
                        command.Parameters.AddWithValue("id", id);
                        // inCome = (decimal)command.ExecuteScalar();
                        var result = command.ExecuteScalar();
                        if (result != null && result != DBNull.Value)
                        {
                            inCome = Convert.ToDecimal(result);
                        }
                        else
                        {
                            // Xử lý trường hợp giá trị trả về là DBNull
                            // Ví dụ: gán giá trị mặc định cho inCome hoặc xử lý một cách phù hợp
                        }
                    }

                    string sqlExp = "SELECT SUM(amount) AS TotalExpense FROM Transactions INNER JOIN Categories ON Transactions.categoryId=Categories.categoryId WHERE transType = 'Expense' AND Transactions.userId=@id";
                    using (SqlCommand command = new SqlCommand(sqlExp, connection))
                    {
                        command.Parameters.AddWithValue("id", id);
                        // outCome = (decimal)command.ExecuteScalar();
                        var result = command.ExecuteScalar();
                        if (result != null && result != DBNull.Value)
                        {
                            outCome = Convert.ToDecimal(result);
                        }
                    }
                    // Tính phần trăm thu nhập và chi tiêu
                    decimal total = inCome + outCome;
                    decimal incomePercentage = total != 0 ? (inCome / total) * 100 : 0; // Tránh chia cho 0
                    decimal expensePercentage = total != 0 ? (outCome / total) * 100 : 0; // Tránh chia cho 0

                    // Truyền giá trị phần trăm thu nhập và chi tiêu sang giao diện
                    ViewData["IncomePercentage"] = incomePercentage;
                    ViewData["ExpensePercentage"] = expensePercentage;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }


        public void OnPost()
        {

            string id = Request.Query["id"];

            Transactions trans = new Transactions();
            trans.amount = Request.Form["amount"];
            trans.cateId = Request.Form["cateId"];
            trans.transDate = Request.Form["transDate"];
            trans.description = Request.Form["description"];

            //check all fields are filled 
            if (string.IsNullOrEmpty(trans.amount) || string.IsNullOrEmpty(trans.cateId) || string.IsNullOrEmpty(trans.transDate) || string.IsNullOrEmpty(trans.description))
            {
                // Lưu thông báo lỗi vào TempData
                TempData["ErrorMessage"] = "All fields are required!!!";
                // Load lại trang với ID
                Response.Redirect("/Users/Transactions?id=" + id);
                return;
            }

            //if ok, save new client to database 
            try
            {
                string connectionString = "Data Source=THANHHOA\\MSSQLSERVER01;Initial Catalog=HFINANCE;Integrated Security = True; Pooling = False; TrustServerCertificate = True";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string addTrans = "INSERT INTO Transactions(userId,amount,categoryId,transDate,description) VALUES (@id, @amount, @cateId, @transDate,@description)";
                    using (SqlCommand command = new SqlCommand(addTrans, connection))
                    {
                        command.Parameters.AddWithValue("@id", id);
                        command.Parameters.AddWithValue("@amount", trans.amount);
                        command.Parameters.AddWithValue("@cateId", trans.cateId);
                        command.Parameters.AddWithValue("@transDate", trans.transDate);
                        command.Parameters.AddWithValue("@description", trans.description);

                        command.ExecuteNonQuery();
                    }


                }

                Response.Redirect("/Users/Transactions?id=" + id);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw;
            }
        }

    }
}
