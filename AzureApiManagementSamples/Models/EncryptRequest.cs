using System.ComponentModel.DataAnnotations;

namespace AzureApiManagementSamples.Models
{
    public class EncryptRequest
    {
        [Required]
        public string PlainText { get; set; }
        [Required]
        public string Key { get; set; }
    }
}
