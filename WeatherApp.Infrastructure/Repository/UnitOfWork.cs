using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeatherApp.Application.Common.Interfaces;
using WeatherApp.Infrastructure.Data;

namespace WeatherApp.Infrastructure.Repository
{
    public class UnitOfWork :IUnitOfWork
    {
        private readonly ApplicationDbContext _db;
        public IApplicationUserRepository Users { get; private set; }
        public IWeatherMesRepository WeatherMes { get; private set; }
        public UnitOfWork(ApplicationDbContext db)
        {
            _db = db;
            Users = new ApplicationUserRepository(_db);
            WeatherMes = new WheaterRepository(_db);
        }
        public void Save()
        {
            _db.SaveChanges();
        }
    }
}
