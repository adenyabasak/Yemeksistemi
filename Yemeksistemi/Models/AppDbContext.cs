using Microsoft.EntityFrameworkCore;
using Yemeksistemi.Models;
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
    : base(options)
    {
    }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Recipe> Recipes { get; set; }
    public DbSet<About> Abouts { get; set; }
    public DbSet<Contact> Contacts { get; set; }
    public DbSet<Food> Foods { get; set; }
}
