using System.ComponentModel.DataAnnotations;

namespace InformBilling_JigsawClient_Example.Models
{
    public class IdentityServerSettingsDto
    {
        [Required]
        [MinLength(1, ErrorMessage = "TokenAccessEndPoint must be supplied")]
        public string TokenAccessEndPoint { get; set; }
        [Required]
        [MinLength(1, ErrorMessage = "ClientId must be supplied")]
        public string ClientId { get; set; }
        [Required]
        [MinLength(1, ErrorMessage = "APIKey must be supplied")]
        public string APIKey { get; set; }
        [Required]
        [MinLength(1, ErrorMessage = "Scope must be supplied")]
        public string Scope { get; set; }
    }
}
