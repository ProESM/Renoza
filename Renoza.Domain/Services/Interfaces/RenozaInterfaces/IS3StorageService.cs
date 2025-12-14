using Renoza.Domain.Services.Interfaces.BaseInterfaces;
using Renoza.Infrastructure.Contexts;

namespace Renoza.Domain.Services.Interfaces.RenozaInterfaces
{
    /// <summary>
    /// Интерфейс сервиса работы с S3 хранилищем
    /// </summary>
    public interface IS3StorageService : IBaseService<RenozaContext>
    {
        /// <summary>
        /// Загрузить файл в S3
        /// </summary>
        /// <param name="stream">Поток с данными файла</param>
        /// <param name="fileName">Имя файла</param>
        /// <param name="contentType">Тип контента</param>
        /// <param name="folder">Папка в бакете (опционально)</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>URL загруженного файла</returns>
        Task<string> UploadFileAsync(Stream stream, string fileName, string contentType, string? folder = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Получить файл из S3
        /// </summary>
        /// <param name="fileKey">Ключ файла в S3</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Поток с данными файла</returns>
        Task<Stream> GetFileAsync(string fileKey, CancellationToken cancellationToken = default);

        /// <summary>
        /// Удалить файл из S3
        /// </summary>
        /// <param name="fileKey">Ключ файла в S3</param>
        /// <param name="cancellationToken">Токен отмены</param>
        Task DeleteFileAsync(string fileKey, CancellationToken cancellationToken = default);

        /// <summary>
        /// Проверить существование файла
        /// </summary>
        /// <param name="fileKey">Ключ файла в S3</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>True, если файл существует</returns>
        Task<bool> FileExistsAsync(string fileKey, CancellationToken cancellationToken = default);

        /// <summary>
        /// Получить публичный URL файла
        /// </summary>
        /// <param name="fileKey">Ключ файла в S3</param>
        /// <returns>Публичный URL</returns>
        string GetPublicUrl(string fileKey);

        /// <summary>
        /// Получить временный URL для скачивания файла
        /// </summary>
        /// <param name="fileKey">Ключ файла в S3</param>
        /// <param name="expiresInMinutes">Время действия ссылки в минутах</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Временный URL</returns>
        Task<string> GetPresignedUrlAsync(string fileKey, int expiresInMinutes = 60, CancellationToken cancellationToken = default);

        /// <summary>
        /// Скопировать файл
        /// </summary>
        /// <param name="sourceKey">Ключ исходного файла</param>
        /// <param name="destinationKey">Ключ файла назначения</param>
        /// <param name="cancellationToken">Токен отмены</param>
        Task CopyFileAsync(string sourceKey, string destinationKey, CancellationToken cancellationToken = default);

        /// <summary>
        /// Получить список файлов в папке
        /// </summary>
        /// <param name="folder">Папка</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Список ключей файлов</returns>
        Task<List<string>> ListFilesAsync(string? folder = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Переместить файл из одного места в другое (копирование + удаление исходного)
        /// </summary>
        /// <param name="sourceKey">Ключ исходного файла</param>
        /// <param name="destinationKey">Ключ файла назначения</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>URL нового местоположения файла</returns>
        Task<string> MoveFileAsync(string sourceKey, string destinationKey, CancellationToken cancellationToken = default);

        /// <summary>
        /// Удалить папку со всем содержимым
        /// </summary>
        /// <param name="folderKey">Ключ папки (должен заканчиваться на /)</param>
        /// <param name="cancellationToken">Токен отмены</param>
        Task DeleteFolderAsync(string folderKey, CancellationToken cancellationToken = default);
    }
}
