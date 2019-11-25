Factory Pattern for Entity Framework Core. It helps for multiple EF DbContext with this pattern.
You can create readonly context and read-write with transaction.

# How to use it

Inherit your dbcontext with commondbcontext 
```csharp
public partial class YourDbContext : CommonDbContext
    {
        public YourDbContext(DbContextOptions<YourDbContext> options)
            : base(options)
        {
        }
    }
```

Dependency Injection
```csharp
services.AddSingleton<IDbFactory, DbFactory>(provider => new DbFactory(connectionString));
```

ServiceCollection Extension
```csharp
Example 1 (No LoggerFactory)
	services.AddEfDbFactory(Configuration.GetConnectionString("DbConnection"));

Example 2 (With LoggerFactory)
	services.AddEfDbFactory(Configuration.GetConnectionString("DbConnection"), MyLoggerFactory, true);

Example 3 (No LoggerFactory And InMemory Database)
    services.AddEfDbFactory("DataSource=:memory:", true));

Example 4 (With LoggerFactory And InMemory Database)
    services.AddEfDbFactory("DataSource=:memory:"), MyLoggerFactory, true, true);
```

Injection in your controller
```csharp
private readonly IDbFactory _factoryConn;

public WriteController(IDbFactory factoryConn)
{
  _factoryConn = factoryConn ?? throw new ArgumentNullException(nameof(factoryConn));
}
```
ReadWrite Factory
```csharp
public async Task CreateBook(int authorId, string title)
        {
            using var factory = await _factoryConn.CreateTransactional(IsolationLevel.Snapshot);
            var context = factory.FactoryFor<BooksDbContext>();

            var book = new Book
            {
                Title = "New book",
                AuthorId = authorId
            };
            context.Book.Add(book);
            await context.SaveChangesAsync();
            factory.CommitTransaction();
        }
```
Readonly factory 
```csharp
public async Task<IEnumerable<Book>> GetAllBooks()
        {
            using var factory = await _factoryConn.CreateReadOnly();
            var context = factory.FactoryFor<BooksDbContext>();
            return context.Book.ToList();
        }
```

# Testing

```csharp
private static IDbFactory GetWritableFactory() => new DbFactory(_connString, true).CreateTransactional().GetAwaiter().GetResult();

 [Fact]
        public async Task Test_WritableFactory_AutoRollBack()
        {
            using (var fac = GetWritableFactory())
            {
                var context = fac.FactoryFor<TestDbContext>();
                var quiz = new Quiz() { Title = "Test 1" };
                context.Quiz.Add(quiz);
                await context.SaveChangesAsync();

                var q = Assert.Single(context.Quiz.ToList());
                Assert.NotNull(q);
                Assert.Equal("Test 1", q.Title);
            }

            using (var fac2 = GetReadonlyFactory())
            {
                var context = fac2.FactoryFor<TestDbContext>();
                Assert.NotEmpty(context.Quiz.ToList());
                Assert.InRange(context.Quiz.ToList().Count, 1, 1);
            }
        }

```