using System.ComponentModel.DataAnnotations;

namespace AzureApiManagementSamples.Models
{
    public class DecryptRequest
    {
        [Required]
        public string CipherTextBase64 { get; set; }
        [Required]
        public string Key { get; set; }
    }
}
