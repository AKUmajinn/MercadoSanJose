using Microsoft.AspNetCore.Mvc;
using MercadoSanJose.Web.Data;
using ClosedXML.Excel;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace MercadoSanJose.Web.Controllers
{
    public class ReportesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReportesController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        // ==========================================
        // 1. EXCEL: Reporte de Morosidad (ClosedXML)
        // ==========================================
        [HttpGet]
        public IActionResult DescargarMorosidadExcel()
        {
            try
            {
                var reporte = (from d in _context.Deudas
                               join p in _context.Puestos on d.PuestoId equals p.Id
                               join r in _context.Personas on d.ResponsableId equals r.Id
                               where d.Estado == 0 // Solo las pendientes
                               group d by new { p.NumeroPuesto, r.Nombre, r.DNI } into grupo
                               select new
                               {
                                   Puesto = grupo.Key.NumeroPuesto,
                                   Responsable = grupo.Key.Nombre,
                                   DNI = grupo.Key.DNI,
                                   // En lugar de mostrar el concepto, mostramos cuántos recibos debe
                                   Concepto = grupo.Count() + " concepto(s) pendiente(s)",
                                   // Tomamos la fecha más antigua de sus deudas
                                   Fecha = grupo.Min(x => x.FechaEmision),
                                   // SUMAMOS el total de lo que debe esa persona en ese puesto
                                   Monto = grupo.Sum(x => x.MontoTotal)
                               }).ToList();

                using var workbook = new XLWorkbook();
                var worksheet = workbook.Worksheets.Add("Morosidad");

                // Diseño de Cabeceras
                var cabeceras = new[] { "N° Puesto", "Responsable", "DNI", "Concepto", "Fecha Emisión", "Deuda (S/.)" };
                for (int i = 0; i < cabeceras.Length; i++)
                {
                    var cell = worksheet.Cell(1, i + 1);
                    cell.Value = cabeceras[i];
                    cell.Style.Font.Bold = true;
                    cell.Style.Font.FontColor = XLColor.White;
                    cell.Style.Fill.BackgroundColor = XLColor.DarkSlateGray; // Color elegante
                    cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                }

                // Llenado de datos con validación de nulos
                for (int i = 0; i < reporte.Count; i++)
                {
                    var row = i + 2;
                    worksheet.Cell(row, 1).Value = reporte[i].Puesto;
                    worksheet.Cell(row, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                    worksheet.Cell(row, 2).Value = reporte[i].Responsable ?? "Sin Nombre";
                    worksheet.Cell(row, 3).Value = reporte[i].DNI ?? "Sin DNI";
                    worksheet.Cell(row, 4).Value = reporte[i].Concepto ?? "Sin Concepto";
                    worksheet.Cell(row, 5).Value = reporte[i].Fecha.ToString("dd/MM/yyyy");

                    worksheet.Cell(row, 6).Value = reporte[i].Monto;
                    worksheet.Cell(row, 6).Style.NumberFormat.Format = "\"S/.\" #,##0.00"; // Formato moneda Excel
                }

                // Ajustes finales de tabla (Filtros, bordes y autoajuste)
                if (reporte.Count > 0)
                {
                    var rangoDatos = worksheet.Range(1, 1, reporte.Count + 1, 6);
                    rangoDatos.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    rangoDatos.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                    rangoDatos.SetAutoFilter();
                }
                worksheet.Columns().AdjustToContents();

                using var stream = new MemoryStream();
                workbook.SaveAs(stream);
                return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Reporte_Morosidad.xlsx");
            }
            catch (System.Exception ex)
            {
                // El paracaídas: Muestra el error exacto en pantalla sin botar el servidor
                return Content($"ERROR CRÍTICO AL GENERAR EXCEL: {ex.Message} \n\nDetalles: {ex.InnerException?.Message}");
            }
        }

        // ==========================================
        // 2. PDF: Recibo de Pago (QuestPDF)
        // ==========================================
        [HttpGet]
        public IActionResult GenerarReciboPdf(int deudaId)
        {
            try
            {
                var reciboData = (from d in _context.Deudas
                                  join p in _context.Puestos on d.PuestoId equals p.Id
                                  join r in _context.Personas on d.ResponsableId equals r.Id
                                  join c in _context.ConceptosDeuda on d.ConceptoDeudaId equals c.Id
                                  where d.Id == deudaId
                                  select new
                                  {
                                      ReciboNro = d.Id.ToString("D6"),
                                      Fecha = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                      Puesto = p.NumeroPuesto,
                                      Responsable = r.Nombre,
                                      Concepto = c.Nombre,
                                      Monto = d.MontoTotal
                                  }).FirstOrDefault();

                if (reciboData == null) return NotFound("No se encontró la deuda.");

                var document = Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        page.Size(PageSizes.A5);
                        page.Margin(1.5f, Unit.Centimetre);
                        page.PageColor(Colors.White);
                        page.DefaultTextStyle(x => x.FontSize(10).FontFamily(Fonts.Arial));

                        page.Content().Column(col =>
                        {
                            col.Spacing(15);

                            // Nombre de la institución
                            col.Item().Text("MERCADO SAN JOSÉ").Bold().FontSize(18).FontColor(Colors.Blue.Darken3);

                            // Caja de Encabezado (Inspirado en tu código de GitHub)
                            col.Item().Background(Colors.Blue.Darken3).Padding(10).Row(row =>
                            {
                                row.RelativeItem().Text("COMPROBANTE DE PAGO").FontColor(Colors.White).FontSize(14).SemiBold();
                                row.RelativeItem().AlignRight().Text($"Nro: REC-{reciboData.ReciboNro}").FontColor(Colors.White).FontSize(12);
                            });

                            // Tabla de detalles con líneas divisorias finas
                            col.Item().Table(tabla =>
                            {
                                tabla.ColumnsDefinition(columns =>
                                {
                                    columns.ConstantColumn(90);
                                    columns.RelativeColumn();
                                });

                                // Función interna para reciclar el estilo de las celdas
                                static IContainer CellStyle(IContainer c) => c.BorderBottom(1).BorderColor(Colors.Grey.Lighten3).PaddingVertical(5);

                                tabla.Cell().Element(CellStyle).Text("Fecha/Hora:").SemiBold();
                                tabla.Cell().Element(CellStyle).Text(reciboData.Fecha);

                                tabla.Cell().Element(CellStyle).Text("Responsable:").SemiBold();
                                tabla.Cell().Element(CellStyle).Text(reciboData.Responsable);

                                tabla.Cell().Element(CellStyle).Text("Nro Puesto:").SemiBold();
                                tabla.Cell().Element(CellStyle).Text(reciboData.Puesto.ToString());

                                tabla.Cell().Element(CellStyle).Text("Concepto:").SemiBold();
                                tabla.Cell().Element(CellStyle).Text(reciboData.Concepto);
                            });

                            // Caja de Total
                            col.Item().Background(Colors.Grey.Lighten4).Padding(10).AlignRight()
                               .Text($"TOTAL PAGADO: S/. {reciboData.Monto:F2}").FontSize(14).Bold().FontColor(Colors.Black);
                        });

                        // Pie de página estandarizado
                        page.Footer().AlignCenter().Text(t =>
                        {
                            t.Span("Este documento es un comprobante de uso interno. Generado el: ");
                            t.Span($"{DateTime.Now:dd/MM/yyyy}").SemiBold();
                        });
                    });
                });

                byte[] pdfBytes = document.GeneratePdf();
                return File(pdfBytes, "application/pdf", $"Recibo_{reciboData.ReciboNro}.pdf");
            }
            catch (System.Exception ex)
            {
                return Content($"ERROR CRÍTICO AL GENERAR PDF: {ex.Message}");
            }
        }
    }
}