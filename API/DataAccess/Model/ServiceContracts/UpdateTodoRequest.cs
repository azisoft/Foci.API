namespace API.DataAccess.Model.ServiceContracts
{
    public class UpdateTodoRequest: CreateTodoRequest
    {
        public int Id { get; set; }
    }
}
