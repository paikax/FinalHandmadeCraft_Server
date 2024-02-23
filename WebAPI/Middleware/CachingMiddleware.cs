using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;

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
            var cacheKey = context.Request.Path.ToString();

            // Check if the request has a header indicating no-cache
            var noCacheHeader = context.Request.Headers["Cache-Control"];
            if (noCacheHeader == "no-cache")
            {
                // Bypass the cache and fetch data from the backend
                await _next(context);
                return;
            }

            // Check if the data is already in the cache
            if (_cache.TryGetValue(cacheKey, out string cachedResponse))
            {
                // Return cached response
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(cachedResponse, Encoding.UTF8);
                return;
            }

            // If not in cache, proceed with the request and cache the response
            using (var memoryStream = new MemoryStream())
            {
                var originalBodyStream = context.Response.Body;
                context.Response.Body = memoryStream;

                await _next(context);

                memoryStream.Seek(0, SeekOrigin.Begin);
                var responseBody = new StreamReader(memoryStream).ReadToEnd();
                _cache.Set(cacheKey, responseBody, TimeSpan.FromMinutes(10));

                memoryStream.Seek(0, SeekOrigin.Begin);
                await memoryStream.CopyToAsync(originalBodyStream);
            }
        }

        
    }
    
}