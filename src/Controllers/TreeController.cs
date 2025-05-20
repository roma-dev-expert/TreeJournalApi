using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TreeJournalApi.Data;
using TreeJournalApi.Models;

namespace TreeJournalApi.Controllers
{
    [ApiController]
    [Route("api.user.tree")]
    public class TreeController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TreeController(AppDbContext context)
        {
            _context = context;
        }

        // POST /api.user.tree.get?treeName=MyTree
        [HttpPost("get")]
        public async Task<IActionResult> GetTree([FromQuery] string treeName)
        {
            var tree = await _context.TreeNodes
                .Include(t => t.Children)
                .FirstOrDefaultAsync(t => t.TreeName == treeName && t.ParentId == null);

            if (tree == null)
            {
                tree = new TreeNode { TreeName = treeName, Name = treeName };

                _context.TreeNodes.Add(tree);
                await _context.SaveChangesAsync();
            }

            return Ok(tree);
        }

        // POST /api.user.tree.node.create?treeName=MyTree&parentNodeId=1&nodeName=NewNode
        [HttpPost("node.create")]
        public async Task<IActionResult> CreateNode([FromQuery] string treeName, [FromQuery] long parentNodeId, [FromQuery] string nodeName)
        {
            var parent = await _context.TreeNodes
                .Include(t => t.Children)
                .FirstOrDefaultAsync(t => t.Id == parentNodeId && t.TreeName == treeName);

            if (parent == null) return BadRequest("Parent node not found.");

            if (parent.Children.Any(c => c.Name.Equals(nodeName, StringComparison.OrdinalIgnoreCase)))
                return BadRequest("Node name must be unique among siblings.");

            var newNode = new TreeNode { Name = nodeName, ParentId = parentNodeId, TreeName = treeName };

            _context.TreeNodes.Add(newNode);
            await _context.SaveChangesAsync();

            return Ok(newNode);
        }

        // POST /api.user.tree.node.delete?treeName=MyTree&nodeId=5
        [HttpPost("node.delete")]
        public async Task<IActionResult> DeleteNode([FromQuery] string treeName, [FromQuery] long nodeId)
        {
            var node = await _context.TreeNodes.FirstOrDefaultAsync(t => t.Id == nodeId && t.TreeName == treeName);
            if (node == null) return BadRequest("Node not found.");

            _context.TreeNodes.Remove(node);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpPost("node.rename")]
        public IActionResult RenameNode([FromQuery] string treeName, [FromQuery] long nodeId, [FromQuery] string newNodeName)
        {
            var node = _context.TreeNodes.FirstOrDefault(n => n.Id == nodeId && n.TreeName == treeName);
            if (node == null)
                return NotFound("Node not found");

            var siblings = _context.TreeNodes.Where(n => n.ParentId == node.ParentId && n.Id != nodeId);
            if (siblings.Any(n => n.Name == newNodeName))
                return BadRequest("Node name must be unique among siblings");

            node.Name = newNodeName;
            _context.SaveChanges();
            return Ok(node);
        }
    }
}
