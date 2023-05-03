using EPM.Mouser.Interview.Models;
using Microsoft.AspNetCore.Mvc;

namespace EPM.Mouser.Interview.Web.Controllers
{
    [Route("/ManageProducts")]
    public class ManageProductsController : Controller
    {

        private readonly WarehouseApi warehouseApi;
        public ManageProductsController(WarehouseApi _warehouseApi)
        {
            warehouseApi = _warehouseApi;
        }

        [HttpGet]
        public IActionResult AddProduct()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AddProduct(Product product)
        {
            ViewBag.result = warehouseApi.AddNewProduct(product).Value;
            
            return View();
        }

       

    }
}
