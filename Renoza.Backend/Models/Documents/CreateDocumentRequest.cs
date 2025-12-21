using System.ComponentModel.DataAnnotations;

namespace Renoza.Backend.Models.Documents
{
    /// <summary>
    /// Запрос на создание документа из шаблона
    /// </summary>
    public class CreateDocumentRequest
    {
        /// <summary>
        /// Идентификатор шаблона документа
        /// </summary>
        [Required(ErrorMessage = "Укажите шаблон документа")]
        public Guid TemplateId { get; set; }

        /// <summary>
        /// Идентификатор формата документа (Docx, Rtf, Pdf)
        /// </summary>
        [Required(ErrorMessage = "Укажите формат документа")]
        public short FormatId { get; set; }

        /// <summary>
        /// Название документа
        /// </summary>
        [Required(ErrorMessage = "Укажите название документа")]
        [MaxLength(500, ErrorMessage = "Название не должно превышать 500 символов")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Данные для замены плейсхолдеров
        /// Например: { "CustomerName": "ООО Рога и копыта", "ContractNumber": "123/2024" }
        /// </summary>
        [Required(ErrorMessage = "Укажите данные для заполнения")]
        public Dictionary<string, string> PlaceholderData { get; set; } = new();

        /// <summary>
        /// Идентификатор заказа (опционально)
        /// </summary>
        public Guid? OrderId { get; set; }

        /// <summary>
        /// Идентификатор заказчика (опционально)
        /// </summary>
        public Guid? CustomerId { get; set; }
    }
}
