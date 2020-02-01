using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using AzureApiManagementSamples.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AzureApiManagementSamples.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CryptoController : ControllerBase
    {
        private readonly ILogger<CryptoController> _logger;
        private readonly byte[] _iv = new byte[16] { 189, 127, 216, 247, 112, 64, 63, 178, 115, 152, 175, 157, 120, 81, 124, 192 };
        private readonly byte[] _salt = new byte[16] { 105, 71, 27, 131, 199, 34, 187, 255, 138, 109, 121, 107, 214, 91, 185, 86 };
        public CryptoController(ILogger<CryptoController> logger)
        {
            _logger = logger;
        }
        [HttpPost("Encrypt")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public IActionResult Encrypt(EncryptRequest request)
        {
            if (ModelState.IsValid)
            {
                string result = EncryptPayload(request.PlainText, request.Key);
                if (string.IsNullOrEmpty(result))
                {
                    return StatusCode(500);
                }
                return Ok(new EncryptResponse
                {
                    CipherTextBase64 = result
                });
            }
            return BadRequest();

        }
        [HttpPost("Decrypt")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public IActionResult Decrypt(DecryptRequest request)
        {
            if (ModelState.IsValid)
            {
                string result = DecryptPayload(request.CipherTextBase64, request.Key);
                if (string.IsNullOrEmpty(result))
                {
                    return StatusCode(500, new { error = "Invalid cipher data/secret key" });
                }
                return Ok(new DecryptResponse
                {
                    PlainText = result
                });
            }
            return BadRequest();
        }
        private string EncryptPayload(string plain, string key)
        {
            try
            {
                using (AesCng aes = new AesCng())
                {
                    byte[] plainBytes = Encoding.UTF8.GetBytes(plain);
                    byte[] keyBytes = CreatePasswordFromKey(key);
                    aes.Key = keyBytes;
                    aes.IV = _iv;
                    using (ICryptoTransform cryptoTransform = aes.CreateEncryptor())
                    {
                        byte[] cipherBytes = cryptoTransform.TransformFinalBlock(plainBytes, 0, plainBytes.Length);
                        return Convert.ToBase64String(cipherBytes);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return string.Empty;
            }
        }
        private string DecryptPayload(string cipher, string key)
        {
            try
            {
                byte[] cipherBytes = Convert.FromBase64String(cipher);
                using (AesCng aes = new AesCng())
                {
                    byte[] keyBytes = CreatePasswordFromKey(key);
                    aes.Key = keyBytes;
                    aes.IV = _iv;
                    using (ICryptoTransform cryptoTransform = aes.CreateDecryptor())
                    {
                        byte[] plainBytes = cryptoTransform.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);
                        return Encoding.UTF8.GetString(plainBytes);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return string.Empty;
            }
        }
        private byte[] CreatePasswordFromKey(string key)
        {
            using (Rfc2898DeriveBytes pwdGen = new Rfc2898DeriveBytes(key, _salt))
            {
                return pwdGen.GetBytes(16);
            }
        }
    }
}