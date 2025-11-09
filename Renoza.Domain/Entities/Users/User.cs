using Newtonsoft.Json;
using Renoza.Common.Base.Entities;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Renoza.Domain.Entities.Users
{
    /// <summary>
    /// Пользователь
    /// </summary>
    [DataContract]
    [Serializable]
    public class User : IEntityWithId<Guid>
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        [Display(Name = "Id")]
        [DataMember]
        [JsonProperty(PropertyName = "Id")]
        public Guid Id { get; set; }
        /// <summary>
        /// Отображаемое имя
        /// </summary>
        [Display(Name = "Отображаемое имя")]
        [DataMember]
        [JsonProperty(PropertyName = "DisplayName")]
        public string DisplayName { get; set; } = string.Empty;
        /// <summary>
        /// Имя пользователя
        /// </summary>
        [Display(Name = "Имя пользователя")]
        [DataMember]
        [JsonProperty(PropertyName = "Name")]
        public string Name { get; set; } = string.Empty;
        /// <summary>
        /// Адрес электронной почты
        /// </summary>
        [Display(Name = "Адрес электронной почты")]
        [DataMember]
        [JsonProperty(PropertyName = "Email")]
        public string Email { get; set; } = string.Empty;
        /// <summary>
        /// Признак подтверждения электронной почты
        /// </summary>
        [Display(Name = "Признак подтверждения электронной почты")]
        [DataMember]
        [JsonProperty(PropertyName = "IsEmailVerified")]
        public bool IsEmailVerified { get; set; }
        /// <summary>
        /// Номер телефона
        /// </summary>
        [Display(Name = "Номер телефона")]
        [DataMember]
        [JsonProperty(PropertyName = "PhoneNumber")]
        public string PhoneNumber { get; set; } = string.Empty;
        /// <summary>
        /// Международный телефонный код (без плюса)
        /// </summary>
        [Display(Name = "Международный телефонный код")]
        [DataMember]
        [JsonProperty(PropertyName = "PhoneCountryCode")]
        public string PhoneCountryCode { get; set; } = string.Empty;
        /// <summary>
        /// Признак подтверждения номера телефона
        /// </summary>
        [Display(Name = "Признак подтверждения номера телефона")]
        [DataMember]
        [JsonProperty(PropertyName = "IsPhoneNumberVerified")]
        public bool IsPhoneNumberVerified { get; set; }
        /// <summary>
        /// Признак активности
        /// </summary>
        [Display(Name = "Признак активности")]
        [DataMember]
        [JsonProperty(PropertyName = "IsActive")]
        public bool IsActive { get; set; }
        /// <summary>
        /// Дата и время создания
        /// </summary>
        [Display(Name = "Дата и время создания")]
        [DataMember]
        [JsonProperty(PropertyName = "CreatedAt")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        /// <summary>
        /// Дата и время редактирования
        /// </summary>
        [Display(Name = "Дата и время редактирования")]
        [DataMember]
        [JsonProperty(PropertyName = "UpdatedAt")]
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}
