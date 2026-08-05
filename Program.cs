using Asp.Versioning;
using LearnApi.Common;
using LearnApi.Data;
using LearnApi.Repository;
using LearnApi.Repository.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi;
using MongoDB.Bson.Serialization.Conventions;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.InvalidModelStateResponseFactory = context =>
        {
            var errors = context.ModelState
                .Where(e => e.Value?.Errors.Count > 0)
                .ToDictionary(
                    e => e.Key,
                    e => e.Value!.Errors.Select(err => err.ErrorMessage).ToArray()
                );

            var response = ApiErrorResponse.Create(
                title: "Validation Error",
                status: StatusCodes.Status400BadRequest,
                detail: "Một hoặc nhiều trường dữ liệu không hợp lệ.",
                instance: context.HttpContext.Request.Path,
                traceId: context.HttpContext.TraceIdentifier,
                errors: errors
            );

            return new BadRequestObjectResult(response)
            {
                ContentTypes = { "application/problem+json" }
            };
        };
    });

// API Versioning
var apiVersioningBuilder = builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
});

apiVersioningBuilder.AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

builder.Services.AddEndpointsApiExplorer();

// Swagger cho từng version
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "LearnApi",
        Version = "v1",
        Description = "Phiên bản 1 - API quản lý bãi đỗ xe"
    });
});

// Exception Handler
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

// MongoDB config
builder.Services.Configure<MongoDbSettings>(
    builder.Configuration.GetSection("MongoDbSettings"));
var camelCaseConvention = new ConventionPack { new CamelCaseElementNameConvention() };
ConventionRegistry.Register("CamelCase", camelCaseConvention, type => true);
builder.Services.AddSingleton<MongoDbContext>();

// Service DI
builder.Services.AddScoped(typeof(IRepository<>), typeof(MongoRepository<>));

var app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "LearnApi v1");
    });
}

app.UseHttpsRedirection();
app.MapControllers();
app.Run();