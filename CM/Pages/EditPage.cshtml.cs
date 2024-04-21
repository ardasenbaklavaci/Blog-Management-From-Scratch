using CM.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;

namespace CM.Pages
{
    public class EditPageModel : PageModel
    {
        public List<Tree> trees = new List<Tree>();

        public Tree curr = new Tree();

        IConfiguration _configuration;

        public EditPageModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public void OnGet()
        {
            // get tree from SQL
            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string sql = "SELECT * FROM tree";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Tree add = new Tree();

                            add.id = reader.GetInt32(0);
                            add.name = reader.GetString(1);
                            add.parent = reader.GetInt32(2);
                            add.htmlcontent = reader.GetString(3);

                            trees.Add(add);
                        }

                    }
                }
            }
            String? str = Request.Query["id"];

            int re = -1;

            if (str != null) {
                re = int.Parse(str);
                curr = trees.Where(a => a.id == re).FirstOrDefault();
            }
            // get tree from SQL Done



        }

        public LocalRedirectResult OnPost()
        {
            String id = Request.Query["id"];

            String editName = Request.Form["nameForm"];
            String Sparent = Request.Form["parentForm"];
            String html = Request.Form["HTMLForm"];

            int parentEdit = -1;
            if(Sparent != null)
            {
                parentEdit = int.Parse(Sparent);
            }

            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            string sql = "UPDATE tree SET name = @name, HTMLContent = @HTMLContent, parent = @parent WHERE ID = @ID";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    // Add parameters to the SQL command to prevent SQL injection
                    command.Parameters.AddWithValue("@name", editName);
                    command.Parameters.AddWithValue("@parent", Sparent);
                    command.Parameters.AddWithValue("@HTMLContent", html);
                    // Assuming you have an Id field for the record to update, replace "Id" with your actual field name
                    command.Parameters.AddWithValue("@ID", id);

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
