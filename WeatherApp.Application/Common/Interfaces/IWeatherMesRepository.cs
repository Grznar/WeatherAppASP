using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeatherApp.Domain.Entities;

namespace WeatherApp.Application.Common.Interfaces
{
    public interface IWeatherMesRepository : IRepository<WeatherMes>
    {
        void Update(WeatherMes entity);
    }
}
