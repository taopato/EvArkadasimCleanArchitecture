using MediatR;

namespace Application.Features.Houses.Commands.AcceptInvitation;

public class AcceptInvitationCommand : IRequest<bool>
{
    public int UserId { get; set; }
    public string InvitationCode { get; set; } = string.Empty;
}
