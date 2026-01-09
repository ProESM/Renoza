using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Renoza.Common.Base.Helpers;
using Renoza.Domain.Entities.Products;
using Renoza.Domain.Services.Implementations.BaseImplementations;
using Renoza.Domain.Services.Interfaces.RenozaInterfaces;
using Renoza.Infrastructure.Contexts;
using Renoza.Infrastructure.Entities.Renoza;
using Renoza.Infrastructure.Repositories.Interfaces.BaseInterfaces;

namespace Renoza.Domain.Services.Implementations.RenozaImplementations
{
    /// <summary>
    /// Сервис для работы со справочником единиц измерения
    /// </summary>
    public class MeasurementUnitService : BaseService<RenozaContext>, IMeasurementUnitService
    {
        #region Репозитории

        /// <summary>
        /// Репозиторий для работы с единицами измерения
        /// </summary>
        private readonly IEntityWithIdRepository<MeasurementUnitDao, Guid> _measurementUnitRepository;

        #endregion

        #region Мапперы

        /// <summary>
        /// Маппер для преобразования между DAO и Domain сущностями
        /// </summary>
        private readonly IMapper _mapper;

        #endregion

        /// <summary>
        /// Сервис для работы со справочником единиц измерения
        /// </summary>
        /// <param name="context">Контекст базы данных</param>
        /// <param name="measurementUnitRepository">Репозиторий для работы с единицами измерения</param>
        /// <param name="mapper">Маппер для преобразования между DAO и Domain сущностями</param>
        public MeasurementUnitService(
            RenozaContext context,
            IEntityWithIdRepository<MeasurementUnitDao, Guid> measurementUnitRepository,
            IMapper mapper) : base(context)
        {
            _measurementUnitRepository = measurementUnitRepository;
            _mapper = mapper;
        }

        /// <summary>
        /// Получить все единицы измерения
        /// </summary>
        /// <param name="includeInactive">Включить неактивные единицы</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Список единиц измерения</returns>
        public async Task<Result<List<MeasurementUnit>>> GetAllAsync(
            bool includeInactive = false, CancellationToken cancellationToken = default)
        {
            var query = _measurementUnitRepository.GetQueryable();

            if (!includeInactive)
                query = query.Where(x => x.IsActive);

            var unitsDaos = await query
                .OrderBy(x => x.TypeId)
                .ThenBy(x => x.Code)
                .ToListAsync(cancellationToken);

            var units = _mapper.Map<List<MeasurementUnit>>(unitsDaos);
            return Result<List<MeasurementUnit>>.Success(units);
        }

        /// <summary>
        /// Получить единицу измерения по ID
        /// </summary>
        /// <param name="id">ID единицы измерения</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Единица измерения</returns>
        public async Task<Result<MeasurementUnit>> GetByIdAsync(
            Guid id, CancellationToken cancellationToken = default)
        {
            var unitDao = await _measurementUnitRepository.GetByIdAsync(id, cancellationToken);

            if (unitDao == null)
                return Result<MeasurementUnit>.Failure("Единица измерения не найдена");

            var unit = _mapper.Map<MeasurementUnit>(unitDao);
            return Result<MeasurementUnit>.Success(unit);
        }

        /// <summary>
        /// Получить единицу измерения по коду
        /// </summary>
        /// <param name="code">Код единицы измерения (например, шт, м², кг)</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Единица измерения</returns>
        public async Task<Result<MeasurementUnit>> GetByCodeAsync(
            string code, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(code))
                return Result<MeasurementUnit>.Failure("Код единицы измерения не может быть пустым");

            var unitDao = await _measurementUnitRepository.GetQueryable()
                .FirstOrDefaultAsync(x => x.Code.ToLower() == code.ToLower(), cancellationToken);

            if (unitDao == null)
                return Result<MeasurementUnit>.Failure($"Единица измерения с кодом '{code}' не найдена");

            var unit = _mapper.Map<MeasurementUnit>(unitDao);
            return Result<MeasurementUnit>.Success(unit);
        }

        /// <summary>
        /// Получить единицы измерения по типу
        /// </summary>
        /// <param name="typeId">ID типа единицы измерения</param>
        /// <param name="includeInactive">Включить неактивные единицы</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Список единиц измерения</returns>
        public async Task<Result<List<MeasurementUnit>>> GetByTypeAsync(
            Guid typeId, bool includeInactive = false, CancellationToken cancellationToken = default)
        {
            var query = _measurementUnitRepository.GetQueryable()
                .Where(x => x.TypeId == typeId);

            if (!includeInactive)
                query = query.Where(x => x.IsActive);

            var unitsDaos = await query
                .OrderBy(x => x.Code)
                .ToListAsync(cancellationToken);

            var units = _mapper.Map<List<MeasurementUnit>>(unitsDaos);
            return Result<List<MeasurementUnit>>.Success(units);
        }
    }
}
