using EPM.Mouser.Interview.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace EPM.Mouser.Interview.Web.Controllers
{
    [Route("")]
    public class HomeController : Controller
    {
        private readonly WarehouseApi warehouseApi;
        public HomeController(WarehouseApi _warehouseApi)
        {
            warehouseApi = _warehouseApi;
        }
        [HttpGet]
        public IActionResult Index()
        {

            ViewBag.Products = warehouseApi.GetPublicInStockProducts().Value;
            return View();
        }

        [HttpGet]
        [Route("ProductDetail/{id}")]        
        public IActionResult ProductDetail(long id)
        {

            ViewBag.Product = warehouseApi.GetProduct(id).Value;
            return View();
        }

        [HttpPost]
        [Route("OrderItem")]
        public IActionResult OrderAnItem(UpdateQuantityRequest updateQuantityRequest)
        {
            ViewBag.response = warehouseApi.OrderItem(updateQuantityRequest);
            return View();
        }
    }
}
