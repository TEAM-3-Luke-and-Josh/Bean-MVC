using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BeanScene.Data;
using BeanScene.Models;

namespace BeanScene.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class MenuController : ControllerBase
    {
        private readonly BeanSceneContext _context;
        private readonly ILogger<MenuController> _logger;

    private static string GetImageUrl(string itemName)
    {
        // Map the category and name to the appropriate image file
        switch (itemName)
        {
            case "Eggs Benedict":
                return "eggs_ben.jpg";
            case "Avocado Toast":
                return "av_toast.jpg";
            case "Beef Burger":
                return "beef_burg.jpg";
            case "Caesar Salad":
                return "ceas_salad.jpg";
            case "Grilled Ribeye Steak":
                return "steak_ribeye.jpg";
            case "Pan-Seared Salmon":
                return "steak_salm.jpg";
            case "Sweet Potato Fries":
                return "sp_fries.jpg";
            case "Garden Salad":
                return "gard_salad.jpg";
            case "Fresh Orange Juice":
                return "org_juice.jpg";
            case "Flat White":
                return "coffee.jpg";
            case "Sticky Date Pudding":
                return "sticky_pud.jpg";
            case "Chocolate Fondant":
                return "choc_pud.jpg";
            default:
                return "/api/placeholder/400/320";
        }
    }

        public MenuController(BeanSceneContext context, ILogger<MenuController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/menu
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MenuItemDto>>> GetMenu()
        {
            try
            {
                var items = await _context.MenuItems
                    .Include(m => m.Category)
                    .Include(m => m.Options)
                    .Include(m => m.Availability)
                    .Select(m => new MenuItemDto
                    {
                        ItemID = m.ItemID,
                        CategoryID = m.CategoryID,
                        CategoryName = m.Category.Name,
                        Name = m.Name,
                        Description = m.Description,
                        Price = m.Price,
                        PrepTime = m.PrepTime,
                        IsAvailable = m.IsAvailable,
                        Options = m.Options.Select(o => new ItemOptionDto
                        {
                            OptionID = o.OptionID,
                            Name = o.Name,
                            Description = o.Description,
                            PriceModifier = o.PriceModifier
                        }).ToList(),
                        AvailableSittings = m.Availability.Select(a => a.SittingType).ToList(),
                        ImageUrl = GetImageUrl(m.Name)
                    })
                    .ToListAsync();

                return Ok(items);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving menu");
                return StatusCode(500, new { message = "Error retrieving menu" });
            }
        }

        // GET: api/menu/categories
        [HttpGet("categories")]
        public async Task<ActionResult<IEnumerable<MenuCategoryDto>>> GetCategories()
        {
            try
            {
                var categories = await _context.MenuCategories
                    .Select(c => new MenuCategoryDto
                    {
                        CategoryID = c.CategoryID,
                        Name = c.Name,
                        Description = c.Description,
                        IsAvailable = c.IsAvailable
                    })
                    .ToListAsync();

                return Ok(categories);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving menu categories");
                return StatusCode(500, new { message = "Error retrieving menu categories" });
            }
        }

        // GET: api/menu/sitting/{sittingType}
        [HttpGet("sitting/{sittingType}")]
        public async Task<ActionResult<IEnumerable<MenuItemDto>>> GetMenuBySitting(string sittingType)
        {
            try
            {
                var items = await _context.MenuAvailability
                    .Where(ma => ma.SittingType == sittingType && ma.MenuItem.IsAvailable)
                    .Include(ma => ma.MenuItem)
                        .ThenInclude(m => m.Category)
                    .Include(ma => ma.MenuItem)
                        .ThenInclude(m => m.Options)
                    .Select(ma => new MenuItemDto
                    {
                        ItemID = ma.MenuItem.ItemID,
                        CategoryID = ma.MenuItem.CategoryID,
                        CategoryName = ma.MenuItem.Category.Name,
                        Name = ma.MenuItem.Name,
                        Description = ma.MenuItem.Description,
                        Price = ma.MenuItem.Price,
                        PrepTime = ma.MenuItem.PrepTime,
                        IsAvailable = ma.MenuItem.IsAvailable,
                        Options = ma.MenuItem.Options.Select(o => new ItemOptionDto
                        {
                            OptionID = o.OptionID,
                            Name = o.Name,
                            Description = o.Description,
                            PriceModifier = o.PriceModifier
                        }).ToList()
                    })
                    .ToListAsync();

                return Ok(items);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving menu for sitting type: {SittingType}", sittingType);
                return StatusCode(500, new { message = "Error retrieving menu" });
            }
        }
    }

    public class MenuItemDto
    {
        public int ItemID { get; set; }
        public int CategoryID { get; set; }
        public string CategoryName { get; set; } = default!;
        public string Name { get; set; } = default!;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int? PrepTime { get; set; }
        public bool IsAvailable { get; set; }
        public List<ItemOptionDto> Options { get; set; } = new();
        public List<string> AvailableSittings { get; set; } = new();
        public string? ImageUrl { get; set; }
    }

    public class ItemOptionDto
    {
        public int OptionID { get; set; }
        public string Name { get; set; } = default!;
        public string? Description { get; set; }
        public decimal PriceModifier { get; set; }
    }

    public class MenuCategoryDto
    {
        public int CategoryID { get; set; }
        public string Name { get; set; } = default!;
        public string? Description { get; set; }
        public bool IsAvailable { get; set; }
    }
}