using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BeanScene.Data;
using BeanScene.Models;

namespace BeanScene.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly BeanSceneContext _context;
        private readonly ILogger<OrdersController> _logger;

        public OrdersController(BeanSceneContext context, ILogger<OrdersController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/orders
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderDto>>> GetOrders()
        {
            try
            {
                var orders = await _context.Orders
                    .Include(o => o.OrderItems)
                        .ThenInclude(oi => oi.MenuItem)
                    .Include(o => o.OrderItems)
                        .ThenInclude(oi => oi.SelectedOptions)
                    .Select(o => new OrderDto
                    {
                        OrderID = o.OrderID,
                        ReservationID = o.ReservationID,
                        TableID = o.TableID,
                        OrderStatus = o.OrderStatus,
                        OrderTime = o.OrderTime,
                        SpecialRequests = o.SpecialRequests,
                        TotalAmount = o.TotalAmount,
                        Items = o.OrderItems.Select(oi => new OrderItemDto
                        {
                            OrderItemID = oi.OrderItemID,
                            ItemID = oi.ItemID,
                            ItemName = oi.MenuItem.Name,
                            Quantity = oi.Quantity,
                            UnitPrice = oi.UnitPrice,
                            Subtotal = oi.Subtotal,
                            SpecialInstructions = oi.SpecialInstructions,
                            ItemStatus = oi.ItemStatus,
                            SelectedOptions = oi.SelectedOptions.Select(so => new SelectedOptionDto
                            {
                                OptionID = so.OptionID,
                                Name = so.Name,
                                PriceModifier = so.PriceModifier
                            }).ToList()
                        }).ToList()
                    })
                    .ToListAsync();

                return Ok(orders);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving orders");
                return StatusCode(500, new { message = "Error retrieving orders" });
            }
        }

        // GET: api/orders/5
        [HttpGet("{id}")]
        public async Task<ActionResult<OrderDto>> GetOrder(int id)
        {
            try
            {
                var order = await _context.Orders
                    .Include(o => o.OrderItems)
                        .ThenInclude(oi => oi.MenuItem)
                    .Include(o => o.OrderItems)
                        .ThenInclude(oi => oi.SelectedOptions)
                    .FirstOrDefaultAsync(o => o.OrderID == id);

                if (order == null)
                {
                    return NotFound(new { message = "Order not found" });
                }

                var orderDto = new OrderDto
                {
                    OrderID = order.OrderID,
                    ReservationID = order.ReservationID,
                    TableID = order.TableID,
                    OrderStatus = order.OrderStatus,
                    OrderTime = order.OrderTime,
                    SpecialRequests = order.SpecialRequests,
                    TotalAmount = order.TotalAmount,
                    Items = order.OrderItems.Select(oi => new OrderItemDto
                    {
                        OrderItemID = oi.OrderItemID,
                        ItemID = oi.ItemID,
                        ItemName = oi.MenuItem.Name,
                        Quantity = oi.Quantity,
                        UnitPrice = oi.UnitPrice,
                        Subtotal = oi.Subtotal,
                        SpecialInstructions = oi.SpecialInstructions,
                        ItemStatus = oi.ItemStatus,
                        SelectedOptions = oi.SelectedOptions.Select(so => new SelectedOptionDto
                        {
                            OptionID = so.OptionID,
                            Name = so.Name,
                            PriceModifier = so.PriceModifier
                        }).ToList()
                    }).ToList()
                };

                return Ok(orderDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving order {OrderId}", id);
                return StatusCode(500, new { message = "Error retrieving order" });
            }
        }

        // GET: api/orders/date/2024-03-20
        [HttpGet("date/{date}")]
        public async Task<ActionResult<IEnumerable<OrderDto>>> GetOrdersByDate(DateTime date)
        {
            try
            {
                var sydneyTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Australia/Sydney");
                var sydneyDate = TimeZoneInfo.ConvertTimeFromUtc(date.ToUniversalTime(), sydneyTimeZone);
                var startOfDay = sydneyDate.Date;
                var endOfDay = startOfDay.AddDays(1).AddTicks(-1);

                var orders = await _context.Orders
                    .Include(o => o.OrderItems)
                        .ThenInclude(oi => oi.MenuItem)
                    .Include(o => o.OrderItems)
                        .ThenInclude(oi => oi.SelectedOptions)
                    .Where(o => o.OrderTime >= startOfDay && o.OrderTime <= endOfDay)
                    .Select(o => new OrderDto
                    {
                        OrderID = o.OrderID,
                        ReservationID = o.ReservationID,
                        TableID = o.TableID,
                        OrderStatus = o.OrderStatus,
                        OrderTime = o.OrderTime,
                        SpecialRequests = o.SpecialRequests,
                        TotalAmount = o.TotalAmount,
                        Items = o.OrderItems.Select(oi => new OrderItemDto
                        {
                            OrderItemID = oi.OrderItemID,
                            ItemID = oi.ItemID,
                            ItemName = oi.MenuItem.Name,
                            Quantity = oi.Quantity,
                            UnitPrice = oi.UnitPrice,
                            Subtotal = oi.Subtotal,
                            SpecialInstructions = oi.SpecialInstructions,
                            ItemStatus = oi.ItemStatus,
                            SelectedOptions = oi.SelectedOptions.Select(so => new SelectedOptionDto
                            {
                                OptionID = so.OptionID,
                                Name = so.Name,
                                PriceModifier = so.PriceModifier
                            }).ToList()
                        }).ToList()
                    })
                    .ToListAsync();

                return Ok(orders);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving orders for date");
                return StatusCode(500, new { message = "Error retrieving orders" });
            }
        }

        // POST: api/orders
        [HttpPost]
        public async Task<ActionResult<OrderDto>> CreateOrder(CreateOrderDto orderDto)
        {
            try
            {
                // Validate items exist and calculate total
                decimal totalAmount = 0;
                var orderItems = new List<OrderItem>();

                foreach (var itemDto in orderDto.Items)
                {
                    var menuItem = await _context.MenuItems
                        .Include(mi => mi.Options)
                        .FirstOrDefaultAsync(mi => mi.ItemID == itemDto.ItemID);

                    if (menuItem == null)
                    {
                        return BadRequest(new { message = $"Menu item {itemDto.ItemID} not found" });
                    }

                    var selectedOptions = new List<ItemOption>();
                    decimal optionsTotal = 0;

                    // Validate and get selected options
                    if (itemDto.SelectedOptionIds != null && itemDto.SelectedOptionIds.Any())
                    {
                        foreach (var optionId in itemDto.SelectedOptionIds)
                        {
                            var option = menuItem.Options.FirstOrDefault(o => o.OptionID == optionId);
                            if (option == null)
                            {
                                return BadRequest(new { message = $"Option {optionId} not found for item {itemDto.ItemID}" });
                            }
                            selectedOptions.Add(option);
                            optionsTotal += option.PriceModifier;
                        }
                    }

                    var unitPrice = menuItem.Price + optionsTotal;
                    var subtotal = unitPrice * itemDto.Quantity;
                    totalAmount += subtotal;

                    var orderItem = new OrderItem
                    {
                        MenuItem = menuItem,  // Set the MenuItem navigation property
                        ItemID = menuItem.ItemID,  // Set the ItemID foreign key
                        Quantity = itemDto.Quantity,
                        UnitPrice = unitPrice,
                        Subtotal = subtotal,
                        SpecialInstructions = itemDto.SpecialInstructions,
                        SelectedOptions = selectedOptions
                    };

                    orderItems.Add(orderItem);
                }

                // Validate TableID exists
                var table = await _context.Tables.FindAsync(orderDto.TableID);
                if (table == null)
                {
                    return BadRequest(new { message = $"Table {orderDto.TableID} not found" });
                }

                var order = new Order
                {
                    ReservationID = orderDto.ReservationID,
                    TableID = orderDto.TableID,
                    Table = table,  // Set the Table navigation property
                    OrderStatus = "Pending",
                    OrderTime = DateTime.Now,
                    SpecialRequests = orderDto.SpecialRequests,
                    TotalAmount = totalAmount,
                    OrderItems = orderItems
                };

                _context.Orders.Add(order);

                // Add explicit relationship tracking
                foreach (var item in orderItems)
                {
                    _context.Entry(item).State = EntityState.Added;
                    foreach (var option in item.SelectedOptions)
                    {
                        _context.Entry(option).State = EntityState.Unchanged;
                    }
                }

                await _context.SaveChangesAsync();

                // Return created order
                return CreatedAtAction(
                    nameof(GetOrder), 
                    new { id = order.OrderID },
                    new { message = "Order created successfully", orderId = order.OrderID }
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating order: {Message}", ex.Message);
                if (ex.InnerException != null)
                {
                    _logger.LogError("Inner Exception: {Message}", ex.InnerException.Message);
                }
                return StatusCode(500, new { message = "An error occurred while creating the order.", details = ex.Message });
            }
        }

        // PUT: api/orders/5/status
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateOrderStatus(int id, [FromBody] UpdateOrderStatusDto statusDto)
        {
            try
            {
                var order = await _context.Orders.FindAsync(id);
                if (order == null)
                {
                    return NotFound(new { message = "Order not found" });
                }

                order.OrderStatus = statusDto.Status;
                await _context.SaveChangesAsync();

                return Ok(new { message = "Order status updated successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating order status for order {OrderId}", id);
                return StatusCode(500, new { message = "Error updating order status" });
            }
        }

        // PUT: api/orders/5/items/6/status
        [HttpPut("{orderId}/items/{itemId}/status")]
        public async Task<IActionResult> UpdateOrderItemStatus(int orderId, int itemId, [FromBody] UpdateOrderStatusDto statusDto)
        {
            try
            {
                var orderItem = await _context.OrderItems
                    .FirstOrDefaultAsync(oi => oi.OrderID == orderId && oi.OrderItemID == itemId);

                if (orderItem == null)
                {
                    return NotFound(new { message = "Order item not found" });
                }

                orderItem.ItemStatus = statusDto.Status;
                await _context.SaveChangesAsync();

                return Ok(new { message = "Order item status updated successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating order item status for order {OrderId}, item {ItemId}", orderId, itemId);
                return StatusCode(500, new { message = "Error updating order item status" });
            }
        }
    }

    public class OrderDto
    {
        public int OrderID { get; set; }
        public int? ReservationID { get; set; }
        public string? TableID { get; set; }
        public string OrderStatus { get; set; } = default!;
        public DateTime OrderTime { get; set; }
        public string? SpecialRequests { get; set; }
        public decimal TotalAmount { get; set; }
        public List<OrderItemDto> Items { get; set; } = new();
    }

    public class OrderItemDto
    {
        public int OrderItemID { get; set; }
        public int ItemID { get; set; }
        public string ItemName { get; set; } = default!;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Subtotal { get; set; }
        public string? SpecialInstructions { get; set; }
        public string ItemStatus { get; set; } = default!;
        public List<SelectedOptionDto> SelectedOptions { get; set; } = new();
    }

    public class SelectedOptionDto
    {
        public int OptionID { get; set; }
        public string Name { get; set; } = default!;
        public decimal PriceModifier { get; set; }
    }

    public class CreateOrderDto
    {
        public int? ReservationID { get; set; }
        public string? TableID { get; set; }
        public string? SpecialRequests { get; set; }
        public List<CreateOrderItemDto> Items { get; set; } = new();
    }

    public class CreateOrderItemDto
    {
        public int ItemID { get; set; }
        public int Quantity { get; set; }
        public string? SpecialInstructions { get; set; }
        public List<int>? SelectedOptionIds { get; set; }
    }

    public class UpdateOrderStatusDto
    {
        public string Status { get; set; } = default!;
    }
}