using DoAnWebNangCao.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DoAnWebNangCao.Controllers
{
    public class SanPhamController : Controller
    {
        // Database context
        private DataClasses1DataContext db = new DataClasses1DataContext("Data Source=VIET;Initial Catalog=MobileShopDB;Integrated Security=True;TrustServerCertificate=True");

        // GET: SanPham
        public ActionResult Index()
        {
            return View();
        }

        // API to get all products with pagination
        [HttpGet]
        public JsonResult GetProducts(int pageNumber = 1, int pageSize = 10)
        {
            var products = db.Products
                .OrderBy(p => p.ProductID)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new {
                    ProductID = p.ProductID,
                    Name = p.Name,
                    Price = p.Price,
                    Quantity = p.Quantity,
                    Description = p.Description,
                    CatalogueName = p.ProductCatalogue.CatalogueName, // Assuming Product has a foreign key to Category
                    CatalogueID = p.ProductCatalogue.CatalogueID
                }).ToList();

            var totalRecord = db.Products.Count();
            var totalPage = (int)Math.Ceiling((double)totalRecord / pageSize);

            return Json(new { data = products, totalPage, totalRecord }, JsonRequestBehavior.AllowGet);
        }

        // API to get all categories
        [HttpGet]
        public JsonResult GetCategories()
        {
            var catalogue = db.ProductCatalogues.Select(c => new {
                CatalogueID = c.CatalogueID,
                CatalogueName = c.CatalogueName,
            }).ToList();

            return Json(catalogue, JsonRequestBehavior.AllowGet);
        }

        // API to add a new product
        [HttpPost]
        public JsonResult AddProduct(Product product)
        {
            if (ModelState.IsValid)
            {
                product.ProductID = Guid.NewGuid().ToString();
                db.Products.InsertOnSubmit(product);
                db.SubmitChanges();
                return Json(new { success = true, message = "Product added successfully." });
            }
            return Json(new { success = false, message = "Invalid product data." });
        }

        // API to update an existing product
        [HttpPut]
        public JsonResult EditProduct(Product product)
        {
            var existingProduct = db.Products.FirstOrDefault(p => p.ProductID == product.ProductID);
            if (existingProduct != null)
            {
                existingProduct.Name = product.Name;
                existingProduct.Price = product.Price;
                existingProduct.Quantity = product.Quantity;
                existingProduct.Description = product.Description;
                existingProduct.CatalougeID= product.CatalougeID;
                db.SubmitChanges();
                return Json(new { success = true, message = "Product updated successfully." });
            }
            return Json(new { success = false, message = "Product not found." });
        }

        // API to delete a product
        [HttpDelete]
        public JsonResult DeleteProduct(string id)
        {
            var product = db.Products.FirstOrDefault(p => p.ProductID == id);
            if (product != null)
            {
                db.Products.DeleteOnSubmit(product);
                db.SubmitChanges();
                return Json(new { success = true, message = "Product deleted successfully." });
            }
            return Json(new { success = false, message = "Product not found." });
        }
    }
}
