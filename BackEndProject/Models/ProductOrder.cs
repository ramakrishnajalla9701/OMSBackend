using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models
{
    public class ProductOrder
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string PONumber { get; set; } = string.Empty;

        [Required]
        public DateTime PODate { get; set; }

        [Required]
        public DateTime POCompletionDate { get; set; }

        [Required]
        [StringLength(200)]
        public string CustomerName { get; set; } = string.Empty;

        [Required]
        [StringLength(500)]
        public string Address { get; set; } = string.Empty;

        [ForeignKey("AssignedToUser")]
        public int? AssignedToUserId { get; set; }

        [StringLength(200)]
        public string ProductName { get; set; } = string.Empty;

        [Range(0, double.MaxValue)]
        public decimal OrderQuantityMeters { get; set; }

        [Range(0, double.MaxValue)]
        public decimal CostPerMeterPO { get; set; }

        [Range(0, double.MaxValue)]
        public decimal CostPerMeterProduction { get; set; }

        [Range(0, double.MaxValue)]
        public decimal TotalPOValue { get; set; }

        [Range(0, double.MaxValue)]
        public decimal TotalProductionValue { get; set; }

        [StringLength(50)]
        public string CurrentStatus { get; set; } = "Pending"; // Pending, In Progress, Completed, On Hold

        [Range(0, double.MaxValue)]
        public decimal MetersDelivered { get; set; }

        [Range(0, double.MaxValue)]
        public decimal MetersToBeDelivered { get; set; }

        [Range(0, double.MaxValue)]
        public decimal AmountReceived { get; set; }

        [Range(0, double.MaxValue)]
        public decimal AmountToBeReceived { get; set; }

        [Range(0, double.MaxValue)]
        public decimal PendingPayments { get; set; }

        [StringLength(1000)]
        public string Comments { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        // Navigation property
        [ForeignKey("AssignedToUserId")]
        public User? AssignedToUser { get; set; }

        // Method to calculate values
        public void CalculateValues()
        {
            TotalPOValue = OrderQuantityMeters * CostPerMeterPO;
            TotalProductionValue = OrderQuantityMeters * CostPerMeterProduction;
            MetersToBeDelivered = OrderQuantityMeters - MetersDelivered;
            AmountToBeReceived = TotalPOValue - AmountReceived;
            PendingPayments = AmountToBeReceived;
        }
    }
}
