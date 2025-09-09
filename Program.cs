using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using InvoiceApi.Data;
using InvoiceApi.Services;

var builder = WebApplication.CreateBuilder(args);

// services
builder.Services.AddSingleton<SqlHelper>();
builder.Services.AddScoped<InvoiceRepository>();
builder.Services.AddScoped<InvoiceService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// CORS - permite tu frontend local (ajusta origen)
builder.Services.AddCors(opts =>
{
    opts.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:4200") 
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();