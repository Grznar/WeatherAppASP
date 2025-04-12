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
    public class WheaterRepository : Repository<WeatherMes>, IWeatherMesRepository
    {

        private readonly ApplicationDbContext _db;

        public WheaterRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }




        public void Update(WeatherMes entity)
        {
            _db.Update(entity);
        }
    }
}
