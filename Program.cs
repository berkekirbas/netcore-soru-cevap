using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SoruCevapPortal.Data;
using SoruCevapPortal.Hubs;
using SoruCevapPortal.Models;
using SoruCevapPortal.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Add DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add Identity
builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
{
    // Password settings
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;
    options.Password.RequiredUniqueChars = 1;

    // Lockout settings
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;

    // User settings
    options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    options.User.RequireUniqueEmail = true;

    // SignIn settings
    options.SignIn.RequireConfirmedEmail = false;
    options.SignIn.RequireConfirmedPhoneNumber = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// Configure Cookie Authentication
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.ExpireTimeSpan = TimeSpan.FromHours(24);
    options.SlidingExpiration = true;
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
});

// Add Repository Pattern
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Add SignalR
builder.Services.AddSignalR();

// Add Admin Notification Service
builder.Services.AddScoped<IAdminNotificationService, AdminNotificationService>();

var app = builder.Build();

// Seed Database
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = services.GetRequiredService<RoleManager<ApplicationRole>>();

        // Ensure database is created
        context.Database.Migrate();

        // Create Admin Role if not exists
        if (!await roleManager.RoleExistsAsync("Admin"))
        {
            await roleManager.CreateAsync(new ApplicationRole 
            { 
                Name = "Admin", 
                Description = "Sistem yöneticisi",
                CreatedAt = DateTime.Now 
            });
        }

        // Create User Role if not exists
        if (!await roleManager.RoleExistsAsync("User"))
        {
            await roleManager.CreateAsync(new ApplicationRole 
            { 
                Name = "User", 
                Description = "Normal kullanıcı",
                CreatedAt = DateTime.Now 
            });
        }

        // Create Moderator Role if not exists
        if (!await roleManager.RoleExistsAsync("Moderator"))
        {
            await roleManager.CreateAsync(new ApplicationRole 
            { 
                Name = "Moderator", 
                Description = "İçerik moderatörü",
                CreatedAt = DateTime.Now 
            });
        }

        // Create Admin User if not exists
        var adminUser = await userManager.FindByNameAsync("admin");
        if (adminUser == null)
        {
            adminUser = new ApplicationUser
            {
                UserName = "admin",
                Email = "admin@example.com",
                FullName = "Sistem Yöneticisi",
                IsActive = true,
                EmailConfirmed = true,
                CreatedAt = DateTime.Now
            };
            
            var result = await userManager.CreateAsync(adminUser, "Admin123!");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }
        }

        // Create sample categories if not exists
        if (!context.Categories.Any())
        {
            var categories = new[]
            {
                new Category { Name = "Genel", Description = "Genel sorular", IsActive = true, CreatedAt = DateTime.Now },
                new Category { Name = "Teknoloji", Description = "Teknoloji ile ilgili sorular", IsActive = true, CreatedAt = DateTime.Now },
                new Category { Name = "Yazılım", Description = "Yazılım geliştirme soruları", IsActive = true, CreatedAt = DateTime.Now },
                new Category { Name = "Bilim", Description = "Bilim ve doğa ile ilgili sorular", IsActive = true, CreatedAt = DateTime.Now },
                new Category { Name = "Eğitim", Description = "Eğitim ve öğretim soruları", IsActive = true, CreatedAt = DateTime.Now }
            };
            context.Categories.AddRange(categories);
            await context.SaveChangesAsync();
        }

        // Create sample questions if not exists
        if (!context.Questions.Any())
        {
            var admin = await userManager.FindByNameAsync("admin");
            if (admin != null)
            {
                var generalCategory = context.Categories.First(c => c.Name == "Genel");
                var techCategory = context.Categories.First(c => c.Name == "Teknoloji");
                var softwareCategory = context.Categories.First(c => c.Name == "Yazılım");

                var questions = new List<Question>
                {
                    new Question
                    {
                        Title = "ASP.NET Core ile web API nasıl oluşturulur?",
                        Content = "Merhaba, ASP.NET Core kullanarak RESTful web API nasıl oluşturabilirim? Başlangıç için önerileriniz nelerdir?",
                        UserId = admin.Id,
                        CategoryId = softwareCategory.Id,
                        IsActive = true,
                        ViewCount = 145,
                        CreatedAt = DateTime.Now.AddDays(-15)
                    },
                    new Question
                    {
                        Title = "Entity Framework Core migration hatası",
                        Content = "Migration yaparken 'The entity type requires a primary key to be defined' hatası alıyorum. Nasıl çözebilirim?",
                        UserId = admin.Id,
                        CategoryId = softwareCategory.Id,
                        IsActive = true,
                        ViewCount = 89,
                        CreatedAt = DateTime.Now.AddDays(-12)
                    },
                    new Question
                    {
                        Title = "Hangi programlama dilini öğrenmeliyim?",
                        Content = "Yazılım dünyasına yeni başlıyorum. İlk hangi programlama dilini öğrenmeliyim? C# mı Python mi JavaScript mi?",
                        UserId = admin.Id,
                        CategoryId = techCategory.Id,
                        IsActive = true,
                        ViewCount = 256,
                        CreatedAt = DateTime.Now.AddDays(-20)
                    },
                    new Question
                    {
                        Title = "React vs Vue.js karşılaştırması",
                        Content = "Yeni bir frontend projesi başlatıyorum. React mi yoksa Vue.js mi kullanmalıyım? Avantajları ve dezavantajları nelerdir?",
                        UserId = admin.Id,
                        CategoryId = softwareCategory.Id,
                        IsActive = true,
                        ViewCount = 187,
                        CreatedAt = DateTime.Now.AddDays(-18)
                    },
                    new Question
                    {
                        Title = "Docker container'larını production'da nasıl kullanırım?",
                        Content = "Docker ile geliştirme ortamında çalışıyorum ama production'a geçişte nelere dikkat etmeliyim?",
                        UserId = admin.Id,
                        CategoryId = techCategory.Id,
                        IsActive = true,
                        ViewCount = 134,
                        CreatedAt = DateTime.Now.AddDays(-10)
                    },
                    new Question
                    {
                        Title = "SQL injection saldırılarından nasıl korunurum?",
                        Content = "Web uygulamamda SQL injection güvenlik açığını nasıl önleyebilirim? En iyi pratikler nelerdir?",
                        UserId = admin.Id,
                        CategoryId = softwareCategory.Id,
                        IsActive = true,
                        ViewCount = 203,
                        CreatedAt = DateTime.Now.AddDays(-8)
                    },
                    new Question
                    {
                        Title = "Microservices mimarisi nedir?",
                        Content = "Microservices mimarisi hakkında bilgi edinmek istiyorum. Monolithic'e göre avantajları nelerdir?",
                        UserId = admin.Id,
                        CategoryId = softwareCategory.Id,
                        IsActive = true,
                        ViewCount = 167,
                        CreatedAt = DateTime.Now.AddDays(-14)
                    },
                    new Question
                    {
                        Title = "Yapay zeka ve makine öğrenmesi arasındaki fark nedir?",
                        Content = "AI ve ML kavramları karışıyor. Aralarındaki temel farklar nelerdir?",
                        UserId = admin.Id,
                        CategoryId = techCategory.Id,
                        IsActive = true,
                        ViewCount = 312,
                        CreatedAt = DateTime.Now.AddDays(-25)
                    }
                };
                context.Questions.AddRange(questions);
                await context.SaveChangesAsync();

                // Add sample answers
                var firstQuestion = questions[0];
                var answers = new List<Answer>
                {
                    new Answer
                    {
                        Content = "ASP.NET Core ile Web API oluşturmak oldukça kolay. Öncelikle Visual Studio'da yeni bir ASP.NET Core Web API projesi oluşturmalısınız. Controller sınıfları oluşturarak endpoint'lerinizi tanımlayabilirsiniz.",
                        QuestionId = firstQuestion.Id,
                        UserId = admin.Id,
                        IsApproved = true,
                        IsActive = true,
                        CreatedAt = DateTime.Now.AddDays(-14)
                    },
                    new Answer
                    {
                        Content = "Yeni başlayanlar için Python önerilir çünkü syntax'ı daha kolay. Ancak web geliştirme için JavaScript, sistem programlama için C# iyi seçeneklerdir.",
                        QuestionId = questions[2].Id,
                        UserId = admin.Id,
                        IsApproved = true,
                        IsActive = true,
                        CreatedAt = DateTime.Now.AddDays(-19)
                    }
                };
                context.Answers.AddRange(answers);
                await context.SaveChangesAsync();
            }
        }
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database.");
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// Map SignalR Hub
app.MapHub<AdminHub>("/adminHub");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
