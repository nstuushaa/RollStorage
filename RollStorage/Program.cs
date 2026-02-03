using Microsoft.EntityFrameworkCore;
using RollStorage.Middleware;
using RollStorage.Models;
using RollStorage.Repositories; 
using RollStorage.Services;   
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.NumberHandling =
            JsonNumberHandling.AllowNamedFloatingPointLiterals;
    });

builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<RollContext>(opt =>
    opt.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IRollRepository, SQLiteRollRepository>();
builder.Services.AddScoped<RollService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();

app.Run();
