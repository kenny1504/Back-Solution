using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> opt) : base(opt) {}

    public DbSet<Persona> Personas => Set<Persona>();
    public DbSet<Cliente> Clientes => Set<Cliente>();
    public DbSet<Cuenta> Cuentas => Set<Cuenta>();
    public DbSet<Movimiento> Movimientos => Set<Movimiento>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<Persona>().ToTable("Personas");
        b.Entity<Cliente>().ToTable("Clientes");
        b.Entity<Cliente>()
            .HasIndex(x => x.ClienteId).IsUnique();
        b.Entity<Cuenta>().ToTable("Cuentas");
        b.Entity<Cuenta>()
            .HasIndex(x => x.Numero).IsUnique();
        b.Entity<Cuenta>()
            .HasOne(x => x.Cliente)
            .WithMany(c => c.Cuentas)
            .HasForeignKey(x => x.ClienteIdFk)
            .OnDelete(DeleteBehavior.Restrict);
        b.Entity<Movimiento>().ToTable("Movimientos");
        b.Entity<Movimiento>()
            .HasOne(x => x.Cuenta)
            .WithMany(c => c.Movimientos)
            .HasForeignKey(x => x.CuentaIdFk)
            .OnDelete(DeleteBehavior.Cascade);
    }
}