using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace BCCM1.Pages.Users
{
    public class LoginModel : PageModel
    {
        public UsersInfo userInfo = new UsersInfo();
        public string errorMessage = ""; //hiển thị thông báo lỗi khi người dùng ko nhập đủ thông tin 

        public void OnGet()
        {
        }

        public void OnPost() //xử lý http method: POST xảy ra khi người dùng click Submit button 
        {
            //property Request biểu diễn các thông tin do user gửi yêu cầu (request) đến Server 
            userInfo.email = Request.Form["email"];
            userInfo.pass = Request.Form["pass"];        

            //check all fields are filled 
            if (string.IsNullOrEmpty(userInfo.email) || string.IsNullOrEmpty(userInfo.pass))
            {
                errorMessage = "All fields are required!!!";
                return;
            }

            //if ok, save new client to database 
            try
            {
                string connectionString = "Data Source=THANHHOA\\MSSQLSERVER01;Initial Catalog=HFINANCE;Integrated Security = True; Pooling = False; TrustServerCertificate = True";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string sql = "SELECT * FROM Users WHERE email=@email AND passWord = @passWord";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@email", userInfo.email);
                        command.Parameters.AddWithValue("@passWord", userInfo.pass);

                        // Thực thi truy vấn và nhận về một SqlDataReader
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            // Kiểm tra xem có dòng dữ liệu nào được trả về không
                            if (reader.Read())
                            {

                                userInfo.id = ""+reader.GetInt32(0);
                                userInfo.email = reader.GetString(1);
                                userInfo.pass = reader.GetString(2);


                                // Chuyển hướng đến trang Homepage với tham số truy vấn URL userId
                                Response.Redirect("/Users/Homepage?id=" + userInfo.id);
                            }
                            else
                            {
                                userInfo.pass = "";
                                errorMessage = "Login Failed! Incorrect account or password!!!";
                                return;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw;
            }
        }
    }
}
