using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherApp.Application.Common.Interfaces
{
    public interface IUnitOfWork
    {
        IApplicationUserRepository Users { get; }
        IWeatherMesRepository WeatherMes { get; }
        void Save();
    }
}
