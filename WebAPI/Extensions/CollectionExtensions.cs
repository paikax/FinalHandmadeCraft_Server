    using System;
    using System.Collections.Generic;
    using Common.Constants;
    using Data.Context;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.OpenApi.Models;
    using MongoDB.Driver;
    using Service.IServices;
    using Service.Service;
    using WebAPI.Mapper;

    namespace WebAPI.Extensions
    {
        public static class CollectionExtensions
        {
            public static IServiceCollection AddServices(this IServiceCollection services)
            {
                // services.AddTransient<ISendMailService, SendMailService>();
                services.AddScoped<IJwtUtils, JwtUtils>();
                services.AddScoped<IUserService, UserService>();
                services.AddScoped<ICategoryService, CategoryService>();
                services.AddScoped<IMaterialService, MaterialService>();
                services.AddScoped<ITutorialService, TutorialService>();
                services.AddScoped<IOrderService, OrderService>();
                
                return services;
            }
            
            public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
            {
                string sqlServerConnectionString = configuration.GetConnectionString("DefaultConnection");
                string mongoDBConnectionString = configuration.GetConnectionString("MongoDBConnection");

                // Add SQL Server DbContext
                services.AddDbContext<AppDbContext>(options =>
                    options.UseSqlServer(sqlServerConnectionString, b => b.MigrationsAssembly("WebAPI")));

                // Add MongoDB
                services.AddSingleton<IMongoClient>(sp =>
                {
                    return new MongoClient(mongoDBConnectionString);
                });

                // Add MongoDB Context
                services.AddScoped<MongoDbContext>(sp =>
                {
                    var mongoClient = sp.GetRequiredService<IMongoClient>();
                    var databaseName = "handmadecraft"; // Replace with your actual MongoDB database name
                    return new MongoDbContext(mongoClient, databaseName);
                });

                return services;
            }
            
            public static IServiceCollection AddPayPalService(this IServiceCollection services, IConfiguration configuration)
            {
                services.AddScoped<IPayPalService, PayPalService>(); // Use IPayPalService as service type
                services.AddHttpClient<PayPalService>(); // Register the HttpClient for PayPalService
                return services;
            }


            public static IServiceCollection AddSwaggerGen(this IServiceCollection services)
            {
                services.AddSwaggerGen(c =>
                {
                    c.SwaggerDoc("v1", new OpenApiInfo() { Title = "WebAPIPhong", Version = "v1" });
                });

                return services;
            }
            
            public static IServiceCollection AddCustomCors(this IServiceCollection services)
            {
                services.AddCors(options =>
                {
                    options.AddPolicy("Policy", builder =>
                    {
                        var corsOrigins = AppSettings.CORS ?? new string[0]; // Null check and fallback to an empty array
                        builder.WithOrigins(corsOrigins)
                            .AllowAnyMethod()
                            .AllowAnyHeader()
                            .AllowCredentials();
                    });
                });

                return services;
            }

            
            public static IServiceCollection AddSwagger(this IServiceCollection service)
            {
                service.AddSwaggerGen(c =>
                {
                    c.SwaggerDoc("v1", new OpenApiInfo { Title = "PayShip2.0", Version = "v1" });

                    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                    {
                        Description =
                            "JWT Authorization header using the Bearer scheme. \r\n\r\n " +
                            "Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\n" +
                            "Example: \"Bearer 12345abcdef\"",
                        Name = "Authorization",
                        In = ParameterLocation.Header,
                        Type = SecuritySchemeType.ApiKey,
                        Scheme = "Bearer"
                    });

                    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
                    {
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "Bearer"
                                },
                                Scheme = "oauth2",
                                Name = "Bearer",
                                In = ParameterLocation.Header,
                            },
                            new List<string>()
                        }
                    });

                });
                
                return service;
            }
            
            public static IServiceCollection BackgroundService(this IServiceCollection services)
            {
                // background service
                // services.AddHostedService<AppointmentSenderService>();
               
                return services;
            }

            public static IServiceCollection AddMapping(this IServiceCollection services)
            {
                // mapping configuration
                var mapper = MappingConfig.RegisterMap().CreateMapper();
                services.AddSingleton(mapper);
                services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
                return services;
            }
        }
    }