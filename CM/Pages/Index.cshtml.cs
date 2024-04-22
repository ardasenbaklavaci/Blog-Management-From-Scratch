using CM.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;
using System.Text;

namespace CM.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IConfiguration _configuration;

        public List<Tree> trees = new List<Tree>();

        public String TreeHtml;
        public IndexModel(ILogger<IndexModel> logger,IConfiguration configuration)
        {
            _logger = logger;   
            _configuration = configuration;
        }

        public static string ConvertTreeToHtml(TreeNode root)
        {
            StringBuilder sb = new StringBuilder();

            // Start the tree
            sb.Append("<div class=\"tree\">");
            sb.Append("<ul>");

            // Generate HTML for the current node
            sb.Append("<li>");
            sb.Append("<a href=\"#\">" + root.Name + "</a>");

            // Recursively process child nodes
            if (root.Children.Count > 0)
            {
                sb.Append("<ul>");
                foreach (var child in root.Children)
                {
                    sb.Append(ConvertTreeToHtml(child));
                }
                sb.Append("</ul>");
            }

            // Close the current node
            sb.Append("</li>");

            // Close the tree
            sb.Append("</ul>");
            sb.Append("</div>");

            return sb.ToString();
        }

        public void OnGet()
        {
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
            TreePrinter tp = new TreePrinter();

            List<TreeNode> nodeList = new List<TreeNode>();
            foreach (Tree tree in trees)
            {
                TreeNode add = new TreeNode();
                add.Id = tree.id;
                add.Name = tree.name;
                add.ParentId = tree.parent;
                add.HtmlContent = tree.htmlcontent;

                nodeList.Add(add);
            }

            TreeNode root = tp.ConstructTree(nodeList);

            String htmlOutput = tp.PrintTree(root);
            TreeHtml = ConvertTreeToHtml(root);

        }

        
        
    }
}
