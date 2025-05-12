using Microsoft.EntityFrameworkCore;
using WebApplication1;
using WebApplication1.Models;

var builder = WebApplication.CreateBuilder(args);

// Конфигурация базы данных
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Конфигурация контроллеров
builder.Services.AddControllers();

// Настройка Swagger

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "darizefir_API", Version = "v1" });
});

// Регистрация фильтров (ваши кастомные фильтры)
builder.Services.AddScoped<ValidateModelAttribute>();
builder.Services.AddScoped<LogActionFilter>();
builder.Services.AddScoped<ConcurrencyCheckFilter>();
builder.Services.AddScoped<StockModificationFilter>();
builder.Services.AddScoped<ClientPhoneNumberFormatFilter>();
builder.Services.AddScoped<OrderPriorityFilter>();
builder.Services.AddScoped<ETagFilter>();
builder.Services.AddScoped<CacheResponseFilter>();
builder.Services.AddSingleton<DataService>();

var app = builder.Build();

// Включаем Swagger всегда (не только в Development)

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Flower Shop API V1");
    c.RoutePrefix = "swagger"; // Явно задаем маршрут
});

app.UseRouting();
app.MapControllers();

//app.MapGet("/", (ApplicationDbContext db) => db.Clients.ToList());
app.MapGet("/", () => Results.Redirect("/swagger"));
app.Run();