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
            // Join explícito para no depender de las propiedades virtuales incompletas de tus compañeros
            var reporte = (from d in _context.Deudas
                           join p in _context.Puestos on d.PuestoId equals p.Id
                           join r in _context.Personas on d.ResponsableId equals r.Id
                           join c in _context.ConceptosDeuda on d.ConceptoDeudaId equals c.Id
                           where d.Estado == 0 // 0 = Pendiente
                           select new
                           {
                               Puesto = p.NumeroPuesto,
                               Responsable = r.Nombre,
                               DNI = r.DNI,
                               Concepto = c.Nombre,
                               Fecha = d.FechaEmision,
                               Monto = d.MontoTotal
                           }).ToList();

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Morosidad");

            // Cabeceras
            worksheet.Cell(1, 1).Value = "N° Puesto";
            worksheet.Cell(1, 2).Value = "Responsable";
            worksheet.Cell(1, 3).Value = "DNI";
            worksheet.Cell(1, 4).Value = "Concepto";
            worksheet.Cell(1, 5).Value = "Fecha Emisión";
            worksheet.Cell(1, 6).Value = "Deuda (S/.)";
            worksheet.Range("A1:F1").Style.Font.Bold = true;
            worksheet.Range("A1:F1").Style.Fill.BackgroundColor = XLColor.LightGray;

            // Llenado de datos
            for (int i = 0; i < reporte.Count; i++)
            {
                var row = i + 2;
                worksheet.Cell(row, 1).Value = reporte[i].Puesto;
                worksheet.Cell(row, 2).Value = reporte[i].Responsable;
                worksheet.Cell(row, 3).Value = reporte[i].DNI;
                worksheet.Cell(row, 4).Value = reporte[i].Concepto;
                worksheet.Cell(row, 5).Value = reporte[i].Fecha.ToString("dd/MM/yyyy");
                worksheet.Cell(row, 6).Value = reporte[i].Monto;
            }

            worksheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            var content = stream.ToArray();

            return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Reporte_Morosidad.xlsx");
        }

        // ==========================================
        // 2. PDF: Recibo de Pago (QuestPDF)
        // ==========================================
        [HttpGet]
        public IActionResult GenerarReciboPdf(int deudaId)
        {
            // Buscamos la deuda cruzando datos nuevamente
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
                    page.Margin(2, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(10));

                    page.Header().Text("MERCADO SAN JOSÉ").SemiBold().FontSize(16).FontColor(Colors.Blue.Darken2);

                    page.Content().PaddingVertical(1, Unit.Centimetre).Column(col =>
                    {
                        col.Item().Text("COMPROBANTE DE PAGO").FontSize(14).SemiBold().AlignCenter();
                        col.Item().PaddingBottom(10).Text($"Nro: REC-{reciboData.ReciboNro}").AlignCenter();

                        col.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten2);
                        col.Item().PaddingVertical(10).Table(tabla =>
                        {
                            tabla.ColumnsDefinition(columns =>
                            {
                                columns.ConstantColumn(80);
                                columns.RelativeColumn();
                            });

                            tabla.Cell().Text("Fecha/Hora:");
                            tabla.Cell().Text(reciboData.Fecha);

                            tabla.Cell().Text("Responsable:");
                            tabla.Cell().Text(reciboData.Responsable);

                            tabla.Cell().Text("Nro Puesto:");
                            tabla.Cell().Text(reciboData.Puesto);

                            tabla.Cell().Text("Concepto:");
                            tabla.Cell().Text(reciboData.Concepto);
                        });
                        col.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten2);

                        col.Item().PaddingTop(10).AlignRight().Text($"TOTAL PAGADO: S/. {reciboData.Monto:F2}").FontSize(12).SemiBold();
                    });

                    page.Footer().AlignCenter().Text("Este documento es un comprobante de uso interno.");
                });
            });

            var pdfStream = new MemoryStream();
            document.GeneratePdf(pdfStream);
            pdfStream.Position = 0;

            return File(pdfStream, "application/pdf", $"Recibo_{reciboData.ReciboNro}.pdf");
        }
    }
}