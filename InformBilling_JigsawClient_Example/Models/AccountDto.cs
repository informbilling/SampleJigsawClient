using System.ComponentModel.DataAnnotations;

namespace InformBilling_JigsawClient_Example.Models
{
    public class AccountDto
    {
        public int Id { get; set; }

        [Required]
        [StringLength(20)]
        public string AccountNumber { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(50)]
        public string Reference { get; set; }

        public int VatCodeId { get; set; }

        public DateTimeOffset DateCreated { get; set; }

        [StringLength(50)]
        public string UserCreated { get; set; }
    }
}
