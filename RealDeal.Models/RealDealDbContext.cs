using Microsoft.EntityFrameworkCore;
using RealDeal.Classes;

namespace RealDeal;

public class RealDealDbContext : DbContext
{
	public RealDealDbContext(DbContextOptions<RealDealDbContext> options) : base(options)
	{

	}

	public DbSet<Bet> Bets { get; set; }

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<Bet>().HasKey(b => b.Id);
		base.OnModelCreating(modelBuilder);
	}
}
