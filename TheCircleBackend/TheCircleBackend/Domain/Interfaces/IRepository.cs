namespace TheCircleBackend.Domain.Interfaces
{
    public interface IRepository<T>
    {
        public T GetById(int id);
        public List<T> GetAll(int? id);

        public bool Create(T entity);
        public T Update(T entity);
        public T Delete(int id);
    }
}
