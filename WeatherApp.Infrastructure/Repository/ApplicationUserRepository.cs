using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeatherApp.Application.Common.Interfaces;
using WeatherApp.Domain.Entities;
using WeatherApp.Infrastructure.Data;

namespace WeatherApp.Infrastructure.Repository
{
    public class ApplicationUserRepository : Repository<ApplicationUser>, IApplicationUserRepository
    {

        private readonly ApplicationDbContext _db;

        public ApplicationUserRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }




        public void Update(ApplicationUser entity)
        {
            _db.Update(entity);
        }
    }
}
