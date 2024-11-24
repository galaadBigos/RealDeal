using Microsoft.EntityFrameworkCore;

namespace RealDeal;

public class RealDealDbContext : DbContext
{
	public RealDealDbContext(DbContextOptions<RealDealDbContext> options) : base(options)
	{

	}

	public DbSet<Shared.Models.Bet> Bets { get; set; }

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<Shared.Models.Bet>().HasKey(b => b.Id);
		base.OnModelCreating(modelBuilder);
	}
}
