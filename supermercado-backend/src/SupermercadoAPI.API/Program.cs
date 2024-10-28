using SupermercadoAPI.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using SupermercadoAPI.Domain.Interfaces;
using SupermercadoAPI.Infrastructure.Repositories;
using System.Reflection;
using SupermercadoAPI.Application.Commands;
using SupermercadoAPI.Application.Handlers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using SupermercadoAPI.API.Settings;
using System.Text;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Configuração do CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder =>
        {
            builder
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader();
        });
});

// Configuração do JWT
var jwtSettingsSection = builder.Configuration.GetSection("JwtSettings");
builder.Services.Configure<JwtSettings>(jwtSettingsSection);

var jwtSettings = jwtSettingsSection.Get<JwtSettings>();
var key = Encoding.ASCII.GetBytes(jwtSettings.Secret);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false; // Falso para desenvolvimento
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidAudience = jwtSettings.ValidoEm,
        ValidIssuer = jwtSettings.Emissor
    };
});

// Configuração dos Controllers
builder.Services.AddControllers();

// Configuração do Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Supermercado API", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: 'Bearer {token}'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
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
});

// Configuração do DbContext
builder.Services.AddDbContext<SupermercadoDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlOptions =>
        {
            sqlOptions.EnableRetryOnFailure(
                maxRetryCount: 5,
                maxRetryDelay: TimeSpan.FromSeconds(30),
                errorNumbersToAdd: null);
        }));

// Registro dos serviços
builder.Services.AddScoped<IProdutoRepository, ProdutoRepository>();

// Registro do MediatR
builder.Services.AddMediatR(cfg => {
    cfg.RegisterServicesFromAssembly(typeof(AdicionarProdutoCommand).Assembly);
    cfg.RegisterServicesFromAssembly(typeof(AdicionarProdutoCommandHandler).Assembly);
    cfg.RegisterServicesFromAssembly(typeof(AtualizarProdutoCommand).Assembly);
    cfg.RegisterServicesFromAssembly(typeof(ExcluirProdutoCommand).Assembly);
    cfg.RegisterServicesFromAssembly(typeof(ObterProdutoPorIdQueryHandler).Assembly);
    cfg.RegisterServicesFromAssembly(typeof(ObterTodosProdutosQueryHandler).Assembly);
});

var app = builder.Build();

// Configuração do ambiente de desenvolvimento
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Supermercado API v1"));
}

// Migração do banco de dados
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<SupermercadoDbContext>();
        context.Database.Migrate();
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Ocorreu um erro ao migrar o banco de dados.");
    }
}

// Configuração do pipeline HTTP
app.UseHttpsRedirection(); // Remova esta linha se quiser usar apenas HTTP em desenvolvimento
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();