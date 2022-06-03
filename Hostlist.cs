using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Sqlite;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Data.SQLite;
namespace whitelist
{
    public class Hostlist
    {

        public void writelist()
        {
            SQLiteConnection.CreateFile("host-list.db");
        }
        public void creatrule()
        {
            
        }
    }
    public class SqlContext : DbContext
    {
        public SqlContext(DbContextOptions<SqlContext> Options) : base(Options)
        {
        }
        public DbSet<A> A { get; set; }
    }

    public class A
    {
        public int ID { get; set; }

    }
    
}
