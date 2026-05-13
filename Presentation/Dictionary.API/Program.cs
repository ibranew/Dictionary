using Dictionary.Application;
using Dictionary.Application.Common.Authorization;
using Dictionary.Infrastructure;
using Dictionary.Persistence;
using Dictionary.Persistence.Seed;
using Microsoft.AspNetCore.Authentication.JwtBearer;

var builder = WebApplication.CreateBuilder(args);

// App Services
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddPersistenceServicesForApi(builder.Configuration);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AngularPolicy", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});


builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
});

/// Uygulamanýn role ve yetkilendirme politikalarýný tanýmladýðý bölümdür.
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(DictionaryPolicies.AdminOnly, policy =>
    {
        policy.RequireRole(DictionaryRoles.Admin);
    });

    options.AddPolicy(DictionaryPolicies.EditorAccess, policy =>
    {
        policy.RequireRole(DictionaryRoles.Admin, DictionaryRoles.Editor);
    });
});

//Swagger'a JWT ekle
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new()
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Bearer {token}"
    });
    options.AddSecurityRequirement(new()
    {
        {
            new() { Reference = new() { Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme, Id = "Bearer" } },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

app.UseCors("AngularPolicy");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// seeder
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    //await Seeder.SeedAll(services);
    //await Seeder.SeedRolesAndAdmin(services);
}


app.Run();