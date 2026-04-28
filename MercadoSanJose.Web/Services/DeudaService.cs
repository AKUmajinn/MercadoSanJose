using Microsoft.EntityFrameworkCore;
using MercadoSanJose.Web.Data;
using MercadoSanJose.Web.Models.Enums;
using MercadoSanJose.Web.Models.ViewModels;
using MercadoSanJose.Web.Models;

namespace MercadoSanJose.Web.Services;

public class DeudaService : IDeudaService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<DeudaService> _logger;

    public DeudaService(ApplicationDbContext context, ILogger<DeudaService> logger)
    {
        _context = context;
        _logger = logger;
    }

    private (int responsableId, string motivo) DeterminarResponsable(Puesto puesto)
    {
        if (puesto is null)
            throw new ArgumentNullException(nameof(puesto));

        if (puesto.Estado == EstadoPuesto.Alquilado)
        {
            if (puesto.InquilinoId.HasValue)
            {
                return (puesto.InquilinoId.Value,
                    $"Puesto alquilado → Responsable: Inquilino ({puesto.Inquilino?.Nombre ?? "?"})");
            }

            var propietarioId = puesto.PropietarioId ??
                throw new InvalidOperationException($"Puesto marcado Alquilado sin inquilino ni propietario (PuestoId={puesto.Id})");

            return (propietarioId,
                $"Puesto marcado Alquilado sin inquilino asignado → Responsable: Propietario ({puesto.Propietario?.Nombre ?? "?"})");
        }

        if (puesto.Estado == EstadoPuesto.Disponible)
        {
            var propietarioId = puesto.PropietarioId ??
                throw new InvalidOperationException($"Puesto disponible sin propietario asignado (PuestoId={puesto.Id})");

            var motivo = puesto.Propietario?.EsGerencia == true
                ? $"Puesto disponible de Gerencia → Deuda cargada a la cuenta de Gerencia"
                : $"Puesto disponible → Responsable: Propietario ({puesto.Propietario?.Nombre ?? "?"})";

            return (propietarioId, motivo);
        }

        if (puesto.Estado == EstadoPuesto.Vendido)
        {
            var propietarioId = puesto.PropietarioId ??
                throw new InvalidOperationException($"Puesto vendido sin propietario asignado (PuestoId={puesto.Id})");

            return (propietarioId,
                $"Puesto vendido → Responsable: Propietario ({puesto.Propietario?.Nombre ?? "?"})");
        }

        throw new InvalidOperationException($"Estado de puesto no reconocido: {puesto.Estado}");
    }

    public async Task<ResultadoGeneracionDeuda> GenerarDeudaIndividualAsync(
        int puestoId, int conceptoDeudaId, DateTime fechaEmision)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var puesto = await _context.Puestos
                .Include(p => p.Propietario)
                .Include(p => p.Inquilino)
                .FirstOrDefaultAsync(p => p.Id == puestoId);

            if (puesto is null)
                return new ResultadoGeneracionDeuda
                {
                    Exitoso = false,
                    Mensaje = $"No se encontró el puesto con ID {puestoId}"
                };

            var concepto = await _context.ConceptosDeuda.FindAsync(conceptoDeudaId);
            if (concepto is null)
                return new ResultadoGeneracionDeuda
                {
                    Exitoso = false,
                    Mensaje = $"No se encontró el concepto de deuda con ID {conceptoDeudaId}"
                };

            var (responsableId, motivo) = DeterminarResponsable(puesto);

            var deuda = new Deuda
            {
                PuestoId = puesto.Id,
                ConceptoDeudaId = concepto.Id,
                FechaEmision = fechaEmision,
                ResponsableId = responsableId,
                MontoTotal = concepto.MontoBase,
                Estado = EstadoDeuda.Pendiente
            };

            _context.Deudas.Add(deuda);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            var responsable = await _context.Personas.FindAsync(responsableId);
            _logger.LogInformation("Deuda #{DeudaId} generada. Puesto {Num}. {Motivo}",
                deuda.Id, puesto.NumeroPuesto, motivo);

            return new ResultadoGeneracionDeuda
            {
                Exitoso = true,
                DeudaId = deuda.Id,
                Mensaje = $"Deuda generada exitosamente para el puesto {puesto.NumeroPuesto}",
                ResponsableAsignado = responsable?.Nombre,
                MotivoAsignacion = motivo
            };
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, "Error generando deuda individual para puesto {PuestoId}", puestoId);
            return new ResultadoGeneracionDeuda
            {
                Exitoso = false,
                Mensaje = $"Error interno: {ex.Message}"
            };
        }
    }

    public async Task<ResultadoGeneracionMasiva> GenerarDeudasMasivasAsync(
        int conceptoDeudaId, DateTime fechaEmision, int[]? puestoIds = null)
    {
        var resultado = new ResultadoGeneracionMasiva();

        var concepto = await _context.ConceptosDeuda.FindAsync(conceptoDeudaId);
        if (concepto is null)
        {
            resultado.TotalErrores++;
            resultado.Detalles.Add(new ResultadoGeneracionDeuda
            {
                Exitoso = false,
                Mensaje = $"Concepto de deuda ID {conceptoDeudaId} no encontrado"
            });
            return resultado;
        }

        IQueryable<Puesto> query = _context.Puestos
            .Include(p => p.Propietario)
            .Include(p => p.Inquilino);

        if (puestoIds is { Length: > 0 })
            query = query.Where(p => puestoIds.Contains(p.Id));

        var puestos = await query.ToListAsync();
        resultado.TotalProcesados = puestos.Count;

        await using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var deudasNuevas = new List<Deuda>();

            foreach (var puesto in puestos)
            {
                var (responsableId, motivo) = DeterminarResponsable(puesto);

                var responsable = puesto.Estado == EstadoPuesto.Alquilado
                    ? puesto.Inquilino
                    : puesto.Propietario;

                deudasNuevas.Add(new Deuda
                {
                    PuestoId = puesto.Id,
                    ConceptoDeudaId = concepto.Id,
                    FechaEmision = fechaEmision,
                    ResponsableId = responsableId,
                    MontoTotal = concepto.MontoBase,
                    Estado = EstadoDeuda.Pendiente
                });

                resultado.TotalExitosos++;
                resultado.MontoTotalGenerado += concepto.MontoBase;
                resultado.Detalles.Add(new ResultadoGeneracionDeuda
                {
                    Exitoso = true,
                    Mensaje = $"Puesto {puesto.NumeroPuesto} ({puesto.Sector})",
                    ResponsableAsignado = responsable?.Nombre,
                    MotivoAsignacion = motivo
                });
            }

            await _context.Deudas.AddRangeAsync(deudasNuevas);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            _logger.LogInformation(
                "Generación masiva completada: {Exitosos}/{Total} deudas. Monto total: {Monto:C}",
                resultado.TotalExitosos, resultado.TotalProcesados, resultado.MontoTotalGenerado);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, "Error en generación masiva de deudas");
            resultado.TotalErrores = resultado.TotalProcesados;
            resultado.TotalExitosos = 0;
            resultado.MontoTotalGenerado = 0;
            resultado.Detalles.Clear();
            resultado.Detalles.Add(new ResultadoGeneracionDeuda
            {
                Exitoso = false,
                Mensaje = $"Error en la transacción: {ex.Message}"
            });
        }

        return resultado;
    }

    public async Task<IEnumerable<DeudaViewModel>> BuscarDeudasAsync(
        string? numeroPuesto, string? dniResponsable)
    {
        var query = _context.Deudas
            .Include(d => d.Puesto)
            .Include(d => d.ConceptoDeuda)
            .Include(d => d.Responsable)
            .Where(d => d.Estado == EstadoDeuda.Pendiente)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(numeroPuesto))
            query = query.Where(d => d.Puesto.NumeroPuesto == numeroPuesto);

        if (!string.IsNullOrWhiteSpace(dniResponsable))
            query = query.Where(d => d.Responsable.DNI == dniResponsable);

        return await query.Select(d => new DeudaViewModel
        {
            Id = d.Id,
            NumeroPuesto = d.Puesto.NumeroPuesto,
            Sector = d.Puesto.Sector,
            EstadoPuesto = d.Puesto.Estado.ToString(),
            ConceptoDeuda = d.ConceptoDeuda.Nombre,
            FechaEmision = d.FechaEmision,
            NombreResponsable = d.Responsable.Nombre,
            DNIResponsable = d.Responsable.DNI,
            MontoTotal = d.MontoTotal,
            Estado = d.Estado
        }).ToListAsync();
    }

    public async Task<IEnumerable<DeudaViewModel>> ObtenerMorosidadAsync()
    {
        return await _context.Deudas
            .Include(d => d.Puesto)
            .Include(d => d.ConceptoDeuda)
            .Include(d => d.Responsable)
            .Where(d => d.Estado == EstadoDeuda.Pendiente)
            .OrderBy(d => d.Responsable.Nombre)
            .ThenBy(d => d.FechaEmision)
            .Select(d => new DeudaViewModel
            {
                Id = d.Id,
                NumeroPuesto = d.Puesto.NumeroPuesto,
                Sector = d.Puesto.Sector,
                EstadoPuesto = d.Puesto.Estado.ToString(),
                ConceptoDeuda = d.ConceptoDeuda.Nombre,
                FechaEmision = d.FechaEmision,
                NombreResponsable = d.Responsable.Nombre,
                DNIResponsable = d.Responsable.DNI,
                MontoTotal = d.MontoTotal,
                Estado = d.Estado
            }).ToListAsync();
    }

    public async Task<List<DeudaListItemVM>> ListarDeudasAsync(FiltroDeudasVM filtro)
    {
        var query = _context.Deudas
            .Include(d => d.Puesto)
            .Include(d => d.ConceptoDeuda)
            .Include(d => d.Responsable)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(filtro.NumeroPuesto))
            query = query.Where(d => d.Puesto.NumeroPuesto.Contains(filtro.NumeroPuesto));

        if (!string.IsNullOrWhiteSpace(filtro.DNIResponsable))
            query = query.Where(d => d.Responsable.DNI == filtro.DNIResponsable);

        if (filtro.Estado.HasValue)
            query = query.Where(d => d.Estado == filtro.Estado.Value);

        if (filtro.FechaDesde.HasValue)
            query = query.Where(d => d.FechaEmision >= filtro.FechaDesde.Value);

        if (filtro.FechaHasta.HasValue)
            query = query.Where(d => d.FechaEmision <= filtro.FechaHasta.Value);

        return await query
            .OrderByDescending(d => d.FechaEmision)
            .Select(d => new DeudaListItemVM
            {
                Id = d.Id,
                NumeroPuesto = d.Puesto.NumeroPuesto,
                Sector = d.Puesto.Sector,
                NombreConcepto = d.ConceptoDeuda.Nombre,
                FechaEmision = d.FechaEmision,
                NombreResponsable = d.Responsable.Nombre,
                DNIResponsable = d.Responsable.DNI,
                EsGerenciaResponsable = d.Responsable.EsGerencia,
                MontoTotal = d.MontoTotal,
                Estado = d.Estado
            })
            .ToListAsync();
    }

    public async Task<ResumenCajaDiariaViewModel> ObtenerCajaDiariaAsync(DateTime fecha)
    {
        var fechaSolo = fecha.Date;

        var pagos = await _context.Pagos
            .Include(p => p.Deuda).ThenInclude(d => d.ConceptoDeuda)
            .Where(p => p.FechaPago.Date == fechaSolo)
            .ToListAsync();

        var detalle = pagos
            .GroupBy(p => p.Deuda.ConceptoDeuda.Nombre)
            .Select(g => new ItemCajaDiaria
            {
                Concepto = g.Key,
                CantidadPagos = g.Count(),
                Total = g.Sum(p => p.MontoPagado)
            }).ToList();

        return new ResumenCajaDiariaViewModel
        {
            Fecha = fechaSolo,
            TotalRecaudado = detalle.Sum(d => d.Total),
            DetallesPorConcepto = detalle
        };
    }
}