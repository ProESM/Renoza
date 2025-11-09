using Microsoft.AspNetCore.Mvc;
using Renoza.Backend.Attributes;
using Renoza.Domain.Services.Interfaces.RenozaInterfaces;

namespace Renoza.Backend.Controllers
{
    /// <summary>
    /// OData Web Api контроллер работы с пользователями
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        #region Логгер

        /// <summary>
        /// Логгер
        /// </summary>
        private readonly ILogger<UsersController> _logger;

        #endregion

        #region Сервисы

        /// <summary>
        /// Сервис работы с пользователями
        /// </summary>
        private readonly IUserService _userService;

        #endregion

        /// <summary>
        /// OData Web Api контроллер работы с пользователями
        /// </summary>
        /// <param name="logger">Логгер</param>
        /// <param name="userService">Сервис работы с пользователями</param>
        public UsersController(
            ILogger<UsersController> logger,
            IUserService userService)
        {
            _logger = logger;
            _userService = userService;
        }

        [HttpGet, UseQuery]
        public ActionResult Get()
        {
            try
            {
                var queryable = _userService.GetQueryable();
                return Ok(queryable);
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e);
            }
        }
    }
}
