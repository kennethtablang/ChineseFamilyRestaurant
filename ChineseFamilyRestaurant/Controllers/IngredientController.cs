using ChineseFamilyRestaurant.Data;
using ChineseFamilyRestaurant.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

        //If you have the View for the Create then this could be handy the GET method
        ////Ingredient/Create
        //[HttpGet]
        //public IActionResult Create()
        //{
        //    return View();
        //}

        // POST: Ingredient/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name, Description")] Ingredient ingredient)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await ingredients.AddAsync(ingredient); // Add ingredient to the repository
                    TempData["ToastMessage"] = $"Ingredient '{ingredient.Name}' has been successfully created."; // Success message
                    return RedirectToAction(nameof(Index)); // Redirect to the Index action
                }
                catch (Exception ex)
                {
                    // Log the exception (if logging is set up)
                    TempData["ToastMessage"] = $"Error: Unable to create ingredient. Details: {ex.Message}";
                }
            }

            TempData["ToastMessage"] = "Invalid input. Please correct the errors and try again.";
            return View(ingredient); // Return to the form with validation errors
        }

        //If you have the View for the Create then this could be handy the GET method
        ////Ingredient/Edit
        //[HttpGet]
        //public async Task<IActionResult> Edit(int id)
        //{
        //    var ingredient = await ingredients.GetByIdAsync(id, new QueryOptions<Ingredient>());
        //    if (ingredient == null)
        //    {
        //        return NotFound();
        //    }
        //    return View(ingredient);
        //}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind("IngredientId, Name, Description")] Ingredient ingredient)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await ingredients.UpdateAsync(ingredient); // Update ingredient
                    TempData["ToastMessage"] = $"Ingredient {ingredient.Name} has been successfully updated."; // Success message
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await ingredients.ExistsAsync(ingredient.IngredientId)) // Ensure ExistsAsync checks for entity existence
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            TempData["ToastMessage"] = "An error occurred while updating the ingredient. Please try again."; // Error message
            return View(ingredient);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ingredient = await ingredients.GetByIdAsync(id, new QueryOptions<Ingredient>());
            if (ingredient == null)
            {
                TempData["ToastMessage"] = "Ingredient not found.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                await ingredients.DeleteAsync(id);
                TempData["ToastMessage"] = $"Ingredient '{ingredient.Name}' has been successfully deleted.";
            }
            catch (Exception)
            {
                TempData["ToastMessage"] = "There was an error deleting the ingredient. Please try again.";
            }

            return RedirectToAction(nameof(Index));
        }

    }
}
