using MakeItSimple.WebApi.Models;

namespace MakeItSimple.WebApi.DataAccessLayer.Repository_Modules.Repository_Interface.IGeneric
{
    public interface IGenericRepository<T> where T : BaseIdEntity
    {
        Task<T?> GetByIdAsync(int id);
        Task<IReadOnlyList<T>> ListAllAsync();
        void Add(T entity);
        void Update(T entity);
        void Remove(T entity);
        Task<bool> SaveAllAsync();
        bool Exists(int id); 

    }
}
