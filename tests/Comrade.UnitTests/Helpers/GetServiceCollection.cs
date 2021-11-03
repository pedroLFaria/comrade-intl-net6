﻿using Comrade.Api;
using Comrade.Api.Modules;
using Comrade.Api.Modules.Common;
using Comrade.Api.Modules.Common.FeatureFlags;
using Comrade.Api.Modules.Common.Swagger;
using Comrade.Application.Bases;
using Comrade.Application.Lookups;
using Comrade.Application.PipelineBehaviors;
using Comrade.Core.Bases.Interfaces;
using Comrade.Domain.Extensions;
using Comrade.Persistence.Bases;
using Comrade.Persistence.DataAccess;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Options;

namespace Comrade.UnitTests.Helpers;

public static class GetServiceCollection
{
    public static ServiceCollection Execute()
    {
        var services = new ServiceCollection();

        IConfiguration configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.test.json")
            .Build();

        services
            .AddFeatureFlags(configuration)
            .AddEntityRepository()
            .AddHealthChecks(configuration)
            .AddAuthentication(configuration)
            .AddVersioning()
            .AddSwagger()
            .AddUseCases()
            .AddCustomControllers()
            .AddCustomCors()
            .AddProxy()
            .AddCustomDataProtection();

        services.AddAutoMapperSetup();
        services.AddLogging();

        services.Configure<MongoDbContextSettings>(
            configuration.GetSection(nameof(MongoDbContextSettings)));
        services.AddSingleton<IMongoDbContextSettings>(sp =>
            sp.GetRequiredService<IOptions<MongoDbContextSettings>>().Value);
        services.AddScoped<IMongoDbContext, MongoDbContext>();

        services.AddScoped(typeof(ILookupService<>), typeof(LookupService<>));
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

        services.AddMediatR(typeof(Startup));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        services.AddValidatorsFromAssemblyContaining<EntityDto>();

        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<HashingOptions>();

        return services;
    }
}