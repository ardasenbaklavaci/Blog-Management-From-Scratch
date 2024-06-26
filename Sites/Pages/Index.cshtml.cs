using CM.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace Sites.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IConfiguration _configuration;

        public List<Tree> trees = new List<Tree>();
        public List<TreeNode> firstnodes = new List<TreeNode>();

        public String mainHtml = ""; // html to render for main page...

        public TreeNode root;
        public IndexModel(ILogger<IndexModel> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }
        public List<TreeNode> FindTopNodes(TreeNode startNode)
        {
            var topNodes = new List<TreeNode>();
            var visited = new HashSet<int>();
            var queue = new Queue<TreeNode>();
            queue.Enqueue(startNode);

            while (queue.Count > 0)
            {
                var currentNode = queue.Dequeue();
                visited.Add(currentNode.tree.id);

                if (currentNode.tree.id != 999 && currentNode.tree.parent == 999)
                {
                    topNodes.Add(currentNode);
                }

                foreach (var childNode in currentNode.Children)
                {
                    if (!visited.Contains(childNode.tree.id))
                    {
                        queue.Enqueue(childNode);
                    }
                }
            }

            return topNodes;
        }
        public TreeNode getTreeNode(int id)
        {
            return GetTreeNode(root, id);
        }
        public List<TreeNode> FindFirstSubNodes(int targetNodeId)
        {
            return FindFirstSubNodes(root, targetNodeId);
        }
        public List<TreeNode> FindFirstSubNodes(TreeNode startNode, int targetNodeId)
        {
            var subNodes = new List<TreeNode>();
            var visited = new HashSet<int>();
            var queue = new Queue<TreeNode>();
            queue.Enqueue(startNode);

            while (queue.Count > 0)
            {
                var currentNode = queue.Dequeue();
                visited.Add(currentNode.tree.id);

                if (currentNode.tree.id == targetNodeId)
                {
                    subNodes.AddRange(currentNode.Children);
                    break; // Stop searching further
                }

                foreach (var childNode in currentNode.Children)
                {
                    if (!visited.Contains(childNode.tree.id))
                    {
                        queue.Enqueue(childNode);
                    }
                }
            }

            return subNodes;
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
                            if (!reader.IsDBNull(6))
                            {
                                add.title = reader.GetString(6);
                            }
                            if(!reader.IsDBNull(7)) 
                            {
                                add.filename = reader.GetString(7);
                            }

                            if (add.filename.Equals("")) // setting default cshtml if user dont enter filename.
                            {
                                add.filename = "default";
                            }

                            trees.Add(add);

                            if(add.id == 999) // root (mainpage)
                            {
                                mainHtml = add.htmlcontent;
                            }
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
                add.tree.title = tree.title;
                add.tree.filename = tree.filename;
                add.tree.HasContent = tree.HasContent;
                add.tree.childcount = tree.childcount;
                nodeList.Add(add);
            }
            root = tp.ConstructTree(nodeList);

            //firstnodes = FindTopNodes(root); // Finding from root node 
            firstnodes = FindFirstSubNodes(999); //Finding from root_id
        }

    }
}
