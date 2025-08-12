using MediatR;

namespace Application.Features.Bills.Commands.UploadBillDocument
{
    public class UploadBillDocumentCommand : IRequest<int> // dönen DocumentId
    {
        public int BillId { get; set; }
        public int RequestUserId { get; set; } // sadece ResponsibleUser
        public string FileName { get; set; } = null!;
        public string FilePathOrUrl { get; set; } = null!; // API katmanında dosyayı kaydedip yolunu gönder
    }
}
