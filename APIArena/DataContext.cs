using APIArena.Models;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.Data;
using System.Reflection;

namespace APIArena.Server
{
    public partial class DataContext : DbContext
    {
        public DataContext()
        {
        }

        public DataContext(DbContextOptions<DataContext> options)
            : base(options)
        {
        }

        public virtual DbSet<ApiKey> ApiKeys { get; set; } = null!;
        public virtual DbSet<Session> Sessions { get; set; } = null!;
        public virtual DbSet<Player> Players { get; set; } = null!;
        public virtual DbSet<Map> Maps { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Make the DefaultValue attribute work as DefaultSqlValue
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                foreach (var property in entityType.GetProperties())
                {
                    var memberInfo = property.PropertyInfo ?? (MemberInfo?)property.FieldInfo;

                    if (memberInfo is null) continue;
                    if (Attribute.GetCustomAttribute(memberInfo, typeof(DefaultValueAttribute)) is not DefaultValueAttribute defaultValue) continue;
                    if (defaultValue.Value is not string sqlValue) continue;

                    property.SetDefaultValueSql(sqlValue);
                }
            }

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
