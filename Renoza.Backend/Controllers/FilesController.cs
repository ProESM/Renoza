using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Renoza.Domain.Options;
using Renoza.Domain.Services.Interfaces.RenozaInterfaces;

namespace Renoza.Backend.Controllers
{
    /// <summary>
    /// Контроллер для работы с файлами
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class FilesController : ControllerBase
    {
        private readonly ILogger<FilesController> _logger;
        private readonly IS3StorageService _s3StorageService;
        private readonly S3Options _s3Options;

        public FilesController(
            ILogger<FilesController> logger,
            IS3StorageService s3StorageService,
            IOptions<S3Options> s3Options)
        {
            _logger = logger;
            _s3StorageService = s3StorageService;
            _s3Options = s3Options.Value;
        }

        /// <summary>
        /// Загрузить файл
        /// </summary>
        /// <param name="file">Файл для загрузки</param>
        /// <param name="folder">Папка в хранилище (опционально)</param>
        /// <returns>URL загруженного файла</returns>
        [HttpPost("upload")]
        public async Task<ActionResult<object>> UploadFile(IFormFile file, [FromQuery] string? folder = null)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return BadRequest(new { message = "Файл не выбран" });
                }

                // Проверка размера файла
                if (file.Length > _s3Options.MaxFileSize)
                {
                    return BadRequest(new { message = $"Файл слишком большой. Максимальный размер: {_s3Options.MaxFileSize / 1024 / 1024} МБ" });
                }

                // Проверка типа файла
                if (_s3Options.AllowedContentTypes.Length > 0 &&
                    !_s3Options.AllowedContentTypes.Contains(file.ContentType))
                {
                    return BadRequest(new { message = "Недопустимый тип файла" });
                }

                // Генерируем уникальное имя файла
                var fileExtension = Path.GetExtension(file.FileName);
                var uniqueFileName = $"{Guid.NewGuid()}{fileExtension}";

                using var stream = file.OpenReadStream();
                var fileUrl = await _s3StorageService.UploadFileAsync(
                    stream,
                    uniqueFileName,
                    file.ContentType,
                    folder);

                _logger.LogInformation("Файл успешно загружен: {FileName} -> {FileUrl}", file.FileName, fileUrl);

                return Ok(new
                {
                    url = fileUrl,
                    fileName = uniqueFileName,
                    originalFileName = file.FileName,
                    contentType = file.ContentType,
                    size = file.Length
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при загрузке файла: {FileName}", file?.FileName);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "Произошла ошибка при загрузке файла" });
            }
        }

        /// <summary>
        /// Загрузить несколько файлов
        /// </summary>
        /// <param name="files">Файлы для загрузки</param>
        /// <param name="folder">Папка в хранилище (опционально)</param>
        /// <returns>Список URL загруженных файлов</returns>
        [HttpPost("upload-multiple")]
        public async Task<ActionResult<object>> UploadMultipleFiles(List<IFormFile> files, [FromQuery] string? folder = null)
        {
            try
            {
                if (files == null || files.Count == 0)
                {
                    return BadRequest(new { message = "Файлы не выбраны" });
                }

                var uploadedFiles = new List<object>();

                foreach (var file in files)
                {
                    if (file.Length > _s3Options.MaxFileSize)
                    {
                        _logger.LogWarning("Файл {FileName} пропущен: слишком большой", file.FileName);
                        continue;
                    }

                    if (_s3Options.AllowedContentTypes.Length > 0 &&
                        !_s3Options.AllowedContentTypes.Contains(file.ContentType))
                    {
                        _logger.LogWarning("Файл {FileName} пропущен: недопустимый тип", file.FileName);
                        continue;
                    }

                    var fileExtension = Path.GetExtension(file.FileName);
                    var uniqueFileName = $"{Guid.NewGuid()}{fileExtension}";

                    using var stream = file.OpenReadStream();
                    var fileUrl = await _s3StorageService.UploadFileAsync(
                        stream,
                        uniqueFileName,
                        file.ContentType,
                        folder);

                    uploadedFiles.Add(new
                    {
                        url = fileUrl,
                        fileName = uniqueFileName,
                        originalFileName = file.FileName,
                        contentType = file.ContentType,
                        size = file.Length
                    });
                }

                _logger.LogInformation("Загружено файлов: {Count} из {Total}", uploadedFiles.Count, files.Count);

                return Ok(new
                {
                    files = uploadedFiles,
                    totalUploaded = uploadedFiles.Count,
                    totalFiles = files.Count
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при загрузке файлов");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "Произошла ошибка при загрузке файлов" });
            }
        }

        /// <summary>
        /// Скачать файл
        /// </summary>
        /// <param name="fileKey">Ключ файла в хранилище</param>
        /// <returns>Файл</returns>
        [HttpGet("download/{**fileKey}")]
        public async Task<IActionResult> DownloadFile(string fileKey)
        {
            try
            {
                var exists = await _s3StorageService.FileExistsAsync(fileKey);
                if (!exists)
                {
                    return NotFound(new { message = "Файл не найден" });
                }

                var stream = await _s3StorageService.GetFileAsync(fileKey);
                var fileName = Path.GetFileName(fileKey);

                return File(stream, "application/octet-stream", fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при скачивании файла: {FileKey}", fileKey);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "Произошла ошибка при скачивании файла" });
            }
        }

        /// <summary>
        /// Получить временную ссылку для скачивания файла
        /// </summary>
        /// <param name="fileKey">Ключ файла в хранилище</param>
        /// <param name="expiresInMinutes">Время действия ссылки в минутах (по умолчанию 60)</param>
        /// <returns>Временная ссылка</returns>
        [HttpGet("presigned-url/{**fileKey}")]
        public async Task<ActionResult<object>> GetPresignedUrl(string fileKey, [FromQuery] int expiresInMinutes = 60)
        {
            try
            {
                var exists = await _s3StorageService.FileExistsAsync(fileKey);
                if (!exists)
                {
                    return NotFound(new { message = "Файл не найден" });
                }

                var url = await _s3StorageService.GetPresignedUrlAsync(fileKey, expiresInMinutes);

                return Ok(new
                {
                    url,
                    expiresIn = expiresInMinutes,
                    fileKey
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при создании временной ссылки: {FileKey}", fileKey);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "Произошла ошибка при создании временной ссылки" });
            }
        }

        /// <summary>
        /// Удалить файл
        /// </summary>
        /// <param name="fileKey">Ключ файла в хранилище</param>
        /// <returns>Результат удаления</returns>
        [HttpDelete("{**fileKey}")]
        public async Task<IActionResult> DeleteFile(string fileKey)
        {
            try
            {
                var exists = await _s3StorageService.FileExistsAsync(fileKey);
                if (!exists)
                {
                    return NotFound(new { message = "Файл не найден" });
                }

                await _s3StorageService.DeleteFileAsync(fileKey);

                _logger.LogInformation("Файл удален: {FileKey}", fileKey);

                return Ok(new { message = "Файл успешно удален" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при удалении файла: {FileKey}", fileKey);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "Произошла ошибка при удалении файла" });
            }
        }

        /// <summary>
        /// Получить список файлов в папке
        /// </summary>
        /// <param name="folder">Папка (опционально)</param>
        /// <returns>Список файлов</returns>
        [HttpGet("list")]
        public async Task<ActionResult<object>> ListFiles([FromQuery] string? folder = null)
        {
            try
            {
                var files = await _s3StorageService.ListFilesAsync(folder);

                return Ok(new
                {
                    files,
                    count = files.Count,
                    folder
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении списка файлов в папке: {Folder}", folder);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "Произошла ошибка при получении списка файлов" });
            }
        }
    }
}
