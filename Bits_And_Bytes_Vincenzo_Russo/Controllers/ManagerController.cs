using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.UI.WebControls;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Bits_And_Bytes_Vincenzo_Russo.Models;
using Bits_And_Bytes_Vincenzo_Russo.Models.ViewModels;
using Microsoft.Ajax.Utilities;
using System.Web.Configuration;
using System.Runtime.Remoting.Contexts;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace Bits_And_Bytes_Vincenzo_Russo.Controllers
{

    //New manager controller that inherits from AccountController, so that it has
    //Access to AccountController methods, like Register(), etc.

    //Ensuring that only a Manager can have access to this entire controller
    [Authorize(Roles = "Manager")]
    public class ManagerController : AccountController
    {

        //Create an instance of BitsBytesDbContext, so that we can use it to manipulate the database
        BitsBytesDbContext db = new BitsBytesDbContext();

        // GET: Manager
        //I'm going to create a Manager area, where Manager's can CRUD customers/sales assistants and their own information
        public ActionResult Index()
        {

            //To get all the users, we will create a new var called 'users', and order them by registration date
            var users = db.Users.OrderBy(u => u.RegisteredAt).ToList();

            //Now that we have all the users, we can send the 'users' var to the Index view, where we will display the information
            return View(users);
        }

        //*********************************CREATING STAFF**********************************
        //*********************************************************************************

        //Using a HttpGet tag to return the empty view form where we will create the staff member
        [HttpGet]
        public ActionResult CreateStaff()
        {
            //We need an instance of csv to assign 
            CreateStaffViewModel staff = new CreateStaffViewModel();

            //Getting the roles from the database that match "Manager" and "Sales Assistant" and store them as a selectedlistitem
            //So the roles can be displayed in a dropdown list
            var roles = db.Roles.Where(r => r.Name == "Manager" || r.Name == "Sales Assistant").OrderBy(r => r.Id).Select(r => new SelectListItem
            {
                Text = r.Name,
                Value = r.Name
            }).ToList();

            //Assign the roles to the staff roles property
            staff.Roles = roles;

            //Send the staff model to the view
            return View(staff);
        }

        //Using a HttpPost tag to write new staff member
        [HttpPost]
        //We are passing the model from the CreateStaff view (Managerial input data)
        //We will use this model to assign it as a new staff member
        public ActionResult CreateStaff(CreateStaffViewModel model)
        {
            //If the model is not null, aka, if somehow, the program got to this point where the model had
            //Zero input data, >>>>
            if (ModelState.IsValid)
            {
                //Build the staff member using by creating a new instance of Staff called newStaff
                Staff newStaff = new Staff
                {
                    UserName = model.EmailAddress,
                    Email = model.EmailAddress,
                    EmailConfirmed = true,
                    Address = model.Address,
                    Town = model.Town,
                    PostCode = model.PostCode,
                    Country = model.Country,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    DOB = (DateTime)model.DOB,
                    PhoneNumber = model.TelephoneNumber,
                    PhoneNumberConfirmed = true,
                    IsSuspended = false,
                    RegisteredAt = DateTime.Now,
                    //Make the staffposition enum value = the string value of role
                    StaffPosition = model.Role.StartsWith("Sales") ? StaffPosition.Sales_Assistant : (StaffPosition)Enum.Parse(typeof(StaffPosition), model.Role)
                };

                //Create Staff member, and store in the database and pass the password in to be hashed
                //For security
                var result = UserManager.Create(newStaff, model.Password);
                //If the user is stored in the database successfully >>>>
                if (result.Succeeded)
                {
                    //Then, we add the user to the role selected in the viewmodel
                    UserManager.AddToRole(newStaff.Id, model.Role);
                    return RedirectToAction("Index", "Manager");
                }
            }

            // If the model state is invalid, we need to populate the Roles property again to prevent crashes
            var roles = db.Roles.Where(r => r.Name == "Manager" || r.Name == "Sales Assistant").OrderBy(r => r.Id).Select(r => new SelectListItem
            {
                Text = r.Name,
                Value = r.Name
            }).ToList();

            model.Roles = roles;
            //If something goes awry, go back to the createestaff view
            return View(model);
        }

        //*********************************END OF CREATING STAFF**********************************
        //****************************************************************************************


        //*********************************EDIT STAFF*********************************************
        //****************************************************************************************

        //HttpGet to collect the staff member to be edited
        //The parameter string id is passed through from the Index view of the corresponding staffmember's id
        [HttpGet]
        public ActionResult EditStaff(string id)
        {
            //If the id is null, throw an exception
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            //Find the Staff member in the database by Id, cast the User as Staff
            Staff staff = db.Users.Find(id) as Staff;

            //If the staff is for some reason null, throw a HttpNotFound error
            if (staff == null)
            {
                return HttpNotFound();
            }

            //Send said Staff member's details to the EditStaffViewModel
            return View(new EditStaffViewModel
            {
                FirstName = staff.FirstName,
                LastName = staff.LastName,
                DOB = staff.DOB,
                Address = staff.Address,
                Town = staff.Town,
                PostCode = staff.PostCode,
                Country = staff.Country,
                TelephoneNumber = staff.PhoneNumber,
                EmailAddress = staff.Email

            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        //Use the bind attribute to combine all attributes into the model passed from EditStaffViewModel
        public async Task<ActionResult> EditStaff(string id,
            [Bind(Include = "FirstName, LastName, DOB, Address, Town, PostCode, Country, TelephoneNumber, EmailAddress")] EditStaffViewModel model)
        {
            if (ModelState.IsValid)
            {
                //Find the User by Id, then cast it as a Staff member
                Staff staff = (Staff)await UserManager.FindByIdAsync(id);

                //Update the new staff details by using the model as a reference
                UpdateModel(staff);

                //Update thew new staff details into the database using UpdateAsync method
                IdentityResult result = await UserManager.UpdateAsync(staff);

                //If the result succeeds >>>
                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Manager");
                }
            }

            return View(model);
        }

        //*********************************END OF EDITING STAFF*********************************************
        //**************************************************************************************************


        //*********************************EDIT CUSTOMER****************************************************
        //**************************************************************************************************

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
                    return RedirectToAction("Index", "Manager");
                }
            }

            return View(model);

        }

        //*********************************END OF EDITING CUSTOMER*********************************************
        //*****************************************************************************************************

        //*********************************DELETE/VIEW USERS****************************************************
        //******************************************************************************************************

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

            //Remove associated cards that match the id of the user
            foreach(var card in db.Cards)
            {
                if(card.CustomerId == id)
                {
                    db.Cards.Remove(card);
                }
            }
            
            //Remove associated ordr
            foreach (var order in db.Orders)
            {
                if (order.CustomerId == id)
                {
                    db.Orders.Remove(order);
                }
            }

         

            //Collecting user by id
            User user = db.Users.SingleOrDefault(p => p.Id == id); 
            //Getting the user by ID

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
            return RedirectToAction("Index", "Manager");

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

        //Details Method that takes in string id parameter of userid from Index
        [HttpGet]
        public ActionResult Details(string id)
        {
            //If the id is null, throw an exception
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            //Collecting user by id
            User user = db.Users.Find(id);

            //If user is null, throw httpnotfound
            if (user == null)
            {
                return HttpNotFound();
            }

            //If the user is a Staff member >>>
            if (user is Staff)
            {
                return View("DetailsStaff", (Staff)user);
            }

            //If the user is a Customer >>>
            if (user is Customer)
            {
                return View("DetailsCustomer", (Customer)user);
            }

            return HttpNotFound();
        }

        //*********************************END OF DELETE/VIEW USERS****************************************************
        //******************************************************************************************************

        //*********************************CHANGE ROLE****************************************************
        //******************************************************************************************************

        [HttpGet]
        public async Task<ActionResult> ChangeRole(string id)
        {

            //If id is null throw exception
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //Can't change your own role:
            if (id == User.Identity.GetUserId())
            {
                return RedirectToAction("Index", "Manager");
            }

            //Get user by ID
            User user = await UserManager.FindByIdAsync(id);

            //Get user's current role
            //Only ever a single role
            string oldRole = (await UserManager.GetRolesAsync(id)).Single();

            //Get all the roles from the dtabase and store them as a list of selectedlistitems
            var items = db.Roles.Select(r => new SelectListItem
            {
                Text = r.Name,
                Value = r.Name,
                Selected = r.Name == oldRole
            }).ToList();

            //Build the changeroleviewmodel object including the list of roles
            //And send that to the view, displaying the roles in a drop down list,
            //With the user's current role displayed as selected
            return View(new ChangeRoleViewModel

            {
                UserName = user.UserName,
                Roles = items,
                OldRole = oldRole
            });

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("ChangeRole")]
        public async Task<ActionResult> ChangeRoleConfirmed(string id, [Bind(Include = "Role")] ChangeRoleViewModel model)
        {
            //Can't change own role
            if (id == User.Identity.GetUserId())
            {
                return RedirectToAction("Index", "Manager");
            }

            if (ModelState.IsValid)
            {
                //Get user by id
                User user = await UserManager.FindByIdAsync(id);

                //Get user's current role
                string oldRole = (await UserManager.GetRolesAsync(id)).Single(); //Only a single role

                //If current role is the same with selected role, then there is no point to update the database
                if (oldRole == model.Role)
                {
                    return RedirectToAction("Index", "Manager");
                }

                //Remove user from the old role first
                await UserManager.RemoveFromRoleAsync(id, oldRole);

                //Now, add the user to the new role
                await UserManager.AddToRoleAsync(id, model.Role);

                //if the user was suspended
                if (model.Role == "Suspended")
                {
                    //Then setisSuspended to true
                    user.IsSuspended = true;

                    //Update the user's details in the database
                    await UserManager.UpdateAsync(user);
                }


                return RedirectToAction("Index", "Manager");
            }

            return View(model);
        }
        


    }
}