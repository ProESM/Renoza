using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Renoza.Common.Helpers;
using Renoza.Domain.Entities.Documents;
using Renoza.Domain.Enums;
using Renoza.Domain.Services.Implementations.BaseImplementations;
using Renoza.Domain.Services.Interfaces.RenozaInterfaces;
using Renoza.Infrastructure.Contexts;
using Renoza.Infrastructure.Entities.Renoza;
using Renoza.Infrastructure.Repositories.Interfaces.BaseInterfaces;

namespace Renoza.Domain.Services.Implementations.RenozaImplementations
{
    /// <summary>
    /// Сервис работы с документами
    /// </summary>
    public class DocumentService : BaseService<RenozaContext>, IDocumentService
    {
        private readonly ILogger<DocumentService> _logger;
        private readonly IMapper _mapper;
        private readonly IS3StorageService _s3StorageService;
        private readonly IDocumentPlaceholderService _placeholderService;
        private readonly IEntityWithIdRepository<DocumentTemplateDao, Guid> _templateRepository;
        private readonly IEntityWithIdRepository<DocumentDao, Guid> _documentRepository;
        private readonly IEntityWithIdRepository<DocumentFormatDao, short> _formatRepository;

        public DocumentService(
            ILogger<DocumentService> logger,
            IMapper mapper,
            RenozaContext dbContext,
            IS3StorageService s3StorageService,
            IDocumentPlaceholderService placeholderService,
            IEntityWithIdRepository<DocumentTemplateDao, Guid> templateRepository,
            IEntityWithIdRepository<DocumentDao, Guid> documentRepository,
            IEntityWithIdRepository<DocumentFormatDao, short> formatRepository)
            : base(dbContext)
        {
            _logger = logger;
            _mapper = mapper;
            _s3StorageService = s3StorageService;
            _placeholderService = placeholderService;
            _templateRepository = templateRepository;
            _documentRepository = documentRepository;
            _formatRepository = formatRepository;
        }

        /// <summary>
        /// Создать документ из шаблона
        /// </summary>
        public async Task<Result<DocumentGenerationResult>> CreateAsync(
            GenerateDocumentInput input,
            CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation($"Начата генерация документа: {input.Name}");

                // Получаем шаблон
                var templateDao = await _templateRepository.GetQueryable()
                    .AsNoTracking()
                    .Include(t => t.TemplateType)
                    .FirstOrDefaultAsync(t => t.Id == input.TemplateId, cancellationToken);

                if (templateDao == null)
                {
                    return Result<DocumentGenerationResult>.Failure($"Шаблон с ID {input.TemplateId} не найден");
                }

                if (!templateDao.IsActive)
                {
                    return Result<DocumentGenerationResult>.Failure("Шаблон неактивен");
                }

                // Получаем формат
                var formatDao = await _formatRepository.GetQueryable()
                    .AsNoTracking()
                    .FirstOrDefaultAsync(f => f.Id == input.FormatId, cancellationToken);

                if (formatDao == null)
                {
                    return Result<DocumentGenerationResult>.Failure($"Формат с ID {input.FormatId} не найден");
                }

                // Скачиваем шаблон из S3
                var templateFileKey = ExtractFileKeyFromUrl(templateDao.FileUrl);
                var templateStream = await _s3StorageService.GetFileAsync(templateFileKey, cancellationToken);

                // Заменяем плейсхолдеры
                var processedStream = await _placeholderService.ReplacePlaceholdersInDocxAsync(
                    templateStream,
                    input.PlaceholderData);

                // Конвертируем в нужный формат (если не .docx)
                Stream finalStream = processedStream;
                string contentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                string fileExtension = ".docx";

                if (input.FormatId == (short)Enums.DocumentFormat.Rtf)
                {
                    finalStream = await _placeholderService.ConvertDocxToRtfAsync(processedStream);
                    contentType = "application/rtf";
                    fileExtension = ".rtf";
                }
                else if (input.FormatId == (short)Enums.DocumentFormat.Pdf)
                {
                    finalStream = await _placeholderService.ConvertDocxToPdfAsync(processedStream);
                    contentType = "application/pdf";
                    fileExtension = ".pdf";
                }

                // Загружаем в S3
                var folder = $"documents/{DateTime.UtcNow:yyyy/MM/dd}";
                var fileName = $"{Guid.NewGuid()}{fileExtension}";

                // Сбрасываем позицию потока перед загрузкой
                finalStream.Position = 0;
                var fileUrl = await _s3StorageService.UploadFileAsync(
                    finalStream,
                    fileName,
                    contentType,
                    folder,
                    cancellationToken);

                // Создаем запись в БД
                var documentDao = new DocumentDao
                {
                    Id = Guid.NewGuid(),
                    TemplateId = input.TemplateId,
                    FormatId = input.FormatId,
                    StatusId = (short)Enums.DocumentStatus.Generated,
                    Name = input.Name,
                    FileUrl = fileUrl,
                    FileSize = finalStream.Length,
                    PlaceholderData = JsonConvert.SerializeObject(input.PlaceholderData),
                    OrderId = input.OrderId,
                    CustomerId = input.CustomerId,
                    CreatedBy = input.CreatedBy,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    DownloadCount = 0
                };

                await _documentRepository.CreateAsync(documentDao, cancellationToken);
                await SaveChangesAsync(cancellationToken);

                _logger.LogInformation($"Документ успешно сгенерирован: {documentDao.Id}");

                // Формируем результат
                var result = new DocumentGenerationResult
                {
                    DocumentId = documentDao.Id,
                    DocumentName = documentDao.Name,
                    FileUrl = documentDao.FileUrl,
                    FileSize = documentDao.FileSize,
                    FormatName = formatDao.Name,
                    FileExtension = formatDao.FileExtension,
                    CreatedAt = documentDao.CreatedAt
                };

                return Result<DocumentGenerationResult>.Success(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Ошибка при генерации документа: {ex.Message}");
                return Result<DocumentGenerationResult>.Failure($"Ошибка при генерации документа: {ex.Message}");
            }
        }

        /// <summary>
        /// Получить документ по идентификатору
        /// </summary>
        public async Task<Result<Document>> GetDocumentByIdAsync(Guid documentId, CancellationToken cancellationToken = default)
        {
            try
            {
                var documentDao = await _documentRepository.GetQueryable()
                    .AsNoTracking()
                    .Include(d => d.Template)
                    .Include(d => d.Format)
                    .Include(d => d.Status)
                    .FirstOrDefaultAsync(d => d.Id == documentId, cancellationToken);

                if (documentDao == null)
                {
                    return Result<Document>.Failure($"Документ с ID {documentId} не найден");
                }

                var document = _mapper.Map<Document>(documentDao);
                return Result<Document>.Success(document);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Ошибка при получении документа {documentId}: {ex.Message}");
                return Result<Document>.Failure($"Ошибка при получении документа: {ex.Message}");
            }
        }

        /// <summary>
        /// Получить список документов
        /// </summary>
        public async Task<Result<List<Document>>> GetDocumentsAsync(
            Guid? templateId = null,
            Guid? customerId = null,
            Guid? orderId = null,
            short? statusId = null,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var query = _documentRepository.GetQueryable()
                    .AsNoTracking()
                    .Include(d => d.Template)
                    .Include(d => d.Format)
                    .Include(d => d.Status)
                    .AsQueryable();

                if (templateId.HasValue)
                {
                    query = query.Where(d => d.TemplateId == templateId.Value);
                }

                if (customerId.HasValue)
                {
                    query = query.Where(d => d.CustomerId == customerId.Value);
                }

                if (orderId.HasValue)
                {
                    query = query.Where(d => d.OrderId == orderId.Value);
                }

                if (statusId.HasValue)
                {
                    query = query.Where(d => d.StatusId == statusId.Value);
                }

                var documentDaos = await query
                    .OrderByDescending(d => d.CreatedAt)
                    .ToListAsync(cancellationToken);

                var documents = _mapper.Map<List<Document>>(documentDaos);
                return Result<List<Document>>.Success(documents);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Ошибка при получении списка документов: {ex.Message}");
                return Result<List<Document>>.Failure($"Ошибка при получении списка документов: {ex.Message}");
            }
        }

        /// <summary>
        /// Скачать документ
        /// </summary>
        public async Task<Result<DocumentDownloadResult>> DownloadDocumentAsync(Guid documentId, CancellationToken cancellationToken = default)
        {
            try
            {
                var documentDao = await _documentRepository.GetQueryable()
                    .Include(d => d.Format)
                    .FirstOrDefaultAsync(d => d.Id == documentId, cancellationToken);

                if (documentDao == null)
                {
                    return Result<DocumentDownloadResult>.Failure($"Документ с ID {documentId} не найден");
                }

                // Обновляем статистику скачиваний
                documentDao.DownloadCount++;
                documentDao.LastDownloadedAt = DateTime.UtcNow;
                if (!documentDao.FirstDownloadedAt.HasValue)
                {
                    documentDao.FirstDownloadedAt = DateTime.UtcNow;
                }

                // Обновляем статус на "Скачан"
                if (documentDao.StatusId == (short)Enums.DocumentStatus.Generated)
                {
                    documentDao.StatusId = (short)Enums.DocumentStatus.Downloaded;
                }

                documentDao.UpdatedAt = DateTime.UtcNow;

                _documentRepository.Update(documentDao);
                await SaveChangesAsync(cancellationToken);

                // Скачиваем файл из S3
                var fileKey = ExtractFileKeyFromUrl(documentDao.FileUrl);
                var stream = await _s3StorageService.GetFileAsync(fileKey, cancellationToken);

                var result = new DocumentDownloadResult
                {
                    FileStream = stream,
                    DocumentName = documentDao.Name,
                    FileExtension = documentDao.Format.FileExtension
                };

                return Result<DocumentDownloadResult>.Success(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Ошибка при скачивании документа {documentId}: {ex.Message}");
                return Result<DocumentDownloadResult>.Failure($"Ошибка при скачивании документа: {ex.Message}");
            }
        }

        /// <summary>
        /// Удалить документ
        /// </summary>
        public async Task<Result<bool>> DeleteDocumentAsync(Guid documentId, CancellationToken cancellationToken = default)
        {
            try
            {
                var documentDao = await _documentRepository.GetQueryable()
                    .FirstOrDefaultAsync(d => d.Id == documentId, cancellationToken);

                if (documentDao == null)
                {
                    return Result<bool>.Failure($"Документ с ID {documentId} не найден");
                }

                // Удаляем файл из S3
                var fileKey = ExtractFileKeyFromUrl(documentDao.FileUrl);
                await _s3StorageService.DeleteFileAsync(fileKey, cancellationToken);

                // Удаляем запись из БД (или помечаем как удаленный)
                documentDao.StatusId = (short)Enums.DocumentStatus.Deleted;
                documentDao.UpdatedAt = DateTime.UtcNow;

                _documentRepository.Update(documentDao);
                await SaveChangesAsync(cancellationToken);

                _logger.LogInformation($"Документ успешно удален: {documentId}");
                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Ошибка при удалении документа {documentId}: {ex.Message}");
                return Result<bool>.Failure($"Ошибка при удалении документа: {ex.Message}");
            }
        }

        /// <summary>
        /// Извлечь ключ файла из URL
        /// </summary>
        private string ExtractFileKeyFromUrl(string fileUrl)
        {
            var uri = new Uri(fileUrl);
            return uri.AbsolutePath.TrimStart('/');
        }
    }
}
