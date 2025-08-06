using Microsoft.EntityFrameworkCore;

namespace GrillPizzeriaOrderMiddleware.DatabaseContexts
{
    public class GrillPizzaDatabaseContext : DbContext
    {
        public GrillPizzaDatabaseContext(DbContextOptions<GrillPizzaDatabaseContext> context) : base(context) {}


    }
}
