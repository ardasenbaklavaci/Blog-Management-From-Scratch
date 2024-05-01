using CM.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Xml.Linq;

namespace CM.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IConfiguration _configuration;
        public IndexModel(ILogger<IndexModel> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public List<Tree> trees = new List<Tree>();

        public TreeNode root;

        public String TreeHtml;
        
        public void UpdateChildCounts(TreeNode node)
        {
            // Set the initial child count of the node
            node.tree.childcount = node.Children.Count;

			string connectionString = _configuration.GetConnectionString("DefaultConnection");

			string updateStatement = "UPDATE tree SET childcount = @childcount WHERE ID = @ConditionValue";

			using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
				using (SqlCommand command = new SqlCommand(updateStatement, connection))
				{
					command.Parameters.AddWithValue("@childcount", node.tree.childcount);
					command.Parameters.AddWithValue("@ConditionValue", node.tree.id);

					int rowsAffected = command.ExecuteNonQuery();

					if (rowsAffected > 0)
					{
						// Update successful
					}
					else
					{
						// No rows updated
					}
				}
			}
			//

			// Recursively traverse each child
			foreach (var child in node.Children)
            {
                // Update child counts for each child node
                UpdateChildCounts(child);
            }
        }

        static List<Tree> GetTreesList(TreeNode rootNode)
        {
            // Initialize a list to store the updated trees
            List<Tree> updatedTrees = new List<Tree>();

            // Function to traverse the tree and add nodes to the list
            void Traverse(TreeNode node)
            {
                // Add the current node's tree info to the list
                updatedTrees.Add(node.tree);

                // Recursively traverse each child
                foreach (var child in node.Children)
                {
                    Traverse(child);
                }
            }

            // Start traversal from the root node
            Traverse(rootNode);

            return updatedTrees;
        }

        public static string ConvertTreeToHtml(TreeNode root)
        {
			StringBuilder sb = new StringBuilder();

			// Start the tree
			sb.Append("<ul class=\"tree\">");

			// Generate HTML for the current node
			sb.Append("<li>");
			sb.Append("<span>" + root.tree.name + "</span>");

			// Recursively process child nodes
			if (root.Children.Count > 0)
			{
				sb.Append("<ul>");
				foreach (var child in root.Children)
				{
					sb.Append("<li>");
					sb.Append(ConvertTreeToHtml(child));
					sb.Append("</li>");
				}
				sb.Append("</ul>");
			}

			// Close the current node
			sb.Append("</li>");

			// Close the tree
			sb.Append("</ul>");

			return sb.ToString();
		}

        public TreeNode getTreeNode(int id)
        {
            return GetTreeNode(root, id);
        }
        TreeNode GetTreeNode(TreeNode node, int id)
        {
            if (node == null)
                return null;

            // Check if the current node's id matches the desired id
            if (node.tree.id == id)
                return node;

            // Search in the children
            foreach (var child in node.Children)
            {
                TreeNode foundNode = GetTreeNode(child, id);
                if (foundNode != null)
                    return foundNode;
            }

            // If not found, return null
            return null;
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
                            if (!reader.IsDBNull(3))
                            {
                                add.htmlcontent = reader.GetString(3);
                            }
                            add.childcount = reader.GetInt32(4);
                            add.HasContent = reader.GetBoolean(5);
                            if (!reader.IsDBNull(6))
                            {
                                add.title = reader.GetString(6);
                            }
                            if (!reader.IsDBNull(7))
                            {
                                add.filename = reader.GetString(7);
                            }

                            /*if (add.id == 999)
                            {
                                mainHtml = add.htmlcontent;
                            }
                            */
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
                add.tree.id = tree.id;
                add.tree.name = tree.name;
                add.tree.parent = tree.parent;
                add.tree.htmlcontent = tree.htmlcontent;
                add.tree.HasContent = tree.HasContent;
                add.tree.title = tree.title;
                add.tree.filename = tree.filename;
                nodeList.Add(add);
            }

            root = tp.ConstructTree(nodeList);

            String htmlOutput = tp.PrintTree(root);

            UpdateChildCounts(root);

            TreeHtml = ConvertTreeToHtml(root);

            trees = GetTreesList(root);
        }

        
        
    }
}
