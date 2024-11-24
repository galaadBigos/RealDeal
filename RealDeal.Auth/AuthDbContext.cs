using Microsoft.EntityFrameworkCore;
using RealDeal.Shared.Models;

namespace RealDeal.Auth;

public class AuthDbContext : DbContext
{
	public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options)
	{

	}

	public DbSet<User> Users { get; set; }

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{

		base.OnModelCreating(modelBuilder);
	}
}
