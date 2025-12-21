using System.ComponentModel.DataAnnotations;

namespace Renoza.Domain.Entities.Documents
{
    /// <summary>
    /// Входные данные для загрузки шаблона документа
    /// </summary>
    public class UploadTemplateInput
    {
        /// <summary>
        /// Идентификатор типа шаблона
        /// </summary>
        [Required(ErrorMessage = "Укажите тип шаблона")]
        public short TemplateTypeId { get; set; }

        /// <summary>
        /// Название шаблона
        /// </summary>
        [Required(ErrorMessage = "Укажите название шаблона")]
        [MaxLength(500, ErrorMessage = "Название не должно превышать 500 символов")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Описание шаблона
        /// </summary>
        [MaxLength(2000, ErrorMessage = "Описание не должно превышать 2000 символов")]
        public string? Description { get; set; }

        /// <summary>
        /// Список доступных плейсхолдеров
        /// </summary>
        public List<string>? AvailablePlaceholders { get; set; }

        /// <summary>
        /// Идентификатор пользователя, загружающего шаблон
        /// </summary>
        [Required(ErrorMessage = "Идентификатор пользователя обязателен")]
        public Guid CreatedBy { get; set; }

        /// <summary>
        /// Содержимое файла шаблона в виде байтового массива
        /// </summary>
        [Required(ErrorMessage = "Файл шаблона обязателен")]
        public byte[] FileContent { get; set; } = Array.Empty<byte>();

        /// <summary>
        /// Имя файла шаблона
        /// </summary>
        [Required(ErrorMessage = "Имя файла обязательно")]
        [MaxLength(255, ErrorMessage = "Имя файла не должно превышать 255 символов")]
        public string FileName { get; set; } = string.Empty;
    }
}
