namespace MyKniga.Tests
{
    using System;
    using Data;
    using Microsoft.EntityFrameworkCore;
    using Utils;

    public abstract class BaseTests
    {
        protected BaseTests()
        {
            TestAutoMapperInitializer.InitializeAutoMapper();
        }

        protected MyKnigaDbContext NewInMemoryDatabase()
        {
            return new MyKnigaDbContext(new DbContextOptionsBuilder<MyKnigaDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options);
        }
    }
}