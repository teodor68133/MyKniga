namespace MyKniga.Data
{
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore;
    using Models;

    public class MyKnigaDbContext : IdentityDbContext<KnigaUser>
    {
        public MyKnigaDbContext(DbContextOptions<MyKnigaDbContext> options)
            : base(options)
        {
        }

        public DbSet<Book> Books { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<BookTag> BookTags { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<BookTag>().HasKey(bt => new {bt.BookId, bt.TagId});
            base.OnModelCreating(builder);
        }
    }
}