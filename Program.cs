using Microsoft.EntityFrameworkCore;
using SoruCevapPortal.Data;
using SoruCevapPortal.Models;
using SoruCevapPortal.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Add DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add Repository Pattern
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Add Authentication
builder.Services.AddAuthentication("CookieAuth")
    .AddCookie("CookieAuth", options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromHours(24);
        options.SlidingExpiration = true;
    });

// Add Authorization
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireClaim("Role", "Admin"));
});

var app = builder.Build();

// Seed Database
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();

        // Create Admin Role if not exists
        if (!context.Set<Role>().Any(r => r.Name == "Admin"))
        {
            var adminRole = new Role { Name = "Admin", CreatedAt = DateTime.Now };
            context.Set<Role>().Add(adminRole);
            context.SaveChanges();
        }

        // Create Admin User if not exists
        if (!context.Set<User>().Any(u => u.Username == "admin"))
        {
            var adminUser = new User
            {
                Username = "admin",
                Email = "admin@example.com",
                Password = "admin123",
                FullName = "Admin User",
                IsActive = true,
                CreatedAt = DateTime.Now
            };
            context.Set<User>().Add(adminUser);
            context.SaveChanges();

            // Assign Admin Role
            var adminRole = context.Set<Role>().First(r => r.Name == "Admin");
            var userRole = new UserRole
            {
                UserId = adminUser.Id,
                RoleId = adminRole.Id,
                CreatedAt = DateTime.Now
            };
            context.Set<UserRole>().Add(userRole);
            context.SaveChanges();
        }

        // Create sample categories if not exists
        if (!context.Set<Category>().Any())
        {
            var categories = new[]
            {
                new Category { Name = "Genel", Description = "Genel sorular", IsActive = true, CreatedAt = DateTime.Now },
                new Category { Name = "Teknoloji", Description = "Teknoloji ile ilgili sorular", IsActive = true, CreatedAt = DateTime.Now },
                new Category { Name = "Yazılım", Description = "Yazılım geliştirme soruları", IsActive = true, CreatedAt = DateTime.Now }
            };
            context.Set<Category>().AddRange(categories);
            context.SaveChanges();
        }

        // Create sample questions if not exists
        if (!context.Set<Question>().Any())
        {
            var adminUser = context.Set<User>().First(u => u.Username == "admin");
            var generalCategory = context.Set<Category>().First(c => c.Name == "Genel");
            var techCategory = context.Set<Category>().First(c => c.Name == "Teknoloji");
            var softwareCategory = context.Set<Category>().First(c => c.Name == "Yazılım");

            var questions = new List<Question>
            {
                new Question
                {
                    Title = "ASP.NET Core ile web API nasıl oluşturulur?",
                    Content = "Merhaba, ASP.NET Core kullanarak RESTful web API nasıl oluşturabilirim? Başlangıç için önerileriniz nelerdir?",
                    UserId = adminUser.Id,
                    CategoryId = softwareCategory.Id,
                    IsActive = true,
                    ViewCount = 145,
                    CreatedAt = DateTime.Now.AddDays(-15)
                },
                new Question
                {
                    Title = "Entity Framework Core migration hatası",
                    Content = "Migration yaparken 'The entity type requires a primary key to be defined' hatası alıyorum. Nasıl çözebilirim?",
                    UserId = adminUser.Id,
                    CategoryId = softwareCategory.Id,
                    IsActive = true,
                    ViewCount = 89,
                    CreatedAt = DateTime.Now.AddDays(-12)
                },
                new Question
                {
                    Title = "Hangi programlama dilini öğrenmeliyim?",
                    Content = "Yazılım dünyasına yeni başlıyorum. İlk hangi programlama dilini öğrenmeliyim? C# mı Python mi JavaScript mi?",
                    UserId = adminUser.Id,
                    CategoryId = techCategory.Id,
                    IsActive = true,
                    ViewCount = 256,
                    CreatedAt = DateTime.Now.AddDays(-20)
                },
                new Question
                {
                    Title = "React vs Vue.js karşılaştırması",
                    Content = "Yeni bir frontend projesi başlatıyorum. React mi yoksa Vue.js mi kullanmalıyım? Avantajları ve dezavantajları nelerdir?",
                    UserId = adminUser.Id,
                    CategoryId = softwareCategory.Id,
                    IsActive = true,
                    ViewCount = 187,
                    CreatedAt = DateTime.Now.AddDays(-18)
                },
                new Question
                {
                    Title = "Docker container'larını production'da nasıl kullanırım?",
                    Content = "Docker ile geliştirme ortamında çalışıyorum ama production'a geçişte nelere dikkat etmeliyim?",
                    UserId = adminUser.Id,
                    CategoryId = techCategory.Id,
                    IsActive = true,
                    ViewCount = 134,
                    CreatedAt = DateTime.Now.AddDays(-10)
                },
                new Question
                {
                    Title = "SQL injection saldırılarından nasıl korunurum?",
                    Content = "Web uygulamamda SQL injection güvenlik açığını nasıl önleyebilirim? En iyi pratikler nelerdir?",
                    UserId = adminUser.Id,
                    CategoryId = softwareCategory.Id,
                    IsActive = true,
                    ViewCount = 203,
                    CreatedAt = DateTime.Now.AddDays(-8)
                },
                new Question
                {
                    Title = "Microservices mimarisi nedir?",
                    Content = "Microservices mimarisi hakkında bilgi edinmek istiyorum. Monolithic'e göre avantajları nelerdir?",
                    UserId = adminUser.Id,
                    CategoryId = softwareCategory.Id,
                    IsActive = true,
                    ViewCount = 167,
                    CreatedAt = DateTime.Now.AddDays(-14)
                },
                new Question
                {
                    Title = "Git merge conflict nasıl çözülür?",
                    Content = "Git'te merge conflict ile karşılaştım. Bu sorunu nasıl çözebilirim?",
                    UserId = adminUser.Id,
                    CategoryId = softwareCategory.Id,
                    IsActive = true,
                    ViewCount = 92,
                    CreatedAt = DateTime.Now.AddDays(-6)
                },
                new Question
                {
                    Title = "Yapay zeka ve makine öğrenmesi arasındaki fark nedir?",
                    Content = "AI ve ML kavramları karışıyor. Aralarındaki temel farklar nelerdir?",
                    UserId = adminUser.Id,
                    CategoryId = techCategory.Id,
                    IsActive = true,
                    ViewCount = 312,
                    CreatedAt = DateTime.Now.AddDays(-25)
                },
                new Question
                {
                    Title = "Clean Code yazmanın en iyi yolları",
                    Content = "Daha okunabilir ve sürdürülebilir kod yazmak için önerileriniz nelerdir?",
                    UserId = adminUser.Id,
                    CategoryId = softwareCategory.Id,
                    IsActive = true,
                    ViewCount = 234,
                    CreatedAt = DateTime.Now.AddDays(-22)
                },
                new Question
                {
                    Title = "RESTful API tasarım prensipleri",
                    Content = "RESTful API tasarlarken hangi kurallara uymalıyım? Best practice'ler nelerdir?",
                    UserId = adminUser.Id,
                    CategoryId = softwareCategory.Id,
                    IsActive = true,
                    ViewCount = 178,
                    CreatedAt = DateTime.Now.AddDays(-16)
                },
                new Question
                {
                    Title = "Agile ve Scrum metodolojisi nedir?",
                    Content = "Yazılım geliştirmede Agile ve Scrum nasıl uygulanır? Temel kavramlar nelerdir?",
                    UserId = adminUser.Id,
                    CategoryId = generalCategory.Id,
                    IsActive = true,
                    ViewCount = 145,
                    CreatedAt = DateTime.Now.AddDays(-11)
                },
                new Question
                {
                    Title = "TypeScript mi JavaScript mi?",
                    Content = "TypeScript kullanmanın JavaScript'e göre avantajları nelerdir? Öğrenmeli miyim?",
                    UserId = adminUser.Id,
                    CategoryId = softwareCategory.Id,
                    IsActive = true,
                    ViewCount = 198,
                    CreatedAt = DateTime.Now.AddDays(-9)
                },
                new Question
                {
                    Title = "Redis cache kullanımı",
                    Content = "Redis nedir ve web uygulamamda nasıl kullanabilirim? Performance artışı sağlar mı?",
                    UserId = adminUser.Id,
                    CategoryId = techCategory.Id,
                    IsActive = true,
                    ViewCount = 156,
                    CreatedAt = DateTime.Now.AddDays(-7)
                },
                new Question
                {
                    Title = "Unit test yazmanın önemi",
                    Content = "Neden unit test yazmalıyım? Hangi test framework'ünü kullanmalıyım?",
                    UserId = adminUser.Id,
                    CategoryId = softwareCategory.Id,
                    IsActive = true,
                    ViewCount = 123,
                    CreatedAt = DateTime.Now.AddDays(-5)
                }
            };
            context.Set<Question>().AddRange(questions);
            context.SaveChanges();

            // Add sample answers
            var answers = new List<Answer>
            {
                new Answer
                {
                    Content = "ASP.NET Core ile Web API oluşturmak oldukça kolay. Öncelikle Visual Studio'da yeni bir ASP.NET Core Web API projesi oluşturmalısınız. Controller sınıfları oluşturarak endpoint'lerinizi tanımlayabilirsiniz. Swagger kullanarak API dokümantasyonunu otomatik oluşturabilirsiniz.",
                    QuestionId = questions[0].Id,
                    UserId = adminUser.Id,
                    IsApproved = true,
                    IsActive = true,
                    CreatedAt = DateTime.Now.AddDays(-14)
                },
                new Answer
                {
                    Content = "Yeni başlayanlar için Python önerilir çünkü syntax'ı daha kolay ve öğrenmesi daha hızlıdır. Ancak web geliştirme yapmak istiyorsanız JavaScript, sistem programlama için C# iyi seçeneklerdir. Hedeflerinize göre karar vermelisiniz.",
                    QuestionId = questions[2].Id,
                    UserId = adminUser.Id,
                    IsApproved = true,
                    IsActive = true,
                    CreatedAt = DateTime.Now.AddDays(-19)
                },
                new Answer
                {
                    Content = "React daha geniş bir ekosisteme sahip ve Facebook tarafından destekleniyor. Vue.js ise daha basit ve öğrenmesi daha kolay. Projenizin büyüklüğüne ve ekip tecrübesine göre seçim yapmalısınız.",
                    QuestionId = questions[3].Id,
                    UserId = adminUser.Id,
                    IsApproved = true,
                    IsActive = true,
                    CreatedAt = DateTime.Now.AddDays(-17)
                },
                new Answer
                {
                    Content = "SQL injection'dan korunmak için asla kullanıcı inputunu direkt SQL sorgusuna eklemeyin. Parameterized queries veya ORM (Entity Framework gibi) kullanın. Input validation yapın ve least privilege prensibini uygulayın.",
                    QuestionId = questions[5].Id,
                    UserId = adminUser.Id,
                    IsApproved = true,
                    IsActive = true,
                    CreatedAt = DateTime.Now.AddDays(-7)
                },
                new Answer
                {
                    Content = "Clean Code için: Anlamlı değişken isimleri kullanın, fonksiyonlarınızı küçük tutun, tekrarlardan kaçının (DRY), yorum yazmak yerine kodu kendini açıklayıcı hale getirin. Robert C. Martin'in 'Clean Code' kitabını okumanızı öneririm.",
                    QuestionId = questions[9].Id,
                    UserId = adminUser.Id,
                    IsApproved = true,
                    IsActive = true,
                    CreatedAt = DateTime.Now.AddDays(-21)
                },
                new Answer
                {
                    Content = "TypeScript, JavaScript'e type safety ekler. Büyük projelerde hataları derleme zamanında yakalamanızı sağlar. Kod editörünüzde daha iyi IntelliSense ve otomatik tamamlama sunar. Öğrenmeye değer!",
                    QuestionId = questions[12].Id,
                    UserId = adminUser.Id,
                    IsApproved = true,
                    IsActive = true,
                    CreatedAt = DateTime.Now.AddDays(-8)
                }
            };
            context.Set<Answer>().AddRange(answers);
            context.SaveChanges();
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
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
