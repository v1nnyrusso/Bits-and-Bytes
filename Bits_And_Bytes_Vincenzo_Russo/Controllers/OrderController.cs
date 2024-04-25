using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Bits_And_Bytes_Vincenzo_Russo.Models;
using iTextSharp.text.pdf;
using iTextSharp.text;
using iTextSharp.tool.xml;
using System.Configuration;
using Microsoft.AspNet.Identity;

namespace Bits_And_Bytes_Vincenzo_Russo.Controllers
{
    public class OrderController : Controller
    {
        //Private global instance of DbContext
        private BitsBytesDbContext db = new BitsBytesDbContext();

        // GET: Orders
        [Authorize(Roles ="Manager")]
        public ActionResult Index()
        {
            //Get the orders, include fk payment and customer, order by order date
            var orders = db.Orders.Include(o => o.Payment).Include(o => o.Customer).OrderBy(u=>u.OrderDate);
            return View(orders.ToList());
        }

        [Authorize(Roles = "Customer")]
        public ActionResult CustomerOrderIndex()
        {
            //userId = the current logged in user
            var userId = User.Identity.GetUserId();

            //Get the orders that match the userId 
            var orders = db.Orders.Include(o => o.Payment).Include(u => u.Customer).Where(o => o.CustomerId == userId);

            //Return view, with orders to list
            return View(orders.ToList());
        }

        [Authorize(Roles = "Manager, Sales Assistant")]
        // GET: Orders/Details/5
        public ActionResult Details(int? id)
        {
            //If id is null, return bad request
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //Find order by id
            Order order = db.Orders.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }

            //Return view with order
            return View(order);
        }

        [Authorize(Roles = "Manager")]
        // GET: Orders/Create
        public ActionResult Create()
        {
            ViewBag.OrderId = new SelectList(db.Payments, "PaymentId", "PaymentId");
            ViewBag.UserId = new SelectList(db.Users, "Id", "FirstName");
            return View();
        }

        // POST: Orders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "OrderId,OrderDate,DateCancelled,OrderTotal,IsPaid,OrderStatus,Subtotal,MembershipDiscount,VatAmount,AddVat,UserId")] Order order)
        {
            if (ModelState.IsValid)
            {
                //Cant pay without users card info, they can pay by card later
                order.IsPaid = false;
                order.OrderStatus = OrderStatus.Placed;
                db.Orders.Add(order);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.OrderId = new SelectList(db.Payments, "PaymentId", "PaymentId", order.OrderId);
            ViewBag.UserId = new SelectList(db.Users, "Id", "FirstName", order.CustomerId);
            return View(order);
        }

        [Authorize(Roles = "Manager")]
        // GET: Orders/Edit/5
        public ActionResult Edit(int? id)
        {
            //If id null, throw exception
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            //Find order by passed in id
            Order order = db.Orders.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }

            //To get all the users, we will create a new var called 'users', and order them by registration date
            var users = db.Users.OrderBy(u => u.RegisteredAt).ToList();

            //Now we can filter the users by role in memory as customers
            var customers = users.OfType<Customer>().ToList();


            ViewBag.OrderId = new SelectList(db.Payments, "PaymentId", "PaymentId", order.OrderId);
            ViewBag.CustomerId = new SelectList(customers, "CustomerId", "FirstName", order.CustomerId);

            //Return view ith order
            return View(order);
        }

        // POST: Orders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "OrderId,OrderDate,DateCancelled,OrderTotal,IsPaid,OrderStatus,Subtotal,MembershipDiscount,VatAmount,AddVat,CustomerId")] Order order)
        {
            if (ModelState.IsValid)
            {
                //Change cancel date
                if (order.OrderStatus == OrderStatus.Cancelled)
                {
                    order.DateCancelled = DateTime.Now;
                }
                db.Entry(order).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }


            //To get all the users, we will create a new var called 'users', and order them by registration date
            var users = db.Users.OrderBy(u => u.RegisteredAt).ToList();

            //Now we can filter the users by role in memory as customers
            var customers = users.OfType<Customer>().ToList();

            ViewBag.OrderId = new SelectList(db.Payments, "PaymentId", "PaymentId", order.OrderId);
            ViewBag.CustomerId = new SelectList(customers, "CustomerId", "FirstName", order.CustomerId);

           
            return View(order);
        }

        // GET: Orders/Delete/5
        [Authorize(Roles = "Manager")]
        public ActionResult Delete(int? id)
        {
            //If id is null, return badrequest exception
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //Find order by id
            Order order = db.Orders.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            //Return view with order
            return View(order);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            //Find order by id
            Order order = db.Orders.Find(id);


            //Cascade delete payment info
            foreach (var item in db.Payments)
            {
                if (order.OrderId == item.PaymentId)
                {
                    db.Payments.Remove(item);
                }
            }

            //Remove order
            db.Orders.Remove(order);

            //Save changes
            db.SaveChanges();

            //Redirect to index
            return RedirectToAction("Index");
        }

        public ActionResult RequestCancellation(int id)
        {
            //Find the order by id
            Order order = db.Orders.Find(id);

            //Make that order's order status cancellation requested
            order.OrderStatus = OrderStatus.CancellationRequested;

            //Save changes, return to customerorderindex
            db.SaveChanges();

            return RedirectToAction("CustomerOrderIndex");
  
        }

        public ActionResult OrderDetails(int id)
        {
            // Find order by id, include product and order lines
            Order order = db.Orders.Include(o => o.OrderLines.Select(ol => ol.Product))
                                   .SingleOrDefault(u => u.OrderId == id);

            //If theres no order, return not fund
            if (order == null)
            {
                return HttpNotFound();
            }

            //Return view, order.orderlines to list
            return View(order.OrderLines.ToList());
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

    
        [HttpPost]
        public FileResult ExportToPDF(int id)
        {
            //Find order by id, include product
            var order = db.Orders.Include(o => o.OrderLines.Select(ol => ol.Product)).SingleOrDefault(u => u.OrderId == id);

            var userName = User.Identity.Name;

            var user = db.Users.SingleOrDefault(u => u.UserName == userName);

            //Check if the order exists
            if (order == null)
            {
                //Handle the case when the order is not found
                return null;
            }


            //Building an HTML string for the receipt
            StringBuilder sb = new StringBuilder();

            //Add the receipt header
            sb.Append("<h2>Order Receipt</h2>");
            sb.Append("<hr />");

            //Add order details
            sb.Append("<h3>Order Information</h3>");
            sb.Append("<p><strong>Order Number:</strong> " + order.OrderId + "</p>");
            sb.Append("<p><strong>Date:</strong> " + order.OrderDate + "</p>");
            sb.Append("<p><strong>Subtotal: £</strong> " + order.Subtotal + "</p>");
            sb.Append("<p><strong>VAT Total: + £</strong> " + order.VatAmount + "</p>");  
            sb.Append("<p><strong>Membership Discount: - £</strong> " +  order.MembershipDiscount + "</p>");
            sb.Append("<p><strong>Total: £</strong> " + order.OrderTotal + "</p>");
            sb.Append("<p><strong>Order Status:</strong> " + order.OrderStatus + "</p>");

            //Add customer details
            sb.Append("<h3>Customer Information</h3>");
            sb.Append("<p><strong>Customer Name:</strong> " + user.FirstName + " " + user.LastName + "</p>");
            sb.Append("<p><strong>Email:</strong> " + user.Email + "</p>");
            sb.Append("<p><strong>Address:</strong> " + order.DeliveryAddress + " " + order.DeliveryTown + " " + order.DeliveryPostCode + " " +  order.DeliveryCountry + "</p>");

            //Add order lines
            sb.Append("<h3>Products Purchased</h3>");
            sb.Append("<table border='1' cellpadding='5' cellspacing='0' style='border: 1px solid #ccc;font-family: Arial; font-size: 10pt;'>");
            sb.Append("<tr>");
            sb.Append("<th style='background-color: #B8DBFD;border: 1px solid #ccc'>Product Name</th>");
            sb.Append("<th style='background-color: #B8DBFD;border: 1px solid #ccc'>Quantity</th>");
            sb.Append("<th style='background-color: #B8DBFD;border: 1px solid #ccc'>BOGOF</th>");
            sb.Append("<th style='background-color: #B8DBFD;border: 1px solid #ccc'>Price</th>");
            sb.Append("<th style='background-color: #B8DBFD;border: 1px solid #ccc'>Discount Price </th>");
            sb.Append("<th style='background-color: #B8DBFD;border: 1px solid #ccc'>Subtotal</th>");
            sb.Append("</tr>");

            //Add to table for each product in order.Orderlines
            foreach (var orderLine in order.OrderLines)
            {
                
                sb.Append("<tr>");
                sb.Append("<td style='border: 1px solid #ccc'>" + orderLine.Product.ProductName + "</td>");
                sb.Append("<td style='border: 1px solid #ccc'>" + orderLine.Quantity + "</td>");
                //Only display if discount is applied
                if(orderLine.Product.IsBOGOF && orderLine.Quantity > 1)
                {
                    sb.Append("<td style='border: 1px solid #ccc'>" + "Discount Applied" + "</td>");
                }
                //Else, N/A
                else
                {
                    sb.Append("<td style='border: 1px solid #ccc'>" + "N/A" + "</td>");
                }
                sb.Append("<td style='border: 1px solid #ccc'>" + "£" + orderLine.Product.ProductPrice + "</td>");
                //Display discount if applicable
                if (orderLine.Product.IsDiscounted)
                {
                    string discountPrice = Math.Round((orderLine.Product.ProductDiscount * orderLine.Product.ProductPrice), 2).ToString();
                    sb.Append("<td style='border: 1px solid #ccc'>" + "- £" + discountPrice + "</td>"); 
                }
                else
                {
                    //Else display £0
                    sb.Append("<td style='border: 1px solid #ccc'>" + "£" + 0.00m + "</td>");
                }  
                sb.Append("<td style='border: 1px solid #ccc'>" + "£" + orderLine.LineTotal + "</td>");
                sb.Append("</tr>");
            }


            sb.Append("</table>");

            //Convert the HTML string to PDF
            using (MemoryStream stream = new MemoryStream())
            {
                StringReader sr = new StringReader(sb.ToString());
                Document pdfDoc = new Document(PageSize.A4, 10f, 10f, 5f, 10f);
                PdfWriter writer = PdfWriter.GetInstance(pdfDoc, stream);
                pdfDoc.Open();
                XMLWorkerHelper.GetInstance().ParseXHtml(writer, pdfDoc, sr);
                pdfDoc.Close();

                //Create file name to ensure all are unique
                string fileName = user.FirstName + user.LastName + order.OrderId;

                //Return the PDF file
                return File(stream.ToArray(), "application/pdf", fileName+".pdf");
            }
        }

    }
}
