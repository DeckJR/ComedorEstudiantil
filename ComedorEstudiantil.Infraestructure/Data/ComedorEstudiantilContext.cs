using System;
using System.Collections.Generic;
using ComedorEstudiantil.Infraestructure.Models;
using Microsoft.EntityFrameworkCore;

namespace ComedorEstudiantil.Infraestructure.Data;

public partial class ComedorEstudiantilContext : DbContext
{
    public ComedorEstudiantilContext(DbContextOptions<ComedorEstudiantilContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Actividad> Actividad { get; set; }

    public virtual DbSet<Bitacora> Bitacora { get; set; }

    public virtual DbSet<Entrega> Entrega { get; set; }

    public virtual DbSet<Estudiante> Estudiante { get; set; }

    public virtual DbSet<Gradoseccion> Gradoseccion { get; set; }

    public virtual DbSet<Menu> Menu { get; set; }

    public virtual DbSet<Repeticionentrega> Repeticionentrega { get; set; }

    public virtual DbSet<Rol> Rol { get; set; }

    public virtual DbSet<Solicitud> Solicitud { get; set; }

    public virtual DbSet<Tipobeneficiario> Tipobeneficiario { get; set; }

    public virtual DbSet<Tipocomida> Tipocomida { get; set; }

    public virtual DbSet<Usuario> Usuario { get; set; }

    public virtual DbSet<VwReporteentregadiaria> VwReporteentregadiaria { get; set; }

    public virtual DbSet<VwReportemarcadodiario> VwReportemarcadodiario { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_unicode_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<Actividad>(entity =>
        {
            entity.HasKey(e => e.IdActividad).HasName("PRIMARY");

            entity.ToTable("actividad");

            entity.Property(e => e.IdActividad).HasColumnType("int(11)");
            entity.Property(e => e.Activo)
                .IsRequired()
                .HasDefaultValueSql("'1'");
            entity.Property(e => e.Descripcion).HasMaxLength(255);
            entity.Property(e => e.Nombre).HasMaxLength(150);
        });

        modelBuilder.Entity<Bitacora>(entity =>
        {
            entity.HasKey(e => e.IdBitacora).HasName("PRIMARY");

            entity.ToTable("bitacora");

            entity.HasIndex(e => e.IdUsuario, "FK_Bitacora_Usuario");

            entity.Property(e => e.IdBitacora).HasColumnType("bigint(20)");
            entity.Property(e => e.Accion).HasMaxLength(100);
            entity.Property(e => e.Detalle).HasColumnType("text");
            entity.Property(e => e.Entidad).HasMaxLength(50);
            entity.Property(e => e.FechaHora)
                .HasDefaultValueSql("current_timestamp()")
                .HasColumnType("datetime");
            entity.Property(e => e.IdEntidad).HasColumnType("int(11)");
            entity.Property(e => e.IdUsuario).HasColumnType("int(11)");
            entity.Property(e => e.IpOrigen).HasMaxLength(45);

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.Bitacora)
                .HasForeignKey(d => d.IdUsuario)
                .HasConstraintName("FK_Bitacora_Usuario");
        });

        modelBuilder.Entity<Entrega>(entity =>
        {
            entity.HasKey(e => e.IdEntrega).HasName("PRIMARY");

            entity.ToTable("entrega");

            entity.HasIndex(e => e.IdUsuarioEntrego, "FK_Entrega_UsuarioEntrego");

            entity.HasIndex(e => e.FechaHoraEntrega, "IX_Entrega_Fecha");

            entity.HasIndex(e => e.IdSolicitud, "IdSolicitud").IsUnique();

            entity.Property(e => e.IdEntrega).HasColumnType("int(11)");
            entity.Property(e => e.FechaHoraEntrega)
                .HasDefaultValueSql("current_timestamp()")
                .HasColumnType("datetime");
            entity.Property(e => e.IdSolicitud).HasColumnType("int(11)");
            entity.Property(e => e.IdUsuarioEntrego).HasColumnType("int(11)");
            entity.Property(e => e.MetodoEntrega)
                .HasDefaultValueSql("'2'")
                .HasColumnType("tinyint(4)");

            entity.HasOne(d => d.IdSolicitudNavigation).WithOne(p => p.Entrega)
                .HasForeignKey<Entrega>(d => d.IdSolicitud)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Entrega_Solicitud");

            entity.HasOne(d => d.IdUsuarioEntregoNavigation).WithMany(p => p.Entrega)
                .HasForeignKey(d => d.IdUsuarioEntrego)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Entrega_UsuarioEntrego");
        });

        modelBuilder.Entity<Estudiante>(entity =>
        {
            entity.HasKey(e => e.IdEstudiante).HasName("PRIMARY");

            entity.ToTable("estudiante");

            entity.HasIndex(e => e.IdGradoSeccion, "FK_Estudiante_GradoSeccion");

            entity.HasIndex(e => e.IdTipoBeneficiario, "FK_Estudiante_TipoBeneficiario");

            entity.HasIndex(e => e.IdUsuario, "IdUsuario").IsUnique();

            entity.Property(e => e.IdEstudiante).HasColumnType("int(11)");
            entity.Property(e => e.Activo)
                .IsRequired()
                .HasDefaultValueSql("'1'");
            entity.Property(e => e.AnioIngreso).HasColumnType("year(4)");
            entity.Property(e => e.IdGradoSeccion).HasColumnType("int(11)");
            entity.Property(e => e.IdTipoBeneficiario).HasColumnType("int(11)");
            entity.Property(e => e.IdUsuario).HasColumnType("int(11)");

            entity.HasOne(d => d.IdGradoSeccionNavigation).WithMany(p => p.Estudiante)
                .HasForeignKey(d => d.IdGradoSeccion)
                .HasConstraintName("FK_Estudiante_GradoSeccion");

            entity.HasOne(d => d.IdTipoBeneficiarioNavigation).WithMany(p => p.Estudiante)
                .HasForeignKey(d => d.IdTipoBeneficiario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Estudiante_TipoBeneficiario");

            entity.HasOne(d => d.IdUsuarioNavigation).WithOne(p => p.Estudiante)
                .HasForeignKey<Estudiante>(d => d.IdUsuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Estudiante_Usuario");
        });

        modelBuilder.Entity<Gradoseccion>(entity =>
        {
            entity.HasKey(e => e.IdGradoSeccion).HasName("PRIMARY");

            entity.ToTable("gradoseccion");

            entity.HasIndex(e => new { e.Grado, e.Seccion }, "UQ_Grado_Seccion").IsUnique();

            entity.Property(e => e.IdGradoSeccion).HasColumnType("int(11)");
            entity.Property(e => e.Grado).HasMaxLength(20);
            entity.Property(e => e.Seccion).HasMaxLength(10);
        });

        modelBuilder.Entity<Menu>(entity =>
        {
            entity.HasKey(e => e.IdMenu).HasName("PRIMARY");

            entity.ToTable("menu");

            entity.HasIndex(e => e.IdActividad, "FK_Menu_Actividad");

            entity.HasIndex(e => e.IdTipoComida, "FK_Menu_TipoComida");

            entity.HasIndex(e => e.IdUsuarioCreador, "FK_Menu_UsuarioCreador");

            entity.HasIndex(e => e.Fecha, "IX_Menu_Fecha");

            entity.HasIndex(e => new { e.Fecha, e.IdTipoComida, e.IdActividad }, "UQ_Menu_Fecha_Tipo_Actividad").IsUnique();

            entity.Property(e => e.IdMenu).HasColumnType("int(11)");
            entity.Property(e => e.Descripcion).HasMaxLength(1000);
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("current_timestamp()")
                .HasColumnType("datetime");
            entity.Property(e => e.IdActividad).HasColumnType("int(11)");
            entity.Property(e => e.IdTipoComida).HasColumnType("int(11)");
            entity.Property(e => e.IdUsuarioCreador).HasColumnType("int(11)");

            entity.HasOne(d => d.IdActividadNavigation).WithMany(p => p.Menu)
                .HasForeignKey(d => d.IdActividad)
                .HasConstraintName("FK_Menu_Actividad");

            entity.HasOne(d => d.IdTipoComidaNavigation).WithMany(p => p.Menu)
                .HasForeignKey(d => d.IdTipoComida)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Menu_TipoComida");

            entity.HasOne(d => d.IdUsuarioCreadorNavigation).WithMany(p => p.Menu)
                .HasForeignKey(d => d.IdUsuarioCreador)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Menu_UsuarioCreador");
        });

        modelBuilder.Entity<Repeticionentrega>(entity =>
        {
            entity.HasKey(e => e.IdRepeticionEntrega).HasName("PRIMARY");

            entity.ToTable("repeticionentrega");

            entity.HasIndex(e => e.IdUsuarioRegistro, "FK_RepeticionEntrega_UsuarioRegistro");

            entity.HasIndex(e => e.FechaHoraRepeticion, "IX_RepeticionEntrega_FechaHora");

            entity.HasIndex(e => e.IdEntrega, "IX_RepeticionEntrega_IdEntrega");

            entity.Property(e => e.IdRepeticionEntrega).HasColumnType("int(11)");
            entity.Property(e => e.FechaHoraRepeticion)
                .HasDefaultValueSql("current_timestamp()")
                .HasColumnType("datetime");
            entity.Property(e => e.IdEntrega).HasColumnType("int(11)");
            entity.Property(e => e.IdUsuarioRegistro).HasColumnType("int(11)");
            entity.Property(e => e.MetodoRegistro)
                .HasDefaultValueSql("'2'")
                .HasColumnType("tinyint(4)");

            entity.HasOne(d => d.IdEntregaNavigation).WithMany(p => p.Repeticionentrega)
                .HasForeignKey(d => d.IdEntrega)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_RepeticionEntrega_Entrega");

            entity.HasOne(d => d.IdUsuarioRegistroNavigation).WithMany(p => p.Repeticionentrega)
                .HasForeignKey(d => d.IdUsuarioRegistro)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_RepeticionEntrega_UsuarioRegistro");
        });

        modelBuilder.Entity<Rol>(entity =>
        {
            entity.HasKey(e => e.IdRol).HasName("PRIMARY");

            entity.ToTable("rol");

            entity.HasIndex(e => e.Nombre, "Nombre").IsUnique();

            entity.Property(e => e.IdRol).HasColumnType("int(11)");
            entity.Property(e => e.Nombre).HasMaxLength(30);
        });

        modelBuilder.Entity<Solicitud>(entity =>
        {
            entity.HasKey(e => e.IdSolicitud).HasName("PRIMARY");

            entity.ToTable("solicitud");

            entity.HasIndex(e => e.IdMenu, "FK_Solicitud_Menu");

            entity.HasIndex(e => e.IdUsuarioMarco, "FK_Solicitud_UsuarioMarco");

            entity.HasIndex(e => e.FechaHoraSolicitud, "IX_Solicitud_Fecha");

            entity.HasIndex(e => new { e.IdUsuario, e.IdMenu }, "UQ_Solicitud_Usuario_Menu").IsUnique();

            entity.Property(e => e.IdSolicitud).HasColumnType("int(11)");
            entity.Property(e => e.Estado).HasColumnType("tinyint(4)");
            entity.Property(e => e.FechaHoraSolicitud)
                .HasDefaultValueSql("current_timestamp()")
                .HasColumnType("datetime");
            entity.Property(e => e.IdMenu).HasColumnType("int(11)");
            entity.Property(e => e.IdUsuario).HasColumnType("int(11)");
            entity.Property(e => e.IdUsuarioMarco).HasColumnType("int(11)");
            entity.Property(e => e.MetodoMarcado).HasColumnType("tinyint(4)");

            entity.HasOne(d => d.IdMenuNavigation).WithMany(p => p.Solicitud)
                .HasForeignKey(d => d.IdMenu)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Solicitud_Menu");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.SolicitudIdUsuarioNavigation)
                .HasForeignKey(d => d.IdUsuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Solicitud_Usuario");

            entity.HasOne(d => d.IdUsuarioMarcoNavigation).WithMany(p => p.SolicitudIdUsuarioMarcoNavigation)
                .HasForeignKey(d => d.IdUsuarioMarco)
                .HasConstraintName("FK_Solicitud_UsuarioMarco");
        });

        modelBuilder.Entity<Tipobeneficiario>(entity =>
        {
            entity.HasKey(e => e.IdTipoBeneficiario).HasName("PRIMARY");

            entity.ToTable("tipobeneficiario");

            entity.HasIndex(e => e.Nombre, "Nombre").IsUnique();

            entity.Property(e => e.IdTipoBeneficiario).HasColumnType("int(11)");
            entity.Property(e => e.Nombre).HasMaxLength(30);
        });

        modelBuilder.Entity<Tipocomida>(entity =>
        {
            entity.HasKey(e => e.IdTipoComida).HasName("PRIMARY");

            entity.ToTable("tipocomida");

            entity.HasIndex(e => e.Nombre, "Nombre").IsUnique();

            entity.Property(e => e.IdTipoComida).HasColumnType("int(11)");
            entity.Property(e => e.Activo)
                .IsRequired()
                .HasDefaultValueSql("'1'");
            entity.Property(e => e.HoraLimiteMarcar).HasColumnType("time");
            entity.Property(e => e.Nombre).HasMaxLength(30);
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.IdUsuario).HasName("PRIMARY");

            entity.ToTable("usuario");

            entity.HasIndex(e => e.Correo, "Correo").IsUnique();

            entity.HasIndex(e => e.IdRol, "FK_Usuario_Rol");

            entity.HasIndex(e => e.Identificacion, "Identificacion").IsUnique();

            entity.HasIndex(e => e.CodigoBarras, "UQ_Usuario_CodigoBarras").IsUnique();

            entity.Property(e => e.IdUsuario).HasColumnType("int(11)");
            entity.Property(e => e.Activo)
                .IsRequired()
                .HasDefaultValueSql("'1'");
            entity.Property(e => e.Apellidos).HasMaxLength(100);
            entity.Property(e => e.CodigoBarras).HasMaxLength(50);
            entity.Property(e => e.ContrasenaHash).HasMaxLength(255);
            entity.Property(e => e.Correo).HasMaxLength(150);
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("current_timestamp()")
                .HasColumnType("datetime");
            entity.Property(e => e.FechaUltimoCambioContrasena).HasColumnType("datetime");
            entity.Property(e => e.IdRol).HasColumnType("int(11)");
            entity.Property(e => e.Identificacion).HasMaxLength(20);
            entity.Property(e => e.Nombre).HasMaxLength(100);

            entity.HasOne(d => d.IdRolNavigation).WithMany(p => p.Usuario)
                .HasForeignKey(d => d.IdRol)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Usuario_Rol");
        });

        modelBuilder.Entity<VwReporteentregadiaria>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vw_reporteentregadiaria");

            entity.Property(e => e.Cedula).HasMaxLength(20);
            entity.Property(e => e.EntregadoPor)
                .HasMaxLength(201)
                .HasDefaultValueSql("''");
            entity.Property(e => e.FechaHoraEntrega)
                .HasDefaultValueSql("current_timestamp()")
                .HasColumnType("datetime");
            entity.Property(e => e.MetodoEntrega)
                .HasDefaultValueSql("'2'")
                .HasColumnType("tinyint(4)");
            entity.Property(e => e.NombreCompleto)
                .HasMaxLength(201)
                .HasDefaultValueSql("''");
            entity.Property(e => e.Rol).HasMaxLength(30);
            entity.Property(e => e.TipoBeneficiario).HasMaxLength(30);
            entity.Property(e => e.TipoComida).HasMaxLength(30);
        });

        modelBuilder.Entity<VwReportemarcadodiario>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vw_reportemarcadodiario");

            entity.Property(e => e.Cedula).HasMaxLength(20);
            entity.Property(e => e.Estado).HasColumnType("tinyint(4)");
            entity.Property(e => e.FechaHoraSolicitud)
                .HasDefaultValueSql("current_timestamp()")
                .HasColumnType("datetime");
            entity.Property(e => e.MetodoMarcado).HasColumnType("tinyint(4)");
            entity.Property(e => e.NombreCompleto)
                .HasMaxLength(201)
                .HasDefaultValueSql("''");
            entity.Property(e => e.Rol).HasMaxLength(30);
            entity.Property(e => e.TipoBeneficiario).HasMaxLength(30);
            entity.Property(e => e.TipoComida).HasMaxLength(30);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
