# Entity Framework Core Factory Pattern for Sqlite

# Introduction 
Factory Pattern for Entity Framework Core. It helps for multiple EF DbContext with this pattern.
You can create readonly context and read-write with transaction.

# How to use it

Inherit your dbcontext with commondbcontext 
```
public partial class YourDbContext : CommonDbContext
    {
        public YourDbContext(DbContextOptions<QuizDbContext> options)
            : base(options)
        {
        }
    }
```

Dependency Injection
```
services.AddSingleton<IDbFactory, DbFactory>(provider => new DbFactory(connectionString));
```

ServiceCollection Extension
```
Example 1 (No LoggerFactory)
	services.AddEfDbFactory(Configuration.GetConnectionString("DbConnection"));

Example 2 (With LoggerFactory)
	services.AddEfDbFactory(Configuration.GetConnectionString("DbConnection"), MyLoggerFactory, true);

```

Injection in your controller
```
private readonly IDbFactory _factoryConn;

public WriteController(IDbFactory factoryConn)
{
  _factoryConn = factoryConn ?? throw new ArgumentNullException(nameof(factoryConn));
}
```
ReadWrite Factory
```
public async Task CreateBook(int authorId, string title)
        {
            using var factory = await factoryConn.Create(IsolationLevel.Snapshot);
            var context = factory.FactoryFor<BooksDbContext>().GetReadWriteWithDbTransaction();

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
```
public async Task<IEnumerable<Book>> GetAllBooks()
        {
            using var factory = await factoryConn.Create();
            var context = factory.FactoryFor<BooksDbContext>().GetReadOnlyWithNoTracking();
            return context.Book.ToList();
        }
```

# Testing

```
private static IDbFactory GetNoCommitFactory() => new DbFactory("YourConnectionString").CreateNoCommit().GetAwaiter().GetResult();

[Fact]
public async Task Test_NoCommitFactory_AutoRollBack()
{
    using (var fac = GetNoCommitFactory())
    {
        var context = fac.FactoryFor<TestDbContext>().GetReadWriteWithDbTransaction();

        var quiz = new Quiz() {Title = "Test 1"};
        context.Quiz.Add(quiz);
        await context.SaveChangesAsync();

        var q = Assert.Single(context.Quiz.ToList());
        Assert.NotNull(q);
        Assert.Equal("Test 1", q.Title);
    }

    using (var fac2 = GetNoCommitFactory())
    {
        var context = fac2.FactoryFor<TestDbContext>().GetReadWriteWithDbTransaction();
        Assert.Empty(context.Quiz.ToList());
    }
}

```

# Sample Projects
```
You can find sample projects under Src/Samples
```

# Feel Free to make it better