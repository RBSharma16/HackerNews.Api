using HackersNews.Service;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddLogging(builder =>
{
    builder.AddConsole();
    builder.AddDebug();
});
builder.Services.AddMemoryCache();
builder.Services.AddHttpClient();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(o => o.AddPolicy("CorsPpolicy", builder =>
{
    builder.AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader();
}));

builder.Services.AddScoped<IHackersNewsApiClient, HackersNewsApiClient>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("CorsPpolicy");
app.UseAuthorization();

app.MapControllers();

app.Run();
