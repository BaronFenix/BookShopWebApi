using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using BookShop.Infrastructure;
using BookShop.Domain;
using Microsoft.Extensions.Hosting;
using BookShop;
using Microsoft.Extensions.DependencyInjection;

namespace BookShop.Infrastructure
{
    public class ApplicationService : BackgroundService
    {
        private IServiceScopeFactory _scopeService;

        public ApplicationService(IServiceScopeFactory scopeFactory)
        {
            _scopeService = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Factory.StartNew(async () => 
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    Console.WriteLine("Записываем начальные данные");
                    try
                    {
                        using var scope = _scopeService.CreateScope();
                        var context = scope.ServiceProvider.GetService<ApplicationContext>();

                        await GetOrCreateBooks(context, stoppingToken);
                        await context.SaveChangesAsync();
                        Console.WriteLine("Данные о книгах записаны");

                        await GetOrCreateAuthors(context, stoppingToken);
                        await context.SaveChangesAsync();
                        Console.WriteLine("Данные о авторах записаны");

                        await GetOrCreateCategories(context, stoppingToken);
                        await context.SaveChangesAsync(stoppingToken);
                        Console.WriteLine("Данные о жанрах записаны");

                        await GetOrCreateBookGenres(context, stoppingToken);
                        await context.SaveChangesAsync(stoppingToken);
                        Console.WriteLine("Данные о связях книга-жанр записаны");

                        await GetOrCreateBookAuthors(context, stoppingToken);
                        await context.SaveChangesAsync(stoppingToken);
                        Console.WriteLine("Данные о связях книга-автор записаны");

                        await GetOrCreatePrices(context, stoppingToken);
                        await context.SaveChangesAsync(stoppingToken);
                        Console.WriteLine("Данные о ценах записаны");
                    }
                    catch(Exception ex)
                    {
                        Console.WriteLine($"Ошибка сохранения данных: {ex}");
                    }
                    await Task.Delay(3000);
                }
            });
        }

        private async Task GetOrCreateBooks(ApplicationContext context, CancellationToken cancellationToken)
        {
            List<Book> books = new List<Book>()
            {
                new Book
                {
                    Title = "Зелёная миля",
                    Description = "Стивен Кинг приглашает читателей в жуткий мир тюремного блока смертников, откуда уходят, чтобы не вернуться, приоткрывает дверь последнего пристанища тех, кто преступил не только человеческий, но и Божий закон. По эту сторону электрического стула нет более смертоносного местечка! Ничто из того, что вы читали раньше, не сравнится с самым дерзким из ужасных опытов Стивена Кинга - с историей, что начинается на Дороге Смерти и уходит в глубины самых чудовищных тайн человеческой души...",
                    Pages = 384,
                    PublishedYear = 2014,
                    PublishName = "АСТ",
                    ImageUrl = "https://s1.livelib.ru/boocover/1000966838/200x305/7df1/boocover.jpg"
                },
                new Book
                {
                    Title = "Унесенные ветром",
                    Description = "Роман Маргарет Митчелл вышел в свет в 1936 году и имел феноменальный успех у читателей. Только в первые годы его тираж превысил три миллиона, и «Унесенные ветром» были признаны «книгой века». В 1939 году на экраны вышел одноименный фильм (с участием Вивьен Ли и Кларком Гейблом), который завоевал восемь премий «Оскар» и стал одной из самых кассовых лент в истории кинематографа. Несмотря на полярные оценки литературных критиков, роман удостоился престижной Пулитцеровской премии, его сравнивали с «Войной и миром» Льва Толстого, а Маргарет Митчелл ставили в один ряд с великими классиками мировой литературы. Книга выдержала более 70 изданий…",
                    Pages = 994,
                    PublishedYear = 2020,
                    PublishName = "Азбука",
                    ImageUrl = "https://s1.livelib.ru/boocover/1000966838/200x305/7df1/boocover.jpg"
                },
               new Book
                {
                    Title = "Граф Монте-Кристо",
                    Description = "Как и сто шестьдесят пять лет назад, \"Граф Монте-Кристо\" Александра Дюма остается одним из самых популярных романов в мировой литературе. К нему писали продолжения, его ставили на сцене, создавали мюзиклы, экранизировали, но и по сей день бесчисленные издания этой книги доставляют удовольствие новым и новым поколениям читателей. История молодого парижанина, которого приятели в шутку засадили в тюрьму, почерпнута автором в архивах парижской полиции. А из-под пера мастера выходит моряк Эдмон Дантес, мученик замка Иф. Не дождавшись правосудия, он решает сам вершить суд и жестоко мстит врагам, разрушившим его счастье.",
                    Pages = 1221,
                    PublishedYear = 2017,
                    PublishName = "Азбука",
                    ImageUrl = "https://s1.livelib.ru/boocover/1002217446/200x305/518c/boocover.jpg"
                },
                new Book
                {
                    Title = "Десять негритят",
                    Description = "Десять никак не связанных между собой людей в особняке на уединенном острове... Кто вызвал их сюда таинственным приглашением? Зачем кто-то убивает их, одного за другим, самыми невероятными способами? Почему все происходящее так тесно переплетено с веселым детским стишком?",
                    Pages = 288,
                    PublishedYear = 2021,
                    PublishName = "Эксмо",
                    ImageUrl = "https://s1.livelib.ru/boocover/1005556180/200x305/434b/boocover.jpg"
                },
                new Book
                {
                    Title = "Побег из Шоушенка",
                    Description = "Страшный сон, ставший реальностью... История невинного человека, приговоренного к пожизненному заключению в тюремном аду. Жесткая история выживания там, где выжить практически невозможно. Увлекательная история побега оттуда, откуда не сумел вырваться еще никто...",
                    Pages = 192,
                    PublishedYear = 2011,
                    PublishName = "Астрель",
                    ImageUrl = "https://s1.livelib.ru/boocover/1001445982/200x305/3a2a/boocover.jpg"
                },
                new Book
                {
                    Title = "Записки о Шерлоке Холмсе",
                    Description = "Английский врач и писатель сэр Артур Конан Дойл известен всему миру как непревзойденный мастер детективного жанра. В настоящем издании представлены наиболее рассказы о гениальном Шерлоке Холмсе (по словам писателя, «самом проницательном и энергичном из всех европейских сыщиков») и его верном спутнике докторе Ватсоне. Талантливо развивший традицию американского писателя Эдгара По, Артур Конан Дойл наряду с ним увенчан лаврами создателя детектива как жанра беллетристики. Читателю гарантировано огромное удовольствие — как от решения занимательных задач на логику, гибкость ума, так и просто от интересного чтения.",
                    Pages = 192,
                    PublishedYear = 2015,
                    PublishName = "Азбука",
                    ImageUrl = "https://s1.livelib.ru/boocover/1001401495/200x305/2379/boocover.jpg"
                },
                new Book
                {
                    Title = "Ведьмак: Час Презрения",
                    Description = "Между Империей Нильфгаард и королевствами нордлингов установился непрочный мир, но, похоже, это лишь затишье перед бурей. Маленькое пограничное королевство Цинтра, а также его наследница Цири, внучка Львицы Калантэ, наследие Старшей Крови, может стать неожиданным козырем в большой игре королей, магов и спецслужб по обе стороны конфликта. Одни хотят уничтожить Дитя-Предназначение, другие - взять под контроль, третьи - сделать символом освобождения. Возможно, все решится во время сбора чародейского Капитула на острове Танедд, где вновь сойдутся пути Цири, чародейки Йеннифэр и ведьмака Геральта по прозвищу Белый Волк. Ведь все ближе Час Белого…",
                    Pages = 374,
                    PublishedYear = 2016,
                    PublishName = "АСТ",
                    ImageUrl = "https://s1.livelib.ru/boocover/1001544280/200x305/cb64/boocover.jpg"
                },                
                new Book
                {
                    Title = "Убийство в \"Восточном экспрессе\"",
                    Description = "Находившийся в Стамбуле великий сыщик Эркюль Пуаро возвращается в Англию на знаменитом \"Восточном экспрессе\", в котором вместе с ним едут, кажется, представители всех возможных национальностей. Один из пассажиров, неприятный американец по фамилии Рэтчетт, предлагает Пуаро стать своим телохранителем, поскольку считает, что его должны убить. Знаменитый бельгиец отмахивается от этой абсурдной просьбы. А на следующий день американца находят мертвым в своем купе, причем двери закрыты, а окно открыто. Пуаро немедленно берется за расследование – и выясняет, что купе полно всевозможных улик, указывающих… практически на всех пассажиров \"Восточного экспресса\". Вдобавок поезд наглухо застревает в снежных заносах в безлюдном месте. Пуаро необходимо найти убийцу до того, как экспресс продолжит свой путь…",
                    Pages = 235,
                    PublishedYear = 2021,
                    PublishName = "Эксмо",
                    ImageUrl = "https://s1.livelib.ru/boocover/1006221715/200x305/cbe4/boocover.jpg"
                },
                new Book
                {
                    Title = "Ведьмак: Кровь Эльфов",
                    Description = "Мечи Геральта из Ривии по-прежнему остры, а на белом свете не стало меньше чудовищ, пусть далеко не все они – клыкастые монстры. И все же, мир, знакомый читателям по первым двум книгам цикла, стремительно меняется. Забудьте о камерности и сказочности! На передний план выходят эпичный размах, высокая политика и… ожидание большой беды. Короли и военачальники, маги и наемники, люди и нелюди ведут сложную игру, не жалея ни себя, ни противника. И в центре этой игры – она: наследная княгиня Цинтры, воспитанница ведьмаков Каэр Морхэна и чародейки Йеннифэр из Венгерберга, Предназначение Белого Волка. Дитя Старшей Крови. Льющейся все чаще крови…",
                    Pages = 354,
                    PublishedYear = 2016,
                    PublishName = "АСТ",
                    ImageUrl = "https://s1.livelib.ru/boocover/1001510824/140/426d/Andzhej_Sapkovskij__Vedmak._Krov_elfov.jpg"
                },
                new Book
                {
                    Title = "Двадцать тысяч лье под водой",
                    Description = "Когда профессор Пьер Аронакс, его слуга Консель и китобой Нед Ленд отправились на поиски неизвестной науке рыбы, они и представить не могли, чем обернется их экспедиция. Странное морское чудище оказывается не живым существом, а… подводной лодкой \"\"Наутилус\"\". Но куда больше вопросов вызывает ее хозяин – загадочный капитан Немо, прошлое которого окутано тайнами. Вместе им предстоит исследовать бескрайние океанские просторы и пройти под водой двадцать тысяч лье…",
                    Pages = 485,
                    PublishedYear = 2018,
                    PublishName = "АСТ",
                    ImageUrl = "https://s1.livelib.ru/boocover/1002889566/200x305/684c/boocover.jpg"
                }
            };
            foreach (var book in books)
            {
                try
                {
                    if(!await context.Books.AnyAsync(p => p.Title == book.Title && p.PublishName == book.PublishName, cancellationToken: cancellationToken))
                        await context.Books.AddAsync(book);
                }
                catch(Exception ex) 
                {
                    Console.WriteLine($"Ошибка добавления данных: {ex}");
                }
            }
        }

        private async Task GetOrCreateAuthors(ApplicationContext context, CancellationToken cancellationToken)
        {
            List<Author> authors = new List<Author>()
            {
                new Author
                {
                    FirstName = "Стивен",
                    LastName = "Кинг",
                    BirthDate = new DateTime(1947, 1, 1),
                    Country = "США",
                    Description = "Американский писатель, работающий в жанрах ужасов, триллера, фантастики и мистики.",
                    ImageUrl = "https://u.livelib.ru/author/1000000048/r/c8eggl18/o-r.jpg"
                },
                new Author
                {
                    FirstName = "Жюль",
                    LastName = "Верн",
                    BirthDate = new DateTime(1828, 1, 1),
                    Country = "Франция",
                    Description = "Французский географ и писатель, классик приключенческой литературы, один из основоположников научной фантастики. Член Французского Географического общества.",
                    ImageUrl = "https://u.livelib.ru/author/1000004668/r/qe7zt1gd/o-r.jpg"
                },
                new Author
                {
                    FirstName = "Агата",
                    LastName = "Кристи",
                    BirthDate = new DateTime(1890, 1, 1),
                    Country = "Великобритания",
                    Description = "(полное имя: Агата Мэри Кларисса Маллоуэн, урожденная Миллер) — английская писательница.",
                    ImageUrl = "https://u.livelib.ru/author/1000001781/r/qfz0xnre/02-r.jpg"
                },
                new Author
                {
                    FirstName = "Артур Конан",
                    LastName = "Дойл",
                    BirthDate = new DateTime(1859, 1, 1),
                    Country = "Великобритания",
                    Description = "Сэр Артур Игнатиус Конан Дойл (Arthur Ignatius Conan Doyle) — шотландский и английский врач и писатель.",
                    ImageUrl = "https://u.livelib.ru/author/1000103808/r/1euigl55/Konan_Dojl-r.jpg"
                },
                new Author
                {
                    FirstName = "Александр",
                    LastName = "Дюма",
                    BirthDate = new DateTime(1802, 1, 1),
                    Country = "Франция",
                    Description = "Александр Дюма — французский писатель, чьи приключенческие романы сделали его одним из самых читаемых французских авторов в мире. Также был драматургом и журналистом. Поскольку его сын также носил имя Александр и также был писателем, для предотвращения путаницы при его упоминании часто добавляют уточнение «-отец».",
                    ImageUrl = "https://library.vladimir.ru/wp-content/uploads/2017/06/0001.jpg"
                },
                new Author
                {
                    FirstName = "Анджей",
                    LastName = "Сапковски",
                    BirthDate = new DateTime(1948, 1, 1),
                    Country = "Польша",
                    Description = "Польский писатель-фантаст и публицист, автор популярной фэнтези-саги «Ведьмак».",
                    ImageUrl = "https://vgtimes.ru/uploads/posts/2020-08/1597658041_4pssiviv12o.jpg"
                },
                new Author
                {
                    FirstName = "Маргарет",
                    LastName = "Митчелл",
                    BirthDate = new DateTime(1900, 1, 1),
                    Country = "США",
                    Description = "Маргарет Манерлин Митчелл (Margaret Munnerlyn Mitchell) — американская писательница, автор романа-бестселлера «Унесённые ветром».",
                    ImageUrl = "https://vgtimes.ru/uploads/posts/2020-08/1597658041_4pssiviv12o.jpg"
                },
            };
            foreach (var author in authors)
            {
                try
                {
                    if (!await context.Authors.AnyAsync(p => p.FirstName == author.FirstName && p.LastName == author.LastName, cancellationToken: cancellationToken))
                        await context.Authors.AddAsync(author);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка добавления данных: {ex}");
                }
            }
        }

        private async Task GetOrCreateCategories(ApplicationContext context, CancellationToken cancellationToken)
        {
            List<Genre> genres = new List<Genre>() 
            {
                 new Genre
                 {
                     Name = "Классическая литература",
                     Description = "Классическое описание"
                 },
                 new Genre
                 {
                     Name = "Детектив",
                     Description = "Загадочное описание"
                 },
                 new Genre
                 {
                     Name = "Фэнтези",
                     Description = "Магическое описание"
                 },
                 new Genre
                 {
                     Name = "Научная фантастика",
                     Description = "Фантастическое описание"
                 },
                 new Genre
                 {
                     Name = "Ужасы, Мистика",
                     Description = "Ужасно мистическе описание"
                 },
                 new Genre
                 {
                     Name = "Приключения",
                     Description = "Приключенческое описание"
                 },
                 new Genre
                 {
                     Name = "Повести, Рассказы",
                     Description = "Краткое описание"
                 },
                 new Genre
                 {
                     Name = "Романы",
                     Description = "Краткое описание"
                 }
            };

            foreach (var genre in genres)
            {
                try
                {
                    if (!await context.Genres.AnyAsync(p => p.Name == genre.Name, cancellationToken: cancellationToken))
                        await context.Genres.AddAsync(genre, cancellationToken);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка добавления данных: {ex}");
                }
            }
        }

        private async Task GetOrCreateBookGenres(ApplicationContext context, CancellationToken cancellationToken)
        {
            List<BookGenre> bookGenres = new List<BookGenre>()
            {
                new BookGenre
                {
                    Book = await context.Books.FindAsync(1),
                    Genre = await context.Genres.FindAsync(5)
                },
                new BookGenre
                {
                    Book = await context.Books.FindAsync(2),
                    Genre = await context.Genres.FindAsync(8)
                },
                new BookGenre
                {
                    Book = await context.Books.FindAsync(3),
                    Genre = await context.Genres.FindAsync(1)
                },
                new BookGenre
                {
                    Book = await context.Books.FindAsync(4),
                    Genre = await context.Genres.FindAsync(2)
                },
                new BookGenre
                {
                    Book = await context.Books.FindAsync(5),
                    Genre = await context.Genres.FindAsync(7)
                },
                new BookGenre
                {
                    Book = await context.Books.FindAsync(6),
                    Genre = await context.Genres.FindAsync(2)
                },
                new BookGenre
                {
                    Book = await context.Books.FindAsync(7),
                    Genre = await context.Genres.FindAsync(3)
                },
                new BookGenre
                {
                    Book = await context.Books.FindAsync(8),
                    Genre = await context.Genres.FindAsync(2)
                },
                new BookGenre
                {
                    Book = await context.Books.FindAsync(9),
                    Genre = await context.Genres.FindAsync(3)
                },
                new BookGenre
                {
                    Book = await context.Books.FindAsync(10),
                    Genre = await context.Genres.FindAsync(4)
                },
            };

            foreach (var item in bookGenres)
            {
                try
                {
                    if (!await context.BookGenres.AnyAsync(p => p.Book.Title == item.Book.Title &&
                                                                p.Genre.Name == item.Genre.Name,
                                                                cancellationToken: cancellationToken))
                        await context.BookGenres.AddAsync(item);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка добавления данных: {ex}");
                }
            }
        }

        private async Task GetOrCreateBookAuthors(ApplicationContext context, CancellationToken cancellationToken)
        {
            List<BookAuthor> bookAuthors = new List<BookAuthor>()
            {
                new BookAuthor
                {
                    Book = await context.Books.FindAsync(1),
                    Author = await context.Authors.FindAsync(1)
                },
                new BookAuthor
                {
                    Book = await context.Books.FindAsync(2),
                    Author = await context.Authors.FindAsync(7)
                },
                new BookAuthor
                {
                    Book = await context.Books.FindAsync(3),
                    Author = await context.Authors.FindAsync(5)
                },
                new BookAuthor
                {
                    Book = await context.Books.FindAsync(4),
                    Author = await context.Authors.FindAsync(3)
                },
                new BookAuthor
                {
                    Book = await context.Books.FindAsync(5),
                    Author = await context.Authors.FindAsync(1)
                },
                new BookAuthor
                {
                    Book = await context.Books.FindAsync(6),
                    Author = await context.Authors.FindAsync(4)
                },
                new BookAuthor
                {
                    Book = await context.Books.FindAsync(7),
                    Author = await context.Authors.FindAsync(6)
                },
                new BookAuthor
                {
                    Book = await context.Books.FindAsync(8),
                    Author = await context.Authors.FindAsync(3)
                },
                                                                                                                                new BookAuthor
                {
                    Book = await context.Books.FindAsync(9),
                    Author = await context.Authors.FindAsync(6)
                },
                new BookAuthor
                {
                    Book = await context.Books.FindAsync(10),
                    Author = await context.Authors.FindAsync(2)
                },
            };

            foreach (var item in bookAuthors)
            {
                try
                {
                    if (!await context.BookAuthors.AnyAsync(p => p.Book.Title == item.Book.Title &&
                                                                p.Author.FirstName == item.Author.FirstName &&
                                                                p.Author.LastName == item.Author.LastName,
                                                                cancellationToken: cancellationToken))
                        await context.BookAuthors.AddAsync(item);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка добавления данных: {ex}");
                }
            }
        }

        private async Task GetOrCreatePrices(ApplicationContext context, CancellationToken cancellationToken)
        {
            List<Price> prices = new List<Price>()
            {
                new Price
                {
                    Total = 4900,
                    Book = await context.Books.FindAsync(1),
                    Created = DateTime.Now,
                    Updated = DateTime.Now,
                },
                new Price
                {
                    Total = 5900,
                    Book = await context.Books.FindAsync(2),
                    Created = DateTime.Now,
                    Updated = DateTime.Now,
                },
                new Price
                {
                    Total = 5900,
                    Book = await context.Books.FindAsync(3),
                    Created = DateTime.Now,
                    Updated = DateTime.Now,
                },
                new Price
                {
                    Total = 4900,
                    Book = await context.Books.FindAsync(4),
                    Created = DateTime.Now,
                    Updated = DateTime.Now,
                },
                new Price
                {
                    Total = 3900,
                    Book = await context.Books.FindAsync(5),
                    Created = DateTime.Now,
                    Updated = DateTime.Now,
                },
                new Price
                {
                    Total = 6780,
                    Book = await context.Books.FindAsync(6),
                    Created = DateTime.Now,
                    Updated = DateTime.Now,
                },
                new Price
                {
                    Total = 5420,
                    Book = await context.Books.FindAsync(7),
                    Created = DateTime.Now,
                    Updated = DateTime.Now,
                },
                new Price
                {
                    Total = 8430,
                    Book = await context.Books.FindAsync(8),
                    Created = DateTime.Now,
                    Updated = DateTime.Now,
                },
                new Price
                {
                    Total = 2460,
                    Book = await context.Books.FindAsync(9),
                    Created = DateTime.Now,
                    Updated = DateTime.Now,
                },
                new Price
                {
                    Total = 4610,
                    Book = await context.Books.FindAsync(10),
                    Created = DateTime.Now,
                    Updated = DateTime.Now,
                },
            };

            foreach (var item in prices)
            {
                try
                {
                    if (!await context.Prices.AnyAsync(p => p.Total == item.Total &&
                                                       p.Book.Title == item.Book.Title,
                                                       cancellationToken: cancellationToken))
                        await context.Prices.AddAsync(item);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка добавления данных: {ex}");
                }
            }
        }
    }
}
