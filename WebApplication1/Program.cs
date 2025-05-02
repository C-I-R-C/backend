using WebApplication1;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// Register filter services
builder.Services.AddScoped<ValidateModelAttribute>();
builder.Services.AddScoped<LogActionFilter>();
builder.Services.AddScoped<ConcurrencyCheckFilter>();
builder.Services.AddScoped<StockModificationFilter>();
builder.Services.AddScoped<ClientPhoneNumberFormatFilter>();
builder.Services.AddScoped<OrderPriorityFilter>();
builder.Services.AddScoped<ETagFilter>();
builder.Services.AddScoped<CacheResponseFilter>();

// Register with parameters

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
