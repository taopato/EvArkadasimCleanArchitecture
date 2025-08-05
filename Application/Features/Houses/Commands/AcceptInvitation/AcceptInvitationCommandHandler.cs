using Application.Services.Repositories;
using MediatR;

namespace Application.Features.Houses.Commands.AcceptInvitation;

public class AcceptInvitationCommandHandler : IRequestHandler<AcceptInvitationCommand, bool>
{
    private readonly IInvitationRepository _invitationRepo;

    public AcceptInvitationCommandHandler(IInvitationRepository invitationRepo)
    {
        _invitationRepo = invitationRepo;
    }

    public async Task<bool> Handle(AcceptInvitationCommand request, CancellationToken cancellationToken)
    {
        var invitation = await _invitationRepo.GetByCodeAsync(request.InvitationCode);

        if (invitation == null || invitation.ExpiresAt < DateTime.UtcNow || invitation.Status == "Accepted")
            return false;

        invitation.Status = "Accepted";
        invitation.AcceptedAt = DateTime.UtcNow;
        invitation.AcceptedByUserId = request.UserId;

        await _invitationRepo.UpdateAsync(invitation);
        return true;
    }
}
