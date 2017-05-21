namespace SurveySystem.Services.Data.Common.Contracts
{
    using System.Linq;
    using SurveySystem.Data.Common.Models;

    public interface IBaseDataWithCreatorService<T> : IBaseDataService<T>
        where T : class, IDeletableEntity, IAuditInfo, IEntityWithCreator
    {
        void Delete(object id, string userId);

        IQueryable<T> GetAllByUser(string userId);
    }
}
