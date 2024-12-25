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
                ViewBag.Operation = "Edit";
                return View();
            }

        }

        /* 
         * ingreidentId <- comes from checkboxes for ingredients
         * catId <- comes from radio buttons for categories
         */
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddEdit(Product product, int[] ingredientId, int catId)
        {

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
                    ViewBag.Categories = await ingredients.GetAllAsync();
                    product.CategoryId = catId;

                    //add ingredients
                    foreach (int id in ingredientId)
                    {
                        product.ProductIngredients?.Add(new ProductIngredient { IngredientId = id, ProductId = product.ProductId });
                    }

                    await products.AddAsync(product);
                    return RedirectToAction("Index", "Product");
                }
                else
                {
                    return RedirectToAction("Index", "Product");
                }
            }

     
            return View(product);
                
          

            
        }

    }
}
