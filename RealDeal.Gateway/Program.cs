using Microsoft.AspNetCore.Mvc;
using RealDeal.Shared.Models;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseUrls("http://*:8080");

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient("BetApiClient", client =>
{
	client.DefaultRequestHeaders.Add("X-Api-Secret", "SecretBetApi");
});
builder.Services.AddHttpClient("AuthApiClient", client =>
{
	client.DefaultRequestHeaders.Add("X-Api-Secret", "SecretAuthApi");
});

var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

#region AUTH

var authApiUrl = app.Configuration.GetValue<string>("ApiUrls:Auth");

app.MapGet("/user/{id:guid}", async ([FromRoute] Guid id, [FromServices] IHttpClientFactory HttpClient) =>
{
	var client = HttpClient.CreateClient("AuthApiClient");
	var response = await client.GetAsync($"{authApiUrl}/user/{id}");

	if (!response.IsSuccessStatusCode)
		return Results.StatusCode((int)response.StatusCode);

	var content = await response.Content.ReadAsStringAsync();
	return Results.Content(content, contentType: "application/json");
});

app.MapPost("/user", async ([FromBody] User user, [FromServices] IHttpClientFactory HttpClient) =>
{
	var client = HttpClient.CreateClient("AuthApiClient");
	var response = await client.PostAsJsonAsync($"{authApiUrl}/user", user);

	if (!response.IsSuccessStatusCode)
		return Results.StatusCode((int)response.StatusCode);

	var content = await response.Content.ReadAsStringAsync();
	return Results.Content(content, contentType: "application/json");
});

async Task<bool> IsAuthenticated(Guid userId, [FromServices] IHttpClientFactory HttpClient)
{
	try
	{
		var client = HttpClient.CreateClient("AuthApiClient");
		var response = await client.GetAsync($"{authApiUrl}/user/{userId}");
		return response.IsSuccessStatusCode;
	}
	catch
	{
		return false;
	}
}

#endregion

#region BET

var betApiUrl = app.Configuration.GetValue<string>("ApiUrls:Bet");




//IL FAUT LIRE CECI


// Il vous reste à ajouter dans tous les endpoints du gateway : [FromHeader] Guid userId dans la signature de la méthode. Ajoutez également le if avec la méthode IsAuthenticated. Il faut mettre dans le header un id d'un user dans la BDD, sinonon est pas autorisé



app.MapGet("/bet/{id:guid}", async ([FromRoute] Guid id, [FromHeader] Guid userId, [FromServices] IHttpClientFactory HttpClient) =>
{
	if (!await IsAuthenticated(userId, HttpClient))
		return Results.Unauthorized();

	var client = HttpClient.CreateClient("BetApiClient");
	var response = await client.GetAsync($"{betApiUrl}/bet/{id}");

	if (!response.IsSuccessStatusCode)
		return Results.StatusCode((int)response.StatusCode);

	var content = await response.Content.ReadAsStringAsync();
	return Results.Content(content, contentType: "application/json");
});

app.MapGet("/bet/list", async ([FromServices] IHttpClientFactory HttpClient) =>
{
	var client = HttpClient.CreateClient("BetApiClient");
	var response = await client.GetAsync(betApiUrl + "/bet/list");

	if (!response.IsSuccessStatusCode)
		return Results.StatusCode((int)response.StatusCode);

	var content = await response.Content.ReadAsStringAsync();
	return Results.Content(content, contentType: "application/json");
});

app.MapPost("/bet", async ([FromBody] Bet bet, [FromServices] IHttpClientFactory HttpClient) =>
{
	var client = HttpClient.CreateClient("BetApiClient");
	var response = await client.PostAsJsonAsync(betApiUrl + "/bet", bet);

	if (!response.IsSuccessStatusCode)
		return Results.StatusCode((int)response.StatusCode);

	var content = await response.Content.ReadAsStringAsync();
	return Results.Content(content, contentType: "application/json");
});

app.MapPut("/bet", async ([FromBody] Bet bet, [FromServices] IHttpClientFactory HttpClient) =>
{
	var client = HttpClient.CreateClient("BetApiClient");
	var response = await client.PutAsJsonAsync(betApiUrl + "/bet", bet);

	if (!response.IsSuccessStatusCode)
		return Results.StatusCode((int)response.StatusCode);

	return Results.NoContent();
});

app.MapDelete("/bet/{id:guid}", async ([FromRoute] Guid id, [FromServices] IHttpClientFactory HttpClient) =>
{
	var client = HttpClient.CreateClient("BetApiClient");
	var response = await client.DeleteAsync($"{betApiUrl}/bet/{id}");

	if (!response.IsSuccessStatusCode)
		return Results.StatusCode((int)response.StatusCode);

	return Results.NoContent();
});


#endregion

#region Match

var matchApiUrl = app.Configuration.GetValue<string>("ApiUrls:Match");


// Il vous reste à ajouter dans tous les endpoints du gateway : [FromHeader] Guid userId dans la signature de la méthode. Ajoutez également le if avec la méthode IsAuthenticated. Il faut mettre dans le header un id d'un user dans la BDD, sinonon est pas autorisé


app.MapGet("/match/{id:guid}", async ([FromRoute] Guid id, [FromHeader] Guid userId, [FromServices] IHttpClientFactory HttpClient) =>
{
	if (!await IsAuthenticated(userId, HttpClient))
		return Results.Unauthorized();

	var client = HttpClient.CreateClient("MatchApiClient");
	var response = await client.GetAsync($"{matchApiUrl}/match/{id}");

	if (!response.IsSuccessStatusCode)
		return Results.StatusCode((int)response.StatusCode);

	var content = await response.Content.ReadAsStringAsync();
	return Results.Content(content, contentType: "application/json");
});

app.MapGet("/match/list", async ([FromHeader] Guid userId, [FromServices] IHttpClientFactory HttpClient) =>
{
	if (!await IsAuthenticated(userId, HttpClient))
		return Results.Unauthorized();

	var client = HttpClient.CreateClient("MatchApiClient");
	var response = await client.GetAsync(matchApiUrl + "/match/list");

	if (!response.IsSuccessStatusCode)
		return Results.StatusCode((int)response.StatusCode);

	var content = await response.Content.ReadAsStringAsync();
	return Results.Content(content, contentType: "application/json");
});


#endregion

app.Run();