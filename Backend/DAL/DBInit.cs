using Microsoft.EntityFrameworkCore;
using Backend.Models;

namespace Backend.DAL
{
    public static class DBInit
    {
        public static void Seed(IApplicationBuilder app)
        {
            try
            {
                using var serviceScope = app.ApplicationServices.CreateScope();
                var context = serviceScope.ServiceProvider.GetRequiredService<AppDbContext>();

                var loggerFactory = app.ApplicationServices.GetRequiredService<ILoggerFactory>();
                var logger = loggerFactory.CreateLogger("DBInit");

                // Check database connection
                if (!context.Database.CanConnect())
                {
                    logger.LogInformation("Database not accessible, ensuring it's created");
                    context.Database.EnsureCreated();
                }

                // Seed images from wwwroot/images folder
                var env = serviceScope.ServiceProvider.GetRequiredService<IWebHostEnvironment>();
                string imagesFolder = Path.Combine(env.WebRootPath, "images");
                if (Directory.Exists(imagesFolder))
                {
                    logger.LogInformation("Seeding images from {folderPath}", imagesFolder);
                    var imageFiles = Directory.GetFiles(imagesFolder);
                    foreach (var filePath in imageFiles)
                    {
                        var fileName = Path.GetFileName(filePath);

                        // Check if the image already exists
                        if (!context.Images.Any(img => img.ImageName == fileName))
                        {
                            context.Images.Add(new Image
                            {
                                ImageName = fileName,
                                ImagePath = $"/images/{fileName}"
                            });
                        }
                    }
                    context.SaveChanges();
                }
                else
                {
                    logger.LogWarning("Images folder not found at {folderPath}", imagesFolder);
                }

                // Opprett kategorier
                if (!context.Categories.Any())
                {
                    logger.LogInformation("Seeding categories");
                    var categories = new List<Category>
                    {
                        new Category { Name = "Grunningredienser" },
                        new Category { Name = "Sirup" },
                        new Category { Name = "Meieri" },
                        new Category { Name = "Topping" },
                        new Category { Name = "Frukt" },
                        new Category { Name = "Brus" },
                        new Category { Name = "KaffeVarm"},
                        new Category { Name = "KaffeKald"},
                        new Category { Name = "MatchaVarm"},
                        new Category { Name = "MatchaKald"},
                        new Category { Name = "Smoothie"},
                    };
                    context.Categories.AddRange(categories);
                    context.SaveChanges();
                }

                // Hent kategorier
                var grunn = context.Categories.First(c => c.Name == "Grunningredienser");
                var sirup = context.Categories.First(c => c.Name == "Sirup");
                var meieri = context.Categories.First(c => c.Name == "Meieri");
                var topping = context.Categories.First(c => c.Name == "Topping");
                var frukt = context.Categories.First(c => c.Name == "Frukt");
                var brus = context.Categories.First(c => c.Name == "Brus");
                var kaffeVarm = context.Categories.First(c => c.Name == "KaffeVarm");
                var kaffeKald = context.Categories.First(c => c.Name == "KaffeKald");
                var matchaVarm = context.Categories.First(c => c.Name == "MatchaVarm");
                var matchaKald = context.Categories.First(c => c.Name == "MatchaKald");
                var smoothie = context.Categories.First(c => c.Name == "Smoothie");

                // Opprett ingredienser
                if (!context.Ingredients.Any())
                {
                    logger.LogInformation("Seeding ingredients");
                    string GetImagePath(string ingredientName, int index)
                    {
                        if (ingredientName.Contains("Espresso", StringComparison.OrdinalIgnoreCase))
                            return "/images/Test2.png";
                        if (ingredientName.Contains("Matcha", StringComparison.OrdinalIgnoreCase))
                            return "/images/Test1.png";
                        if (ingredientName.Contains("Smoothie", StringComparison.OrdinalIgnoreCase))
                            return "/images/Test3.png";

                        // Alternate filler images:
                        return (index % 2 == 0)
                            ? "/images/Filler1.png"
                            : "/images/Filler2.png";
                    }

                    var allIngredients = new List<Ingredient>
                    {
                        // Grunningredienser – 25%
                        new Ingredient
                        {
                            Name = "Espresso",
                            Description = "Intens kaffeshot\nGir kraftig smak og energi\nKoffein: ca. 63mg per shot",
                            Category = grunn,
                            IsAvailable = true,
                            UnitPrice = 10,
                            FillLevel = 25,
                            Color = "#4B2E2E" // Deep espresso brown
                        },
                        new Ingredient
                        {
                            Name = "Matcha",
                            Description = "Finmalt grønn tepulver\nRik på antioksidanter\nKoffein: ca. 70mg per ts",
                            Category = grunn,
                            IsAvailable = true,
                            UnitPrice = 10,
                            FillLevel = 25,
                            Color = "#7DD071" // Vibrant green for matcha
                        },

                        // Sirup – 10%
                        new Ingredient
                        {
                            Name = "Sjokolade saus",
                            Description = "Søt sjokoladesmak\nGir fylde og kakaoaroma\nPerfekt for dessertdrikker",
                            Category = sirup,
                            IsAvailable = true,
                            UnitPrice = 5,
                            FillLevel = 10,
                            Color = "#6F4E37" // Chocolate brown
                        },
                        new Ingredient
                        {
                            Name = "Vanilje",
                            Description = "Myk vaniljesmak\nGir sødme og rundhet\nKlassisk sirupvalg",
                            Category = sirup,
                            IsAvailable = true,
                            UnitPrice = 5,
                            FillLevel = 10,
                            Color = "#F3E5AB" // Warm vanilla tone
                        },
                        new Ingredient
                        {
                            Name = "Kokos sirup",
                            Description = "Lett kokosaroma\nTropisk preg\nPasser godt med melk",
                            Category = sirup,
                            IsAvailable = true,
                            UnitPrice = 5,
                            FillLevel = 10,
                            Color = "#F0EAD6" // Light coconut beige
                        },
                        new Ingredient
                        {
                            Name = "Karamell saus",
                            Description = "Fyldig karamellsmak\nSøt og smøraktig\nGir myk munnfølelse",
                            Category = sirup,
                            IsAvailable = true,
                            UnitPrice = 5,
                            FillLevel = 10,
                            Color = "#C68E17" // Caramel gold
                        },
                        new Ingredient
                        {
                            Name = "Blåbærsaus",
                            Description = "Fruktig blåbærsmak\nNaturlig sødme\nGir frisk kontrast",
                            Category = sirup,
                            IsAvailable = true,
                            UnitPrice = 5,
                            FillLevel = 10,
                            Color = "#4B0082" // Deep blueberry purple
                        },
                        new Ingredient
                        {
                            Name = "Mint sirup",
                            Description = "Frisk myntesmak\nGir kjølende effekt\nPasser godt med sjokolade",
                            Category = sirup,
                            IsAvailable = true,
                            UnitPrice = 5,
                            FillLevel = 10,
                            Color = "#98FF98" // Minty green
                        },
                        new Ingredient
                        {
                            Name = "Hvit sjokolade saus",
                            Description = "Kremet hvit sjokolade\nSøt og mild\nPasser med bær og nøtter",
                            Category = sirup,
                            IsAvailable = true,
                            UnitPrice = 5,
                            FillLevel = 10,
                            Color = "#F8F8FF" // Almost white
                        },
                        new Ingredient
                        {
                            Name = "Honning",
                            Description = "Naturlig sødme\nLett floral aroma\nGir varme og dybde",
                            Category = sirup,
                            IsAvailable = true,
                            UnitPrice = 5,
                            FillLevel = 10,
                            Color = "#FEE5AC" // Honey gold
                        },
                        new Ingredient
                        {
                            Name = "Kondensert melk",
                            Description = "Søt og tykk melk\nGir karamellisert smak\nFløyelsmyk tekstur",
                            Category = sirup,
                            IsAvailable = true,
                            UnitPrice = 5,
                            FillLevel = 10,
                            Color = "#F8E5C7" // Creamy light beige
                        },

                        // Meieri – 15%
                        new Ingredient
                        {
                            Name = "Melk - hel",
                            Description = "Helmelk for fyldig smak\nGir kremet konsistens\nInneholder naturlig fett",
                            Category = meieri,
                            IsAvailable = true,
                            UnitPrice = 5,
                            FillLevel = 15,
                            Color = "#FFFFFF" // Pure white
                        },
                        new Ingredient
                        {
                            Name = "Krem",
                            Description = "Fløtekrem til topping\nLuftig og rik\nBrukes på varme og kalde drikker",
                            Category = meieri,
                            IsAvailable = true,
                            UnitPrice = 5,
                            FillLevel = 15,
                            Color = "#FFFDD0" // Cream color
                        },
                        new Ingredient
                        {
                            Name = "Kremost",
                            Description = "Myk kremost\nGir syrlig og kremet smak\nVanlig i te-drinker",
                            Category = meieri,
                            IsAvailable = true,
                            UnitPrice = 5,
                            FillLevel = 15,
                            Color = "#FFF8DC" // Light cream cheese tone
                        },
                        new Ingredient
                        {
                            Name = "Iskrem",
                            Description = "Kremete iskuler\nGir kulde og fylde\nBrukes ofte i dessertdrikker",
                            Category = meieri,
                            IsAvailable = true,
                            UnitPrice = 5,
                            FillLevel = 15,
                            Color = "#FFFFF0" // Off-white
                        },
                        new Ingredient
                        {
                            Name = "Mandelmelk",
                            Description = "Alternativ melk av mandel\nLaktosefri\nNøtteaktig smak",
                            Category = meieri,
                            IsAvailable = true,
                            UnitPrice = 5,
                            FillLevel = 15,
                            Color = "#EFECCA" // Light almond tone
                        },

                        // Topping – 5%
                        new Ingredient
                        {
                            Name = "Kokosflak",
                            Description = "Sprø kokosbiter\nGir tekstur og tropisk smak\nFlott til pynt",
                            Category = topping,
                            IsAvailable = true,
                            UnitPrice = 3,
                            FillLevel = 5,
                            Color = "#FFEFD5" // Pale coconut flakes
                        },
                        new Ingredient
                        {
                            Name = "Oreo kjeks",
                            Description = "Knust Oreokjeks\nGir crunch og sjokoladesmak\nBarnas favoritt",
                            Category = topping,
                            IsAvailable = true,
                            UnitPrice = 3,
                            FillLevel = 5,
                            Color = "#353839" // Oreo dark grey
                        },
                        new Ingredient
                        {
                            Name = "Digestive",
                            Description = "Digestive-smuler\nLett søt kjeks\nGir god tekstur",
                            Category = topping,
                            IsAvailable = true,
                            UnitPrice = 3,
                            FillLevel = 5,
                            Color = "#D2B48C" // Digestive biscuit tan
                        },
                        new Ingredient
                        {
                            Name = "After Eight",
                            Description = "Mintsjokolade\nGir mynte og sjokoladepreg\nFrisk avslutning",
                            Category = topping,
                            IsAvailable = true,
                            UnitPrice = 3,
                            FillLevel = 5,
                            Color = "#006400" // Dark minty green
                        },
                        new Ingredient
                        {
                            Name = "Peanøtter-smør",
                            Description = "Myk nøttesmak\nGir rik og salt-søt balanse\nAllergen",
                            Category = topping,
                            IsAvailable = true,
                            UnitPrice = 3,
                            FillLevel = 5,
                            Color = "#C3A381" // Peanut butter brown
                        },
                        new Ingredient
                        {
                            Name = "Sukker",
                            Description = "Hvit sukker\nGir enkel sødme\nLett å dosere",
                            Category = topping,
                            IsAvailable = true,
                            UnitPrice = 1,
                            FillLevel = 5,
                            Color = "#FFFFFF" // White sugar
                        },
                        new Ingredient
                        {
                            Name = "Kardemomme",
                            Description = "Aromatisk krydder\nTypisk i chai\nGir varm og eksotisk smak",
                            Category = topping,
                            IsAvailable = true,
                            UnitPrice = 1,
                            FillLevel = 5,
                            Color = "#C8B560" // Cardamom hue
                        },
                        new Ingredient
                        {
                            Name = "Salt",
                            Description = "En liten klype salt\nForsterker smaker\nGir dybde",
                            Category = topping,
                            IsAvailable = true,
                            UnitPrice = 1,
                            FillLevel = 5,
                            Color = "#ECECEC" // Salt grayish-white
                        },

                        // Frukt – 5%
                        new Ingredient
                        {
                            Name = "Sitron",
                            Description = "Syrlig smak\nGir friskhet og kontrast\nPerfekt med te",
                            Category = frukt,
                            IsAvailable = true,
                            UnitPrice = 4,
                            FillLevel = 5,
                            Color = "#FFF44F" // Bright lemon yellow
                        },
                        new Ingredient
                        {
                            Name = "Appelsin",
                            Description = "Frisk sitrussmak\nSøtere enn sitron\nGod i kombinasjoner",
                            Category = frukt,
                            IsAvailable = true,
                            UnitPrice = 4,
                            FillLevel = 5,
                            Color = "#FFA500" // Orange
                        },
                        new Ingredient
                        {
                            Name = "Mango",
                            Description = "Søt og tropisk\nGir fylde og aroma\nSolrik smak",
                            Category = frukt,
                            IsAvailable = true,
                            UnitPrice = 4,
                            FillLevel = 5,
                            Color = "#FFD166" // Sunny mango color
                        },
                        new Ingredient
                        {
                            Name = "Avokado",
                            Description = "Kremet konsistens\nGir naturlig tykkelse\nMild smak",
                            Category = frukt,
                            IsAvailable = true,
                            UnitPrice = 4,
                            FillLevel = 5,
                            Color = "#568203" // Avocado green
                        },
                        new Ingredient
                        {
                            Name = "Banan",
                            Description = "Søt banansmak\nGir naturlig sødme\nGod med sjokolade",
                            Category = frukt,
                            IsAvailable = true,
                            UnitPrice = 4,
                            FillLevel = 5,
                            Color = "#FFE135" // Banana yellow
                        },

                        // Brus – 10%
                        new Ingredient
                        {
                            Name = "Tonic",
                            Description = "Lett bitter brus\nInneholder kinin\nGir frisk bobleeffekt",
                            Category = brus,
                            IsAvailable = true,
                            UnitPrice = 6,
                            FillLevel = 10,
                            Color = "#FAFAFA" // Almost white tonic
                        },
                        new Ingredient
                        {
                            Name = "Kokosvann",
                            Description = "Oppfriskende kokos\nHydrerende\nLett søt og naturlig",
                            Category = brus,
                            IsAvailable = true,
                            UnitPrice = 6,
                            FillLevel = 10,
                            Color = "#F2EFE4" // Light coconut water hue
                        }
                    };

                    // Now apply the image choice:
                    for (int i = 0; i < allIngredients.Count; i++)
                    {
                        var ing = allIngredients[i];
                        ing.ImagePath = GetImagePath(ing.Name, i);
                    }

                    context.Ingredients.AddRange(allIngredients);
                    context.SaveChanges();
                }

                // Eksempel på drikker
                if (!context.Drinks.Any())
                {
                    logger.LogInformation("Seeding drinks");
                    var drinks = new List<Drink>
                    {
                        new Drink
                        {
                            Name = "Minty Midnight",
                            SalePrice = 75,
                            TimesFavorite = 6,
                            Ingredients = new List<Ingredient>
                            {
                                context.Ingredients.First(i => i.Name == "Espresso"),
                                context.Ingredients.First(i => i.Name == "Mint sirup"),
                                context.Ingredients.First(i => i.Name == "Melk - hel"),
                                context.Ingredients.First(i => i.Name == "Sjokolade saus")
                            }
                        },
                        new Drink
                        {
                            Name = "Blåbærdrøm",
                            SalePrice = 70,
                            TimesFavorite = 3,
                            Ingredients = new List<Ingredient>
                            {
                                context.Ingredients.First(i => i.Name == "Espresso"),
                                context.Ingredients.First(i => i.Name == "Hvit sjokolade saus"),
                                context.Ingredients.First(i => i.Name == "Blåbærsaus"),
                                context.Ingredients.First(i => i.Name == "Melk - hel")
                            }
                        },
                        new Drink
                        {
                            Name = "Kos",
                            SalePrice = 80,
                            TimesFavorite = 0,
                            Ingredients = new List<Ingredient>
                            {
                                context.Ingredients.First(i => i.Name == "Banan"),
                                context.Ingredients.First(i => i.Name == "Peanøtter-smør"),
                                context.Ingredients.First(i => i.Name == "Mandelmelk"),
                                context.Ingredients.First(i => i.Name == "Iskrem")
                            }
                        },
                        new Drink
                        {
                            Name = "Soloppgang",
                            SalePrice = 72,
                            TimesFavorite = 1,
                            Ingredients = new List<Ingredient>
                            {
                                context.Ingredients.First(i => i.Name == "Matcha"),
                                context.Ingredients.First(i => i.Name == "Appelsin"),
                                context.Ingredients.First(i => i.Name == "Sitron"),
                                context.Ingredients.First(i => i.Name == "Kokosvann")
                            }
                        },
                        new Drink
                        {
                            Name = "vetikke",
                            SalePrice = 78,
                            TimesFavorite = 5,
                            Ingredients = new List<Ingredient>
                            {
                                context.Ingredients.First(i => i.Name == "Espresso"),
                                context.Ingredients.First(i => i.Name == "Karamell saus"),
                                context.Ingredients.First(i => i.Name == "Iskrem"),
                                context.Ingredients.First(i => i.Name == "Oreo kjeks")
                            }
                        },
                        new Drink
                        {
                            Name = "Spiced Avocado Latte",
                            SalePrice = 85,
                            TimesFavorite = 1,
                            Ingredients = new List<Ingredient>
                            {
                                context.Ingredients.First(i => i.Name == "Espresso"),
                                context.Ingredients.First(i => i.Name == "Avokado"),
                                context.Ingredients.First(i => i.Name == "Kondensert melk"),
                                context.Ingredients.First(i => i.Name == "Kardemomme")
                            }
                        },
                        new Drink
                        {
                            Name = "Mango-klem",
                            SalePrice = 74,
                            TimesFavorite = 3,
                            Ingredients = new List<Ingredient>
                            {
                                context.Ingredients.First(i => i.Name == "Mango"),
                                context.Ingredients.First(i => i.Name == "Kokos sirup"),
                                context.Ingredients.First(i => i.Name == "Mandelmelk"),
                                context.Ingredients.First(i => i.Name == "Krem")
                            }
                        }
                    };

                    context.Drinks.AddRange(drinks);
                    context.SaveChanges();
                }

                // Eksempel på meny
                if (!context.MenuItems.Any())
                {
                    logger.LogInformation("Seeding menu items");
                    var menuItems = new List<MenuItem>
                    {
                    // KaffeKald
                    new MenuItem { Name = "Islatte Mocha", Description =" ", Category= kaffeKald, IsAvailable= true, Price = 65},
                    new MenuItem { Name = "Islatte Vanilje", Description =" ", Category= kaffeKald, IsAvailable= true, Price = 65},
                    new MenuItem { Name = "Islatte Bounty", Description =" ", Category= kaffeKald, IsAvailable= true, Price = 79},
                    new MenuItem { Name = "Islatte Karamell og Sjokolade", Description =" ", Category= kaffeKald, IsAvailable= true, Price = 79},
                    new MenuItem { Name = "Islatte Oreo", Description =" ", Category= kaffeKald, IsAvailable= true, Price = 79},
                    new MenuItem { Name = "Blåbær Ostekake Islatte", Description =" ", Category= kaffeKald, IsAvailable= true, Price = 85},
                    new MenuItem { Name = "Mocha Iskrem Latte", Description =" ", Category= kaffeKald, IsAvailable= true, Price = 85},
                    new MenuItem { Name = "Islatte After Eight", Description =" ", Category= kaffeKald, IsAvailable= true, Price = 79},
                    new MenuItem { Name = "Islatte Rafaello", Description =" ", Category= kaffeKald, IsAvailable= true, Price = 79},
                    new MenuItem { Name = "Snickers Iscappuncino", Description =" ", Category= kaffeKald, IsAvailable= true, Price = 79},
                    new MenuItem { Name = "Appelsin Is Americano", Description =" ", Category= kaffeKald, IsAvailable= true, Price = 65},
                    new MenuItem { Name = "Tonic Blåbær Is Americano", Description =" ", Category= kaffeKald, IsAvailable= true, Price = 69},
                    new MenuItem { Name = "Avokado Islatte", Description =" ", Category= kaffeKald, IsAvailable= true, Price = 79},
                    new MenuItem { Name = "Banan Mocha Islatte", Description =" ", Category= kaffeKald, IsAvailable= true, Price = 79},

                    // KaffeVarm
                    new MenuItem { Name = "Mocha Latte", Description =" ", Category= kaffeVarm, IsAvailable= true, Price = 65},
                    new MenuItem { Name = "Vanilje Latte", Description =" ", Category= kaffeVarm, IsAvailable= true, Price = 65},
                    new MenuItem { Name = "Latte Bounty", Description =" ", Category= kaffeVarm, IsAvailable= true, Price = 79},
                    new MenuItem { Name = "Karamel og Sjokolade Latte", Description =" ", Category= kaffeVarm, IsAvailable= true, Price = 79},
                    new MenuItem { Name = "Oreo Latte", Description =" ", Category= kaffeVarm, IsAvailable= true, Price = 79},
                    new MenuItem { Name = "Latte After Eight", Description =" ", Category= kaffeVarm, IsAvailable= true, Price = 79},
                    new MenuItem { Name = "Latte Rafaello", Description =" ", Category= kaffeVarm, IsAvailable= true, Price = 79},
                    new MenuItem { Name = "Snickers Cappuncino", Description =" ", Category= kaffeVarm, IsAvailable= true, Price = 69},
                    
                    // MatchaKald
                    new MenuItem { Name = "Skummet Is Vanilje Matcha", Description =" ", Category= matchaKald, IsAvailable= true, Price = 69},
                    new MenuItem { Name = "Matcha Islatte", Description =" ", Category= matchaKald, IsAvailable= true, Price = 69},
                    new MenuItem { Name = "Mango Matcha Islatte", Description =" ", Category= matchaKald, IsAvailable= true, Price = 79},
                    new MenuItem { Name = "Blåbær Matcha Islatte", Description =" ", Category= matchaKald, IsAvailable= true, Price = 79},
                    new MenuItem { Name = "Honning & Kardemmome Is Matcha Latte", Description =" ", Category= matchaKald, IsAvailable= true, Price = 79},
                    new MenuItem { Name = "Honning & Lemonade Is Matcha", Description =" ", Category= matchaKald, IsAvailable= true, Price = 79},
                    new MenuItem { Name = "Karamel Is Matcha Latte", Description =" ", Category= matchaKald, IsAvailable= true, Price = 79},
                    new MenuItem { Name = "Dalgona Matcha på kokosvann", Description =" ", Category= matchaKald, IsAvailable= true, Price = 79},
                    new MenuItem { Name = "Blåbær Ostekake Is Matcha Latte", Description =" ", Category= matchaKald, IsAvailable= true, Price = 85},
                    new MenuItem { Name = "Rafaello Is Matcha Latte", Description =" ", Category= matchaKald, IsAvailable= true, Price = 79},
                    new MenuItem { Name = "Dirty Matcha Islatte", Description =" ", Category= matchaKald, IsAvailable= true, Price = 79},
                    new MenuItem { Name = "Banan Matcha Islatte", Description =" ", Category= matchaKald, IsAvailable= true, Price = 79},
                    
                    // MatchaVarm
                    new MenuItem { Name = "Matcha Latte", Description =" ", Category= matchaVarm, IsAvailable= true, Price = 69},
                    new MenuItem { Name = "Karamel Matcha Latte", Description =" ", Category= matchaVarm, IsAvailable= true, Price = 79},
                    new MenuItem { Name = "Rafaello Matcha Latte", Description =" ", Category= matchaVarm, IsAvailable= true, Price = 79},
                    new MenuItem { Name = "Dirty Matcha Latte", Description =" ", Category= matchaVarm, IsAvailable= true, Price = 79},

                    // Smoothie
                    new MenuItem { Name= "Appelsin Juice", Description= "Fersk Juice",   Category= smoothie, IsAvailable= true, Price= 49 },
                    new MenuItem { Name= "Lemonade",  Description= "Lemon juice",  Category= smoothie, IsAvailable= true, Price= 49 }
                };
                    context.MenuItems.AddRange(menuItems);
                    context.SaveChanges();
                }

                logger.LogInformation("Database seeding completed successfully");
            }
            catch (Exception ex)
            {
                var loggerFactory = app.ApplicationServices.GetRequiredService<ILoggerFactory>();
                var logger = loggerFactory.CreateLogger("DBInit");
                logger.LogError(ex, "An error occurred while seeding the database");
            }
        }
    }
}