using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using Bits_And_Bytes_Vincenzo_Russo.Models;
using System.Text;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Web.UI.WebControls;
using System.Drawing;

namespace Bits_And_Bytes_Vincenzo_Russo.Controllers
{
    [Authorize]
    public class ProductsController : Controller
    {
        private BitsBytesDbContext db = new BitsBytesDbContext();

        //***********************************CRUD PRODUCTS*****************************

        // GET: Products
        [Authorize(Roles = ("Manager, Sales Assistant"))]
        public ActionResult Index()
        {
            //Get products, include the category fk, return view to list
            var products = db.Products.Include(p => p.Category);
            return View(products.ToList());
        }

        // GET: Products/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }


        // GET: Products/Create
        [Authorize (Roles = "Manager")]
        public ActionResult Create()
        {         

            ViewBag.CategoryId = new SelectList(db.Categories, "CategoryId", "Name");
            
            return View();
        }

        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Manager")]
        [HttpPost]
        public ActionResult CreateProduct([Bind(Include = "ProductId,ProductName,ProductDescription,ProductPrice,ProductStock,IsDiscounted,ProductDiscount,IsBOGOF,InStock,ProductType,CategoryId")] Product product, HttpPostedFileBase imageFile)
        {
            if (imageFile != null && imageFile.ContentLength > 0)
            {
                //Generate a unique file name for the uploaded image
                string fileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(imageFile.FileName);

                string filePath;

                //Check what product type it is, depending on what it is, change file path location
                if(product.ProductType == ProductType.Monitors)
                {
                     filePath = Path.Combine(Server.MapPath("~/Images/Monitors/"), fileName);

                    //Save the image file to the specified path
                    imageFile.SaveAs(filePath);

                    //Set the ImageUrl property of the product to the saved file path
                    product.ImageUrl = "/Images/Monitors/" + fileName;

                }
                else if(product.ProductType == ProductType.Speakers)
                {
                    filePath = Path.Combine(Server.MapPath("~/Images/Speakers/"), fileName);

                    //Save the image file to the specified path
                    imageFile.SaveAs(filePath);

                    //Set the ImageUrl property of the product to the saved file path
                    product.ImageUrl = "/Images/Speakers/" + fileName;
                }
                else if(product.ProductType == ProductType.PCs)
                {
                    filePath = Path.Combine(Server.MapPath("~/Images/PCs/"), fileName);

                    //Save the image file to the specified path
                    imageFile.SaveAs(filePath);

                    //Set the ImageUrl property of the product to the saved file path
                    product.ImageUrl = "/Images/PCs/" + fileName;
                }
                else if(product.ProductType == ProductType.Peripherals)
                {
                    filePath = Path.Combine(Server.MapPath("~/Images/Peripherals/"), fileName);

                    //Save the image file to the specified path
                    imageFile.SaveAs(filePath);

                    //Set the ImageUrl property of the product to the saved file path
                    product.ImageUrl = "/Images/Peripherals/" + fileName;
                }
                else
                {
                    filePath = Path.Combine(Server.MapPath("~/Images/Laptops/"), fileName);

                    //Save the image file to the specified path
                    imageFile.SaveAs(filePath);

                    //Set the ImageUrl property of the product to the saved file path
                    product.ImageUrl = "/Images/Laptops/" + fileName;
                }
      
            }

            //If the modelstate is valid, add the product, save changes, return to index
            if (ModelState.IsValid)
            {
                db.Products.Add(product);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            //If we got here something went wrong
            return View(product);

        }





    [Authorize(Roles = "Manager")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }



            // Fetch the categories from the database
            List<Category> categories = db.Categories.ToList();

            // Convert the categories to SelectListItems
            IEnumerable<SelectListItem> categoryList = categories.Select(c => new SelectListItem
            {
                Value = c.CategoryId.ToString(),
                Text = c.Name
            });

            // Pass the categoryList to the view
            ViewBag.CategoryId = categoryList;

            return View(product);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ProductId,ProductName,ProductDescription,ProductPrice,ProductStock,IsDiscounted,ProductDiscount,IsBOGOF,InStock,ProductType,CategoryId")] Product product, HttpPostedFileBase imageFile)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (imageFile != null && imageFile.ContentLength > 0)
                    {
                        //If there is already an image, keep it
                        if (product.ImageUrl != null)
                        {
                            string existingImageUrl = Request.Form["ImageUrl"];
                            product.ImageUrl = existingImageUrl;
                        }
                        else
                        {
                            //Generate a unique file name for the uploaded image
                            string fileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(imageFile.FileName);

                            string filePath;

                            //Check what product type it is, depending on what it is, change file path location
                            if (product.ProductType == ProductType.Monitors)
                            {
                                filePath = Path.Combine(Server.MapPath("~/Images/Monitors/"), fileName);

                                //Save the image file to the specified path
                                imageFile.SaveAs(filePath);

                                //Set the ImageUrl property of the product to the saved file path
                                product.ImageUrl = "/Images/Monitors/" + fileName;

                            }
                            else if (product.ProductType == ProductType.Speakers)
                            {
                                filePath = Path.Combine(Server.MapPath("~/Images/Speakers/"), fileName);

                                //Save the image file to the specified path
                                imageFile.SaveAs(filePath);

                                //Set the ImageUrl property of the product to the saved file path
                                product.ImageUrl = "/Images/Speakers/" + fileName;
                            }
                            else if (product.ProductType == ProductType.PCs)
                            {
                                filePath = Path.Combine(Server.MapPath("~/Images/PCs/"), fileName);

                                //Save the image file to the specified path
                                imageFile.SaveAs(filePath);

                                //Set the ImageUrl property of the product to the saved file path
                                product.ImageUrl = "/Images/PCs/" + fileName;
                            }
                            else if (product.ProductType == ProductType.Peripherals)
                            {
                                filePath = Path.Combine(Server.MapPath("~/Images/Peripherals/"), fileName);

                                //Save the image file to the specified path
                                imageFile.SaveAs(filePath);

                                //Set the ImageUrl property of the product to the saved file path
                                product.ImageUrl = "/Images/Peripherals/" + fileName;
                            }
                            else
                            {
                                filePath = Path.Combine(Server.MapPath("~/Images/Laptops/"), fileName);

                                //Save the image file to the specified path
                                imageFile.SaveAs(filePath);

                                //Set the ImageUrl property of the product to the saved file path
                                product.ImageUrl = "/Images/Laptops/" + fileName;
                            }

                        }


                    }
                    //Modify the product entry,save changes, redirect to index action
                    db.Entry(product).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }

                return View(product);
            }
            catch(Exception ex)
            {
                throw new Exception(ex .Message);
            }
           
        }


        // GET: Products/Delete/5
        [Authorize(Roles = "Manager")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        [Authorize(Roles = "Manager, Sales Assistant")]
        [HttpPost]
        public FileResult ExportToPDF()
        {
           
            //Collect all products data from dbcontext, into object list
            List<object> products = (from product in db.Products.ToList()
                                      select new
                                      {
                                          product.ProductId,
                                          product.ProductName,
                                          product.ProductDescription,
                                          product.Category,
                                          product.ProductType,
                                          product.InStock,
                                          product.ProductStock,
                                          product.ProductPrice,
                                          product.IsDiscounted,
                                          product.ProductDiscount,
                                          product.IsBOGOF
                                          
                                      }).ToList<object>();

            //Building an HTML string
            StringBuilder sb = new StringBuilder();

            //Table start.
            sb.Append("<table border='1' cellpadding='5' cellspacing='0' style='border: 1px solid #ccc;font-family: Arial; font-size: 10pt;'>");

            //Building the Header row
            sb.Append("<tr>");
            sb.Append("<th style='background-color: #B8DBFD;border: 1px solid #ccc'>ProductId</th>");
            sb.Append("<th style='background-color: #B8DBFD;border: 1px solid #ccc'>ProductName</th>");
            sb.Append("<th style='background-color: #B8DBFD;border: 1px solid #ccc'>ProductDescription</th>");
            sb.Append("<th style='background-color: #B8DBFD;border: 1px solid #ccc'>ProductType</th>");
            sb.Append("<th style='background-color: #B8DBFD;border: 1px solid #ccc'>InStock</th>");
            sb.Append("<th style='background-color: #B8DBFD;border: 1px solid #ccc'>ProductStock</th>");
            sb.Append("<th style='background-color: #B8DBFD;border: 1px solid #ccc'>ProductPrice</th>");
            sb.Append("<th style='background-color: #B8DBFD;border: 1px solid #ccc'>IsDiscounted</th>");
            sb.Append("<th style='background-color: #B8DBFD;border: 1px solid #ccc'>ProductDiscount</th>");
            sb.Append("<th style='background-color: #B8DBFD;border: 1px solid #ccc'>IsBOGOF</th>");
            sb.Append("</tr>");

            //Building the Data rows
            for (int i = 0; i < products.Count; i++)
            {
                //Cast as dynamic, so we can convert it to a var
                var product = (dynamic)products[i]; 

                sb.Append("<tr>");
                sb.Append("<td style='border: 1px solid #ccc'>");
                sb.Append(product.ProductId);
                sb.Append("</td>");
                sb.Append("<td style='border: 1px solid #ccc'>");
                sb.Append(product.ProductName);
                sb.Append("</td>");
                sb.Append("<td style='border: 1px solid #ccc'>");
                sb.Append(product.ProductDescription);
                sb.Append("</td>");
                sb.Append("<td style='border: 1px solid #ccc'>");
                sb.Append(product.ProductType);
                sb.Append("</td>");
                sb.Append("<td style='border: 1px solid #ccc'>");
                sb.Append(product.InStock);
                sb.Append("</td>");
                sb.Append("<td style='border: 1px solid #ccc'>");
                sb.Append(product.ProductStock);
                sb.Append("</td>");
                sb.Append("<td style='border: 1px solid #ccc'>");
                sb.Append(product.ProductPrice);
                sb.Append("</td>");
                sb.Append("<td style='border: 1px solid #ccc'>");
                sb.Append(product.IsDiscounted);
                sb.Append("</td>");
                sb.Append("<td style='border: 1px solid #ccc'>");
                sb.Append(product.ProductDiscount);
                sb.Append("</td>");
                sb.Append("<td style='border: 1px solid #ccc'>");
                sb.Append(product.IsBOGOF);
                sb.Append("</td>");

                sb.Append("</tr>");
            }


            //Table end
            sb.Append("</table>");

            using (MemoryStream stream = new MemoryStream())
            {
                StringReader sr = new StringReader(sb.ToString());
                Document pdfDoc = new Document(PageSize.A4.Rotate(), 70f, 10f, 5f, 0f);
                PdfWriter writer = PdfWriter.GetInstance(pdfDoc, stream);
                pdfDoc.Open();
                XMLWorkerHelper.GetInstance().ParseXHtml(writer, pdfDoc, sr);
                pdfDoc.Close();
                return File(stream.ToArray(), "application/pdf", "Products.pdf");
            }
        }


        [Authorize(Roles = "Manager, Sales Assistant")]
        [HttpPost]
        public ActionResult ExportProductsToExcel()
        {
            //Retrieve all products from the database
            var products = db.Products.ToList(); 

            //Create a new Excel package
            using (var package = new ExcelPackage())
            {
                //Add a new worksheet to the Excel package
                var worksheet = package.Workbook.Worksheets.Add("Products");

                //Define the header row
                worksheet.Cells[1, 1].Value = "Product ID";
                worksheet.Cells[1, 2].Value = "Product Name";
                worksheet.Cells[1, 3].Value = "Product Description";
                worksheet.Cells[1, 4].Value = "Product Price";
                worksheet.Cells[1, 5].Value = "Product Stock";
                //Add more headers for other product properties

                //Style the header row
                using (var range = worksheet.Cells[1, 1, 1, 5])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(Color.LightGray);
                }

                //Populate the data rows
                for (int i = 0; i < products.Count; i++)
                {
                    var product = products[i];
                    //Start from the second row
                    int row = i + 2;

                    worksheet.Cells[row, 1].Value = product.ProductId;
                    worksheet.Cells[row, 2].Value = product.ProductName;
                    worksheet.Cells[row, 3].Value = product.ProductDescription;
                    worksheet.Cells[row, 4].Value = product.ProductPrice;
                    worksheet.Cells[row, 5].Value = product.ProductStock;
                    //Add some cells for each product attribute
                }

                //Auto-fit columns for better readability
                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                //Convert the Excel package to a byte array
                byte[] excelBytes = package.GetAsByteArray();

                //Send the Excel file to the client, name the spreadsheet file
                return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Products.xlsx");
            }
        }


        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Product product = db.Products.Find(id);
            db.Products.Remove(product);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        //Dispose from database
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
