using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace WebAPI.Middleware
{
    public class CachingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IMemoryCache _cache;

        public CachingMiddleware(RequestDelegate next, IMemoryCache cache)
        {
            _next = next;
            _cache = cache;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Skip caching for non-GET requests
            if (!string.Equals(context.Request.Method, "GET", StringComparison.OrdinalIgnoreCase))
            {
                await _next(context);
                return;
            }

            // Generate a unique cache key based on the request path
            var cacheKey = context.Request.Path.ToString();

            // Check if the response is already cached
            if (_cache.TryGetValue(cacheKey, out string cachedResponse))
            {
                // If cached response exists, return it directly
                await context.Response.WriteAsync(cachedResponse);
                return;
            }

            // If response is not cached, proceed with the request pipeline
            var originalBodyStream = context.Response.Body;
            using (var responseBody = new MemoryStream())
            {
                context.Response.Body = responseBody;

                await _next(context);

                // Read the response body
                var responseBodyBytes = responseBody.ToArray();
                cachedResponse = Encoding.UTF8.GetString(responseBodyBytes);

                // Cache the response for future GET requests with the same cache key
                _cache.Set(cacheKey, cachedResponse, TimeSpan.FromMinutes(5)); // Cache for 5 minutes
                await context.Response.Body.WriteAsync(responseBodyBytes, 0, responseBodyBytes.Length);
            }
        }
    }
}
