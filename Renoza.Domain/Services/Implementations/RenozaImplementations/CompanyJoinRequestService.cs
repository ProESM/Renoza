using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Renoza.Common.Base.Helpers;
using Renoza.Domain.Constants.Company;
using Renoza.Domain.Entities.CompanyMembers;
using Renoza.Domain.Enums;
using Renoza.Domain.Services.Implementations.BaseImplementations;
using Renoza.Domain.Services.Interfaces.RenozaInterfaces;
using Renoza.Infrastructure.Contexts;
using Renoza.Infrastructure.Entities.Renoza;
using Renoza.Infrastructure.Repositories.Interfaces.BaseInterfaces;

namespace Renoza.Domain.Services.Implementations.RenozaImplementations
{
    /// <summary>
    /// Сервис для управления запросами на вступление в компанию
    /// </summary>
    public class CompanyJoinRequestService : BaseService<RenozaContext>, ICompanyJoinRequestService
    {
        /// <summary>
        /// Репозиторий для работы с запросами на вступление
        /// </summary>
        private readonly IEntityWithIdRepository<CompanyJoinRequestDao, Guid> _companyJoinRequestRepository;

        /// <summary>
        /// Сервис для управления участниками компании
        /// </summary>
        private readonly ICompanyMemberService _companyMemberService;

        /// <summary>
        /// Маппер для преобразования между DAO и Domain сущностями
        /// </summary>
        private readonly IMapper _mapper;

        /// <summary>
        /// Конструктор сервиса для работы с запросами на вступление в компанию
        /// </summary>
        /// <param name="dbContext">Контекст базы данных</param>
        /// <param name="companyJoinRequestRepository">Репозиторий для работы с запросами на вступление</param>
        /// <param name="companyMemberService">Сервис для управления участниками компании</param>
        /// <param name="mapper">Маппер для преобразования между DAO и Domain сущностями</param>
        public CompanyJoinRequestService(
            RenozaContext dbContext,
            IEntityWithIdRepository<CompanyJoinRequestDao, Guid> companyJoinRequestRepository,
            ICompanyMemberService companyMemberService,
            IMapper mapper) : base(dbContext)
        {
            _companyJoinRequestRepository = companyJoinRequestRepository;
            _companyMemberService = companyMemberService;
            _mapper = mapper;
        }

        /// <summary>
        /// Создать запрос на вступление в компанию
        /// </summary>
        /// <param name="userId">ID пользователя</param>
        /// <param name="companyProfileId">ID профиля компании</param>
        /// <param name="requestMessage">Сообщение от пользователя</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Созданный запрос на вступление</returns>
        public async Task<Result<CompanyJoinRequest>> CreateJoinRequestAsync(
            Guid userId,
            Guid companyProfileId,
            string? requestMessage = null,
            CancellationToken cancellationToken = default)
        {
            // Проверяем, нет ли уже активного запроса от этого пользователя для этой компании
            var existingPendingRequestDao = await _companyJoinRequestRepository.GetQueryable()
                .FirstOrDefaultAsync(r =>
                    r.UserId == userId &&
                    r.CompanyProfileId == companyProfileId &&
                    r.StatusId == (short)CompanyJoinRequestStatus.Pending,
                    cancellationToken);

            if (existingPendingRequestDao != null)
                return Result<CompanyJoinRequest>.Failure("У вас уже есть ожидающий запрос на вступление в эту компанию");

            // Проверяем, не является ли пользователь уже участником компании
            var existingCompanyMember = await _companyMemberService.GetMemberAsync(userId, companyProfileId, cancellationToken);
            if (existingCompanyMember.IsSuccess)
                return Result<CompanyJoinRequest>.Failure("Вы уже являетесь участником этой компании");

            // Создаем новый запрос
            var companyJoinRequestDao = new CompanyJoinRequestDao
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                CompanyProfileId = companyProfileId,
                StatusId = (short)CompanyJoinRequestStatus.Pending,
                RequestMessage = requestMessage,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _companyJoinRequestRepository.CreateAsync(companyJoinRequestDao, cancellationToken);
            await SaveChangesAsync(cancellationToken);

            var companyJoinRequest = _mapper.Map<CompanyJoinRequest>(companyJoinRequestDao);
            return Result<CompanyJoinRequest>.Success(companyJoinRequest);
        }

        /// <summary>
        /// Одобрить запрос на вступление в компанию
        /// </summary>
        /// <param name="requestId">ID запроса</param>
        /// <param name="reviewerId">ID пользователя, который одобряет запрос</param>
        /// <param name="responseMessage">Сообщение от проверяющего</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Обновленный запрос</returns>
        public async Task<Result<CompanyJoinRequest>> ApproveJoinRequestAsync(
            Guid requestId,
            Guid reviewerId,
            string? responseMessage = null,
            CancellationToken cancellationToken = default)
        {
            var companyJoinRequestDao = await _companyJoinRequestRepository.GetQueryable()
                .FirstOrDefaultAsync(r => r.Id == requestId, cancellationToken);

            if (companyJoinRequestDao == null)
                return Result<CompanyJoinRequest>.Failure("Запрос на вступление не найден");

            if (companyJoinRequestDao.StatusId != (short)CompanyJoinRequestStatus.Pending)
                return Result<CompanyJoinRequest>.Failure("Запрос уже обработан");

            // TODO: Проверить, что reviewerId является владельцем или менеджером компании

            // Сначала добавляем пользователя как участника компании с ролью Employee
            var addCompanyMemberResult = await _companyMemberService.AddMemberAsync(
                companyJoinRequestDao.UserId,
                companyJoinRequestDao.CompanyProfileId,
                CompanyMemberRoleIds.Employee,
                cancellationToken: cancellationToken);

            if (!addCompanyMemberResult.IsSuccess)
                return Result<CompanyJoinRequest>.Failure($"Не удалось добавить пользователя в компанию: {addCompanyMemberResult.ErrorMessage}");

            // После успешного добавления участника обновляем статус запроса
            companyJoinRequestDao.StatusId = (short)CompanyJoinRequestStatus.Approved;
            companyJoinRequestDao.ReviewerId = reviewerId;
            companyJoinRequestDao.ReviewedAt = DateTime.UtcNow;
            companyJoinRequestDao.ResponseMessage = responseMessage;
            companyJoinRequestDao.UpdatedAt = DateTime.UtcNow;

            _companyJoinRequestRepository.Update(companyJoinRequestDao);
            await SaveChangesAsync(cancellationToken);

            var companyJoinRequest = _mapper.Map<CompanyJoinRequest>(companyJoinRequestDao);
            return Result<CompanyJoinRequest>.Success(companyJoinRequest);
        }

        /// <summary>
        /// Отклонить запрос на вступление в компанию
        /// </summary>
        /// <param name="requestId">ID запроса</param>
        /// <param name="reviewerId">ID пользователя, который отклоняет запрос</param>
        /// <param name="responseMessage">Сообщение от проверяющего</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Обновленный запрос</returns>
        public async Task<Result<CompanyJoinRequest>> RejectJoinRequestAsync(
            Guid requestId,
            Guid reviewerId,
            string? responseMessage = null,
            CancellationToken cancellationToken = default)
        {
            var companyJoinRequestDao = await _companyJoinRequestRepository.GetQueryable()
                .FirstOrDefaultAsync(r => r.Id == requestId, cancellationToken);

            if (companyJoinRequestDao == null)
                return Result<CompanyJoinRequest>.Failure("Запрос на вступление не найден");

            if (companyJoinRequestDao.StatusId != (short)CompanyJoinRequestStatus.Pending)
                return Result<CompanyJoinRequest>.Failure("Запрос уже обработан");

            // TODO: Проверить, что reviewerId является владельцем или менеджером компании

            // Обновляем статус запроса
            companyJoinRequestDao.StatusId = (short)CompanyJoinRequestStatus.Rejected;
            companyJoinRequestDao.ReviewerId = reviewerId;
            companyJoinRequestDao.ReviewedAt = DateTime.UtcNow;
            companyJoinRequestDao.ResponseMessage = responseMessage;
            companyJoinRequestDao.UpdatedAt = DateTime.UtcNow;

            _companyJoinRequestRepository.Update(companyJoinRequestDao);
            await SaveChangesAsync(cancellationToken);

            var companyJoinRequest = _mapper.Map<CompanyJoinRequest>(companyJoinRequestDao);
            return Result<CompanyJoinRequest>.Success(companyJoinRequest);
        }

        /// <summary>
        /// Отменить запрос на вступление (пользователем)
        /// </summary>
        /// <param name="requestId">ID запроса</param>
        /// <param name="userId">ID пользователя, который отменяет запрос</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Обновленный запрос</returns>
        public async Task<Result<CompanyJoinRequest>> CancelJoinRequestAsync(
            Guid requestId,
            Guid userId,
            CancellationToken cancellationToken = default)
        {
            var companyJoinRequestDao = await _companyJoinRequestRepository.GetQueryable()
                .FirstOrDefaultAsync(r => r.Id == requestId, cancellationToken);

            if (companyJoinRequestDao == null)
                return Result<CompanyJoinRequest>.Failure("Запрос на вступление не найден");

            if (companyJoinRequestDao.UserId != userId)
                return Result<CompanyJoinRequest>.Failure("Вы не можете отменить чужой запрос");

            if (companyJoinRequestDao.StatusId != (short)CompanyJoinRequestStatus.Pending)
                return Result<CompanyJoinRequest>.Failure("Можно отменить только ожидающий запрос");

            // Обновляем статус запроса
            companyJoinRequestDao.StatusId = (short)CompanyJoinRequestStatus.Cancelled;
            companyJoinRequestDao.UpdatedAt = DateTime.UtcNow;

            _companyJoinRequestRepository.Update(companyJoinRequestDao);
            await SaveChangesAsync(cancellationToken);

            var companyJoinRequest = _mapper.Map<CompanyJoinRequest>(companyJoinRequestDao);
            return Result<CompanyJoinRequest>.Success(companyJoinRequest);
        }

        /// <summary>
        /// Получить запрос по ID
        /// </summary>
        /// <param name="requestId">ID запроса</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Запрос на вступление</returns>
        public async Task<Result<CompanyJoinRequest>> GetJoinRequestAsync(
            Guid requestId,
            CancellationToken cancellationToken = default)
        {
            var companyJoinRequestDao = await _companyJoinRequestRepository.GetQueryable()
                .FirstOrDefaultAsync(r => r.Id == requestId, cancellationToken);

            if (companyJoinRequestDao == null)
                return Result<CompanyJoinRequest>.Failure("Запрос на вступление не найден");

            var companyJoinRequest = _mapper.Map<CompanyJoinRequest>(companyJoinRequestDao);
            return Result<CompanyJoinRequest>.Success(companyJoinRequest);
        }

        /// <summary>
        /// Получить все запросы пользователя
        /// </summary>
        /// <param name="userId">ID пользователя</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Список запросов пользователя</returns>
        public async Task<Result<List<CompanyJoinRequest>>> GetUserJoinRequestsAsync(
            Guid userId,
            CancellationToken cancellationToken = default)
        {
            var companyJoinRequestDaos = await _companyJoinRequestRepository.GetQueryable()
                .Where(r => r.UserId == userId)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync(cancellationToken);

            var companyJoinRequests = _mapper.Map<List<CompanyJoinRequest>>(companyJoinRequestDaos);
            return Result<List<CompanyJoinRequest>>.Success(companyJoinRequests);
        }

        /// <summary>
        /// Получить все запросы для компании
        /// </summary>
        /// <param name="companyProfileId">ID профиля компании</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Список запросов для компании</returns>
        public async Task<Result<List<CompanyJoinRequest>>> GetCompanyJoinRequestsAsync(
            Guid companyProfileId,
            CancellationToken cancellationToken = default)
        {
            var companyJoinRequestDaos = await _companyJoinRequestRepository.GetQueryable()
                .Where(r => r.CompanyProfileId == companyProfileId)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync(cancellationToken);

            var companyJoinRequests = _mapper.Map<List<CompanyJoinRequest>>(companyJoinRequestDaos);
            return Result<List<CompanyJoinRequest>>.Success(companyJoinRequests);
        }

        /// <summary>
        /// Получить ожидающие рассмотрения запросы для компании
        /// </summary>
        /// <param name="companyProfileId">ID профиля компании</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Список ожидающих запросов</returns>
        public async Task<Result<List<CompanyJoinRequest>>> GetPendingJoinRequestsAsync(
            Guid companyProfileId,
            CancellationToken cancellationToken = default)
        {
            var companyJoinRequestDaos = await _companyJoinRequestRepository.GetQueryable()
                .Where(r => r.CompanyProfileId == companyProfileId && r.StatusId == (short)CompanyJoinRequestStatus.Pending)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync(cancellationToken);

            var companyJoinRequests = _mapper.Map<List<CompanyJoinRequest>>(companyJoinRequestDaos);
            return Result<List<CompanyJoinRequest>>.Success(companyJoinRequests);
        }
    }
}
