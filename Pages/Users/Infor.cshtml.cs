using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace BCCM1.Pages.Users
{
    public class InforModel : PageModel
    {
        public UsersInfo userInfo = new UsersInfo();
        public string errorMessage = "";
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
                                userInfo.pass = reader.GetString(2);
                                userInfo.fName = reader.GetString(3);
                                userInfo.lName = reader.GetString(4);
                                userInfo.gender = reader.GetString(5);
                                userInfo.birthDay = reader.IsDBNull(6) ? " " : reader.GetString(6);
                                userInfo.address = reader.IsDBNull(7) ? " " : reader.GetString(7);
                                userInfo.phone = reader.IsDBNull(8) ? " " : reader.GetString(8);
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

        public void OnPost()
        {
            userInfo.id = Request.Form["id"];
            userInfo.fName = Request.Form["fName"];
            userInfo.lName = Request.Form["lName"];
            userInfo.birthDay = Request.Form["birthDay"];
            userInfo.gender = Request.Form["gender"];
            userInfo.email = Request.Form["email"];
            userInfo.pass = Request.Form["pass"];
            userInfo.phone = Request.Form["phone"];
            userInfo.address = Request.Form["address"];


            //check all fields are filled 
            if (string.IsNullOrEmpty(userInfo.email) || string.IsNullOrEmpty(userInfo.pass) || string.IsNullOrEmpty(userInfo.fName) || string.IsNullOrEmpty(userInfo.lName) 
                || string.IsNullOrEmpty(userInfo.gender)|| string.IsNullOrEmpty(userInfo.birthDay) || string.IsNullOrEmpty(userInfo.phone) || string.IsNullOrEmpty(userInfo.address))
            {
                errorMessage = "All fields are required!!!";
                return;
            }

            try
            {
                string connectionString = "Data Source=THANHHOA\\MSSQLSERVER01;Initial Catalog=HFINANCE;Integrated Security = True; Pooling = False; TrustServerCertificate = True";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string sql = "UPDATE Users SET email=@email,passWord=@pass,firstName=@fName,lastName=@lName,gender=@gender,birthDay=@birthDay,address=@address,phone=@phone WHERE userId=@id";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@email", userInfo.email);
                        command.Parameters.AddWithValue("@pass", userInfo.pass);
                        command.Parameters.AddWithValue("@fName", userInfo.fName);
                        command.Parameters.AddWithValue("@lName", userInfo.lName);
                        command.Parameters.AddWithValue("@gender", userInfo.gender);
                        command.Parameters.AddWithValue("@birthDay",userInfo.birthDay);
                        command.Parameters.AddWithValue("@address",userInfo.address);
                        command.Parameters.AddWithValue("@phone",userInfo.phone);
                        command.Parameters.AddWithValue("@id", userInfo.id);

                        command.ExecuteNonQuery();
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw;
            }
            Response.Redirect("/Users/Infor?id=" + userInfo.id);
        }  
    }
}
