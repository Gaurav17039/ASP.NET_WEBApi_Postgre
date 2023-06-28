using CRUD_Migration_Logging_XunitTesting.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CRUD_Migration_Logging_XunitTesting.Controllers
{
    [Route("api/controller")]
    [ApiController]

    public class MyShoppingCartController : ControllerBase
    {
        private readonly ShoppingContext _SContext;

        private readonly ILogger<MyShoppingCartController> _logger;

        public MyShoppingCartController(ShoppingContext SContext, ILogger<MyShoppingCartController> logger)
        {
            _SContext = SContext;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ShoppingItem>>> GetItems()
        {
            _logger.LogInformation("Getting all the Information for Items...");

            if (_SContext.ShoppingItems == null)
            {
                _logger.LogWarning("No Items present!");
                return NotFound();
            }
            return await _SContext.ShoppingItems.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ShoppingItem>> GetItems(Guid id)
        {
            if (_SContext.ShoppingItems == null)
            {
                _logger.LogWarning("Items not found!");
                return NotFound();
            }

            _logger.LogDebug("Getting by ID method Executing...");

            var item = await _SContext.ShoppingItems.FindAsync(id);

            if (item == null)
            {
                _logger.LogWarning($"Item with Id {id} not found");
                return NotFound();
            }

            return item;
        }

        [HttpPost]
        public async Task<ActionResult<ShoppingItem>> PostItems(ShoppingItem item)
        {
            _SContext.ShoppingItems.Add(item);
            await _SContext.SaveChangesAsync();

            _logger.LogInformation("Push Items to database Executing...");

            return CreatedAtAction(nameof(GetItems), new { id = item.Id }, item);
        }

        [HttpPut]

        public async Task<IActionResult> PutItems(Guid id, ShoppingItem item)
        {
            if (id != item.Id)
            {
                _logger.LogWarning("No matches found!");
                return BadRequest();
            }
            _SContext.Entry(item).State = EntityState.Modified;

            try
            {
                await _SContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ItemAvailable(id))
                {
                    _logger.LogWarning($"Item with Id {id} not found");
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return Ok();
        }

        private bool ItemAvailable(Guid id)
        {
            return (_SContext.ShoppingItems?.Any(x => x.Id == id)).GetValueOrDefault();
        }

        [HttpDelete("{id}")]

        public async Task<IActionResult> DeleteItems(Guid id)
        {
            if (_SContext.ShoppingItems == null)
            {
                _logger.LogWarning("No items Matched!");
                return NotFound();
            }

            var item = await _SContext.ShoppingItems.FindAsync(id);
            if (item == null)
            {
                _logger.LogWarning($"Item with Id {id} not found");
                return NotFound();
            }

            _logger.LogInformation("Delete Items Executing...");

            _SContext.ShoppingItems.Remove(item);

            await _SContext.SaveChangesAsync();

            return Ok();
        }
    }
}
