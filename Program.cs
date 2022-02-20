using moviesAPI.Data;
using moviesAPI.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<MoviesDatabaseSettings>(builder.Configuration.GetSection("MoviesDatabaseSettings"));
builder.Services.AddSingleton<MoviesService>();

// configurar swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// usar swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/", () => "Movies API");

app.MapGet("/api/movies", async (MoviesService moviesService) => await moviesService.GetAll());

app.MapGet("/api/movies/{id}", async (MoviesService moviesService, string id) =>
{
    var movie = await moviesService.Get(id);

    return movie is null ? Results.NotFound() : Results.Ok(movie);
});

app.MapPost("/api/movies", async (MoviesService moviesService, Movie movie) =>
{
    await moviesService.Create(movie);
    return Results.Ok();
});

app.MapPut("/api/movies/{id}", async (MoviesService moviesService, string id, Movie updatedMovie) =>
{
    var movie = await moviesService.Get(id);
    if(movie is null) return Results.NotFound();

    updatedMovie.Id = movie.Id;
    await moviesService.Update(id, updatedMovie);

    return Results.Ok();
});

app.MapDelete("/api/movies/{id}", async (MoviesService moviesService, string id) =>
{
    Movie movie = await moviesService.Get(id);

    if (movie is null) return Results.NotFound();

    await moviesService.Delete(movie.Id);

    return Results.NoContent();
});

app.Run();
