using System.Text;

namespace CM.Models
{
    public class TreePrinter
    {
        public String PrintTree(TreeNode root, string indent = "", bool last = true)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(indent);
            if (last)
            {
                sb.Append("\\-");
                indent += "  ";
            }
            else
            {
                sb.Append("|-");
                indent += "| ";
            }
            sb.AppendLine(root.tree.name);

            var childrenCount = root.Children.Count;
            for (int i = 0; i < childrenCount; i++)
            {
                sb.Append(PrintTree(root.Children[i], indent, i == childrenCount - 1));
            }

            return sb.ToString();
        }

        public void PrintNode(TreeNode node, StringBuilder sb, int level = 0)
        {
            sb.Append(new string('-', level * 4));
            sb.AppendLine($"<div>{node.tree.name}</div>");
            foreach (var child in node.Children)
            {
                PrintNode(child, sb, level + 1);
            }
        }

        public TreeNode ConstructTree(List<TreeNode> nodeList)
        {
            Dictionary<int, TreeNode> nodeMap = new Dictionary<int, TreeNode>();

            // Create tree nodes and store them in a dictionary for easy access
            foreach (var node in nodeList)
            {
                nodeMap[node.tree.id] = node;
            }

            // Link child nodes to their parent nodes
            foreach (var node in nodeList)
            {
                if (node.tree.parent != -1) // Skip root node (with parent ID of -1)
                {
                    TreeNode parentNode = nodeMap[node.tree.parent];
                    TreeNode childNode = nodeMap[node.tree.id];
                    parentNode.Children.Add(childNode);
                }
            }

            // Find the root node (with ID of 999)
            TreeNode root = nodeMap.Values.FirstOrDefault(node => node.tree.id == 999);

            return root;
        }

    }
}
