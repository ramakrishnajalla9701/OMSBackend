using Backend.Data;
using Backend.DTOs;
using Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend.Services
{
    public interface IProductOrderService
    {
        Task<List<ProductOrderDto>> GetAllOrdersAsync();
        Task<ProductOrderDto?> GetOrderByIdAsync(int id);
        Task<ProductOrderDto> CreateOrderAsync(CreateProductOrderRequest request);
        Task<ProductOrderDto> UpdateOrderAsync(int id, UpdateProductOrderRequest request);
        Task<bool> DeleteOrderAsync(int id);
        Task<List<ProductOrderDto>> GetOrdersByStatusAsync(string status);
        Task<List<ProductOrderDto>> GetOrdersByUserAsync(int userId);
        Task<OrderStatisticsDto> GetStatisticsAsync();
        Task<List<MonthlyRevenueDto>> GetMonthlyRevenueAsync();
    }

    public class ProductOrderService : IProductOrderService
    {
        private readonly ApplicationDbContext _context;

        public ProductOrderService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<ProductOrderDto>> GetAllOrdersAsync()
        {
            var orders = await _context.ProductOrders
                .Include(po => po.AssignedToUser)
                .OrderByDescending(po => po.CreatedAt)
                .ToListAsync();

            return orders.Select(MapToDto).ToList();
        }

        public async Task<ProductOrderDto?> GetOrderByIdAsync(int id)
        {
            var order = await _context.ProductOrders
                .Include(po => po.AssignedToUser)
                .FirstOrDefaultAsync(po => po.Id == id);

            return order == null ? null : MapToDto(order);
        }

        public async Task<ProductOrderDto> CreateOrderAsync(CreateProductOrderRequest request)
        {
            // Validation
            if (string.IsNullOrWhiteSpace(request.PONumber))
                throw new ArgumentException("PO Number is required");

            if (request.OrderQuantityMeters <= 0)
                throw new ArgumentException("Order Quantity must be greater than 0");

            if (request.CostPerMeterPO <= 0)
                throw new ArgumentException("Cost per Meter (PO) must be greater than 0");

            if (request.PODate > request.POCompletionDate)
                throw new ArgumentException("PO Completion Date must be after PO Date");

            var order = new ProductOrder
            {
                PONumber = request.PONumber,
                PODate = request.PODate,
                POCompletionDate = request.POCompletionDate,
                CustomerName = request.CustomerName,
                Address = request.Address,
                AssignedToUserId = request.AssignedToUserId,
                AssignedToName = request.AssignedToName ?? string.Empty,
                ProductName = request.ProductName,
                OrderQuantityMeters = request.OrderQuantityMeters,
                CostPerMeterPO = request.CostPerMeterPO,
                CostPerMeterProduction = request.CostPerMeterProduction,
                CurrentStatus = request.CurrentStatus,
                Comments = request.Comments,
                UploadedImages = request.UploadedImages ?? new List<string>(),
                CreatedAt = DateTime.UtcNow
            };

            order.CalculateValues();

            _context.ProductOrders.Add(order);
            await _context.SaveChangesAsync();

            // Reload to get user info
            var createdOrder = await _context.ProductOrders
                .Include(po => po.AssignedToUser)
                .FirstOrDefaultAsync(po => po.Id == order.Id);

            return MapToDto(createdOrder!);
        }

        public async Task<ProductOrderDto> UpdateOrderAsync(int id, UpdateProductOrderRequest request)
        {
            var order = await _context.ProductOrders
                .Include(po => po.AssignedToUser)
                .FirstOrDefaultAsync(po => po.Id == id);

            if (order == null)
                throw new KeyNotFoundException($"Order with ID {id} not found");

            // Update fields if provided
            if (!string.IsNullOrWhiteSpace(request.PONumber))
                order.PONumber = request.PONumber;

            if (request.PODate.HasValue)
                order.PODate = request.PODate.Value;

            if (request.POCompletionDate.HasValue)
                order.POCompletionDate = request.POCompletionDate.Value;

            if (!string.IsNullOrWhiteSpace(request.CustomerName))
                order.CustomerName = request.CustomerName;

            if (!string.IsNullOrWhiteSpace(request.Address))
                order.Address = request.Address;

            if (request.AssignedToUserId.HasValue)
                order.AssignedToUserId = request.AssignedToUserId.Value == 0 ? null : request.AssignedToUserId.Value;

            if (request.AssignedToName != null)
                order.AssignedToName = request.AssignedToName;

            if (!string.IsNullOrWhiteSpace(request.ProductName))
                order.ProductName = request.ProductName;

            if (request.OrderQuantityMeters.HasValue && request.OrderQuantityMeters > 0)
                order.OrderQuantityMeters = request.OrderQuantityMeters.Value;

            if (request.CostPerMeterPO.HasValue && request.CostPerMeterPO > 0)
                order.CostPerMeterPO = request.CostPerMeterPO.Value;

            if (request.CostPerMeterProduction.HasValue && request.CostPerMeterProduction > 0)
                order.CostPerMeterProduction = request.CostPerMeterProduction.Value;

            if (request.MetersDelivered.HasValue)
                order.MetersDelivered = request.MetersDelivered.Value;

            if (request.AmountReceived.HasValue)
                order.AmountReceived = request.AmountReceived.Value;

            if (request.UploadedImages != null)
                order.UploadedImages = request.UploadedImages;

            if (!string.IsNullOrWhiteSpace(request.CurrentStatus))
                order.CurrentStatus = request.CurrentStatus;

            if (!string.IsNullOrWhiteSpace(request.Comments))
                order.Comments = request.Comments;

            order.UpdatedAt = DateTime.UtcNow;
            order.CalculateValues();

            _context.ProductOrders.Update(order);
            await _context.SaveChangesAsync();

            return MapToDto(order);
        }

        public async Task<bool> DeleteOrderAsync(int id)
        {
            var order = await _context.ProductOrders.FirstOrDefaultAsync(po => po.Id == id);
            if (order == null)
                return false;

            _context.ProductOrders.Remove(order);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<ProductOrderDto>> GetOrdersByStatusAsync(string status)
        {
            var orders = await _context.ProductOrders
                .Include(po => po.AssignedToUser)
                .Where(po => po.CurrentStatus == status)
                .OrderByDescending(po => po.CreatedAt)
                .ToListAsync();

            return orders.Select(MapToDto).ToList();
        }

        public async Task<List<ProductOrderDto>> GetOrdersByUserAsync(int userId)
        {
            var orders = await _context.ProductOrders
                .Include(po => po.AssignedToUser)
                .Where(po => po.AssignedToUserId == userId)
                .OrderByDescending(po => po.CreatedAt)
                .ToListAsync();

            return orders.Select(MapToDto).ToList();
        }

        public async Task<OrderStatisticsDto> GetStatisticsAsync()
        {
            var orders = await _context.ProductOrders.ToListAsync();

            var stats = new OrderStatisticsDto
            {
                TotalOrderValue = orders.Sum(o => o.TotalPOValue),
                TotalProductionCost = orders.Sum(o => o.TotalProductionValue),
                TotalProfit = orders.Sum(o => o.TotalPOValue - o.TotalProductionValue),
                TotalAmountReceived = orders.Sum(o => o.AmountReceived),
                TotalAmountPending = orders.Sum(o => o.AmountToBeReceived),
                TotalOrders = orders.Count,
                CompletedOrders = orders.Count(o => o.CurrentStatus == "Completed"),
                PendingOrders = orders.Count(o => o.CurrentStatus == "Pending"),
                InProgressOrders = orders.Count(o => o.CurrentStatus == "In Progress")
            };

            if (stats.TotalOrderValue > 0)
                stats.ProfitMarginPercentage = (stats.TotalProfit / stats.TotalOrderValue) * 100;

            return stats;
        }

        public async Task<List<MonthlyRevenueDto>> GetMonthlyRevenueAsync()
        {
            var orders = await _context.ProductOrders.ToListAsync();

            var monthlyRevenue = orders
                .GroupBy(o => new { o.PODate.Year, o.PODate.Month })
                .Select(g => new MonthlyRevenueDto
                {
                    Month = g.Key.Month,
                    Year = g.Key.Year,
                    TotalRevenue = g.Sum(o => o.TotalPOValue),
                    TotalCost = g.Sum(o => o.TotalProductionValue),
                    TotalProfit = g.Sum(o => o.TotalPOValue - o.TotalProductionValue)
                })
                .OrderByDescending(mr => mr.Year)
                .ThenByDescending(mr => mr.Month)
                .ToList();

            return monthlyRevenue;
        }

        private ProductOrderDto MapToDto(ProductOrder order)
        {
            return new ProductOrderDto
            {
                Id = order.Id,
                PONumber = order.PONumber,
                PODate = order.PODate,
                POCompletionDate = order.POCompletionDate,
                CustomerName = order.CustomerName,
                Address = order.Address,
                AssignedToUserId = order.AssignedToUserId,
                AssignedToUserName = order.AssignedToUser?.Username,
                AssignedToName = string.IsNullOrWhiteSpace(order.AssignedToName) ? order.AssignedToUser?.Username ?? string.Empty : order.AssignedToName,
                ProductName = order.ProductName,
                OrderQuantityMeters = order.OrderQuantityMeters,
                CostPerMeterPO = order.CostPerMeterPO,
                CostPerMeterProduction = order.CostPerMeterProduction,
                TotalPOValue = order.TotalPOValue,
                TotalProductionValue = order.TotalProductionValue,
                CurrentStatus = order.CurrentStatus,
                MetersDelivered = order.MetersDelivered,
                MetersToBeDelivered = order.MetersToBeDelivered,
                AmountReceived = order.AmountReceived,
                AmountToBeReceived = order.AmountToBeReceived,
                PendingPayments = order.PendingPayments,
                UploadedImages = order.UploadedImages,
                Comments = order.Comments,
                CreatedAt = order.CreatedAt,
                UpdatedAt = order.UpdatedAt
            };
        }
    }

    public class MonthlyRevenueDto
    {
        public int Month { get; set; }
        public int Year { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal TotalCost { get; set; }
        public decimal TotalProfit { get; set; }
    }
}
