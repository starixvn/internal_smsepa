namespace SMS.EPA.ServicesAPI.Controllers
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Web.Http;
    using SMS.EPA.ServicesAPI.Models.Todolist;
    using SMS.EPA.ServicesAPI.Services;

    [Authorize]
    public class TodolistController : ApiController
    {
        private readonly ITodolistService todolistService;

        public TodolistController() : this(null)
        {
        }

        public TodolistController(ITodolistService todolistService)
        {
            this.todolistService = todolistService ?? new TodolistService();
        }

        public async Task<IHttpActionResult> GetByUserId(int userId)
        {
            IList<TodolistItem> items = await todolistService.GetEPAByUserIdAsync(userId);
            return Json(items);
        }
    }
}