using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace BCCM1.Pages.Users
{
    public class SignupModel : PageModel
    {
        public UsersInfo userInfo = new UsersInfo();
        public string errorMessage = ""; //hiển thị thông báo lỗi khi người dùng ko nhập đủ thông tin 

        public void OnGet()
        {
        }

        public void OnPost()
        {
            userInfo.fName = Request.Form["firstname"];
            userInfo.lName = Request.Form["lastname"];
            userInfo.email = Request.Form["email"];
            userInfo.pass = Request.Form["pass"];
            userInfo.gender = Request.Form["gender"];



            //check all fields are filled 
            if (string.IsNullOrEmpty(userInfo.email) || string.IsNullOrEmpty(userInfo.pass) || string.IsNullOrEmpty(userInfo.fName) || string.IsNullOrEmpty(userInfo.lName) || string.IsNullOrEmpty(userInfo.gender))
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
                    string checkEmailQuery = "SELECT COUNT(*) FROM Users WHERE email = @Email";
                    using (SqlCommand checkEmailCommand = new SqlCommand(checkEmailQuery, connection))
                    {
                        checkEmailCommand.Parameters.AddWithValue("@Email", userInfo.email);
                        int emailCount = (int)checkEmailCommand.ExecuteScalar();

                        // Nếu email đã tồn tại, hiển thị thông báo lỗi
                        if (emailCount > 0)
                        {
                            errorMessage = "Email already exists in this domain!!";
                            return;
                        }
                    }

                    string sql = "INSERT INTO Users(email,passWord,firstName,lastName,gender) VALUES (@Email, @Password, @FirstName, @LastName, @Gender)";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@Email", userInfo.email);
                        command.Parameters.AddWithValue("@Password", userInfo.pass);
                        command.Parameters.AddWithValue("@FirstName", userInfo.fName);
                        command.Parameters.AddWithValue("@LastName", userInfo.lName);
                        command.Parameters.AddWithValue("@Gender", userInfo.gender);

                        command.ExecuteNonQuery();
                    }
                }

                // Clear thông tin cho lần nhập tiếp theo
                userInfo.fName = "";
                userInfo.lName = "";
                userInfo.email = "";
                userInfo.pass = "";
                userInfo.gender = "";

                // Redirect đến trang đăng nhập
                Response.Redirect("/Users/Login");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw;
            }
        }
    }
}
