using LogiTransPro.API.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace LogiTransPro.API.Data
{
    public class LogiTransProDbContext : DbContext
    {
        public LogiTransProDbContext(DbContextOptions<LogiTransProDbContext> options)
            : base(options)
        {
        }

        // DbSets para cada entidad
        public DbSet<Rol> Roles { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<LoginAttempt> LoginAttempts { get; set; }
        public DbSet<Vehiculo> Vehiculos { get; set; }
        public DbSet<Mantenimiento> Mantenimientos { get; set; }
        public DbSet<ParteMantenimiento> PartesMantenimiento { get; set; }
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<OrdenCarga> OrdenesCarga { get; set; }
        public DbSet<Ruta> Rutas { get; set; }
        public DbSet<Viaje> Viajes { get; set; }
        public DbSet<Incidente> Incidentes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ======================================================
            // CONVERTIR TODOS LOS DATETIME DE UTC A UNSPECIFIED
            // ======================================================
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                foreach (var property in entityType.GetProperties())
                {
                    if (property.ClrType == typeof(DateTime))
                    {
                        property.SetValueConverter(new Microsoft.EntityFrameworkCore.Storage.ValueConversion.ValueConverter<DateTime, DateTime>(
                            v => v.Kind == DateTimeKind.Utc ? DateTime.SpecifyKind(v, DateTimeKind.Unspecified) : v,
                            v => DateTime.SpecifyKind(v, DateTimeKind.Unspecified)));
                    }
                    else if (property.ClrType == typeof(DateTime?))
                    {
                        property.SetValueConverter(new Microsoft.EntityFrameworkCore.Storage.ValueConversion.ValueConverter<DateTime?, DateTime?>(
                            v => v.HasValue ? (v.Value.Kind == DateTimeKind.Utc ? DateTime.SpecifyKind(v.Value, DateTimeKind.Unspecified) : v.Value) : v,
                            v => v.HasValue ? DateTime.SpecifyKind(v.Value, DateTimeKind.Unspecified) : v));
                    }
                }
            }

            // ======================================================
            // CONFIGURACIÓN PARA POSTGRESQL - MANEJO DE FECHAS
            // ======================================================
            // Configurar todas las propiedades DateTime para usar timestamp sin zona horaria
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                foreach (var property in entityType.GetProperties())
                {
                    if (property.ClrType == typeof(DateTime) || property.ClrType == typeof(DateTime?))
                    {
                        property.SetColumnType("timestamp without time zone");
                    }
                }
            }

            // Configurar esquemas y nombres de tablas
            modelBuilder.Entity<Rol>().ToTable("rol", "seguridad");
            modelBuilder.Entity<Usuario>().ToTable("usuario", "seguridad");
            modelBuilder.Entity<RefreshToken>().ToTable("refresh_token", "seguridad");
            modelBuilder.Entity<LoginAttempt>().ToTable("login_attempt", "seguridad");

            modelBuilder.Entity<Vehiculo>().ToTable("vehiculo", "flotilla");
            modelBuilder.Entity<Mantenimiento>().ToTable("mantenimiento", "flotilla");
            modelBuilder.Entity<ParteMantenimiento>().ToTable("parte_mantenimiento", "flotilla");

            modelBuilder.Entity<Cliente>().ToTable("cliente", "comercial");
            modelBuilder.Entity<OrdenCarga>().ToTable("orden_carga", "comercial");

            modelBuilder.Entity<Ruta>().ToTable("ruta", "logistica");
            modelBuilder.Entity<Viaje>().ToTable("viaje", "logistica");
            modelBuilder.Entity<Incidente>().ToTable("incidente", "logistica");

            // Configurar llaves primarias
            modelBuilder.Entity<Rol>().HasKey(r => r.RolId);
            modelBuilder.Entity<Usuario>().HasKey(u => u.UsuarioId);
            modelBuilder.Entity<RefreshToken>().HasKey(rt => rt.RefreshTokenId);
            modelBuilder.Entity<LoginAttempt>().HasKey(la => la.LoginAttemptId);
            modelBuilder.Entity<Vehiculo>().HasKey(v => v.VehiculoId);
            modelBuilder.Entity<Mantenimiento>().HasKey(m => m.MantenimientoId);
            modelBuilder.Entity<ParteMantenimiento>().HasKey(p => p.ParteId);
            modelBuilder.Entity<Cliente>().HasKey(c => c.ClienteId);
            modelBuilder.Entity<OrdenCarga>().HasKey(oc => oc.OrdenCargaId);
            modelBuilder.Entity<Ruta>().HasKey(r => r.RutaId);
            modelBuilder.Entity<Viaje>().HasKey(v => v.ViajeId);
            modelBuilder.Entity<Incidente>().HasKey(i => i.IncidenteId);

            // Configurar propiedades (mapeo de columnas)
            // Usuario
            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.Property(u => u.UsuarioId).HasColumnName("usuario_id");
                entity.Property(u => u.NombreCompleto).HasColumnName("nombre_completo");
                entity.Property(u => u.CorreoElectronico).HasColumnName("correo_electronico");
                entity.Property(u => u.ContrasenaHash).HasColumnName("contrasena_hash");
                entity.Property(u => u.Telefono).HasColumnName("telefono");
                entity.Property(u => u.Estado).HasColumnName("estado");
                entity.Property(u => u.FechaRegistro).HasColumnName("fecha_registro");
                entity.Property(u => u.UltimoAcceso).HasColumnName("ultimo_acceso");
                entity.Property(u => u.RolId).HasColumnName("rol_id");
                entity.Property(u => u.UltimoCambioPassword).HasColumnName("ultimo_cambio_password");
                entity.Property(u => u.RequiereCambioPassword).HasColumnName("requiere_cambio_password");
                entity.Property(u => u.IntentosFallidos).HasColumnName("intentos_fallidos");
                entity.Property(u => u.BloqueadoHasta).HasColumnName("bloqueado_hasta");
            });

            // Vehiculo
            modelBuilder.Entity<Vehiculo>(entity =>
            {
                entity.Property(v => v.VehiculoId).HasColumnName("vehiculo_id");
                entity.Property(v => v.Placa).HasColumnName("placa");
                entity.Property(v => v.Vin).HasColumnName("vin");
                entity.Property(v => v.Marca).HasColumnName("marca");
                entity.Property(v => v.Modelo).HasColumnName("modelo");
                entity.Property(v => v.Anio).HasColumnName("anio");
                entity.Property(v => v.CapacidadCarga).HasColumnName("capacidad_carga");
                entity.Property(v => v.CapacidadVolumen).HasColumnName("capacidad_volumen");
                entity.Property(v => v.KilometrajeActual).HasColumnName("kilometraje_actual");
                entity.Property(v => v.NivelCombustible).HasColumnName("nivel_combustible");
                entity.Property(v => v.EstadoMotor).HasColumnName("estado_motor");
                entity.Property(v => v.EstadoGeneral).HasColumnName("estado_general");
                entity.Property(v => v.FechaRegistro).HasColumnName("fecha_registro");
                entity.Property(v => v.Activo).HasColumnName("activo");
            });

            // Viaje
            modelBuilder.Entity<Viaje>(entity =>
            {
                entity.Property(v => v.ViajeId).HasColumnName("viaje_id");
                entity.Property(v => v.NumeroViaje).HasColumnName("numero_viaje");
                entity.Property(v => v.OrdenCargaId).HasColumnName("orden_carga_id");
                entity.Property(v => v.VehiculoId).HasColumnName("vehiculo_id");
                entity.Property(v => v.ChoferId).HasColumnName("chofer_id");
                entity.Property(v => v.RutaId).HasColumnName("ruta_id");
                entity.Property(v => v.FechaSalidaProgramada).HasColumnName("fecha_salida_programada");
                entity.Property(v => v.FechaLlegadaProgramada).HasColumnName("fecha_llegada_programada");
                entity.Property(v => v.FechaSalidaReal).HasColumnName("fecha_salida_real");
                entity.Property(v => v.FechaLlegadaReal).HasColumnName("fecha_llegada_real");
                entity.Property(v => v.KilometrajeInicial).HasColumnName("kilometraje_inicial");
                entity.Property(v => v.KilometrajeFinal).HasColumnName("kilometraje_final");
                entity.Property(v => v.ConsumoCombustible).HasColumnName("consumo_combustible");
                entity.Property(v => v.Estatus).HasColumnName("estatus");
                entity.Property(v => v.Observaciones).HasColumnName("observaciones");
                entity.Property(v => v.FechaRegistro).HasColumnName("fecha_registro");
            });

            // Configurar relaciones
            modelBuilder.Entity<Usuario>()
                .HasOne(u => u.Rol)
                .WithMany(r => r.Usuarios)
                .HasForeignKey(u => u.RolId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Viaje>()
                .HasOne(v => v.Vehiculo)
                .WithMany(v => v.Viajes)
                .HasForeignKey(v => v.VehiculoId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Viaje>()
                .HasOne(v => v.Chofer)
                .WithMany(u => u.ViajesAsignados)
                .HasForeignKey(v => v.ChoferId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Viaje>()
                .HasOne(v => v.Ruta)
                .WithMany(r => r.Viajes)
                .HasForeignKey(v => v.RutaId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Viaje>()
                .HasOne(v => v.OrdenCarga)
                .WithMany(o => o.Viajes)
                .HasForeignKey(v => v.OrdenCargaId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Mantenimiento>()
                .HasOne(m => m.Vehiculo)
                .WithMany(v => v.Mantenimientos)
                .HasForeignKey(m => m.VehiculoId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ParteMantenimiento>()
                .HasOne(p => p.Mantenimiento)
                .WithMany(m => m.Partes)
                .HasForeignKey(p => p.MantenimientoId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<RefreshToken>()
                .HasOne(rt => rt.Usuario)
                .WithMany(u => u.RefreshTokens)
                .HasForeignKey(rt => rt.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Incidente>()
                .HasOne(i => i.Viaje)
                .WithMany(v => v.Incidentes)
                .HasForeignKey(i => i.ViajeId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Incidente>()
                .HasOne(i => i.ReportadoPorUsuario)
                .WithMany(u => u.IncidentesReportados)
                .HasForeignKey(i => i.ReportadoPor)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}