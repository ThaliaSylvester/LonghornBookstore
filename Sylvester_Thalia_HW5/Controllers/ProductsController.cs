using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Sylvester_Thalia_HW5.DAL;
using Sylvester_Thalia_HW5.Models;

[Authorize(Roles = "Admin")]
public class ProductsController : Controller
{
    private readonly AppDbContext _context;

    public ProductsController(AppDbContext context)
    {
        _context = context;
    }

    // GET: Products
    [AllowAnonymous]
    public async Task<IActionResult> Index()
    {
        return View(await _context.Products
            .Include(c => c.Suppliers)
            .ToListAsync());
    }

    // GET: Products/Details/5
    [AllowAnonymous]
    public async Task<IActionResult> Details(int? id)
    {
        //id was not specified - show the user an error
        if (id == null)
        {
            return View("Error", new String[] { "Please specify a Product to view!" });
        }

        //find the Product in the database
        //be sure to include the relevant navigational data
        Product product = await _context.Products
            .Include(c => c.Suppliers)
            .FirstOrDefaultAsync(m => m.ProductID == id);

        //Product was not found in the database
        if (product == null)
        {
            return View("Error", new String[] { "That Product was not found in the database." });
        }

        return View(product);
    }

    // GET: Products/Create
    public IActionResult Create()
    {
        ViewBag.AllSuppliers = GetSupplierSelectList();
        return View();
    }

    // POST: Products/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Product product, int[] SelectedSuppliers)
    {
        //This code has been modified so that if the model state is not valid
        //we immediately go to the "sad path" and give the user a chance to try again
        if (ModelState.IsValid == false)
        {
            //re-populate the view bag with the Suppliers
            ViewBag.AllSuppliers = GetSupplierSelectList();
            //go back to the Create view to try again
            return View(product);
        }

        //if code gets to this point, we know the model is valid and
        //we can add the Product to the database

        //add the Product to the database and save changes
        _context.Add(product);
        await _context.SaveChangesAsync();

        //add the associated Suppliers to the Product
        //loop through the list of deparment ids selected by the user
        foreach (int SupplierID in SelectedSuppliers)
        {
            //find the Supplier associated with that id
            Supplier dbSupplier = _context.Suppliers.Find(SupplierID);

            //add the Supplier to the Product's list of Suppliers and save changes
            product.Suppliers.Add(dbSupplier);
            _context.SaveChanges();
        }

        //Send the user to the page with all the Suppliers
        return RedirectToAction(nameof(Index));
    }

    // GET: Products/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        //if the user didn't specify a Product id, we can't show them 
        //the data, so show an error instead
        if (id == null)
        {
            return View("Error", new string[] { "Please specify a Product to edit!" });
        }

        //find the Product in the database
        //be sure to change the data type to Product instead of 'var'
        Product product = await _context.Products.Include(c => c.Suppliers)
                                       .FirstOrDefaultAsync(c => c.ProductID == id);

        //if the Product does not exist in the database, then show the user
        //an error message
        if (product == null)
        {
            return View("Error", new string[] { "This Product was not found!" });
        }

        //populate the viewbag with existing Suppliers
        ViewBag.AllSuppliers = GetSupplierSelectList(product);
        return View(product);
    }

    // POST: Products/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(int id, Product product, int[] SelectedSuppliers)
    {
        //this is a security check to see if the user is trying to modify
        //a different record.  Show an error message
        if (id != product.ProductID)
        {
            return View("Error", new string[] { "Please try again!" });
        }

        if (ModelState.IsValid == false) //there is something wrong
        {
            ViewBag.AllSuppliers = GetSupplierSelectList(product);
            return View(product);
        }

        //if code gets this far, attempt to edit the Product
        try
        {
            //Find the Product to edit in the database and include relevant 
            //navigational properties
            Product dbProduct = _context.Products
                .Include(c => c.Suppliers)
                .FirstOrDefault(c => c.ProductID == product.ProductID);

            //create a list of Suppliers that need to be removed
            List<Supplier> SuppliersToRemove = new List<Supplier>();

            //find the Suppliers that should no longer be selected because the
            //user removed them
            //remember, SelectedSuppliers = the list from the HTTP request (listbox)
            foreach (Supplier Supplier in dbProduct.Suppliers)
            {
                //see if the new list contains the Supplier id from the old list
                if (SelectedSuppliers.Contains(Supplier.SupplierID) == false)//this Supplier is not on the new list
                {
                    SuppliersToRemove.Add(Supplier);
                }
            }

            //remove the Suppliers you found in the list above
            //this has to be 2 separate steps because you can't iterate (loop)
            //over a list that you are removing things from
            foreach (Supplier supplier in SuppliersToRemove)
            {
                //remove this Product Supplier from the Product's list of Suppliers
                dbProduct.Suppliers.Remove(supplier);
                _context.SaveChanges();
            }

            //add the Suppliers that aren't already there
            foreach (int supplierID in SelectedSuppliers)
            {
                if (dbProduct.Suppliers.Any(d => d.SupplierID == supplierID) == false)//this Supplier is NOT already associated with this Product
                {
                    //Find the associated Supplier in the database
                    Supplier dbSupplier = _context.Suppliers.Find(supplierID);

                    //Add the Supplier to the Product's list of Suppliers
                    dbProduct.Suppliers.Add(dbSupplier);
                    _context.SaveChanges();
                }
            }

            //update the Product's scalar properties
            dbProduct.Price = product.Price;
            dbProduct.Name = product.Name;
            dbProduct.Description = product.Description;


            //save the changes
            _context.Products.Update(dbProduct);
            _context.SaveChanges();

        }
        catch (Exception ex)
        {
            return View("Error", new string[] { "There was an error editing this Product.", ex.Message });
        }

        //if code gets this far, everything is okay
        //send the user back to the page with all the Products
        return RedirectToAction(nameof(Index));
    }
    private MultiSelectList GetSupplierSelectList()
    {
        //Create a new list of Suppliers and get the list of the Suppliers
        //from the database
        List<Supplier> allSuppliers = _context.Suppliers.ToList();

        //Multi-select lists do not require a selection, so you don't need 
        //to add a dummy record like you do for select lists

        //use the MultiSelectList constructor method to get a new MultiSelectList
        MultiSelectList mslAllSuppliers = new MultiSelectList(allSuppliers.OrderBy(d => d.SupplierName), "SupplierID", "SupplierName");

        //return the MultiSelectList
        return mslAllSuppliers;
    }

    private MultiSelectList GetSupplierSelectList(Product Product)
    {
        //Create a new list of Suppliers and get the list of the Suppliers
        //from the database
        List<Supplier> allSuppliers = _context.Suppliers.ToList();

        //loop through the list of Product Suppliers to find a list of Supplier ids
        //create a list to store the Supplier ids
        List<Int32> selectedSupplierIDs = new List<Int32>();

        //Loop through the list to find the SupplierIDs
        foreach (Supplier associatedSupplier in Product.Suppliers)
        {
            selectedSupplierIDs.Add(associatedSupplier.SupplierID);
        }

        //use the MultiSelectList constructor method to get a new MultiSelectList
        MultiSelectList mslAllSuppliers = new MultiSelectList(allSuppliers.OrderBy(d => d.SupplierName), "SupplierID", "SupplierName", selectedSupplierIDs);

        //return the MultiSelectList
        return mslAllSuppliers;
    }
}


