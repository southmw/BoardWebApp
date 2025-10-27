using BoardApp.WebApp.Client.Pages;
using BoardApp.WebApp.Components;
using BoardApp.WebApp.Data;
using BoardApp.WebApp.Models;
using MudBlazor.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Configure Kestrel to accept large requests (for base64 images in editor)
builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxRequestBodySize = 100 * 1024 * 1024; // 100 MB
});

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

// Configure SignalR for large messages (for base64 images)
builder.Services.AddSignalR(options =>
{
    options.MaximumReceiveMessageSize = 100 * 1024 * 1024; // 100 MB
});

// Add MudBlazor services
builder.Services.AddMudServices();

// Add DbContext
builder.Services.AddDbContextFactory<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add Identity
builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
{
    // Password settings
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = false;

    // User settings
    options.User.RequireUniqueEmail = true;

    // Sign-in settings
    options.SignIn.RequireConfirmedEmail = false;
    options.SignIn.RequireConfirmedAccount = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// Add Google Authentication
builder.Services.AddAuthentication()
    .AddGoogle(options =>
    {
        options.ClientId = builder.Configuration["Authentication:Google:ClientId"] ?? "";
        options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"] ?? "";
    });

// Configure Application Cookie
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/account/login";
    options.LogoutPath = "/account/logout";
    options.AccessDeniedPath = "/account/access-denied";
    options.ExpireTimeSpan = TimeSpan.FromDays(7);
    options.SlidingExpiration = true;
});

// Add Authorization
builder.Services.AddAuthorizationBuilder()
    .AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"))
    .AddPolicy("RequireUserRole", policy => policy.RequireRole("User", "Admin"));

// Add Cascading Authentication State
builder.Services.AddCascadingAuthenticationState();

// Add Services
builder.Services.AddScoped<BoardApp.WebApp.Services.IBoardService, BoardApp.WebApp.Services.BoardService>();
builder.Services.AddScoped<BoardApp.WebApp.Services.IFileUploadService, BoardApp.WebApp.Services.FileUploadService>();
builder.Services.AddScoped<BoardApp.WebApp.Services.ICommentService, BoardApp.WebApp.Services.CommentService>();
builder.Services.AddScoped<BoardApp.WebApp.Services.ICategoryService, BoardApp.WebApp.Services.CategoryService>();
builder.Services.AddScoped<BoardApp.WebApp.Services.IUserService, BoardApp.WebApp.Services.UserService>();
builder.Services.AddScoped<BoardApp.WebApp.Services.IRoleService, BoardApp.WebApp.Services.RoleService>();
builder.Services.AddScoped<BoardApp.WebApp.Services.UserStateService>();

// Add HttpContextAccessor for accessing HttpContext in components
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Auto-migrate database
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var contextFactory = services.GetRequiredService<IDbContextFactory<ApplicationDbContext>>();
        using var context = await contextFactory.CreateDbContextAsync();
        await context.Database.MigrateAsync();
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while migrating the database.");
    }
}

// Seed default admin user
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();
    try
    {
        logger.LogInformation("Starting admin user creation process...");

        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = services.GetRequiredService<RoleManager<ApplicationRole>>();

        // Create admin user if it doesn't exist
        var adminEmail = "admin@southmw.com";
        var adminUser = await userManager.FindByEmailAsync(adminEmail);

        if (adminUser == null)
        {
            logger.LogInformation("Admin user not found, creating new admin user...");

            adminUser = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true,
                DisplayName = "관리자",
                CreatedAt = DateTime.Now
            };

            var result = await userManager.CreateAsync(adminUser, "Admin123!");

            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
                logger.LogInformation("Admin user created successfully with email: {Email}", adminEmail);
            }
            else
            {
                logger.LogError("Failed to create admin user. Errors: {Errors}",
                    string.Join(", ", result.Errors.Select(e => e.Description)));
            }
        }
        else
        {
            logger.LogInformation("Admin user already exists with email: {Email}", adminEmail);
        }
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "An error occurred while creating admin user.");
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseAntiforgery();

// Map authentication endpoints
app.MapPost("/account/perform-login", async (HttpContext context, SignInManager<ApplicationUser> signInManager) =>
{
    var form = await context.Request.ReadFormAsync();
    var email = form["email"].ToString();
    var password = form["password"].ToString();
    var rememberMe = form["rememberMe"].ToString() == "on";
    var returnUrl = form["returnUrl"].ToString();

    if (string.IsNullOrEmpty(returnUrl))
    {
        returnUrl = "/";
    }

    var result = await signInManager.PasswordSignInAsync(email, password, rememberMe, lockoutOnFailure: false);

    if (result.Succeeded)
    {
        context.Response.Redirect(returnUrl);
    }
    else
    {
        context.Response.Redirect($"/account/login?error=invalid&returnUrl={Uri.EscapeDataString(returnUrl)}");
    }
});

app.MapPost("/account/perform-register", async (HttpContext context, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager) =>
{
    var form = await context.Request.ReadFormAsync();
    var email = form["email"].ToString();
    var password = form["password"].ToString();
    var confirmPassword = form["confirmPassword"].ToString();
    var displayName = form["displayName"].ToString();

    if (password != confirmPassword)
    {
        context.Response.Redirect("/account/register?error=password-mismatch");
        return;
    }

    var user = new ApplicationUser
    {
        UserName = email,
        Email = email,
        EmailConfirmed = true,
        DisplayName = displayName,
        CreatedAt = DateTime.Now
    };

    var result = await userManager.CreateAsync(user, password);

    if (result.Succeeded)
    {
        await userManager.AddToRoleAsync(user, "User");
        await signInManager.SignInAsync(user, isPersistent: false);
        context.Response.Redirect("/");
    }
    else
    {
        var errors = Uri.EscapeDataString(string.Join(", ", result.Errors.Select(e => e.Description)));
        context.Response.Redirect($"/account/register?error={errors}");
    }
});

app.MapPost("/account/perform-logout", async (HttpContext context, SignInManager<ApplicationUser> signInManager) =>
{
    await signInManager.SignOutAsync();
    context.Response.Redirect("/");
});

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(BoardApp.WebApp.Client._Imports).Assembly);

app.Run();
