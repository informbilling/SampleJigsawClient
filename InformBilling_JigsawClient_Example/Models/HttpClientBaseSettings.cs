using System.ComponentModel.DataAnnotations;

namespace InformBilling_JigsawClient_Example.Models
{
    public class HttpClientBaseSettings
    {
        public string ServiceName { get; set; }

        [Required]
        [MinLength(1, ErrorMessage = "BaseAddress must be supplied")]
        public string BaseAddress { get; set; }

        public Dictionary<string, string> ExtraHeaderValues { get; set; }
    }
}
