namespace CM.Models
{
    public class TreeNode
    {
        public Tree tree = new Tree();
        public List<TreeNode> Children { get; set; } = new List<TreeNode>();
    }
}
