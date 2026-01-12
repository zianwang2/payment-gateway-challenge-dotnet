using Microsoft.AspNetCore.Mvc;

using PaymentGateway.Api.Models;
using PaymentGateway.Api.Models.Responses;
using PaymentGateway.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IPaymentsRepository, PaymentsRepository>();

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var errors = context.ModelState.Select(x => 
            {
                var key = x.Key;
                var errorMessage = string.Join(",", x.Value.Errors.Select(e => e.ErrorMessage));
                return $"{key}: {errorMessage}.";
            });

        var response = new PostPaymentResponse
        {
            Id = Guid.NewGuid(),
            Status = PaymentStatus.Rejected,
            ErrorMessage = string.Join(" ", errors)
        };

        return new BadRequestObjectResult(response);
    };
});

builder.Services.AddHttpClient<IBankServiceClient, BankServiceClient>(
    client =>
    {
        client.BaseAddress = new Uri("http://localhost:8080");
        client.Timeout = TimeSpan.FromSeconds(10);
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
