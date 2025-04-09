namespace AIApp.Lib.Interfaces
{
    public interface IApiService
    {
        Task<string> Get(string endpoint);
        Task<string> Post(string endpoint, string json);
        Task<string> Put(string endpoint, string json);
        Task<string> Delete(string endpoint);
    }
}