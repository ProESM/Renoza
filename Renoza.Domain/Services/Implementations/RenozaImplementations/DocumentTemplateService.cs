using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Renoza.Common.Base.Helpers;
using Renoza.Domain.Entities.Documents;
using Renoza.Domain.Services.Implementations.BaseImplementations;
using Renoza.Domain.Services.Interfaces.RenozaInterfaces;
using Renoza.Infrastructure.Contexts;
using Renoza.Infrastructure.Entities.Renoza;
using Renoza.Infrastructure.Repositories.Interfaces.BaseInterfaces;

namespace Renoza.Domain.Services.Implementations.RenozaImplementations
{
    /// <summary>
    /// Сервис работы с шаблонами документов
    /// </summary>
    public class DocumentTemplateService : BaseService<RenozaContext>, IDocumentTemplateService
    {
        #region Репозитории

        /// <summary>
        /// Репозиторий для работы с шаблонами документов
        /// </summary>
        private readonly IEntityWithIdRepository<DocumentTemplateDao, Guid> _templateRepository;

        #endregion

        #region Сервисы

        /// <summary>
        /// Сервис для работы с S3 хранилищем
        /// </summary>
        private readonly IS3StorageService _s3StorageService;

        /// <summary>
        /// Сервис для работы с плейсхолдерами документов
        /// </summary>
        private readonly IDocumentPlaceholderService _placeholderService;

        #endregion

        #region Мапперы

        /// <summary>
        /// Маппер для преобразования между DAO и Domain сущностями
        /// </summary>
        private readonly IMapper _mapper;

        #endregion

        #region Логгеры

        /// <summary>
        /// Логгер
        /// </summary>
        private readonly ILogger<DocumentTemplateService> _logger;

        #endregion

        /// <summary>
        /// Сервис работы с шаблонами документов
        /// </summary>
        /// <param name="logger">Логгер</param>
        /// <param name="mapper">Маппер для преобразования между DAO и Domain сущностями</param>
        /// <param name="context">Контекст базы данных</param>
        /// <param name="s3StorageService">Сервис для работы с S3 хранилищем</param>
        /// <param name="placeholderService">Сервис для работы с плейсхолдерами документов</param>
        /// <param name="templateRepository">Репозиторий для работы с шаблонами документов</param>
        public DocumentTemplateService(
            ILogger<DocumentTemplateService> logger,
            IMapper mapper,
            RenozaContext context,
            IS3StorageService s3StorageService,
            IDocumentPlaceholderService placeholderService,
            IEntityWithIdRepository<DocumentTemplateDao, Guid> templateRepository)
            : base(context)
        {
            _logger = logger;
            _mapper = mapper;
            _s3StorageService = s3StorageService;
            _placeholderService = placeholderService;
            _templateRepository = templateRepository;
        }

        /// <summary>
        /// Загрузить шаблон документа
        /// </summary>
        public async Task<Result<Guid>> UploadTemplateAsync(UploadTemplateInput input, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation($"Начата загрузка шаблона: {input.Name}");

                // Проверяем, что файл имеет расширение .docx
                if (!input.FileName.EndsWith(".docx", StringComparison.OrdinalIgnoreCase))
                {
                    return Result<Guid>.Failure("Поддерживаются только файлы формата .docx");
                }

                // Извлекаем плейсхолдеры из шаблона (если не указаны вручную)
                List<string>? placeholders = input.AvailablePlaceholders;
                if (placeholders == null || !placeholders.Any())
                {
                    using var templateStream = new MemoryStream(input.FileContent);
                    placeholders = await _placeholderService.ExtractPlaceholdersFromDocxAsync(templateStream);
                    _logger.LogInformation($"Извлечено {placeholders.Count} плейсхолдеров из шаблона");
                }

                // Загружаем файл в S3
                using var fileStream = new MemoryStream(input.FileContent);
                var folder = $"templates/{input.TemplateTypeId}";
                var fileName = $"{Path.GetFileNameWithoutExtension(input.FileName)}_{Guid.NewGuid()}.docx";
                var fileUrl = await _s3StorageService.UploadFileAsync(
                    fileStream,
                    fileName,
                    "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                    folder,
                    cancellationToken);

                // Создаем запись в БД
                var templateDao = new DocumentTemplateDao
                {
                    Id = Guid.NewGuid(),
                    TemplateTypeId = input.TemplateTypeId,
                    Name = input.Name,
                    Description = input.Description,
                    FileUrl = fileUrl,
                    FileSize = input.FileContent.Length,
                    AvailablePlaceholders = placeholders.Any()
                        ? JsonConvert.SerializeObject(placeholders)
                        : null,
                    CreatedBy = input.CreatedBy,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                await _templateRepository.CreateAsync(templateDao, cancellationToken);
                await SaveChangesAsync(cancellationToken);

                _logger.LogInformation($"Шаблон успешно загружен: {templateDao.Id}");
                return Result<Guid>.Success(templateDao.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Ошибка при загрузке шаблона: {ex.Message}");
                return Result<Guid>.Failure($"Ошибка при загрузке шаблона: {ex.Message}");
            }
        }

        /// <summary>
        /// Получить шаблон по идентификатору
        /// </summary>
        public async Task<Result<DocumentTemplate>> GetTemplateByIdAsync(Guid templateId, CancellationToken cancellationToken = default)
        {
            try
            {
                var templateDao = await _templateRepository.GetQueryable()
                    .AsNoTracking()
                    .Include(t => t.TemplateType)
                    .FirstOrDefaultAsync(t => t.Id == templateId, cancellationToken);

                if (templateDao == null)
                {
                    return Result<DocumentTemplate>.Failure($"Шаблон с ID {templateId} не найден");
                }

                var template = _mapper.Map<DocumentTemplate>(templateDao);
                return Result<DocumentTemplate>.Success(template);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Ошибка при получении шаблона {templateId}: {ex.Message}");
                return Result<DocumentTemplate>.Failure($"Ошибка при получении шаблона: {ex.Message}");
            }
        }

        /// <summary>
        /// Получить список шаблонов
        /// </summary>
        public async Task<Result<List<DocumentTemplate>>> GetTemplatesAsync(
            short? templateTypeId = null,
            bool? isActive = null,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var query = _templateRepository.GetQueryable()
                    .AsNoTracking()
                    .Include(t => t.TemplateType)
                    .AsQueryable();

                if (templateTypeId.HasValue)
                {
                    query = query.Where(t => t.TemplateTypeId == templateTypeId.Value);
                }

                if (isActive.HasValue)
                {
                    query = query.Where(t => t.IsActive == isActive.Value);
                }

                var templateDaos = await query
                    .OrderByDescending(t => t.CreatedAt)
                    .ToListAsync(cancellationToken);

                var templates = _mapper.Map<List<DocumentTemplate>>(templateDaos);
                return Result<List<DocumentTemplate>>.Success(templates);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Ошибка при получении списка шаблонов: {ex.Message}");
                return Result<List<DocumentTemplate>>.Failure($"Ошибка при получении списка шаблонов: {ex.Message}");
            }
        }

        /// <summary>
        /// Удалить шаблон
        /// </summary>
        public async Task<Result<bool>> DeleteTemplateAsync(Guid templateId, CancellationToken cancellationToken = default)
        {
            try
            {
                var templateDao = await _templateRepository.GetQueryable()
                    .FirstOrDefaultAsync(t => t.Id == templateId, cancellationToken);

                if (templateDao == null)
                {
                    return Result<bool>.Failure($"Шаблон с ID {templateId} не найден");
                }

                // Удаляем файл из S3
                var fileKey = ExtractFileKeyFromUrl(templateDao.FileUrl);
                await _s3StorageService.DeleteFileAsync(fileKey, cancellationToken);

                // Удаляем запись из БД
                _templateRepository.Delete(templateDao);
                await SaveChangesAsync(cancellationToken);

                _logger.LogInformation($"Шаблон успешно удален: {templateId}");
                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Ошибка при удалении шаблона {templateId}: {ex.Message}");
                return Result<bool>.Failure($"Ошибка при удалении шаблона: {ex.Message}");
            }
        }

        /// <summary>
        /// Скачать файл шаблона
        /// </summary>
        public async Task<Result<Stream>> DownloadTemplateFileAsync(Guid templateId, CancellationToken cancellationToken = default)
        {
            try
            {
                var templateDao = await _templateRepository.GetQueryable()
                    .AsNoTracking()
                    .FirstOrDefaultAsync(t => t.Id == templateId, cancellationToken);

                if (templateDao == null)
                {
                    return Result<Stream>.Failure($"Шаблон с ID {templateId} не найден");
                }

                var fileKey = ExtractFileKeyFromUrl(templateDao.FileUrl);
                var stream = await _s3StorageService.GetFileAsync(fileKey, cancellationToken);

                return Result<Stream>.Success(stream);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Ошибка при скачивании файла шаблона {templateId}: {ex.Message}");
                return Result<Stream>.Failure($"Ошибка при скачивании файла: {ex.Message}");
            }
        }

        /// <summary>
        /// Извлечь ключ файла из URL
        /// </summary>
        private string ExtractFileKeyFromUrl(string fileUrl)
        {
            // Предполагаем, что URL имеет формат: https://bucket.s3.region.amazonaws.com/key
            // или CloudFront URL, нужно извлечь ключ
            var uri = new Uri(fileUrl);
            return uri.AbsolutePath.TrimStart('/');
        }
    }
}
