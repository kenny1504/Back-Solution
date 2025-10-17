using Application;
using Infrastructure;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;

var builder = WebApplication.CreateBuilder(args);
var connStr = builder.Configuration.GetConnectionString("Default");

// DI
builder.Services.AddInfrastructure(connStr);
builder.Services.AddApplication();
builder.Services.AddControllers()
  .ConfigureApiBehaviorOptions(opt => opt.SuppressModelStateInvalidFilter = true);

// API Versioning
builder.Services.AddApiVersioning(opt =>
{
  opt.DefaultApiVersion = new ApiVersion(1, 0);
  opt.AssumeDefaultVersionWhenUnspecified = true;
  opt.ReportApiVersions = true;
  opt.ApiVersionReader = new UrlSegmentApiVersionReader();
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
const string AllowAngular = "AllowAngular";
builder.Services.AddCors(options =>
{
  options.AddPolicy(AllowAngular, policy =>
    policy
      .WithOrigins(
        "http://localhost:4200", // Angular dev
        "http://127.0.0.1:4200"
      )
      .AllowAnyHeader()
      .AllowAnyMethod()
      .WithExposedHeaders("Content-Disposition")
  );
});

var app = builder.Build();

// Middleware global de errores
app.UseExceptionHandler(errorApp =>
{
  errorApp.Run(async context =>
  {
    var feature = context.Features.Get<IExceptionHandlerFeature>();
    var ex = feature?.Error;

    var status = ex switch
    {
      KeyNotFoundException => StatusCodes.Status404NotFound,
      ArgumentException => StatusCodes.Status400BadRequest,
      _ => StatusCodes.Status500InternalServerError
    };

    var problem = new ProblemDetails
    {
      Status = status,
      Title = "Error en la solicitud",
      Detail = ex?.Message
    };

    context.Response.StatusCode = status;
    context.Response.ContentType = "application/problem+json";
    await context.Response.WriteAsJsonAsync(problem);
  });
});

app.UseSwagger();
app.UseSwaggerUI();
app.UseCors(AllowAngular);
app.MapControllers();

// Auto-migrar
using (var scope = app.Services.CreateScope())
{
  var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
  db.Database.EnsureCreated();
}

app.Run();
