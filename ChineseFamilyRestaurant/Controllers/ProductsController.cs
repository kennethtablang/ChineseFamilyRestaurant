using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ChineseFamilyRestaurant.Data;
using ChineseFamilyRestaurant.Models;

namespace ChineseFamilyRestaurant.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Products
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Products.Include(p => p.Category);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.ProductIngredients)
                    .ThenInclude(pi => pi.Ingredient)
                .FirstOrDefaultAsync(m => m.ProductId == id);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }


        // GET: Products/Create
        public IActionResult Create()
        {
            ViewBag.Ingredients = _context.Ingredients.ToList();
            ViewBag.CategoryId = new SelectList(_context.Categories, "CategoryId", "Name");
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProductId,Name,Description,Price,Stock,ImageFile,CategoryId")] Product product, int[] SelectedIngredients)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (product.ImageFile != null)//this keeps getting false
                    {
                        // Generate a unique file name
                        var fileName = Path.GetFileName(product.ImageFile.FileName);
                        var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");

                        // Ensure the directory exists
                        if (!Directory.Exists(uploadPath))
                        {
                            Directory.CreateDirectory(uploadPath);
                        }

                        // Full path to save the image
                        var filePath = Path.Combine(uploadPath, fileName);

                        // Save the file
                        using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await product.ImageFile.CopyToAsync(fileStream);
                        }

                        // Set the relative URL for the image
                        product.ImageUrl = "/images/" + fileName;
                    }
                    else
                    {
                        product.ImageUrl = "/images";
                    }

                    // Add selected ingredients
                    if (SelectedIngredients != null && SelectedIngredients.Length > 0)
                    {
                        product.ProductIngredients = SelectedIngredients.Select(id => new ProductIngredient
                        {
                            IngredientId = id,
                            Product = product
                        }).ToList();
                    }

                    _context.Add(product);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "An error occurred while uploading the image: " + ex.Message);
                }
            }
            // Re-populate ViewBag.Ingredients if the form submission fails
            ViewBag.Ingredients = _context.Ingredients.ToList();
            ViewBag.CategoryId = new SelectList(_context.Categories, "CategoryId", "Name", product.CategoryId);
            return View(product);
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                                        .Include(p => p.ProductIngredients)
                                        .ThenInclude(pi => pi.Ingredient)
                                        .FirstOrDefaultAsync(p => p.ProductId == id);

            if (product == null)
            {
                return NotFound();
            }

            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "Name", product.CategoryId);
            ViewBag.Ingredients = await _context.Ingredients.ToListAsync();
            return View(product);
        }


        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        // POST: Products/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProductId,Name,Description,Price,Stock,ImageFile,ImageUrl,CategoryId")] Product product, int[] SelectedIngredients)
        {
            if (id != product.ProductId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (product.ImageFile != null)
                    {
                        var fileName = Path.GetFileName(product.ImageFile.FileName);
                        var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");

                        if (!Directory.Exists(uploadPath))
                        {
                            Directory.CreateDirectory(uploadPath);
                        }

                        var filePath = Path.Combine(uploadPath, fileName);

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await product.ImageFile.CopyToAsync(fileStream);
                        }

                        product.ImageUrl = "/images/" + fileName;
                    }

                    // Update the product in the database
                    _context.Update(product);

                    // Update product ingredients
                    var existingIngredients = _context.ProductIngredients.Where(pi => pi.ProductId == id).ToList();
                    _context.ProductIngredients.RemoveRange(existingIngredients);

                    if (SelectedIngredients != null && SelectedIngredients.Length > 0)
                    {
                        foreach (var ingredientId in SelectedIngredients)
                        {
                            _context.ProductIngredients.Add(new ProductIngredient
                            {
                                ProductId = id,
                                IngredientId = ingredientId
                            });
                        }
                    }

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.ProductId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "Name", product.CategoryId);
            ViewBag.Ingredients = await _context.Ingredients.ToListAsync();
            return View(product);
        }


        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.ProductId == id);
        }
    }
}
