using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Application.Services.Repositories;

namespace Application.Features.Bills.Commands.UploadBillDocument
{
    public class UploadBillDocumentCommandHandler : IRequestHandler<UploadBillDocumentCommand, int>
    {
        private readonly IUtilityBillRepository _billRepo;
        private readonly IBillDocumentRepository _docRepo;

        public UploadBillDocumentCommandHandler(IUtilityBillRepository billRepo, IBillDocumentRepository docRepo)
        {
            _billRepo = billRepo;
            _docRepo = docRepo;
        }

        public async Task<int> Handle(UploadBillDocumentCommand request, CancellationToken ct)
        {
            var bill = await _billRepo.GetAsync(request.BillId)
                        ?? throw new System.Exception("Bill not found.");

            if (bill.ResponsibleUserId != request.RequestUserId)
                throw new System.Exception("Sadece sorumlu kişi doküman yükleyebilir.");

            var doc = await _docRepo.AddAsync(new Domain.Entities.BillDocument
            {
                BillId = request.BillId,
                FileName = request.FileName,
                FilePathOrUrl = request.FilePathOrUrl,
                UploadedByUserId = request.RequestUserId
            });

            return doc.Id;
        }
    }
}
