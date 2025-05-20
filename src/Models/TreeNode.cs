namespace TreeJournalApi.Models
{
    public class TreeNode
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public long? ParentId { get; set; }
        public TreeNode Parent { get; set; }
        public List<TreeNode> Children { get; set; } = new List<TreeNode>();
        public string TreeName { get; set; }
    }

}
