using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using RealDeal.Auth;
using RealDeal.Shared.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseUrls("http://*:8081");

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddDbContext<AuthDbContext>(options =>
{
	options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IConnectionFactory>(new ConnectionFactory
{
	HostName = "rabbitmq",
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

//app.Use(async (context, next) =>
//{
//	var secret = "SecretAuthApi";
//	if (!context.Request.Headers.TryGetValue("X-Api-Secret", out var providedSecret) || providedSecret != secret)
//	{
//		context.Response.StatusCode = StatusCodes.Status403Forbidden;
//		await context.Response.WriteAsync("Forbidden");
//		return;
//	}
//	await next();
//});

using (var scope = app.Services.CreateScope())
{
	var context = scope.ServiceProvider.GetRequiredService<AuthDbContext>();
	context.Database.EnsureCreated();
}

app.MapPost("/user", async ([FromBody] User user, [FromServices] AuthDbContext context, IConnectionFactory factory) =>
{
	context.Users.Add(user);
	await context.SaveChangesAsync();

	using var connection = await factory.CreateConnectionAsync();
	using var channel = await connection.CreateChannelAsync();

	await channel.QueueDeclareAsync(queue: "users", durable: false, exclusive: false, autoDelete: false, arguments: null);
	var body = Encoding.UTF8.GetBytes($"Confirmation de la création  d'un nouvel utilisateur : \n\tNom : {user.Name}\n\tAdresse mail : {user.EmailAddress}");
	await channel.BasicPublishAsync(exchange: string.Empty, routingKey: "users", body: body);

	return Results.Created($"/create/{user.Id}", user);
});

app.MapGet("/user/list", ([FromServices] AuthDbContext context) =>
{
	var result = context.Users;
	return Results.Ok(result);
});

app.MapGet("/user/{id:guid}", ([FromRoute] Guid id, [FromServices] AuthDbContext context) =>
{
	var result = context.Users.Where(u => u.Id == id).FirstOrDefault();
	return result is not null ? Results.Ok(result) : Results.NotFound();
});

app.Run();
