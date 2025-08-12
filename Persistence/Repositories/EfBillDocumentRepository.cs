using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Services.Repositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Persistence.Contexts;

namespace Persistence.Repositories
{
    public class EfBillDocumentRepository : IBillDocumentRepository
    {
        private readonly AppDbContext _ctx;
        public EfBillDocumentRepository(AppDbContext ctx) { _ctx = ctx; }

        public async Task<BillDocument> AddAsync(BillDocument doc)
        {
            var e = await _ctx.BillDocuments.AddAsync(doc);
            await _ctx.SaveChangesAsync();
            return e.Entity;
        }

        public Task<List<BillDocument>> GetByBillAsync(int billId)
            => _ctx.BillDocuments.Where(x => x.BillId == billId)
                                 .OrderByDescending(x => x.UploadedAt)
                                 .AsNoTracking().ToListAsync();

        public async Task DeleteAsync(int id)
        {
            var e = await _ctx.BillDocuments.FirstOrDefaultAsync(x => x.Id == id);
            if (e != null)
            {
                _ctx.BillDocuments.Remove(e);
                await _ctx.SaveChangesAsync();
            }
        }
    }
}
