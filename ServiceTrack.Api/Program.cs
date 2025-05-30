using AuthApp.application;
using AuthApp.infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using AuthApp.infrastructure.Data;
using Microsoft.OpenApi.Models;
using System.Reflection;
using AuthApp.Api.Middleware;
using ServiceTrack.Api.Middleware; // Убедитесь, что это ваш правильный namespace
using ServiceTrack.Api.Middleware.Logging; // Убедитесь, что это ваш правильный namespace
using Scalar.AspNetCore; // Для Scalar UI

var builder = WebApplication.CreateBuilder(args);

// Добавление сервисов в контейнер
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer(); // Необходим для Swagger/OpenAPI

// Конфигурация Swagger/OpenAPI через AddSwaggerGen
var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "ServiceTrack API",
        Version = "v1",
        Description = "API для управления оборудованием и его компонентами"
    });

    // Настройка авторизации для Swagger UI
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });

    // Подключение XML-комментариев
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

// Удаляем AddOpenApi, так как AddSwaggerGen делает всё необходимое
// builder.Services.AddOpenApi(options => { ... });

// Добавление слоев Application и Infrastructure
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// Настройка аутентификации JWT
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var key = Encoding.ASCII.GetBytes(jwtSettings["Secret"]); // Убедитесь, что ключ достаточно длинный (минимум 16 байт для HMACSHA256)

builder.Services.AddAuthentication(x =>
    {
        x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(x =>
    {
        x.RequireHttpsMetadata = false; // В production лучше true
        x.SaveToken = true;
        x.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = true, // Установите true, если вы проверяете издателя
            ValidateAudience = true, // Установите true, если вы проверяете аудиторию
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            ClockSkew = TimeSpan.Zero // Рекомендуется для точной проверки времени жизни токена
        };
    });

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger(); // Middleware для генерации swagger.json (по умолчанию /swagger/v1/swagger.json)

    // Настройка Scalar UI
    app.MapScalarApiReference("/", options =>
    {
        // Путь, по которому будет доступен пользовательский интерфейс Scalar.
        // Например, "/api-docs" или "/reference". По умолчанию "/reference".// Задает префикс маршрута для Scalar
        options.OpenApiRoutePattern = "/swagger/v1/swagger.json";
        // URL к файлу спецификации OpenAPI (который генерируется app.UseSwagger()).
        // По умолчанию app.UseSwagger() предоставляет swagger.json по пути /swagger/{documentName}/swagger.json
        // (т.е. /swagger/v1/swagger.json для документа "v1").
        // options.SpecUrl = "/swagger/v1/swagger.json"; // Это правильное свойство для указания URL спецификации

        // Заголовок страницы Scalar. В вашем исходном коде было options.Title.
        options.Title = "ServiceTrack API"; // Используем 'Title'

        // Эти настройки темы и макета из вашего исходного кода.
        // Если ScalarTheme и ScalarLayout существуют в вашем проекте и это валидные
        // опции для вашей версии Scalar.AspNetCore, оставьте их.
        // В стандартном Scalar.AspNetCore таких опций для C# напрямую может не быть,
        // темы обычно настраиваются через JS. Если они вызывают ошибку, закомментируйте.
        // options.Theme = ScalarTheme.BluePlanet;
        // options.Layout = ScalarLayout.Modern;
    });

    // Для удобства можно сделать редирект с корня на документацию в dev
    app.MapGet("/", context =>
    {
        context.Response.Redirect("/api-docs"); // Убедитесь, что этот путь совпадает с options.RoutePrefix
        return Task.CompletedTask;
    });
}


app.UseStaticFiles();
app.UseHttpsRedirection();

// Add request-response logging middleware before authentication
app.UseRequestResponseLogging(); // Убедитесь, что этот middleware существует и корректно работает

app.UseAuthentication();
app.UseAuthorization();

// Add exception handling middleware
app.UseMiddleware<ExceptionHandlingMiddleware>(); // Убедитесь, что этот middleware существует

app.MapControllers();

// Initialize database
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        await DbInitializer.InitializeAsync(context); // Убедитесь, что DbInitializer существует
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while initializing the database.");
    }
}

app.Run();