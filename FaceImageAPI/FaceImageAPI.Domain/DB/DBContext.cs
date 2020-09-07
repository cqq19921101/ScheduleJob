using System.Configuration;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace FaceImageAPI.Domain.DB
{
    public class DBContext : DbContext, System.IDisposable
    {
        public DBContext()
            : base(ConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString)
        {
            Database.SetInitializer<DBContext>(null);
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {

            base.OnModelCreating(modelBuilder);
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }

    }
}
