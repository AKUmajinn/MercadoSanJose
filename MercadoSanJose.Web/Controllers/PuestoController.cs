using Microsoft.AspNetCore.Mvc;
using MercadoSanJose.Web.Models;
using MercadoSanJose.Web.Repositories.Interfaces;

namespace MercadoSanJose.Web.Controllers;

public class PuestoController : Controller
{
    private readonly IPuesto _dao;

    public PuestoController(IPuesto dao)
    {
        _dao = dao;
    }

    public IActionResult Index() => View(_dao.getAll());
    public IActionResult Create() => View();
    public IActionResult Edit(int id) => View(_dao.getById(id));
}