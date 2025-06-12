using System.ComponentModel.DataAnnotations;

namespace WebApplication2.Models.DTOs
{
    public class CreateClientRentalDto
    {
        [Required]
        public CreateClientDto Client { get; set; } = new CreateClientDto();

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "CarId must be a positive integer.")]
        public int CarId { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime DateFrom { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime DateTo { get; set; }
    }
    
    public class CreateClientDto
    {
        [Required]
        [StringLength(50, MinimumLength = 1)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(100, MinimumLength = 1)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [StringLength(100, MinimumLength = 5)]
        public string Address { get; set; } = string.Empty;
    }
}