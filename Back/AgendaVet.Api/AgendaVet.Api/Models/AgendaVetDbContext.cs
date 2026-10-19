using AgendaVet.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace AgendaVet.Api.Models
{
    public class AgendaVetDbContext : DbContext
    {
        public AgendaVetDbContext(DbContextOptions<AgendaVetDbContext> options) : base(options)
        {
        }

        public DbSet<Rol> Roles { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Propietario> Propietarios { get; set; }
        public DbSet<Especie> Especies { get; set; }
        public DbSet<Raza> Razas { get; set; }
        public DbSet<Paciente> Pacientes { get; set; }
        public DbSet<Servicio> Servicios { get; set; }
        public DbSet<Cita> Citas { get; set; }
        public DbSet<HistoriaClinica> HistoriasClinicas { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 1. ROL
            modelBuilder.Entity<Rol>(entity =>
            {
                entity.HasKey(e => e.IdRol);
                entity.Property(e => e.Nombre).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Descripcion).HasMaxLength(200);
            });

            // 2. USUARIO
            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.HasKey(e => e.IdUsuario);
                entity.Property(e => e.Nombre).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Apellido).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
                entity.HasIndex(e => e.Email).IsUnique();
                entity.Property(e => e.Password).IsRequired().HasMaxLength(255);
                entity.Property(e => e.Telefono).HasMaxLength(20);
                entity.Property(e => e.Activo).HasDefaultValue(true);
                entity.Property(e => e.FechaRegistro).HasDefaultValueSql("CURRENT_TIMESTAMP(6)");

                entity.HasOne<Rol>()
                      .WithMany()
                      .HasForeignKey(e => e.IdRol)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // 3. PROPIETARIO
            modelBuilder.Entity<Propietario>(entity =>
            {
                entity.HasKey(e => e.IdPropietario);
                entity.Property(e => e.Nombre).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Apellido).IsRequired().HasMaxLength(50);
                entity.Property(e => e.DniCe).IsRequired().HasMaxLength(15);
                entity.HasIndex(e => e.DniCe).IsUnique();
                entity.Property(e => e.Telefono).IsRequired().HasMaxLength(20);
                entity.Property(e => e.Email).HasMaxLength(100);
                entity.Property(e => e.Direccion).HasMaxLength(200);
                entity.Property(e => e.FechaRegistro).HasDefaultValueSql("CURRENT_TIMESTAMP(6)");
            });

            // 4. ESPECIE
            modelBuilder.Entity<Especie>(entity =>
            {
                entity.HasKey(e => e.IdEspecie);
                entity.Property(e => e.Nombre).IsRequired().HasMaxLength(50);
            });

            // 5. RAZA
            modelBuilder.Entity<Raza>(entity =>
            {
                entity.HasKey(e => e.IdRaza);
                entity.Property(e => e.Nombre).IsRequired().HasMaxLength(50);

                entity.HasOne<Especie>()
                      .WithMany()
                      .HasForeignKey(e => e.IdEspecie)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // 6. PACIENTE
            modelBuilder.Entity<Paciente>(entity =>
            {
                entity.HasKey(e => e.IdPaciente);
                entity.Property(e => e.Nombre).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Sexo).IsRequired().HasMaxLength(10);
                entity.Property(e => e.Color).HasMaxLength(30);
                entity.Property(e => e.Observaciones).HasMaxLength(500);
                entity.Property(e => e.FechaRegistro).HasDefaultValueSql("CURRENT_TIMESTAMP(6)");

                entity.HasOne<Propietario>()
                      .WithMany()
                      .HasForeignKey(e => e.IdPropietario)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne<Especie>()
                      .WithMany()
                      .HasForeignKey(e => e.IdEspecie)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne<Raza>()
                      .WithMany()
                      .HasForeignKey(e => e.IdRaza)
                      .OnDelete(DeleteBehavior.SetNull);
            });

            // 7. SERVICIO
            modelBuilder.Entity<Servicio>(entity =>
            {
                entity.HasKey(e => e.IdServicio);
                entity.Property(e => e.Nombre).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Descripcion).HasMaxLength(255);
                entity.Property(e => e.Precio).HasPrecision(10, 2).IsRequired();
                entity.Property(e => e.DuracionEstimadaMin).HasDefaultValue(30);
                entity.Property(e => e.Activo).HasDefaultValue(true);
            });

            // 8. CITA
            modelBuilder.Entity<Cita>(entity =>
            {
                entity.HasKey(e => e.IdCita);
                entity.Property(e => e.Estado).IsRequired().HasMaxLength(30).HasDefaultValue("Pendiente");
                entity.Property(e => e.Motivo).HasMaxLength(255);
                entity.Property(e => e.NotasAgendamiento).HasMaxLength(500);
                entity.Property(e => e.FechaCreacion).HasDefaultValueSql("CURRENT_TIMESTAMP(6)");

                entity.HasOne<Paciente>()
                      .WithMany()
                      .HasForeignKey(e => e.IdPaciente)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne<Usuario>()
                      .WithMany()
                      .HasForeignKey(e => e.IdVeterinario)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne<Servicio>()
                      .WithMany()
                      .HasForeignKey(e => e.IdServicio)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // 9. HISTORIA CLINICA
            modelBuilder.Entity<HistoriaClinica>(entity =>
            {
                entity.HasKey(e => e.IdHistoria);
                entity.Property(e => e.PesoKg).HasPrecision(5, 2);
                entity.Property(e => e.Temperatura).HasPrecision(4, 2);
                entity.Property(e => e.Diagnostico).IsRequired();

                entity.HasOne<Cita>()
                      .WithMany()
                      .HasForeignKey(e => e.IdCita)
                      .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne<Paciente>()
                      .WithMany()
                      .HasForeignKey(e => e.IdPaciente)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne<Usuario>()
                      .WithMany()
                      .HasForeignKey(e => e.IdVeterinario)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // SEED DATA
            modelBuilder.Entity<Rol>().HasData(
                new Rol { IdRol = 1, Nombre = "Administrador", Descripcion = "Acceso total" },
                new Rol { IdRol = 2, Nombre = "Veterinario", Descripcion = "Atención médica" },
                new Rol { IdRol = 3, Nombre = "Recepcionista", Descripcion = "Gestión de citas y clientes" }
            );

            modelBuilder.Entity<Especie>().HasData(
                new Especie { IdEspecie = 1, Nombre = "Canino" },
                new Especie { IdEspecie = 2, Nombre = "Felino" },
                new Especie { IdEspecie = 3, Nombre = "Ave" },
                new Especie { IdEspecie = 4, Nombre = "Roedor" }
            );
        }
    }
}