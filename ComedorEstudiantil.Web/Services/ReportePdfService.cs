using ComedorEstudiantil.Application.DTOs;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace ComedorEstudiantil.Web.Services
{
    public class ReportePdfService : IReportePdfService
    {
        public byte[] GenerarSolicitudes(
            ReporteGeneralDTO reporte)
        {
            return Document.Create(document =>
            {
                document.Page(page =>
                {
                    ConfigurarPagina(page);

                    page.Header().Element(container =>
                        CrearEncabezado(
                            container,
                            "Reporte de solicitudes",
                            reporte));

                    page.Content().Column(column =>
                    {
                        column.Spacing(10);

                        column.Item().Element(container =>
                            CrearResumen(container, reporte));

                        column.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.ConstantColumn(55);
                                columns.ConstantColumn(65);
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(2);
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                            });

                            table.Header(header =>
                            {
                                EncabezadoCelda(header.Cell(), "Fecha");
                                EncabezadoCelda(header.Cell(), "Tipo");
                                EncabezadoCelda(header.Cell(), "Identificación");
                                EncabezadoCelda(header.Cell(), "Usuario");
                                EncabezadoCelda(header.Cell(), "Rol");
                                EncabezadoCelda(header.Cell(), "Beneficio");
                                EncabezadoCelda(header.Cell(), "Estado");
                                EncabezadoCelda(header.Cell(), "Entrega");
                            });

                            foreach (ReporteSolicitudDTO solicitud
                                in reporte.Solicitudes)
                            {
                                Celda(table.Cell(),
                                    solicitud.FechaMenu
                                        .ToString("dd/MM/yyyy"));

                                Celda(table.Cell(),
                                    solicitud.TipoComida);

                                Celda(table.Cell(),
                                    solicitud.Identificacion);

                                Celda(table.Cell(),
                                    solicitud.NombreUsuario);

                                Celda(table.Cell(),
                                    solicitud.Rol);

                                Celda(table.Cell(),
                                    solicitud.TipoBeneficiario);

                                Celda(table.Cell(),
                                    solicitud.Estado);

                                Celda(table.Cell(),
                                    solicitud.Entregada
                                        ? "Entregada"
                                        : "Pendiente");
                            }
                        });
                    });

                    page.Footer().Element(CrearPiePagina);
                });
            }).GeneratePdf();
        }

        public byte[] GenerarEntregas(
            ReporteGeneralDTO reporte)
        {
            return Document.Create(document =>
            {
                document.Page(page =>
                {
                    ConfigurarPagina(page);

                    page.Header().Element(container =>
                        CrearEncabezado(
                            container,
                            "Reporte de entregas",
                            reporte));

                    page.Content().Column(column =>
                    {
                        column.Spacing(10);

                        column.Item().Text(
                            $"Total de entregas: {reporte.Entregas.Count}")
                            .SemiBold();

                        column.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.ConstantColumn(55);
                                columns.ConstantColumn(65);
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(2);
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                                columns.RelativeColumn(2);
                            });

                            table.Header(header =>
                            {
                                EncabezadoCelda(header.Cell(), "Fecha");
                                EncabezadoCelda(header.Cell(), "Tipo");
                                EncabezadoCelda(header.Cell(), "Identificación");
                                EncabezadoCelda(header.Cell(), "Usuario");
                                EncabezadoCelda(header.Cell(), "Rol");
                                EncabezadoCelda(header.Cell(), "Hora");
                                EncabezadoCelda(header.Cell(), "Registrado por");
                            });

                            foreach (ReporteEntregaDTO entrega
                                in reporte.Entregas)
                            {
                                Celda(table.Cell(),
                                    entrega.FechaMenu
                                        .ToString("dd/MM/yyyy"));

                                Celda(table.Cell(),
                                    entrega.TipoComida);

                                Celda(table.Cell(),
                                    entrega.Identificacion);

                                Celda(table.Cell(),
                                    entrega.NombreUsuario);

                                Celda(table.Cell(),
                                    entrega.Rol);

                                Celda(table.Cell(),
                                    entrega.FechaHoraEntrega
                                        .ToString("HH:mm"));

                                Celda(table.Cell(),
                                    entrega.EntregadoPor);
                            }
                        });
                    });

                    page.Footer().Element(CrearPiePagina);
                });
            }).GeneratePdf();
        }

        private static void ConfigurarPagina(
            PageDescriptor page)
        {
            page.Size(PageSizes.A4.Landscape());
            page.Margin(25);
            page.DefaultTextStyle(style =>
                style.FontSize(8));
        }

        private static void CrearEncabezado(
            IContainer container,
            string titulo,
            ReporteGeneralDTO reporte)
        {
            container
                .PaddingBottom(10)
                .BorderBottom(1)
                .BorderColor(Colors.Grey.Medium)
                .Column(column =>
                {
                    column.Item()
                        .Text("Comedor Estudiantil")
                        .FontSize(16)
                        .Bold();

                    column.Item()
                        .Text(titulo)
                        .FontSize(13)
                        .SemiBold();

                    column.Item().Text(
                        $"Periodo: {reporte.Filtro.FechaInicio:dd/MM/yyyy} al {reporte.Filtro.FechaFin:dd/MM/yyyy}");

                    if (!string.IsNullOrWhiteSpace(
                        reporte.Filtro.TipoComida))
                    {
                        column.Item().Text(
                            $"Tipo de comida: {reporte.Filtro.TipoComida}");
                    }

                    column.Item().Text(
                        $"Generado: {DateTime.Now:dd/MM/yyyy HH:mm}");
                });
        }

        private static void CrearResumen(
            IContainer container,
            ReporteGeneralDTO reporte)
        {
            container.Row(row =>
            {
                row.RelativeItem().Text(
                    $"Solicitudes: {reporte.TotalSolicitudes}");

                row.RelativeItem().Text(
                    $"Activas: {reporte.TotalActivas}");

                row.RelativeItem().Text(
                    $"Canceladas: {reporte.TotalCanceladas}");

                row.RelativeItem().Text(
                    $"Entregadas: {reporte.TotalEntregadas}");

                row.RelativeItem().Text(
                    $"Pendientes: {reporte.TotalPendientes}");
            });
        }

        private static void EncabezadoCelda(
            IContainer container,
            string texto)
        {
            container
                .Background(Colors.Blue.Darken2)
                .Border(0.5f)
                .BorderColor(Colors.Grey.Medium)
                .Padding(4)
                .Text(texto)
                .FontColor(Colors.White)
                .SemiBold();
        }

        private static void Celda(
            IContainer container,
            string texto)
        {
            container
                .BorderBottom(0.5f)
                .BorderColor(Colors.Grey.Lighten2)
                .Padding(4)
                .Text(texto);
        }

        private static void CrearPiePagina(
            IContainer container)
        {
            container
                .AlignCenter()
                .Text(text =>
                {
                    text.Span("Página ");
                    text.CurrentPageNumber();
                    text.Span(" de ");
                    text.TotalPages();
                });
        }
    }
}