using Microsoft.Extensions.Caching.Memory;

namespace BackMange.Controllers.API
{
    public class VerificationService
    {
        private readonly IMemoryCache _cache;
        private readonly TimeSpan _expiry = TimeSpan.FromMinutes(10);

        public VerificationService(IMemoryCache cache)
        {
            _cache = cache;
        }

        // 產生驗證碼並存入快取
        public string GenerateVerificationCode(string email)
        {
            var code = new Random().Next(100000, 999999).ToString(); // 產生 6 位數驗證碼
            _cache.Set(email, code, _expiry);
            return code;
        }

        // 驗證碼比對
        public bool ValidateCode(string email, string inputCode)
        {
            if (_cache.TryGetValue(email, out string? storedCode) && storedCode == inputCode)
            {
                _cache.Remove(email); // 驗證成功後刪除
                return true;
            }
            return false;
        }
    }
}
