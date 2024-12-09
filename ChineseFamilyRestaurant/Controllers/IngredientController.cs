using ChineseFamilyRestaurant.Data;
using ChineseFamilyRestaurant.Models;
using Microsoft.AspNetCore.Mvc;

namespace ChineseFamilyRestaurant.Controllers
{
    public class IngredientController : Controller
    {
        private Repository<Ingredient> ingredients;

        public IngredientController(ApplicationDbContext context, ILogger<Repository<Ingredient>> logger)
        {
            ingredients = new Repository<Ingredient>(context, logger);
        }

        public async Task<IActionResult> Index()
        {
            return View(await ingredients.GetAllAsync()); 
        }

        public async Task<IActionResult> Details(int id)
        {
            var ingredient = await ingredients.GetByIdAsync(id,
                new QueryOptions<Ingredient>
                {
                    Includes = "ProductIngredients.Product"
                });
            if (ingredient == null)
            {
                return NotFound();
            }
            return View(ingredient);
        }


        //Ingredient/Create
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        //POST that handles form submission for the //Ingredient/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IngredientId, Name")] Ingredient ingredient)
        {
            if (ModelState.IsValid)
            {
                await ingredients.AddAsync(ingredient);
                return RedirectToAction("Index");
            }
            return View(ingredient);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var ingredient = await ingredients.GetByIdAsync(id, new QueryOptions<Ingredient>());
            if (ingredient == null)
            {
                return NotFound();
            }
            return View(ingredient);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IngredientId, Name, Description")] Ingredient ingredient)
        {
            if (id != ingredient.IngredientId)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await ingredients.UpdateAsync(ingredient);
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    // Log the error
                    Console.WriteLine(ex.Message);
                    ModelState.AddModelError("", "Unable to save changes. Try again.");
                }
            }
            return View(ingredient);
        }

    }
}
