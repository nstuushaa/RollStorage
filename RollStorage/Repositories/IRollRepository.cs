using RollStorage.Models;

namespace RollStorage.Repositories
{
    public interface IRollRepository
    {
        Task<List<Roll>> GetAllRollsAsync();
        Task<Roll?> GetRollByIdAsync(int id);
        Task AddRollAsync(Roll roll);
        Task UpdateRollAsync(Roll roll);
    }
}
