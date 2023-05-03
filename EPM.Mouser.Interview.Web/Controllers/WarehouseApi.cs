using Microsoft.AspNetCore.Mvc;

using EPM.Mouser.Interview.Data;
using EPM.Mouser.Interview.Models;

namespace EPM.Mouser.Interview.Web.Controllers
{
    public class WarehouseApi : Controller
    {
        private readonly IWarehouseRepository _warehouseRepository;
        public WarehouseApi(IWarehouseRepository warehouseRepository)
        {
            _warehouseRepository = warehouseRepository; 
        }
      
        /*
         *  Action: GET
         *  Url: api/warehouse/id
         *  This action should return a single product for an Id
         */
        [HttpGet]
        public JsonResult GetProduct(long id)
        {            
          
             if (id < 0)
                {
                    return Json(ErrorReason.InvalidRequest);
                }
             var item= _warehouseRepository.Get(id).Result;
             return Json(item);
           
        }

        /*
         *  Action: GET
         *  Url: api/warehouse
         *  This action should return a collection of products in stock
         *  In stock means In Stock Quantity is greater than zero and In Stock Quantity is greater than the Reserved Quantity
         */
        [HttpGet]
        public JsonResult GetPublicInStockProducts()
        {
            var items = _warehouseRepository.Query(product=>product.InStockQuantity>0 &&  product.InStockQuantity > product.ReservedQuantity).Result;
                        
            return Json(items);
        }


        /*
         *  Action: GET
         *  Url: api/warehouse/order
         *  This action should return a EPM.Mouser.Interview.Models.UpdateResponse
         *  This action should have handle an input parameter of EPM.Mouser.Interview.Models.UpdateQuantityRequest in JSON format in the body of the request
         *       {
         *           "id": 1,
         *           "quantity": 1
         *       }
         *
         *  This action should increase the Reserved Quantity for the product requested by the amount requested
         *
         *  This action should return failure (success = false) when:
         *     - ErrorReason.NotEnoughQuantity when: The quantity being requested would increase the Reserved Quantity to be greater than the In Stock Quantity.
         *     - ErrorReason.QuantityInvalid when: A negative number was requested
         *     - ErrorReason.InvalidRequest when: A product for the id does not exist
        */
        public JsonResult OrderItem(UpdateQuantityRequest updateQuantityRequest)
        {
            UpdateResponse result;
            if(updateQuantityRequest.Quantity<0)
            {
                result = new UpdateResponse()
                {
                    ErrorReason = ErrorReason.QuantityInvalid,
                    Success = false
                };
                return Json(result);
            }
            var product = _warehouseRepository.Query(product => product.Id == updateQuantityRequest.Id).Result.FirstOrDefault();
            
           
            if (product == null)
            {
                result = new UpdateResponse()
                {
                    ErrorReason = ErrorReason.InvalidRequest,
                    Success = false
                };
                return Json(result);
            }
            if(product.ReservedQuantity > (product.InStockQuantity+ updateQuantityRequest.Quantity))
            {
                result = new UpdateResponse()
                {
                    ErrorReason = ErrorReason.NotEnoughQuantity,
                    Success = false
                };
                return Json(result);    
            }
            product.ReservedQuantity += updateQuantityRequest.Quantity;
            var updateResponse = _warehouseRepository.UpdateQuantities(product);
            result = new UpdateResponse()
            {
                ErrorReason = null,
                Success = true
            };
            return Json(result);
        }

        /*
         *  Url: api/warehouse/ship
         *  This action should return a EPM.Mouser.Interview.Models.UpdateResponse
         *  This action should have handle an input parameter of EPM.Mouser.Interview.Models.UpdateQuantityRequest in JSON format in the body of the request
         *       {
         *           "id": 1,
         *           "quantity": 1
         *       }
         *
         *
         *  This action should:
         *     - decrease the Reserved Quantity for the product requested by the amount requested to a minimum of zero.
         *     - decrease the In Stock Quantity for the product requested by the amount requested
         *
         *  This action should return failure (success = false) when:
         *     - ErrorReason.NotEnoughQuantity when: The quantity being requested would cause the In Stock Quantity to go below zero.
         *     - ErrorReason.QuantityInvalid when: A negative number was requested
         *     - ErrorReason.InvalidRequest when: A product for the id does not exist
        */
        public JsonResult ShipItem(UpdateQuantityRequest updateQuantityRequest)
        {
            UpdateResponse result;  
            if(updateQuantityRequest.Quantity<0)
            {
                result = new UpdateResponse()
                {
                    ErrorReason = ErrorReason.QuantityInvalid,
                    Success = false
                };
                return Json(result);
            }
            var product = _warehouseRepository.Query(product => product.Id == updateQuantityRequest.Id).Result.FirstOrDefault();
            if (product == null)
            {
                result = new UpdateResponse()
                {
                    ErrorReason = ErrorReason.InvalidRequest,
                    Success = false
                };
                return Json(result);
            }
            if(product.InStockQuantity < updateQuantityRequest.Quantity)
            {
                result = new UpdateResponse()
                {
                    ErrorReason = ErrorReason.NotEnoughQuantity,
                    Success = false
                };
                return Json(result);

            }
            if(product.ReservedQuantity < updateQuantityRequest.Quantity)
            {
                product.ReservedQuantity = 0;
               
            }
            else
            {
                product.ReservedQuantity-= updateQuantityRequest.Quantity;
            }

            product.InStockQuantity -= updateQuantityRequest.Quantity;
            var updateResponse = _warehouseRepository.UpdateQuantities(product);
            result = new UpdateResponse()
            {
                ErrorReason = null,
                Success = true
            };

            return Json(result);
        }

        /*
        *  Url: api/warehouse/restock
        *  This action should return a EPM.Mouser.Interview.Models.UpdateResponse
        *  This action should have handle an input parameter of EPM.Mouser.Interview.Models.UpdateQuantityRequest in JSON format in the body of the request
        *       {
        *           "id": 1,
        *           "quantity": 1
        *       }
        *
        *
        *  This action should:
        *     - increase the In Stock Quantity for the product requested by the amount requested
        *
        *  This action should return failure (success = false) when:
        *     - ErrorReason.QuantityInvalid when: A negative number was requested
        *     - ErrorReason.InvalidRequest when: A product for the id does not exist
        */
        public JsonResult RestockItem(UpdateQuantityRequest updateQuantityRequest)
        {
            UpdateResponse result;
            if(updateQuantityRequest.Quantity<0)
            {
                 result = new UpdateResponse()
                {
                    ErrorReason = ErrorReason.QuantityInvalid,
                    Success = false
                };
                return Json(result);
            }
            var product = _warehouseRepository.Query(product => product.Id == updateQuantityRequest.Id).Result.FirstOrDefault();
            if (product == null)
            {
                result = new UpdateResponse()
                {
                    ErrorReason = ErrorReason.InvalidRequest,
                    Success = false
                };
                return Json(result);
            }
            product.InStockQuantity += updateQuantityRequest.Quantity;
            result = new UpdateResponse()
            {
                ErrorReason = null,
                Success = true
            };
            return Json(result);
        }

        /*
        *  Url: api/warehouse/add
        *  This action should return a EPM.Mouser.Interview.Models.CreateResponse<EPM.Mouser.Interview.Models.Product>
        *  This action should have handle an input parameter of EPM.Mouser.Interview.Models.Product in JSON format in the body of the request
        *       {
        *           "id": 1,
        *           "inStockQuantity": 1,
        *           "reservedQuantity": 1,
        *           "name": "product name"
        *       }
        *
        *
        *  This action should:
        *     - create a new product with:
        *          - The requested name - But forced to be unique - see below
        *          - The requested In Stock Quantity
        *          - The Reserved Quantity should be zero
        *
        *       UNIQUE Name requirements
        *          - No two products can have the same name
        *          - Names should have no leading or trailing whitespace before checking for uniqueness
        *          - If a new name is not unique then append "(x)" to the name [like windows file system does, where x is the next avaiable number]
        *
        *
        *  This action should return failure (success = false) and an empty Model property when:
        *     - ErrorReason.QuantityInvalid when: A negative number was requested for the In Stock Quantity
        *     - ErrorReason.InvalidRequest when: A blank or empty name is requested
        */
        public JsonResult AddNewProduct(Product product)
        {
            CreateResponse<Product> result;
            if (product.InStockQuantity < 0)
            {
                result = new CreateResponse<Product>
                {
                    ErrorReason = ErrorReason.QuantityInvalid,
                    Success = false
                };

            }
            else if (string.IsNullOrEmpty(product.Name))
            {
                result = new CreateResponse<Product>
                {
                    ErrorReason = ErrorReason.InvalidRequest,
                    Success = false
                };

            }
            else
            {
                var existingProductName = _warehouseRepository.Query(x => x.Name == product.Name).Result;
                if (existingProductName != null)
                {
                    product.Name = product.Name + "(" + existingProductName.Count()+1 + ")";
                }
                _warehouseRepository.Insert(product);
                result = new CreateResponse<Product> { Success = true, ErrorReason = null };
            }
            return Json(result);
        }
    }
}
