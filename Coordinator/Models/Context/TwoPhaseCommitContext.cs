using Microsoft.EntityFrameworkCore;

namespace Coordinator.Models.Context
{
    public class TwoPhaseCommitContext : DbContext
    {
        
        public TwoPhaseCommitContext(DbContextOptions<TwoPhaseCommitContext> options) : base(options)
        {
        }

        DbSet<Node> Nodes { get; set; }
        DbSet<NodeState> NodeStates { get; set; }
    }
}
