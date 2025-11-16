using Renoza.Domain.Entities.Countries;
using Renoza.Domain.Services.Interfaces.BaseInterfaces;
using Renoza.Infrastructure.Contexts;

namespace Renoza.Domain.Services.Interfaces.RenozaInterfaces
{
    /// <summary>
    /// Интерфейс сервиса работы с международными телефонными кодами
    /// </summary>
    public interface IPhoneCountryCodeService : IBaseService<RenozaContext>
    {
        /// <summary>
        /// Возвращает список всех международных телефонных кодов с информацией о странах
        /// </summary>
        /// <returns>Список международных телефонных кодов</returns>
        Task<List<PhoneCountryCode>> GetAllAsync();

        /// <summary>
        /// Возвращает список активных международных телефонных кодов
        /// </summary>
        /// <returns>Список активных международных телефонных кодов</returns>
        Task<List<PhoneCountryCode>> GetActiveAsync();
    }
}
