using Microsoft.EntityFrameworkCore;
using BeanScene.Data;
using BeanScene.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<BeanSceneContext>(options =>
        options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))
    ));

builder.Services.AddScoped<IReservationService, ReservationService>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.ASCII.GetBytes(builder.Configuration["Jwt:Secret"]!)),
        ValidateIssuer = false,
        ValidateAudience = false,
        ClockSkew = TimeSpan.Zero
    };

    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var token = context.Request.Headers["Authorization"]
                .FirstOrDefault()?.Split(" ").Last() 
                ?? context.Request.Cookies["jwt_token"];

            if (!string.IsNullOrEmpty(token))
            {
                context.Token = token;
            }
            return Task.CompletedTask;
        },
        OnChallenge = async context =>
        {
            // Don't redirect to login page for API endpoints
            if (!context.Request.Path.StartsWithSegments("/api"))
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Unauthorized");
                context.HandleResponse(); // Suppress the default redirect
            }
        }
    };
});

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.WithOrigins("http://localhost:3000")
               .AllowAnyMethod()
               .AllowAnyHeader()
               .AllowCredentials();
    });
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireManagerRole", policy => 
        policy.RequireRole("Manager"));
    options.AddPolicy("RequireStaffRole", policy => 
        policy.RequireRole("Manager", "Staff"));
    options.AddPolicy("RequireMemberRole", policy => 
        policy.RequireRole("Manager", "Staff", "Member"));
});

builder.Services.AddScoped<IAuthService, AuthService>();

// Build App
var app = builder.Build();

// SEED DATABASE
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<BeanSceneContext>();
     
        // Apply migrations
        if (context.Database.GetPendingMigrations().Any())
        {
            context.Database.Migrate();
        }
        
        // Seed data
        DbSeeder.Initialize(context);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while creating/seeding the database.");
        throw; // Re-throw the exception if you want to prevent the app from starting with a broken DB
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseCors();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();