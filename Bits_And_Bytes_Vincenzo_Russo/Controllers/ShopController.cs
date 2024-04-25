using Bits_And_Bytes_Vincenzo_Russo.Models;
using Bits_And_Bytes_Vincenzo_Russo.Models.ViewModels;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Services.Description;
using System.Web.UI.WebControls;

namespace Bits_And_Bytes_Vincenzo_Russo.Controllers
{
    //Users have to be logged in to access this controller
    [Authorize]
    public class ShopController : Controller
    {
        private BitsBytesDbContext context = new BitsBytesDbContext();

    

        // GET: Shop
        public ActionResult Index()
        {
            //getting all the products that are not discontinued
            var products = context.Products.Where(p => p.InStock == true).ToList();

            foreach (var product in products)
            {
                context.Entry(product).Reload(); //Refresh entities
            }

            //Send all the categories in a viewbag
            ViewBag.Categories = context.Categories.ToList();

            //Send the products to the products view
            return View("Products", products);

        }

        public ActionResult ComputerParts()
        {
            //Find the category
            var computerParts = context.Categories.Where(c => c.Name == "Computer Parts").SingleOrDefault();

            //Getting all the products that are not discontinued and are in a specific category
            var products = context.Products.Where(p => p.InStock != false).Where(p => p.CategoryId == computerParts.CategoryId).ToList();

            //And send all categories to the ViewBag
            //Also send all categories to the viewbag
            ViewBag.Categories = context.Categories.ToList();

            return View("Products", products);
        }

        public ActionResult ComputerSystems()
        {
            //Find the category
            var computerSystems = context.Categories.Where(c => c.Name == "Computer Systems").SingleOrDefault();

            //Getting all the products that are not discontinued and are in a specific category
            var products = context.Products.Where(p => p.InStock != false).Where(p => p.CategoryId == computerSystems.CategoryId).ToList();

            //And send all categories to the ViewBag
            //Also send all categories to the viewbag
            ViewBag.Categories = context.Categories.ToList();

            return View("Products", products);


        }
        [Authorize(Roles = "Customer")]
        public ActionResult CartIndex()
        {
            //If the cart session is null, return view
            if (Session["cart"] == null)
            {
                return View();
            }
            else
            {
                //Instantiate cart
                List<Cart> cart = (List<Cart>)Session["cart"];

                ViewBag.Total = CartTotal();
               
                //Send cart and display view
                return View(cart);
            }
            
        }


        //***************************************DISPLAYING PRODUCTS***********************************

        public ActionResult Products(int? id)
        {
            //Getting all the products that are not discontinued and are in a specific category
            var products = context.Products.Where(p => p.InStock == true).Where(p => p.CategoryId == id).ToList();

            //Also send all categories to the viewbag
            ViewBag.Categories = context.Categories.ToList();

            return View("Products", products);
        }

        public ActionResult Product(int? id)
        {
            //If id is null return a bad request error
            if (id == null)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);

            }

            //Find product by id
            Product product = context.Products.Find(id);

            //If product is not in the database, return not found error
            if (product == null)
            {
                return HttpNotFound();
            }

            // Send all the catagories in a viewbag
            ViewBag.Categories = context.Categories.ToList();


            if (product.IsDiscounted)
            {
                //Get the price before the discount is added
                decimal originalPrice = product.ProductPrice;

                //Send this in a viewbag to display to customers that the product is on sale
                ViewBag.OriginalPrice = originalPrice;


                // Make sure number doesn't round to the nearest whole number
                product.ProductPrice = product.ProductPrice - Math.Round(product.ProductPrice * product.ProductDiscount, 2);

                // If the product is on sale, pass the discount to the view to display to customer the percentage off
                product.ProductDiscount = product.ProductDiscount * 100;

                //Return the product
                return View(product);

            }

            //Otherwise, send the product to the view
            return View("Product", product);
        }


        [Authorize(Roles = "Customer")]
        public ActionResult AddToCart(int ProductId)
        {
            //Checking if this is the first instance of adding to the cart
            if (Session["cart"] == null)
            {
                var product = context.Products.FirstOrDefault(p => p.ProductId == ProductId);

                //Only add to basket if product is in stock
                if(product.ProductStock >= 1)
                {
                    List<Cart> cartItem = new List<Cart>();

                    //Create a new cart object, making the quantity 1, as well as finding the product by id
                    Cart cartObj = new Cart
                    {
                        Product = product,
                        Count = 1
                    };

                    //Detect if inproper item is up for sale, don't let customer add it to basket
                    if (cartObj.Product.ProductPrice < 0.00m)
                    {
                        return View("Index");
                    }
                    else
                    {
                        //Add the cartobj to cartItem list
                        cartItem.Add(cartObj);


                        if (cartObj.Product.IsDiscounted)
                        {
                            ViewBag.Discount = cartObj.Product.ProductPrice - Math.Round((cartObj.Product.ProductPrice * cartObj.Product.ProductDiscount), 2);
                        }


                        //Creating a new session to = cartItem list
                        Session["cart"] = cartItem;
                    }
                }
                //Return an error if product is not in stock
                else
                {
                    return RedirectToAction("CartIndex", ViewBag.StockError = "This product is not in stock.");
                }      
              
            }
            //Else, if a product is already in the cart >>>
            else
            {

                //Collect existing cart, cast it as a list since it is an object
                List<Cart> cart = (List<Cart>)Session["cart"];

                
                int index = IsInCart(ProductId);

                //If the retrieved index is not -1, increment the index
                if (index != -1)
                {
                    cart[index].Count++;     
                }
                else
                {
                    //Else, add the new product, and increase count by 1
                    cart.Add(new Cart()
                    {
                        Product = context.Products.Find(ProductId),
                        Count = 1
                    });
                }

                
                //Save cart session
                Session["cart"] = cart;

                
            }

            //Redirect to cart index
            return RedirectToAction("CartIndex");
        }

        //Authorize only customers to access this action
        [Authorize(Roles = "Customer")]
        public ActionResult RemoveFromCart(int ProductId)
        {
            //Retrieve the cart from the session
            List<Cart> cart = (List<Cart>)Session["cart"];

            //Check if the product is in the cart
            int index = IsInCart(ProductId);

            //Remove the product from the cart
            cart.RemoveAt(index);

            //Update the cart in the session
            Session["cart"] = cart;

            //Redirect the user to the CartIndex action
            return RedirectToAction("CartIndex");
        }

        [Authorize(Roles = "Customer")]
        public ActionResult DecreaseByOne(int ProductId)
        {
            // Retrieve the cart from the session
            List<Cart> cart = (List<Cart>)Session["cart"];

            // Check if the product is in the cart
            int index = IsInCart(ProductId);

            if (index != -1)
            {
                // Decrease the quantity by 1
                if (cart[index].Count > 1)
                {
                    cart[index].Count--;
                }
                else
                {
                    // If the quantity becomes 0, remove the product from the cart
                    cart.RemoveAt(index);
                }
            }

            // Update the cart in the session
            Session["cart"] = cart;

            // Redirect the user to the CartIndex action
            return RedirectToAction("CartIndex");
        }



        public int IsInCart(int ProductId)
        {
            //Retrieve the cart from the session
            List<Cart> cart = (List<Cart>)Session["cart"];

            //Loop through the cart to find the product
            for (int i = 0; i < cart.Count; i++)
            {
                
                //If the product is found, return its index
                if (cart[i].Product.ProductId == ProductId)
                {
                    return i;
                    
                }
            }

            //If the product is not found, return -1
            return -1;
        }


        public decimal CalculateSubtotal()
        {
            //Instance of cart session
            List<Cart> cart = (List<Cart>)Session["cart"];

            //Get the current user's information
            var userName = User.Identity.Name;
            var user = context.Users.FirstOrDefault(u => u.UserName == userName);
            var userId = user.Id;

            //Total starting values
            decimal total = 0;

            //Foreach item in cart
            foreach (Cart item in cart)
            {
                //Get total of item price * count of item
                decimal sumOfProducts = item.Product.ProductPrice * item.Count;

                //If the product has a discount, the total = the sum of the products in the basket - (price  * discount)
                if (item.Product.IsDiscounted)
                {

                    total += sumOfProducts - (sumOfProducts * item.Product.ProductDiscount);

                }
                //If the product is BOGOF, half the price if the count is even
                else if (item.Product.IsBOGOF && item.Count % 2 == 0)
                {
                    total += sumOfProducts / 2;
                }
                //Else, if the product is BOGOF and count is an odd number, find the even number and
                //only add the price of that calculation to total     
                else if (item.Product.IsBOGOF && item.Count % 2 != 0)
                {
                    //int oddCountForCalculation = count of item cast as a decimal
                    //so we can divide it by 2 and keep the .5 float point number
                    //round up to nearest int, cast as int
                    int oddCountForCalculation = (int)Math.Ceiling((decimal)item.Count / 2);

                    //Add to total the even count * product price
                    total += (oddCountForCalculation * item.Product.ProductPrice);
                }
                // Else total is as normal
                else
                {
                    total += sumOfProducts;
                }

            }

            return Math.Round(total, 2);
        }


        public decimal CartTotal()
        {
            //Instance of cart session
            List<Cart> cart = (List<Cart>)Session["cart"];

            // Get the current user's information
            var userName = User.Identity.Name;
            var user = context.Users.FirstOrDefault(u => u.UserName == userName) as Customer;
            var userId = user.Id;

            //Total starting values
            decimal total = 0;
            decimal totalBeforeVat = 0;
            decimal vat = 0.20m;
            decimal membershipdiscount = 0;

            //Foreach item in cart
            foreach (Cart item in cart)
            {                 
                //Get total of item price * count of item
                decimal sumOfProducts = item.Product.ProductPrice * item.Count;
              
                //If the product has a discount, the total = the sum of the products in the basket - (price  * discount)
                if (item.Product.IsDiscounted)
                {                 

                    total += sumOfProducts - (sumOfProducts * item.Product.ProductDiscount);                 
                    
                }
                //If the product is BOGOF, half the price if the count is even
                else if(item.Product.IsBOGOF && item.Count % 2 == 0)
                {
                    total += sumOfProducts / 2;
                }
                //Else, if the product is BOGOF and count is an odd number, find the even number and
                //only add the price of that calculation to total     
                else if(item.Product.IsBOGOF && item.Count % 2 != 0)
                {
                    //int oddCountForCalculation = count of item cast as a decimal
                    //so we can divide it by 2 and keep the .5 float point number
                    //round up to nearest int, cast as int
                    int oddCountForCalculation = (int)Math.Ceiling((decimal)item.Count / 2);

                    //Add to total the even count * product price
                    total += (oddCountForCalculation * item.Product.ProductPrice);                 
                }
                // Else total is as normal
                else
                {
                    total += sumOfProducts;      
                }

            }
            //If user's order count is less than 30, only apply vat
            if(user.OrderCount <30)
            {
              
                totalBeforeVat = total;

                membershipdiscount = totalBeforeVat * 0.05m;

                total = totalBeforeVat + (totalBeforeVat * vat);
            }
            //If customer's order count is more than 30, but less than 60, apply vat and 5% discount
            else if (user.OrderCount >=30 && user.OrderCount <60)
            {              

                totalBeforeVat = total - (total * 0.05m);

                membershipdiscount = totalBeforeVat * 0.010m;

                total = totalBeforeVat + (totalBeforeVat * vat);
       
            }
            //If customer's order count is more than 60, but less than 90, apply vat and 10% discount
            else if (user.OrderCount >= 60 && user.OrderCount < 90)
            {

                totalBeforeVat = total - (total * 0.10m);

                membershipdiscount = totalBeforeVat * 0.05m;

                total = totalBeforeVat + (totalBeforeVat * vat);

            }
            //Else, customer's order count is >90, apply 10% discount, no vat
            else
            {
                total -= (total * 0.10m);

                membershipdiscount = 0m;

            }

            //Return the total, only rounding to two decimal places at the end to save float point accuracy
            return Math.Round(total, 2);
        }

        private decimal CalculateMembershipDiscount()
        {
            //Instance of cart session
            List<Cart> cart = (List<Cart>)Session["cart"];

            // Get the current user's information
            var userName = User.Identity.Name;
            var user = context.Users.FirstOrDefault(u => u.UserName == userName) as Customer;
            var userId = user.Id;

            //Total starting values
            decimal total = 0;
            decimal membershipdiscount = 0;

            //Foreach item in cart
            foreach (Cart item in cart)
            {
                //Get total of item price * count of item
                decimal sumOfProducts = item.Product.ProductPrice * item.Count;

                //If the product has a discount, the total = the sum of the products in the basket - (price  * discount)
                if (item.Product.IsDiscounted)
                {

                    total += sumOfProducts - (sumOfProducts * item.Product.ProductDiscount);

                }
                //If the product is BOGOF, half the price if the count is even
                else if (item.Product.IsBOGOF && item.Count % 2 == 0)
                {
                    total += sumOfProducts / 2;
                }
                //Else, if the product is BOGOF and count is an odd number, find the even number and
                //only add the price of that calculation to total     
                else if (item.Product.IsBOGOF && item.Count % 2 != 0)
                {
                    //int oddCountForCalculation = count of item cast as a decimal
                    //so we can divide it by 2 and keep the .5 float point number
                    //round up to nearest int, cast as int
                    int oddCountForCalculation = (int)Math.Ceiling((decimal)item.Count / 2);

                    //Add to total the even count * product price
                    total += (oddCountForCalculation * item.Product.ProductPrice);
                }
                // Else total is as normal
                else
                {
                    total += sumOfProducts;
                }

            }
            //If user's order count is less than 30, only apply vat
            if (user.OrderCount < 30)
            {


                membershipdiscount = 0m;


            }
            //If customer's order count is more than 30, but less than 60, apply vat and 5% discount
            else if (user.OrderCount >= 30 && user.OrderCount < 60)
            {

            

                membershipdiscount = total * 0.05m;


            }
            //If customer's order count is more than 60, but less than 90, apply vat and 10% discount
            else if (user.OrderCount >= 60 && user.OrderCount < 90)
            {


                membershipdiscount = total * 0.10m;


            }
            //Else, customer's order count is >90, apply 10% discount, no vat
            else
            {

                membershipdiscount = total * 0.10m;

            }

            //Return the total, only rounding to two decimal places at the end to save float point accuracy
            return membershipdiscount;
        }

        //Calculate VAT Price for an Order
        private decimal CalculateVatPrice()
        {
            // Get the current user's information
            var userName = User.Identity.Name;
            var user = context.Users.FirstOrDefault(u => u.UserName == userName) as Customer;
            var userId = user.Id;

            //Total starting values
            decimal vatRate = 0;
            decimal totalBeforeVat = 0;
            decimal vat = 0.20m;

            //Instance of cart session
            List<Cart> cart = (List<Cart>)Session["cart"];

            //Foreach item in cart
            foreach (Cart item in cart)
            {
                //Get total of item price * count of item
                decimal sumOfProducts = item.Product.ProductPrice * item.Count;

                //If the product has a discount, the total = the sum of the products in the basket - (price  * discount)
                if (item.Product.IsDiscounted)
                {
                    vatRate += sumOfProducts - (sumOfProducts * item.Product.ProductDiscount);
                }
                //If the product is BOGOF, half the price if the count is even
                else if (item.Product.IsBOGOF && item.Count % 2 == 0)
                {
                    vatRate += sumOfProducts / 2;
                }
                //Else, if the product is BOGOF and count is an odd number, find the even number and
                //only add the price of that calculation to total     
                else if (item.Product.IsBOGOF && item.Count % 2 != 0)
                {
                    //int oddCountForCalculation = count of item cast as a decimal
                    //so we can divide it by 2 and keep the .5 float point number
                    //round up to nearest int, cast as int
                    int oddCountForCalculation = (int)Math.Ceiling((decimal)item.Count / 2);

                    //Add to total the even count * product price
                    vatRate += (oddCountForCalculation * item.Product.ProductPrice);
                }
                // Else total is as normal
                else
                {
                    vatRate += sumOfProducts;
                }
            }

            //If user's order count is less than 30, only apply vat
            if (user.OrderCount < 30)
            {
                totalBeforeVat = vatRate;
                vatRate = totalBeforeVat * vat;
            }
            //If customer's order count is more than 30, but less than 60, apply vat and 5% discount
            else if (user.OrderCount >= 30 && user.OrderCount < 60)
            {
                totalBeforeVat = vatRate - (vatRate * 0.05m);
                vatRate = totalBeforeVat * vat;
            }
            //If customer's order count is more than 60, but less than 90, apply vat and 10% discount
            else if (user.OrderCount >= 60 && user.OrderCount < 90)
            {
                totalBeforeVat = vatRate - (vatRate * 0.10m);
                vatRate = totalBeforeVat * vat;
            }
            //Else, customer's order count is >90, apply 10% discount, no vat
            else
            {
                vatRate = 0m;
            }

            return vatRate;
        }



        [Authorize(Roles = "Customer")]
        public ActionResult Checkout()
        {
            //Check if the cart is empty
            if (Session["cart"] == null)
            {
                return RedirectToAction("CartIndex");
            }

            //Get the current user's information
            var userName = User.Identity.Name;
            var user = context.Users.FirstOrDefault(u => u.UserName == userName) as Customer;
            //Get the cart from the session
            var cart = (List<Cart>)Session["cart"];


             //Collect if user pays vat for view
             if(user.OrderCount >=90)
             {
                  ViewBag.IsVat = false;
             }
             else
             {
                ViewBag.IsVat = true;
             }

            ViewBag.Total = CartTotal();
      
            //Create a new instance of the CheckoutViewModel
            var viewModel = new CheckoutViewModel();
           
            //Checking if the user is a member, so the viewmodel ismember attribute to check in view works correctly
            if (!user.IsMember)
            {
                viewModel.IsMember = false;
            }
            else
            {
                viewModel.IsMember = true;
            }

            return View(viewModel);
        }

        [HttpPost]
        [Authorize(Roles = "Customer")]
        public ActionResult PlaceOrder(CheckoutViewModel viewModel)
        {
            // Check if the model is invalid
            if (!ModelState.IsValid)
            {
                return View("Checkout", viewModel);
            }

            //If there is no current payment active, create one
            if (viewModel.Payment == null)
            {
                viewModel.Payment = new Payment();
            }

            // Update stock levels
            var cart = (List<Cart>)Session["cart"];
            foreach (var item in cart)
            {
                var product = context.Products.SingleOrDefault(p => p.ProductId == item.Product.ProductId);
                if (product != null)
                {
                    product.ProductStock -= item.Count;

                    // Set stock to not in stock if it reaches 0
                    if (product.ProductStock <= 0)
                    {
                        product.InStock = false;
                    }
                }
                else
                {
                    throw new Exception("Product not found.");
                }
            }

            //Get the current user's information
            var customer = context.Users.FirstOrDefault(u => u.UserName == User.Identity.Name) as Customer;

            //Increment order count for members
            if (customer.IsMember && User.IsInRole("Customer"))
            {
                customer.OrderCount += 1;
            }
            else
            {
                // et order count to 1 for non-members
                customer.IsMember = viewModel.IsMember;
                customer.OrderCount = 1;
            }


            //Determine if VAT should be added, if order count is less than or equal to 90 - addvat is true
            bool addVat = customer.OrderCount <= 90;
         

            //Create a new order object
            var order = new Order
            {
                OrderDate = DateTime.Now,
                OrderTotal = CartTotal(),
                IsPaid = true,
                OrderStatus = OrderStatus.Placed,
                AddVat = addVat,
                CustomerId = customer.Id,
                Subtotal = CalculateSubtotal(),
                VatAmount = Math.Round(CalculateVatPrice(), 2),
                MembershipDiscount = Math.Round(CalculateMembershipDiscount(), 2),
                DeliveryAddress = viewModel.Address,
                DeliveryCountry = viewModel.Country,
                DeliveryPostCode = viewModel.PostCode,
                DeliveryTown = viewModel.Town
                
            };

            //If the customer's name was null, i.e, external login, forms appear on checkout to collect it
            //If the viewModel customer is not null, assign the input data to the customer in database
            if(viewModel.Customer != null)
            {
                customer.FirstName = viewModel.Customer.FirstName;
                customer.LastName = viewModel.Customer.LastName;
                customer.DOB = viewModel.Customer.DOB;
                customer.PhoneNumber = viewModel.Customer.PhoneNumber;
                customer.Address = viewModel.Address;
                customer.Town = viewModel.Town;
                customer.PostCode = viewModel.PostCode;
                customer.Country = viewModel.Country;
                
            }

            //Add order lines to the database
            foreach (var item in cart)
            {
                var product = context.Products.SingleOrDefault(p => p.ProductId == item.Product.ProductId);
                if (product != null)
                {
                    var orderLine = new OrderLine
                    {
                        OrderId = order.OrderId,
                        ProductId = item.Product.ProductId,
                        Quantity = item.Count,
                        Price = item.Product.ProductPrice - Math.Round(item.Product.ProductPrice * item.Product.ProductDiscount, 2),
                        Product = product
                    };

                    decimal productPrice = item.Product.ProductPrice;

                    //If the product is discounted, apply the appropriate discount
                    if (item.Product.IsDiscounted)
                    {
                        decimal discountedPrice = productPrice - (productPrice * item.Product.ProductDiscount);
                        orderLine.LineTotal = discountedPrice * item.Count;
                    }
                    //New way to find bogof
                    else if (item.Product.IsBOGOF)
                    {
                        //If the count is even, half the price
                        if (item.Count % 2 == 0)
                        {
                            decimal halfPrice = productPrice / 2;
                            orderLine.LineTotal = halfPrice * item.Count;
                        }
                        else
                        {
                            //Else, calculate the total based on the BOGOF rules
                            int evenCount = item.Count - 1;
                            decimal evenTotal = (evenCount / 2) * productPrice;
                            orderLine.LineTotal = evenTotal + productPrice;
                        }
                    }
                    else
                    {
                        //Else, total is as normal
                        orderLine.LineTotal = productPrice * item.Count;
                    }

                    order.OrderLines.Add(orderLine);
                    context.OrderLines.Add(orderLine);
                }
                else
                {
                    throw new Exception("Product not found.");
                }
            }

            

            //Save card and payment information
            viewModel.Card.CustomerId = User.Identity.GetUserId();
            //Don't collect security codelkm
            viewModel.Card.Cvv2 = null;
            //Only collect the last four digits of the card number
            string cardLastFourDigits = "**** **** **** " + viewModel.Card.CardNumber.Substring(viewModel.Card.CardNumber.Length - 4);
            viewModel.Card.CardNumber = cardLastFourDigits;
            context.Cards.Add(viewModel.Card);

            viewModel.Payment.IsSuccessful = true;
            viewModel.Payment.PaymentTotal = order.OrderTotal;
            viewModel.Payment.PaymentDate = DateTime.Now;
            viewModel.Payment.PaymentId = order.OrderId;
            order.Payment = viewModel.Payment;
            context.Payments.Add(viewModel.Payment);

            //Add order to orders db
            context.Orders.Add(order);

            //Final save changes
            context.SaveChanges();

            //Clear the cart
            Session["cart"] = null;

            //Redirect to the order confirmation view, pass the order
            return View("Confirmation", order);
        }



    }
}