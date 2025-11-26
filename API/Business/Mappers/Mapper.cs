using API.DataAccess.Model.ServiceContracts;
using API.DataAccess.Model.Domain;

namespace API.Business.Mappers
{
    public static class Mappers
    {
        public static TodoResponse ToResponse(this Todo todo)
        {
            var ret = new TodoResponse();
            ret.Id = todo.Id;
            ret.Title = todo.Title;
            ret.Description = todo.Description;
            ret.DueDate = todo.DueDate;
            ret.IsCompleted = todo.IsCompleted;
            ret.CreatedAt = todo.CreatedAt;
            return ret;
        }

        public static Todo ToDomain(this CreateTodoRequest request)
        {
            var ret = new Todo();
            ret.ApplyRequest(request);
            return ret;
        }

        public static Todo ToDomain(this UpdateTodoRequest request)
        {
            var ret = ((CreateTodoRequest)request).ToDomain();
            ret.Id = request.Id;
            return ret;
        }

        public static void ApplyRequest(this Todo todo, CreateTodoRequest request)
        {
            todo.Title = request.Title;
            todo.Description = request.Description;
            todo.DueDate = request.DueDate;
            todo.IsCompleted = request.IsCompleted;
        }


        public static void ApplyPatch(this Todo todo, PatchTodoRequest patch)
        {
            if (todo == null) throw new ArgumentNullException(nameof(todo));
            if (patch == null) return;

            if (patch.Title != null)
                todo.Title = patch.Title;

            if (patch.Description != null)
                todo.Description = patch.Description;

            if (patch.DueDate.HasValue)
                todo.DueDate = patch.DueDate;

            if (patch.IsCompleted.HasValue)
                todo.IsCompleted = patch.IsCompleted;
        }

    }
}
