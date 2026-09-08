using ClosedXML.Excel;
using ComedorEstudiantil.Application.DTOs;

namespace ComedorEstudiantil.Web.Services
{
    public class ReporteExcelService :
        IReporteExcelService
    {
        public byte[] GenerarSolicitudes(
            ReporteGeneralDTO reporte)
        {
            using var libro = new XLWorkbook();

            IXLWorksheet hoja =
                libro.Worksheets.Add("Solicitudes");

            CrearEncabezado(
                hoja,
                "Reporte de solicitudes",
                reporte,
                13);

            hoja.Cell(3, 1).Value =
                $"Solicitudes: {reporte.TotalSolicitudes} | " +
                $"Activas: {reporte.TotalActivas} | " +
                $"Canceladas: {reporte.TotalCanceladas} | " +
                $"Entregadas: {reporte.TotalEntregadas} | " +
                $"Pendientes: {reporte.TotalPendientes}";

            string[] encabezados =
            {
                "Fecha",
                "Tipo de comida",
                "Menú",
                "Identificación",
                "Usuario",
                "Rol",
                "Beneficio",
                "Grupo",
                "Fecha y hora de solicitud",
                "Estado",
                "Método de marcado",
                "Marcado por",
                "Entrega"
            };

            CrearEncabezadosTabla(
                hoja,
                encabezados);

            int fila = 6;

            foreach (ReporteSolicitudDTO solicitud
                in reporte.Solicitudes)
            {
                hoja.Cell(fila, 1).Value =
                    solicitud.FechaMenu.ToDateTime(
                        TimeOnly.MinValue);

                hoja.Cell(fila, 2).Value =
                    solicitud.TipoComida;

                hoja.Cell(fila, 3).Value =
                    solicitud.DescripcionMenu;

                hoja.Cell(fila, 4).Value =
                    solicitud.Identificacion;

                hoja.Cell(fila, 5).Value =
                    solicitud.NombreUsuario;

                hoja.Cell(fila, 6).Value =
                    solicitud.Rol;

                hoja.Cell(fila, 7).Value =
                    solicitud.TipoBeneficiario;

                hoja.Cell(fila, 8).Value =
                    solicitud.GradoSeccion;

                hoja.Cell(fila, 9).Value =
                    solicitud.FechaHoraSolicitud;

                hoja.Cell(fila, 10).Value =
                    solicitud.Estado;

                hoja.Cell(fila, 11).Value =
                    solicitud.MetodoMarcado;

                hoja.Cell(fila, 12).Value =
                    solicitud.MarcadoPor;

                hoja.Cell(fila, 13).Value =
                    solicitud.Entregada
                        ? "Entregada"
                        : "Pendiente";

                fila++;
            }

            AplicarFormatoTabla(
                hoja,
                fila - 1,
                encabezados.Length);

            hoja.Column(1).Style.DateFormat.Format =
                "dd/MM/yyyy";

            hoja.Column(9).Style.DateFormat.Format =
                "dd/MM/yyyy HH:mm";

            return GuardarLibro(libro);
        }
        public byte[] GenerarEntregas(
    ReporteGeneralDTO reporte)
        {
            using var libro = new XLWorkbook();

            IXLWorksheet hoja =
                libro.Worksheets.Add("Entregas");

            CrearEncabezado(
                hoja,
                "Reporte de entregas",
                reporte,
                14);

            hoja.Cell(3, 1).Value =
                $"Entregas iniciales: {reporte.TotalEntregasIniciales} | " +
                $"Repeticiones: {reporte.TotalRepeticiones} | " +
                $"Total de platos servidos: {reporte.TotalPlatosServidos}";

            string[] encabezados =
            {
        "Fecha",
        "Tipo de comida",
        "Menú",
        "Identificación",
        "Usuario",
        "Rol",
        "Beneficio",
        "Grupo",
        "Fecha y hora de entrega",
        "Método de entrega",
        "Registrado por",
        "Entrega inicial",
        "Repeticiones",
        "Platos consumidos"
    };

            CrearEncabezadosTabla(
                hoja,
                encabezados);

            int fila = 6;

            foreach (ReporteEntregaDTO entrega
                in reporte.Entregas)
            {
                hoja.Cell(fila, 1).Value =
                    entrega.FechaMenu.ToDateTime(
                        TimeOnly.MinValue);

                hoja.Cell(fila, 2).Value =
                    entrega.TipoComida;

                hoja.Cell(fila, 3).Value =
                    entrega.DescripcionMenu;

                hoja.Cell(fila, 4).Value =
                    entrega.Identificacion;

                hoja.Cell(fila, 5).Value =
                    entrega.NombreUsuario;

                hoja.Cell(fila, 6).Value =
                    entrega.Rol;

                hoja.Cell(fila, 7).Value =
                    entrega.TipoBeneficiario;

                hoja.Cell(fila, 8).Value =
                    entrega.GradoSeccion;

                hoja.Cell(fila, 9).Value =
                    entrega.FechaHoraEntrega;

                hoja.Cell(fila, 10).Value =
                    entrega.MetodoEntrega;

                hoja.Cell(fila, 11).Value =
                    entrega.EntregadoPor;

                hoja.Cell(fila, 12).Value = 1;

                hoja.Cell(fila, 13).Value =
                    entrega.CantidadRepeticiones;

                hoja.Cell(fila, 14).Value =
                    entrega.CantidadPlatosConsumidos;

                fila++;
            }

            AplicarFormatoTabla(
                hoja,
                fila - 1,
                encabezados.Length);

            hoja.Column(1).Style.DateFormat.Format =
                "dd/MM/yyyy";

            hoja.Column(9).Style.DateFormat.Format =
                "dd/MM/yyyy HH:mm";

            return GuardarLibro(libro);
        }

        private static void CrearEncabezado(
            IXLWorksheet hoja,
            string titulo,
            ReporteGeneralDTO reporte,
            int cantidadColumnas)
        {
            IXLRange tituloRango =
                hoja.Range(
                    1,
                    1,
                    1,
                    cantidadColumnas);

            tituloRango.Merge();
            tituloRango.Value =
                $"Comedor Estudiantil - {titulo}";

            tituloRango.Style.Font.Bold = true;
            tituloRango.Style.Font.FontSize = 16;
            tituloRango.Style.Font.FontColor =
                XLColor.White;

            tituloRango.Style.Fill.BackgroundColor =
                XLColor.FromHtml("#0D6EFD");

            tituloRango.Style.Alignment.Horizontal =
                XLAlignmentHorizontalValues.Center;

            tituloRango.Style.Alignment.Vertical =
                XLAlignmentVerticalValues.Center;

            hoja.Row(1).Height = 28;

            IXLRange periodoRango =
                hoja.Range(
                    2,
                    1,
                    2,
                    cantidadColumnas);

            periodoRango.Merge();

            string filtroTipo =
                string.IsNullOrWhiteSpace(
                    reporte.Filtro.TipoComida)
                    ? "Todos"
                    : reporte.Filtro.TipoComida;

            string filtroEstado =
                reporte.Filtro.Estado switch
                {
                    0 => "Activa",
                    1 => "Cancelada",
                    _ => "Todos"
                };

            periodoRango.Value =
                $"Periodo: {reporte.Filtro.FechaInicio:dd/MM/yyyy} al " +
                $"{reporte.Filtro.FechaFin:dd/MM/yyyy} | " +
                $"Tipo de comida: {filtroTipo} | " +
                $"Estado: {filtroEstado}";

            periodoRango.Style.Font.Bold = true;
            periodoRango.Style.Alignment.Horizontal =
                XLAlignmentHorizontalValues.Center;

            IXLRange resumenRango =
                hoja.Range(
                    3,
                    1,
                    3,
                    cantidadColumnas);

            resumenRango.Merge();
            resumenRango.Style.Font.Bold = true;

            resumenRango.Style.Fill.BackgroundColor =
                XLColor.FromHtml("#E9ECEF");

            resumenRango.Style.Alignment.Horizontal =
                XLAlignmentHorizontalValues.Center;

            IXLRange generadoRango =
                hoja.Range(
                    4,
                    1,
                    4,
                    cantidadColumnas);

            generadoRango.Merge();
            generadoRango.Value =
                $"Generado: {DateTime.Now:dd/MM/yyyy HH:mm}";

            generadoRango.Style.Alignment.Horizontal =
                XLAlignmentHorizontalValues.Right;

            generadoRango.Style.Font.Italic = true;
        }

        private static void CrearEncabezadosTabla(
            IXLWorksheet hoja,
            IReadOnlyList<string> encabezados)
        {
            for (int columna = 0;
                 columna < encabezados.Count;
                 columna++)
            {
                hoja.Cell(5, columna + 1).Value =
                    encabezados[columna];
            }

            IXLRange rango =
                hoja.Range(
                    5,
                    1,
                    5,
                    encabezados.Count);

            rango.Style.Font.Bold = true;
            rango.Style.Font.FontColor =
                XLColor.White;

            rango.Style.Fill.BackgroundColor =
                XLColor.FromHtml("#0D6EFD");

            rango.Style.Alignment.Horizontal =
                XLAlignmentHorizontalValues.Center;

            rango.Style.Alignment.Vertical =
                XLAlignmentVerticalValues.Center;

            rango.Style.Border.OutsideBorder =
                XLBorderStyleValues.Thin;

            rango.Style.Border.InsideBorder =
                XLBorderStyleValues.Thin;

            hoja.Row(5).Height = 24;
        }

        private static void AplicarFormatoTabla(
            IXLWorksheet hoja,
            int ultimaFila,
            int ultimaColumna)
        {
            int filaFinal =
                Math.Max(ultimaFila, 5);

            IXLRange tabla =
                hoja.Range(
                    5,
                    1,
                    filaFinal,
                    ultimaColumna);

            tabla.Style.Border.OutsideBorder =
                XLBorderStyleValues.Thin;

            tabla.Style.Border.InsideBorder =
                XLBorderStyleValues.Thin;

            tabla.Style.Border.OutsideBorderColor =
                XLColor.LightGray;

            tabla.Style.Border.InsideBorderColor =
                XLColor.LightGray;

            tabla.Style.Alignment.Vertical =
                XLAlignmentVerticalValues.Center;

            tabla.Style.Alignment.WrapText = true;
            tabla.SetAutoFilter();

            hoja.SheetView.FreezeRows(5);
            hoja.ColumnsUsed().AdjustToContents();

            foreach (IXLColumn columna
                in hoja.ColumnsUsed())
            {
                if (columna.Width > 45)
                {
                    columna.Width = 45;
                }

                if (columna.Width < 12)
                {
                    columna.Width = 12;
                }
            }
        }

        private static byte[] GuardarLibro(
            XLWorkbook libro)
        {
            using var flujo = new MemoryStream();

            libro.SaveAs(flujo);

            return flujo.ToArray();
        }
    }
}