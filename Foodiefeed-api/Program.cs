using Foodiefeed_api;
using Foodiefeed_api.entities;
using Foodiefeed_api.services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddDbContext<dbContext>();
builder.Services.AddScoped<IUserService,UserService>();
builder.Services.AddScoped<IFriendService, FriendService>();
builder.Services.AddScoped<IPostService, PostService>();
builder.Services.AddScoped<IFollowerService, FollowerService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<ICommentService, CommentService>();
builder.Services.AddScoped<IRecipeService, RecipeService>();
builder.Services.AddSingleton<IAzureBlobStorageSerivce, AzureBlobStorageService>();
builder.Services.AddScoped<IPasswordHasher<User> , PasswordHasher<User>>();
builder.Services.AddScoped<ErrorHandlingMiddleware>();
builder.Services.AddScoped(typeof(IEntityRepository<>), typeof(EntityRepository<>));
builder.Configuration.AddUserSecrets<Program>();

var app = builder.Build();

app.UseMiddleware<ErrorHandlingMiddleware>();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    DatabaseSeeder.SeedData(serviceProvider: app.Services);
}

app.MapGet("/get-all-tags", async (dbContext context, CancellationToken token) =>
{
    token.ThrowIfCancellationRequested();
    var tags = await context.Tags.ToListAsync();
    return Results.Ok(tags);
});

app.MapGet("/get-all-products", async (dbContext context,CancellationToken token) =>
{
    token.ThrowIfCancellationRequested();
    var products = await context.Products.ToListAsync();
    return Results.Ok(products);
});


//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
