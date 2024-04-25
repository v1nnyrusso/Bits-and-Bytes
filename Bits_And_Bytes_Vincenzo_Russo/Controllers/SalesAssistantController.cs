using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Bits_And_Bytes_Vincenzo_Russo.Models;
using Bits_And_Bytes_Vincenzo_Russo.Models.ViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Bits_And_Bytes_Vincenzo_Russo.Controllers
{
    public class SalesAssistantController : AccountController
    {

        BitsBytesDbContext db = new BitsBytesDbContext();

        // GET: SalesAssistant
        public ActionResult Index()
        {

            //To get all the users, we will create a new var called 'users', and order them by registration date
            var users = db.Users.OrderBy(u => u.RegisteredAt).ToList();

            //Now we can filter the users by role in memory
            var customers = users.OfType<Customer>().ToList();

            //Now that we have all the customers, we can send the 'customers' var to the Index view, where we will display the information
            return View(customers);
        }


        // GET: SalesAssistant/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customer customer = (Customer)db.Users.Find(id);
            if (customer == null)
            {
                return HttpNotFound();
            }
            return View(customer);
        }

        // GET: SalesAssistant/Create
        public ActionResult Create()
        {
            return View();
        }

        //Using a HttpGet tag to return the empty view form where we will create the customer
        [HttpGet]
        [Authorize(Roles = "Sales Assistant, Manager")]
        public ActionResult CreateCustomer()
        {
            //We need an instance of CreateCustomerViewModel to assign 
            CreateCustomerViewModel customer = new CreateCustomerViewModel();

            //Assign the customer model to the view
            return View(customer);
        }

        //Using a HttpPost tag to write new customer
        [HttpPost]
        //We are passing the model from the CreateCustomer view (input data)
        //We will use this model to assign it as a new customer
        public ActionResult CreateCustomer(CreateCustomerViewModel model)
        {
            //If the model is not null, aka, if somehow, the program got to this point where the model had
            //Zero input data, >>>>
            if (ModelState.IsValid)
            {
               
                //Build the customer by creating a new instance of Customer called newCustomer
                Customer newCustomer = new Customer
                {
                    UserName = model.EmailAddress,
                    Email = model.EmailAddress,
                    EmailConfirmed = true,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    DOB = (DateTime)model.DOB,
                    Address = model.Address,
                    Country = model.Country,
                    Town = model.Town,
                    PostCode = model.PostCode,
                    PhoneNumber = model.TelephoneNumber,
                    //Default false value upon sign up
                    IsMember = false,
                    //Record the date and time from when the user registered
                    RegisteredAt = DateTime.Now,
                    //Default false value upon sign up
                    IsSuspended = false,
                    OrderCount = model.OrderCount,
                    
                };
                //Make customer a member if SA assigns order count to be over 0
                if (model.OrderCount > 0)
                {
                    newCustomer.IsMember = true;
                }
                try
                {
                  

                    //Create Customer, and store in the database and pass the password in to be hashed
                    //For security
                    var result = UserManager.Create(newCustomer, model.Password);
                    //If the user is stored in the database successfully >>>>
                    if (result.Succeeded)
                    {
                        //Then, we add the user to the role 
                        UserManager.AddToRole(newCustomer.Id, "Customer");
                        db.SaveChanges();
                    }

                }
                catch (DbEntityValidationException ex)
                {
                    foreach (var entityValidationErrors in ex.EntityValidationErrors)
                    {
                        foreach (var validationError in entityValidationErrors.ValidationErrors)
                        {
                            Console.WriteLine("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
                        }
                    }
                }      
                if(User.IsInRole("Manager"))
                {
                    return RedirectToAction("Index", "Manager");
                }
                else
                {
                    return RedirectToAction("Index", "SalesAssistant");
                }

                
                
            }

            //If something goes awry, go back to the createcustomer view
            return View(model);
        }


        [HttpGet]
        public ActionResult EditCustomer(string id)
        {
            //If the id is null, throw an exception
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            //Find user by id and return it as a Customer
            Customer customer = db.Users.Find(id) as Customer;

            //If the customer is for some reason null, throw a HttpNotFound error
            if (customer == null)
            {
                return HttpNotFound();
            }

            //Send the customer's details to the ViewModel
            return View(new EditCustomerViewModel
            {
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                Town = customer.Town,
                Address = customer.Address,
                PostCode = customer.PostCode,
                Country = customer.Country,
                DOB = customer.DOB,
                TelephoneNumber = customer.PhoneNumber,
                EmailAddress = customer.Email,
                IsMember = customer.IsMember,
                IsSuspended = customer.IsSuspended,
                OrderCount = customer.OrderCount
            });

        }

        // POST: SalesAssistant/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditCustomer(string id,
            [Bind(Include = "FirstName, LastName,DOB, Address, Town, PostCode, Country, TelephoneNumber," +
            " EmailAddress, OrderCount, IsMember, IsSuspended")] EditCustomerViewModel model)
        {
            if (ModelState.IsValid)
            {
                //Find the User by Id, then cast it as a Customer
                Customer customer = (Customer)await UserManager.FindByIdAsync(id);

                //Update the new customer details by using the model as a reference
                UpdateModel(customer);

                //Update thew new customer details into the database using UpdateAsync method
                IdentityResult result = await UserManager.UpdateAsync(customer);

                //If the result succeeds >>>
                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "SalesAssistant");
                }
            }

            return View(model);

        }


        public ActionResult DeleteUser(string id)
        {
            //If the id is null, throw an exception
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            //Check to make sure we are not deleting our own account
            if (id == User.Identity.GetUserId())
            {
                return RedirectToAction("Index", "Manager");
            }

            //Make sure foreign key relationships' data are deleted too
            foreach (var card in db.Cards)
            {
                if (card.CustomerId == id)
                {
                    db.Cards.Remove(card);
                }
            }

            foreach (var order in db.Orders)
            {
                if (order.CustomerId == id)
                {
                    db.Orders.Remove(order);
                }
            }

            //Collecting user by id
            User user = db.Users.SingleOrDefault(p => p.Id == id); //Getting the user by ID

            //If the user doesn't exist

            if (user == null)
            {
                return HttpNotFound();
            }

            //Delete user
            db.Users.Remove(user);

            //Save changes
            db.SaveChanges();

            //Create a string message that will be passed to viewbag
            string message = "Successfully deleted user!";

            ViewBag.Message = message;

            //Redirect back to Manager Home
            return RedirectToAction("Index", "SalesAssistant");

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
