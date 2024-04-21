namespace CM.Models
{
    public class TreeNode
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int ParentId { get; set; }
        public string HtmlContent { get; set; }
        public List<TreeNode> Children { get; set; } = new List<TreeNode>();
    }
}
