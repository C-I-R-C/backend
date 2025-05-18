using Microsoft.EntityFrameworkCore;
using WebApplication1;
using WebApplication1.Models;
using WebApplication1.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
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
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme",
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[]{}
        }
    });
});

builder.Services.AddScoped<ClientsService>();
builder.Services.AddScoped<OrderService>();
builder.Services.AddScoped<IngredientsService>();
builder.Services.AddScoped<ItemsService>();
builder.Services.AddScoped<FlowersService>();
builder.Services.AddScoped<FlowerIngredientsService>();
builder.Services.AddScoped<ItemFlowersService>();
builder.Services.AddScoped<TokenService>();

builder.Services.AddScoped<ValidateModelAttribute>();
builder.Services.AddScoped<LogActionFilter>();
builder.Services.AddScoped<ConcurrencyCheckFilter>();
builder.Services.AddScoped<StockModificationFilter>();
builder.Services.AddScoped<ClientPhoneNumberFormatFilter>();
builder.Services.AddScoped<OrderPriorityFilter>();
builder.Services.AddScoped<ETagFilter>();
builder.Services.AddScoped<CacheResponseFilter>();

builder.Services.AddCors(option =>
{
    option.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:3000");
        policy.AllowAnyHeader();
        policy.AllowAnyMethod();
    });
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? string.Empty))
        };
    });
builder.Services.AddAuthorization();
var app = builder.Build();

// Включаем Swagger всегда (не только в Development)

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Flower Shop API V1");
    c.RoutePrefix = "swagger"; // Явно задаем маршрут
});

app.UseRouting();
app.UseCors();
app.MapControllers();

app.UseAuthentication();
app.UseAuthorization();

//app.MapGet("/", (ApplicationDbContext db) => db.Clients.ToList());
app.MapGet("/", () => Results.Redirect("/swagger"));
app.Run();