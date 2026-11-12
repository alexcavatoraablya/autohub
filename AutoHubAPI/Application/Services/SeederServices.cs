using Application.Interfaces;
using Domain;
using Domain.Entities;
using Domain.Entities.TestHub;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Application.Services;

public class SeederService(AppDbContext db) : ISeederService
{
    public async Task UpdateDatabase()
    {
        await db.Database.MigrateAsync();
    }

    public async Task SeedVehicleStatusesAsync()
    {
        if (await db.VehicleStatuses.AnyAsync())
            return;

        var vStatuses = new List<VehicleStatusEntity>
        {
            new () { Name = "В наявності" },
            new () { Name = "Заброньовано" },
            new () { Name = "Продано" }
        };
        await db.VehicleStatuses.AddRangeAsync(vStatuses);
        await db.SaveChangesAsync();
    }

    public async Task SeedLeadStatusesAsync()
    {
        if (await db.LeadStatuses.AnyAsync())
            return;

        var lStatuses = new List<LeadStatusEntity>
        {
             new () { Name = "Нова" },
             new () { Name = "В роботі" },
             new () { Name = "Виконана" },
             new () { Name = "Відхилена" }
        };
        await db.LeadStatuses.AddRangeAsync(lStatuses);
        await db.SaveChangesAsync();
    }

    public async Task SeedFuelTypesAsync()
    {
        if (await db.FuelTypes.AnyAsync())
            return;

        var entities = new List<FuelTypeEntity>
        {
             new () { Name = "Дизель" },
             new () { Name = "Бензин" },
             new () { Name = "Газ" },
             new () { Name = "Газ/бензин" },
             new () { Name = "Гібрид" },
             new () { Name = "Електро" }
        };
        await db.FuelTypes.AddRangeAsync(entities);
        await db.SaveChangesAsync();
    }

    public async Task SeedBodyTypesAsync()
    {
        if (await db.BodyTypes.AnyAsync())
            return;

        var entities = new List<BodyTypeEntity>
        {
            new() { Name = "Бортовий", Slug = "bortovyi", SortOrder = 1 },
            new() { Name = "Рефрижератор", Slug = "refrizherator", SortOrder = 2 },
            new() { Name = "Тент", Slug = "tent", SortOrder = 3 },
            new() { Name = "Фургон", Slug = "furhon", SortOrder = 4 },
            new() { Name = "Дубль-кабіна борт", Slug = "dubl-kabina-bort", SortOrder = 5 },
            new() { Name = "Самоскид", Slug = "samoskyd", SortOrder = 6 },
            new() { Name = "Евакуатор", Slug = "evakuator", SortOrder = 7 },
            new() { Name = "Платформа", Slug = "platforma", SortOrder = 8 }
        };
        await db.BodyTypes.AddRangeAsync(entities);
        await db.SaveChangesAsync();
    }

    public async Task SeedBrandsAsync()
    {
        if (await db.Brands.AnyAsync())
            return;

        var entities = new List<BrandEntity>
        {
            new () { Name = "Volkswagen" },
            new () { Name = "Mercedes-Benz" },
            new () { Name = "MAN" },
            new () { Name = "Iveco" },
            new () { Name = "Renault" },
            new () { Name = "Ford" },
            new () { Name = "DAF" },
            new () { Name = "Volvo" },
            new () { Name = "Scania" },
            new () { Name = "Peugeot" }
        };
        await db.Brands.AddRangeAsync(entities);
        await db.SaveChangesAsync();
    }

    public async Task SeedVehiclesAsync()
    {
        if (await db.Vehicles.AnyAsync())
            return;

        // Завантажуємо словники для зв'язків
        var brands = await db.Brands.ToDictionaryAsync(x => x.Name);
        var bodies = await db.BodyTypes.ToDictionaryAsync(x => x.Name);
        var fuels = await db.FuelTypes.ToDictionaryAsync(x => x.Name);
        var statuses = await db.VehicleStatuses.ToDictionaryAsync(x => x.Name);

        var now = DateTime.UtcNow;
        var counter = 0;

        // Статична локальна функція для уникнення алокацій замикання
        static VehicleEntity V(
            string brand, string body, string model, string? subtitle,
            int year, int mileage, decimal engine, string fuel, decimal price,
            string description, string status, int daysAgo,
            Dictionary<string, BrandEntity> brandsDict,
            Dictionary<string, BodyTypeEntity> bodiesDict,
            Dictionary<string, FuelTypeEntity> fuelsDict,
            Dictionary<string, VehicleStatusEntity> statusesDict,
            DateTime nowTime, ref int idCounter,
            params (string Name, string Value)[] specs)
        {
            idCounter++;
            var vehicle = new VehicleEntity
            {
                Brand = brandsDict[brand],
                BodyType = bodiesDict[body],
                Model = model,
                Subtitle = subtitle,
                Year = year,
                Mileage = mileage,
                EngineVolume = engine,
                FuelType = fuelsDict[fuel],
                Price = price,
                Vin = $"SEED{idCounter:D13}",
                Description = description,
                Status = statusesDict[status],
                CreatedAt = nowTime.AddDays(-daysAgo),
                UpdatedAt = daysAgo > 10 ? nowTime.AddDays(-daysAgo + 5) : null
            };

            for (var i = 1; i <= 3; i++)
            {
                vehicle.Images.Add(new VehicleImageEntity
                {
                    Url = $"https://placehold.co/1200x800?text={Uri.EscapeDataString($"{brand} {model} {i}")}",
                    IsMain = i == 1,
                    SortOrder = i
                });
            }

            var order = 1;
            foreach (var (name, value) in specs)
            {
                vehicle.Specifications.Add(new VehicleSpecificationEntity
                {
                    Name = name,
                    Value = value,
                    SortOrder = order++
                });
            }

            return vehicle;
        }

        // Наповнюємо список
        var vehicles = new List<VehicleEntity>
        {
            V("Volkswagen", "Бортовий", "Crafter", "Комерційний автомобіль",
                2018, 245_000, 2.0m, "Дизель", 18_500m,
                "Бортовий Crafter в хорошому стані, обслуговувався у дилера. Дерев'яний борт, без ДТП.",
                "В наявності", 3, brands, bodies, fuels, statuses, now, ref counter,
                ("Коробка передач", "Механіка, 6 ст."), ("Потужність", "140 к.с."),
                ("Колісна база", "3640 мм"), ("Вантажопідйомність", "1 300 кг"), ("Привід", "Передній")),

            V("Volkswagen", "Фургон", "Crafter", "Цільнометалевий фургон",
                2019, 198_000, 2.0m, "Дизель", 21_900m,
                "Цільнометалевий фургон, обшита вантажна частина, дві розсувні двері.",
                "В наявності", 5, brands, bodies, fuels, statuses, now, ref counter,
                ("Коробка передач", "Механіка, 6 ст."), ("Потужність", "177 к.с."),
                ("Об'єм кузова", "11,3 м³"), ("Колісна база", "3640 мм"), ("Кондиціонер", "Так")),

            V("Mercedes-Benz", "Фургон", "Sprinter 316", "Комерційний автомобіль",
                2017, 320_000, 2.2m, "Дизель", 17_200m,
                "Sprinter 316 з Німеччини, свіжа розмальовка не потрібна, технічно справний.",
                "В наявності", 8, brands, bodies, fuels, statuses, now, ref counter,
                ("Коробка передач", "Механіка, 6 ст."), ("Потужність", "163 к.с."),
                ("Об'єм кузова", "10,5 м³"), ("Екологічний клас", "Euro 6")),

            V("Mercedes-Benz", "Рефрижератор", "Sprinter 314", "Ізотермічний рефрижератор",
                2016, 410_000, 2.1m, "Дизель", 15_800m,
                "Рефрижератор з установкою Thermo King, температурний режим від -20 до +20.",
                "Заброньовано", 14, brands, bodies, fuels, statuses, now, ref counter,
                ("Коробка передач", "Механіка, 6 ст."), ("Потужність", "143 к.с."),
                ("Холодильна установка", "Thermo King V-200"), ("Об'єм кузова", "14 м³")),

            V("Mercedes-Benz", "Дубль-кабіна борт", "Vario 815", "Дубль-кабіна, 7 місць",
                2008, 520_000, 4.3m, "Дизель", 14_300m,
                "Дубль-кабіна на 7 осіб, дерев'яна платформа 4,2 м, ідеально для бригад.",
                "В наявності", 20, brands, bodies, fuels, statuses, now, ref counter,
                ("Коробка передач", "Механіка, 5 ст."), ("Потужність", "150 к.с."),
                ("Довжина платформи", "4 200 мм"), ("Кількість місць", "7")),

            V("MAN", "Тент", "TGL 12.250", "Тентований вантажний",
                2015, 480_000, 6.9m, "Дизель", 24_500m,
                "Тентований MAN TGL, підвіска пневмо, кузов 7,2 м, сідлові ТО пройдені.",
                "В наявності", 11, brands, bodies, fuels, statuses, now, ref counter,
                ("Коробка передач", "Механіка, 6 ст."), ("Потужність", "250 к.с."),
                ("Довжина кузова", "7 200 мм"), ("Вантажопідйомність", "6 000 кг"))
        };

        // Вимикаємо відстеження змін для пришвидшення вставки великої кількості зв'язків
        db.ChangeTracker.AutoDetectChangesEnabled = false;
        try
        {
            await db.Vehicles.AddRangeAsync(vehicles);
            await db.SaveChangesAsync();
        }
        finally
        {
            db.ChangeTracker.AutoDetectChangesEnabled = true;
        }
    }


    /// <summary>
    /// Сідинг TestHub з файлу SeedData/tests.json (копіюється в output при build/publish).
    /// Ідемпотентний: тести шукаються за Title, питання — за Text у межах тесту; додаються лише відсутні.
    /// </summary>
    public async Task SeedTestHubAsync()
    {
        if (db.Tests.Any())
            return; //немає потреби виконувати цей seed якщо тест уже є

        var path = Path.Combine(AppContext.BaseDirectory, "SeedData", "tests.json");
        if (!File.Exists(path))
            throw new FileNotFoundException("Файл сідингу тестів не знайдено", path);

        var json = await File.ReadAllTextAsync(path);
        // Newtonsoft за замовчуванням зіставляє властивості без урахування регістру (title → Title)
        var seedTests = JsonConvert.DeserializeObject<List<TestSeedModel>>(json)
                        ?? new List<TestSeedModel>();

        foreach (var seed in seedTests)
        {
            var test = await db.Tests
                .Include(t => t.Questions)
                .FirstOrDefaultAsync(t => t.Title == seed.Title);

            if (test == null)
            {
                test = new TestEntity { Title = seed.Title, CreatedAt = DateTime.UtcNow };
                db.Tests.Add(test);
            }

            test.Description = seed.Description;
            test.TimeLimitMinutes = seed.TimeLimitMinutes;

            var existing = test.Questions.Select(q => q.Text).ToHashSet();
            for (var i = 0; i < seed.Questions.Count; i++)
            {
                var q = seed.Questions[i];
                if (existing.Contains(q.Text))
                    continue;

                test.Questions.Add(new QuestionEntity
                {
                    Text = q.Text,
                    OrderIndex = i + 1,
                    Points = q.Points,
                    AnswerOptions = q.Answers
                        .Select(a => new AnswerOptionEntity { Text = a.Text, IsCorrect = a.IsCorrect })
                        .ToList()
                });
            }
        }

        await db.SaveChangesAsync();
    }



}
