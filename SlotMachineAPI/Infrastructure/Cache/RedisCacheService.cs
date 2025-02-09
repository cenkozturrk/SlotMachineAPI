//using StackExchange.Redis;
//using Newtonsoft.Json;

//namespace SlotMachineAPI.Infrastructure.Cache
//{
//    public class RedisCacheService : ICacheService
//    {
//        private readonly IDatabase _redisDb;

//        public RedisCacheService(IConnectionMultiplexer redis)
//        {
//            _redisDb = redis.GetDatabase();
//        }

//        public async Task<T?> GetAsync<T>(string key)
//        {
//            var cachedData = await _redisDb.StringGetAsync(key);
//            if (cachedData.IsNullOrEmpty)
//                return default;

//            return JsonConvert.DeserializeObject<T>(cachedData);
//        }

//        public async Task SetAsync<T>(string key, T value, TimeSpan expiration)
//        {
//            var jsonData = JsonConvert.SerializeObject(value);
//            await _redisDb.StringSetAsync(key, jsonData, expiration);
//        }

//        public async Task RemoveAsync(string key)
//        {
//            await _redisDb.KeyDeleteAsync(key);
//        }
//    }
//}
