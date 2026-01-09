using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Options;
using Renoza.Domain.Options;
using Renoza.Domain.Services.Implementations.BaseImplementations;
using Renoza.Domain.Services.Interfaces.RenozaInterfaces;
using Renoza.Infrastructure.Contexts;

namespace Renoza.Domain.Services.Implementations.RenozaImplementations
{
    /// <summary>
    /// Сервис работы с S3 хранилищем
    /// </summary>
    public class S3StorageService : BaseService<RenozaContext>, IS3StorageService
    {
        #region Клиенты

        /// <summary>
        /// Клиент для работы с Amazon S3
        /// </summary>
        private readonly IAmazonS3 _s3Client;

        #endregion

        #region Настройки

        /// <summary>
        /// Настройки S3 хранилища
        /// </summary>
        private readonly S3Options _s3Options;

        #endregion

        /// <summary>
        /// Сервис работы с S3 хранилищем
        /// </summary>
        /// <param name="context">Контекст базы данных</param>
        /// <param name="s3Client">Клиент для работы с Amazon S3</param>
        /// <param name="s3Options">Настройки S3 хранилища</param>
        public S3StorageService(
            RenozaContext context,
            IAmazonS3 s3Client,
            IOptions<S3Options> s3Options) : base(context)
        {
            _s3Client = s3Client;
            _s3Options = s3Options.Value;
        }

        /// <summary>
        /// Загрузить файл в S3
        /// </summary>
        public async Task<string> UploadFileAsync(Stream stream, string fileName, string contentType, string? folder = null, CancellationToken cancellationToken = default)
        {
            var fileKey = string.IsNullOrEmpty(folder)
                ? fileName
                : $"{folder.TrimEnd('/')}/{fileName}";

            var request = new PutObjectRequest
            {
                BucketName = _s3Options.BucketName,
                Key = fileKey,
                InputStream = stream,
                ContentType = contentType,
                CannedACL = S3CannedACL.Private // Или PublicRead если нужен публичный доступ
            };

            await _s3Client.PutObjectAsync(request, cancellationToken);

            return GetPublicUrl(fileKey);
        }

        /// <summary>
        /// Получить файл из S3
        /// </summary>
        public async Task<Stream> GetFileAsync(string fileKey, CancellationToken cancellationToken = default)
        {
            var request = new GetObjectRequest
            {
                BucketName = _s3Options.BucketName,
                Key = fileKey
            };

            var response = await _s3Client.GetObjectAsync(request, cancellationToken);
            return response.ResponseStream;
        }

        /// <summary>
        /// Удалить файл из S3
        /// </summary>
        public async Task DeleteFileAsync(string fileKey, CancellationToken cancellationToken = default)
        {
            var request = new DeleteObjectRequest
            {
                BucketName = _s3Options.BucketName,
                Key = fileKey
            };

            await _s3Client.DeleteObjectAsync(request, cancellationToken);
        }

        /// <summary>
        /// Проверить существование файла
        /// </summary>
        public async Task<bool> FileExistsAsync(string fileKey, CancellationToken cancellationToken = default)
        {
            try
            {
                var request = new GetObjectMetadataRequest
                {
                    BucketName = _s3Options.BucketName,
                    Key = fileKey
                };

                await _s3Client.GetObjectMetadataAsync(request, cancellationToken);
                return true;
            }
            catch (AmazonS3Exception ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return false;
            }
        }

        /// <summary>
        /// Получить публичный URL файла
        /// </summary>
        public string GetPublicUrl(string fileKey)
        {
            var baseUrl = _s3Options.PublicUrl ?? _s3Options.ServiceUrl;
            if (string.IsNullOrEmpty(baseUrl))
            {
                throw new InvalidOperationException("PublicUrl или ServiceUrl должен быть настроен");
            }

            return $"{baseUrl.TrimEnd('/')}/{_s3Options.BucketName}/{fileKey}";
        }

        /// <summary>
        /// Получить временный URL для скачивания файла
        /// </summary>
        public async Task<string> GetPresignedUrlAsync(string fileKey, int expiresInMinutes = 60, CancellationToken cancellationToken = default)
        {
            var request = new GetPreSignedUrlRequest
            {
                BucketName = _s3Options.BucketName,
                Key = fileKey,
                Expires = DateTime.UtcNow.AddMinutes(expiresInMinutes)
            };

            return await Task.FromResult(_s3Client.GetPreSignedURL(request));
        }

        /// <summary>
        /// Скопировать файл
        /// </summary>
        public async Task CopyFileAsync(string sourceKey, string destinationKey, CancellationToken cancellationToken = default)
        {
            var request = new CopyObjectRequest
            {
                SourceBucket = _s3Options.BucketName,
                SourceKey = sourceKey,
                DestinationBucket = _s3Options.BucketName,
                DestinationKey = destinationKey
            };

            await _s3Client.CopyObjectAsync(request, cancellationToken);
        }

        /// <summary>
        /// Получить список файлов в папке
        /// </summary>
        public async Task<List<string>> ListFilesAsync(string? folder = null, CancellationToken cancellationToken = default)
        {
            var request = new ListObjectsV2Request
            {
                BucketName = _s3Options.BucketName,
                Prefix = folder
            };

            var response = await _s3Client.ListObjectsV2Async(request, cancellationToken);
            return response.S3Objects != null
                ? response.S3Objects.Select(o => o.Key).ToList()
                : new List<string>();
        }

        /// <summary>
        /// Переместить файл из одного места в другое (копирование + удаление исходного)
        /// </summary>
        public async Task<string> MoveFileAsync(string sourceKey, string destinationKey, CancellationToken cancellationToken = default)
        {
            // Копируем файл в новое место
            await CopyFileAsync(sourceKey, destinationKey, cancellationToken);

            // Удаляем исходный файл
            await DeleteFileAsync(sourceKey, cancellationToken);

            // Возвращаем URL нового местоположения
            return GetPublicUrl(destinationKey);
        }

        /// <summary>
        /// Удалить папку со всем содержимым
        /// </summary>
        public async Task DeleteFolderAsync(string folderKey, CancellationToken cancellationToken = default)
        {
            // Нормализуем ключ папки - убеждаемся, что он заканчивается на /
            var normalizedFolderKey = folderKey.TrimEnd('/') + "/";

            // Получаем список всех файлов в папке
            var fileKeys = await ListFilesAsync(normalizedFolderKey, cancellationToken);

            // Если файлов нет, выходим
            if (fileKeys == null || fileKeys.Count == 0)
            {
                return;
            }

            // Удаляем все файлы в папке
            // S3 поддерживает пакетное удаление до 1000 объектов за раз
            foreach (var batch in fileKeys.Chunk(1000))
            {
                var deleteRequest = new DeleteObjectsRequest
                {
                    BucketName = _s3Options.BucketName,
                    Objects = batch.Select(key => new KeyVersion { Key = key }).ToList()
                };

                await _s3Client.DeleteObjectsAsync(deleteRequest, cancellationToken);
            }
        }
    }
}
