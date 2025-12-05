using DeteksiJalanRusak.Web.Entities;
using Microsoft.EntityFrameworkCore;

namespace DeteksiJalanRusak.Web.Database;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<Analisis>().HasKey(x => x.Id);
        builder.Entity<Analisis>().HasMany(x => x.DaftarSegmen).WithOne(x => x.Analisis);

        builder.Entity<Segmen>().HasKey(x => x.Id);
        builder.Entity<Segmen>().HasMany(x => x.DaftarFoto).WithOne(x => x.Segmen);
        builder.Entity<Segmen>().HasOne(x => x.Analisis).WithMany(x => x.DaftarSegmen);

        builder.Entity<FotoKerusakan>().HasKey(x => x.Id);
        builder.Entity<FotoKerusakan>().HasMany(x => x.DaftarKerusakan).WithOne(x => x.FotoKerusakan);
        builder.Entity<FotoKerusakan>().HasOne(x => x.Segmen).WithMany(x => x.DaftarFoto);

        builder.Entity<Kerusakan>().HasKey(x => x.Id);
        builder.Entity<Kerusakan>().HasOne(x => x.FotoKerusakan).WithMany(x => x.DaftarKerusakan);

        base.OnModelCreating(builder);
    }

    public DbSet<Analisis> TblAnalisis { get; set; }
    public DbSet<Segmen> TblSegmen { get; set; }
    public DbSet<FotoKerusakan> TblFoto { get; set; }
    public DbSet<Kerusakan> TblKerusakan { get; set; }
}
