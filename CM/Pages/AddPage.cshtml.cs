using CM.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace CM.Pages
{
    public class AddPageModel : PageModel
    {
        IConfiguration _configuration;

        public Tree add = new Tree();

        String name = "";
        String Sid = "";
        String Sparent = "";
        String Html = "";

        public Boolean chc = false;

        public AddPageModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public void OnGet()
        {
            chc = false;
        }
		
		public LocalRedirectResult OnPost()
        {
            Sid = Request.Form["idForm"];
            name = Request.Form["nameForm"];
            Sparent = Request.Form["parentForm"];
            Html = Request.Form["HTMLForm"];
            chc = Request.Form["checkbox1"] == "on";

            string sql = "INSERT INTO tree (ID, name, parent, HTMLContent, HasContent) VALUES (@ID, @name, @parent, @HTMLContent, @HasContent)";

            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    // Add parameters to the SQL command to prevent SQL injection
                    command.Parameters.AddWithValue("@ID", Sid);
                    command.Parameters.AddWithValue("@name", name);
                    command.Parameters.AddWithValue("@parent", Sparent);
                    command.Parameters.AddWithValue("@HTMLContent", Html);
                    if (chc==true)
                    {
                        int bi = 1;
                        command.Parameters.AddWithValue("@HasContent", bi);
                    }
                    else
                    {
                        int bi = 0;
                        command.Parameters.AddWithValue("@HasContent", bi);
                    }

                    try
                    {
                        // Open the connection and execute the command
                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        // Handle any exceptions
                        Console.WriteLine(ex.Message);
                        // You might want to handle exceptions differently in a production environment
                    }
                }
            }

            return LocalRedirect("/Index");


        }
    }
}
