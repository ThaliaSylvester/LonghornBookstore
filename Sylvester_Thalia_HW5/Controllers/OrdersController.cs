using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Sylvester_Thalia_HW5.DAL;
using Sylvester_Thalia_HW5.Models;

namespace Sylvester_Thalia_HW5.Controllers
{
    [Authorize]
    public class OrdersController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public OrdersController(AppDbContext context, UserManager<AppUser> userManger)
        {
            _context = context;
            _userManager = userManger;

        }

        // GET: Order
        public IActionResult Index()
        {
            List<Order> Orders = new List<Order>();
            if (User.IsInRole("Admin"))
            {
                Orders = _context.Orders
                        .Include(o => o.OrderDetails)
                        .ToList();
            }
            else //user is a customer
            {
                Orders = _context.Orders
                         .Include(o => o.OrderDetails)
                         .Where(o => o.User.UserName == User.Identity.Name)
                         .ToList();
            }

            return View(Orders);
        }

        // GET: Order/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            //the user did not specify a order to view
            if (id == null)
            {
                return View("Error", new String[] { "Please specify a order to view!" });
            }

            //find the order in the database
            Order order = await _context.Orders
                                              .Include(r => r.OrderDetails)
                                              .ThenInclude(r => r.product)
                                              .Include(r => r.User)
                                              .FirstOrDefaultAsync(m => m.OrderID == id);

            //order was not found in the database
            if (order == null)
            {
                return View("Error", new String[] { "This order was not found!" });
            }

            //make sure this order belongs to this user
            if (User.IsInRole("Customer") && order.User.UserName != User.Identity.Name)
            {
                return View("Error", new String[] { "This is not your order!  Don't be such a snoop!" });
            }

            //Send the user to the details page
            return View(order);
        }
        // GET: Order/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Order/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize(Roles = "Customer")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Order order)
        {
            //Find the next order number from the utilities class
            order.OrderNumber = Utilities.GenerateNextOrderNumber.GetNextOrderNumber(_context);
            // set the date
            order.OrderDate = DateTime.Now;

            order.User = await _userManager.FindByNameAsync(User.Identity.Name);

            _context.Add(order);
            await _context.SaveChangesAsync();

            return RedirectToAction("Create", "OrderDetails", new { orderID = order.OrderID });
        }


        // GET: Order/Edit/5
        public IActionResult Edit(int? id)
        {
            //user did not specify a registration to edit
            if (id == null)
            {
                return View("Error", new String[] { "Please specify a order to edit" });
            }

            //find the registration in the database, and be sure to include details
            Order order = _context.Orders
                                       .Include(r => r.OrderDetails)
                                       .ThenInclude(r => r.product)
                                       .Include(r => r.User)
                                       .FirstOrDefault(r => r.OrderID == id);
            //registration was nout found in the database
            if (order == null)
            {
                return View("Error", new String[] { "This order was not found in the database!" });
            }

            //registration does not belong to this user
            if (User.IsInRole("Customer") && order.User.UserName != User.Identity.Name)
            {
                return View("Error", new String[] { "You are not authorized to edit this order!" });
            }

            //send the user to the registration edit view
            return View(order);
        }

        // POST: Order/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Order order)
        {
            //this is a security measure to make sure the user is editing the correct registration
            if (id != order.OrderID)
            {
                return View("Error", new String[] { "There was a problem editing this order. Try again!" });
            }

            //if there is something wrong with this order, try again
            if (ModelState.IsValid == false)
            {
                return View(order);
            }

            //if code gets this far, update the record
            try
            {
                //find the record in the database
                Order dbOrder = _context.Orders.Find(order.OrderID);

                //update the notes
                dbOrder.OrderNotes = order.OrderNotes;

                _context.Update(dbOrder);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return View("Error", new String[] { "There was an error updating this order!", ex.Message });
            }

            //send the user to the Registrations Index page.
            return RedirectToAction(nameof(Index));
        }
    }

}
