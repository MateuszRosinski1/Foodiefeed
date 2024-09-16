namespace Foodiefeed_api
{
    public interface IEntityRepository<T> where T : class
    {
        T? FindById(int id);
    }
}
