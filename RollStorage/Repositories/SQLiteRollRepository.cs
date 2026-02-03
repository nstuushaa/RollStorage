using Microsoft.EntityFrameworkCore;
using RollStorage.Models;

namespace RollStorage.Repositories
{
    public class SQLiteRollRepository : IRollRepository
    {
        private readonly RollContext _context;

        public SQLiteRollRepository(RollContext context)
        {
            _context = context;
        }
        public async Task<List<Roll>> GetAllRollsAsync() 
        {
            return await _context.Rolls.ToListAsync();
        }

        public async Task<Roll?> GetRollByIdAsync(int id)
        {
            var roll = await _context.Rolls.FindAsync(id);

            if (roll == null)
                return null;

            return roll;
        }
        public async Task AddRollAsync(Roll roll) 
        {
            _context.Rolls.Add(roll);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateRollAsync(Roll roll)
        {
            _context.Rolls.Update(roll);
            await _context.SaveChangesAsync();
        }

    }
}
