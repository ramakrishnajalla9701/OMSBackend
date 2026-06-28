namespace Backend.DTOs
{
    public class CreateProductOrderRequest
    {
        public string PONumber { get; set; } = string.Empty;
        public DateTime PODate { get; set; }
        public DateTime POCompletionDate { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public int? AssignedToUserId { get; set; }
        public string? AssignedToName { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public List<string>? UploadedImages { get; set; }
        public decimal OrderQuantityMeters { get; set; }
        public decimal CostPerMeterPO { get; set; }
        public decimal CostPerMeterProduction { get; set; }
        public string CurrentStatus { get; set; } = "Pending";
        public string Comments { get; set; } = string.Empty;
    }

    public class UpdateProductOrderRequest
    {
        public string? PONumber { get; set; }
        public DateTime? PODate { get; set; }
        public DateTime? POCompletionDate { get; set; }
        public string? CustomerName { get; set; }
        public string? Address { get; set; }
        public int? AssignedToUserId { get; set; }
        public string? AssignedToName { get; set; }
        public string? ProductName { get; set; }
        public List<string>? UploadedImages { get; set; }
        public decimal? OrderQuantityMeters { get; set; }
        public decimal? CostPerMeterPO { get; set; }
        public decimal? CostPerMeterProduction { get; set; }
        public decimal? MetersDelivered { get; set; }
        public decimal? AmountReceived { get; set; }
        public string? CurrentStatus { get; set; }
        public string? Comments { get; set; }
    }

    public class ProductOrderDto
    {
        public int Id { get; set; }
        public string PONumber { get; set; } = string.Empty;
        public DateTime PODate { get; set; }
        public DateTime POCompletionDate { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public int? AssignedToUserId { get; set; }
        public string? AssignedToUserName { get; set; }
        public string AssignedToName { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public decimal OrderQuantityMeters { get; set; }
        public decimal CostPerMeterPO { get; set; }
        public decimal CostPerMeterProduction { get; set; }
        public decimal TotalPOValue { get; set; }
        public decimal TotalProductionValue { get; set; }
        public string CurrentStatus { get; set; } = string.Empty;
        public decimal MetersDelivered { get; set; }
        public decimal MetersToBeDelivered { get; set; }
        public decimal AmountReceived { get; set; }
        public decimal AmountToBeReceived { get; set; }
        public decimal PendingPayments { get; set; }
        public List<string> UploadedImages { get; set; } = new List<string>();
        public string Comments { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class OrderStatisticsDto
    {
        public decimal TotalOrderValue { get; set; }
        public decimal TotalProductionCost { get; set; }
        public decimal TotalProfit { get; set; }
        public decimal TotalAmountReceived { get; set; }
        public decimal TotalAmountPending { get; set; }
        public int TotalOrders { get; set; }
        public int CompletedOrders { get; set; }
        public int PendingOrders { get; set; }
        public int InProgressOrders { get; set; }
        public decimal ProfitMarginPercentage { get; set; }
    }
}
