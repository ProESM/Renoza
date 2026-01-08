namespace Renoza.Backend.Models.ProductCategories
{
    /// <summary>
    /// Запрос на перемещение категории в другую родительскую категорию
    /// </summary>
    public class MoveProductCategoryRequest
    {
        /// <summary>
        /// ID новой родительской категории (null для перемещения в корень)
        /// </summary>
        public Guid? NewParentId { get; set; }
    }
}
