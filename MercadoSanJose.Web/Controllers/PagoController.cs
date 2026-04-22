using Microsoft.AspNetCore.Mvc;
using MercadoSanJose.Web.Models;
using MercadoSanJose.Web.Repositories.Interfaces;

namespace MercadoSanJose.Web.Controllers;

public class PagoController : Controller
{
    private readonly IPago _dao;

    public PagoController(IPago dao)
    {
        _dao = dao;
    }

    public IActionResult Index() => View(_dao.getAll());
    public IActionResult Create() => View();
}