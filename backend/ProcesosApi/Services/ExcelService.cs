using ClosedXML.Excel;
using MongoDB.Driver;
using ProcesosApi.Data;
using ProcesosApi.Models;

namespace ProcesosApi.Services;

public class ExcelService : IExcelService
{
    private readonly MongoDbContext _db;

    public ExcelService(MongoDbContext db) => _db = db;

    public async Task<(int importados, List<string> errores)> ImportarAsync(Stream fileStream)
    {
        var errores = new List<string>();
        var importados = 0;

        using var workbook = new XLWorkbook(fileStream);
        var sheet = workbook.Worksheets.First();
        var lastRow = sheet.LastRowUsed()?.RowNumber() ?? 0;

        for (int row = 2; row <= lastRow; row++)
        {
            try
            {
                var numCell = sheet.Cell(row, 1).GetString().Trim();
                if (string.IsNullOrWhiteSpace(numCell)) continue;

                DateTime fecha;
                var fechaCell = sheet.Cell(row, 2);
                if (fechaCell.DataType == XLDataType.Number)
                    fecha = DateTime.FromOADate(fechaCell.GetDouble());
                else if (!DateTime.TryParse(fechaCell.GetString(), out fecha))
                    fecha = DateTime.UtcNow;

                var demandante = sheet.Cell(row, 3).GetString().Trim();
                var demandado = sheet.Cell(row, 4).GetString().Trim();

                if (string.IsNullOrWhiteSpace(demandante) && string.IsNullOrWhiteSpace(demandado))
                    continue;

                var radicadoCell = sheet.Cell(row, 5);
                string radicado;
                if (radicadoCell.DataType == XLDataType.Number)
                    radicado = radicadoCell.GetDouble().ToString("F0");
                else
                    radicado = radicadoCell.GetString().Trim();

                var juzgado = sheet.Cell(row, 6).GetString().Trim();
                var ciudad = sheet.Cell(row, 7).GetString().Trim();
                var claseProceso = sheet.Cell(row, 8).GetString().Trim();
                var representamos = sheet.Cell(row, 9).GetString().Trim();
                var ingresadoPor = sheet.Cell(row, 10).GetString().Trim();

                var honorariosCell = sheet.Cell(row, 11);
                string? honorarios = honorariosCell.DataType == XLDataType.Number
                    ? honorariosCell.GetDouble().ToString("N0")
                    : honorariosCell.GetString().Trim();

                // Map porcentajes to Comisiones array
                var comisiones = new List<Comision>();

                var porcPaolaCell = sheet.Cell(row, 12);
                string? porcPaola = porcPaolaCell.DataType == XLDataType.Number
                    ? (porcPaolaCell.GetDouble() * 100).ToString("0.##") + "%"
                    : porcPaolaCell.GetString().Trim();
                if (!string.IsNullOrWhiteSpace(porcPaola))
                    comisiones.Add(new Comision { Persona = "Paola", Porcentaje = porcPaola });

                var porcMIsabelCell = sheet.Cell(row, 13);
                string? porcMIsabel = porcMIsabelCell.DataType == XLDataType.Number
                    ? (porcMIsabelCell.GetDouble() * 100).ToString("0.##") + "%"
                    : porcMIsabelCell.GetString().Trim();
                if (!string.IsNullOrWhiteSpace(porcMIsabel))
                    comisiones.Add(new Comision { Persona = "M. Isabel", Porcentaje = porcMIsabel });

                var pagosRealizados = sheet.Cell(row, 14).GetString().Trim();

                var estadoParts = new List<string>();
                for (int col = 15; col <= 22; col++)
                {
                    var val = sheet.Cell(row, col).GetString().Trim();
                    if (!string.IsNullOrWhiteSpace(val))
                        estadoParts.Add(val);
                }
                var estadoActual = string.Join(" | ", estadoParts);

                var proceso = new Proceso
                {
                    Fecha = fecha,
                    Demandante = demandante,
                    Demandado = demandado,
                    Radicado = radicado,
                    Juzgado = juzgado,
                    Ciudad = ciudad,
                    ClaseProceso = claseProceso,
                    Representamos = string.IsNullOrWhiteSpace(representamos) ? null : representamos,
                    ProcesoIngresadoPor = string.IsNullOrWhiteSpace(ingresadoPor) ? null : ingresadoPor,
                    Honorarios = string.IsNullOrWhiteSpace(honorarios) ? null : honorarios,
                    Comisiones = comisiones,
                    EstadoActual = string.IsNullOrWhiteSpace(estadoActual) ? null : estadoActual,
                    FechaCreacion = DateTime.UtcNow,
                    FechaActualizacion = DateTime.UtcNow
                };

                // Store pagos realizados as initial note if present
                if (!string.IsNullOrWhiteSpace(pagosRealizados))
                {
                    proceso.Notas.Add(new Nota
                    {
                        Contenido = $"[Importado del Excel] Pagos realizados: {pagosRealizados}",
                        FechaCreacion = DateTime.UtcNow,
                        FechaActualizacion = DateTime.UtcNow
                    });
                }

                await _db.Procesos.InsertOneAsync(proceso);
                importados++;
            }
            catch (Exception ex)
            {
                errores.Add($"Fila {row}: {ex.Message}");
            }
        }

        return (importados, errores);
    }

    public async Task<byte[]> ExportarAsync(List<string>? ids)
    {
        var filter = Builders<Proceso>.Filter.Eq(p => p.Activo, true);

        if (ids != null && ids.Count > 0)
            filter &= Builders<Proceso>.Filter.In(p => p.Id, ids);

        var procesos = await _db.Procesos
            .Find(filter)
            .SortBy(p => p.Fecha)
            .ToListAsync();

        using var workbook = new XLWorkbook();

        var ws = workbook.AddWorksheet("Procesos");
        var headers = new[] { "#", "Fecha", "Demandante", "Demandado", "Radicado", "Juzgado", "Ciudad",
            "Clase de Proceso", "Representamos", "Ingresado Por", "Honorarios",
            "Comisiones", "Estado Actual" };

        for (int i = 0; i < headers.Length; i++)
        {
            ws.Cell(1, i + 1).Value = headers[i];
            ws.Cell(1, i + 1).Style.Font.Bold = true;
            ws.Cell(1, i + 1).Style.Fill.BackgroundColor = XLColor.LightGray;
        }

        for (int i = 0; i < procesos.Count; i++)
        {
            var p = procesos[i];
            var excelRow = i + 2;
            ws.Cell(excelRow, 1).Value = i + 1;
            ws.Cell(excelRow, 2).Value = p.Fecha;
            ws.Cell(excelRow, 2).Style.NumberFormat.Format = "yyyy-MM-dd";
            ws.Cell(excelRow, 3).Value = p.Demandante;
            ws.Cell(excelRow, 4).Value = p.Demandado;
            ws.Cell(excelRow, 5).SetValue(p.Radicado);
            ws.Cell(excelRow, 5).Style.NumberFormat.Format = "@";
            ws.Cell(excelRow, 6).Value = p.Juzgado;
            ws.Cell(excelRow, 7).Value = p.Ciudad;
            ws.Cell(excelRow, 8).Value = p.ClaseProceso;
            ws.Cell(excelRow, 9).Value = p.Representamos ?? "";
            ws.Cell(excelRow, 10).Value = p.ProcesoIngresadoPor ?? "";
            ws.Cell(excelRow, 11).Value = p.Honorarios ?? "";
            ws.Cell(excelRow, 12).Value = string.Join(", ", p.Comisiones.Select(c => $"{c.Persona}: {c.Porcentaje}"));
            ws.Cell(excelRow, 13).Value = p.EstadoActual ?? "";
        }

        ws.Columns().AdjustToContents();

        var wsPagos = workbook.AddWorksheet("Pagos");
        var pagosHeaders = new[] { "Proceso (Demandante)", "Radicado", "Fecha", "Monto", "Concepto", "Metodo de Pago" };
        for (int i = 0; i < pagosHeaders.Length; i++)
        {
            wsPagos.Cell(1, i + 1).Value = pagosHeaders[i];
            wsPagos.Cell(1, i + 1).Style.Font.Bold = true;
            wsPagos.Cell(1, i + 1).Style.Fill.BackgroundColor = XLColor.LightGray;
        }

        var pagoRow = 2;
        foreach (var p in procesos)
        {
            foreach (var pago in p.Pagos.OrderBy(x => x.Fecha))
            {
                wsPagos.Cell(pagoRow, 1).Value = p.Demandante;
                wsPagos.Cell(pagoRow, 2).SetValue(p.Radicado);
                wsPagos.Cell(pagoRow, 3).Value = pago.Fecha;
                wsPagos.Cell(pagoRow, 3).Style.NumberFormat.Format = "yyyy-MM-dd";
                wsPagos.Cell(pagoRow, 4).Value = pago.Monto;
                wsPagos.Cell(pagoRow, 4).Style.NumberFormat.Format = "$#,##0";
                wsPagos.Cell(pagoRow, 5).Value = pago.Concepto ?? "";
                wsPagos.Cell(pagoRow, 6).Value = pago.MetodoPago ?? "";
                pagoRow++;
            }
        }
        wsPagos.Columns().AdjustToContents();

        using var ms = new MemoryStream();
        workbook.SaveAs(ms);
        return ms.ToArray();
    }
}
