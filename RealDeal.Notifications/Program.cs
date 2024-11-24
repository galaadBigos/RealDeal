using Microsoft.AspNetCore.Authentication.Negotiate;
using Microsoft.AspNetCore.Mvc;
using RealDeal.Shared.Models;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseUrls("http://*:8084");

// Add services to the container.
builder.Services.AddAuthentication(NegotiateDefaults.AuthenticationScheme)
   .AddNegotiate();

builder.Services.AddAuthorization(options =>
{
	// By default, all incoming requests will be authorized according to the default policy.
	options.FallbackPolicy = options.DefaultPolicy;
});

var app = builder.Build();

// Configure the HTTP request pipeline.


app.MapPost("/notification", ([FromBody] User user, string message) =>
{
	Console.WriteLine($"{user.EmailAddress} : {message}");
});

app.Run();