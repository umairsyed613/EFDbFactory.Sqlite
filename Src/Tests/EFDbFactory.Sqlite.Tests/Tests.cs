using System;
using System.Linq;
using System.Threading.Tasks;

using Xunit;

namespace EFDbFactory.Sqlite.Tests
{
    public class Tests
    {
        private const string _connString = "DataSource=:memory:";

        private static IDbFactory GetWritableFactory() => new DbFactory(_connString, true).CreateTransactional().GetAwaiter().GetResult();
        private static IDbFactory GetReadonlyFactory() => new DbFactory(_connString, true).CreateReadOnly().GetAwaiter().GetResult();
        [Fact]
        public static void Test_ReadonlyFactory_ThrowsExceptionWhenSaveChanges()
        {
            using var fac = GetReadonlyFactory();
            var context = fac.FactoryFor<TestDbContext>();

            var quiz = new Quiz() {Title = "Test 1"};
            context.Quiz.Add(quiz);

            Assert.Throws<InvalidOperationException>(() => context.SaveChanges());
        }

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
    }
}