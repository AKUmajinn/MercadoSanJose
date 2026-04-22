using Microsoft.AspNetCore.Mvc;
using MercadoSanJose.Web.Models;
using MercadoSanJose.Web.Repositories.Interfaces;

namespace MercadoSanJose.Web.Controllers;

public class ConceptoDeudaController : Controller
{
    private readonly IConceptoDeuda _dao;

    public ConceptoDeudaController(IConceptoDeuda dao)
    {
        _dao = dao;
    }

    public IActionResult Index() => View(_dao.getAll());
}