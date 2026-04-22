using Microsoft.AspNetCore.Mvc;
using MercadoSanJose.Web.Models;
using MercadoSanJose.Web.Repositories.Interfaces;

namespace MercadoSanJose.Web.Controllers;

public class DeudaController : Controller
{
    private readonly IDeuda _dao;

    public DeudaController(IDeuda dao)
    {
        _dao = dao;
    }

    public IActionResult Index() => View(_dao.getAll());
    public IActionResult Create() => View();
}