using Microsoft.AspNetCore.Mvc;
using TequilasRestaurant.Data;
using TequilasRestaurant.Models;

namespace TequilasRestaurant.Controllers
{
    public class ProductController : Controller
    {
        private Repository<Product> products;
        private Repository<Ingredient> ingredients;
        private Repository<Category> categories;
        private readonly IWebHostEnvironment _webHostEnvironment;

        //Constructor
        //This a dependency inject of context
        //In our services we set up context instead of Repository some other apps would doc
        public ProductController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            products = new Repository<Product>(context);
            ingredients = new Repository<Ingredient>(context);
            categories = new Repository<Category>(context);
            _webHostEnvironment = webHostEnvironment;
        }
        //Returns - the list to product to its view
        public async Task<IActionResult> Index()
        {
            return View(await products.GetAllAsync());
        }

        [HttpGet]
        public async Task<IActionResult> AddEdit(int id)
        {
            // Allows for selecting any categories for each product
            ViewBag.Categories = await categories.GetAllAsync();
            // Allows for selecting any ingredients for each product
            ViewBag.Ingredients = await ingredients.GetAllAsync();
            
            // if id == 0, means its a new product so add it
            if (id == 0)
            {
                ViewBag.Operation = "Add";
                return View(new Product());
            }
            else
            {
                //Getting the Product by Id to edit
                Product product = await products.GetByIdAsync(id, new QueryOptions<Product>
                {
                    Includes = "ProductIngredients.Ingredient, Category"
                });

                ViewBag.Operation = "Edit";

                //Send product to view so user can edit upon w/ GUI
                return View(product);
            }

        }

        /* 
         * ingreidentId <- comes from checkboxes for ingredients
         * catId <- comes from radio buttons for categories
         */
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddEdit(Product product, int[] ingredientIds, int catId)
        {
            ViewBag.Ingredients = await ingredients.GetAllAsync();
            ViewBag.Categories = await categories.GetAllAsync();

            if (ModelState.IsValid)
            {
                if (product.ImageFile != null)
                {
                    string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + product.ImageFile.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    //This saves the image to wwwroot folder in project
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await product.ImageFile.CopyToAsync(fileStream);
                    }

                    product.ImageUrl = uniqueFileName;
                }

                //If product DNE, create it
                if (product.ProductId == 0)
                {
                    ViewBag.Ingredients = await ingredients.GetAllAsync();
                    ViewBag.Categories = await categories.GetAllAsync();
                    product.CategoryId = catId;

                    //add ingredients
                    foreach (int id in ingredientIds)
                    {
                        product.ProductIngredients?.Add(new ProductIngredient { IngredientId = id, ProductId = product.ProductId });
                    }

                    await products.AddAsync(product);
                    return RedirectToAction("Index", "Product");
                }
                //Else update the product
                else
                {
                    //The Product product gotten from the parameter has been change is it's not like the one on the DB anymore
                    //so using that will not work
                    //Therefore, we need to find that Product on the db using the ProductId first b/c that has not changed
                    //The Product on the db to be changed by paramter product
                    var existingProduct = await products.GetByIdAsync(product.ProductId, new QueryOptions<Product> { Includes = "ProductIngredients" });

                    //If the product DNE, go back to the Edit View on that Product
                    if (existingProduct == null)
                    {
                        ModelState.AddModelError("", "Product not found.");
                        ViewBag.Ingredients = await ingredients.GetAllAsync();
                        ViewBag.CategoryIds = await categories.GetAllAsync();
                        return View(product);
                    }

                    // Set the existing product to update
                    existingProduct.Name = product.Name;
                    existingProduct.Description = product.Description; 
                    existingProduct.Price = product.Price;
                    existingProduct.Stock = product.Stock;
                    existingProduct.CategoryId = catId;

                    if (product.ImageFile != null)
                        existingProduct.ImageUrl = product.ImageUrl;

                    // Update the ingredients
                    // Clear the list
                    existingProduct.ProductIngredients?.Clear();
                    //Populate with new Ingredients
                    foreach (int id in ingredientIds)
                    {
                        existingProduct.ProductIngredients?.Add(new ProductIngredient { IngredientId=id, ProductId = product.ProductId });
                    }

                    try
                    {
                        await products.UpdateAsync(existingProduct);
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("", $"Error: {ex.GetBaseException().Message}");
                        ViewBag.Ingredients = await ingredients.GetAllAsync();
                        ViewBag.Categories = await categories.GetAllAsync();
                        return View(product);
                    }

                    //Returns to the Index with the Product controller
                    return RedirectToAction("Index", "Product");
                }
            }

     
            return View(product);
                
          

            
        }

    }
}
