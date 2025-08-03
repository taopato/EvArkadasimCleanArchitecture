using AutoMapper;
using Domain.Entities;
using MediatR;
using Application.Features.Users.Dtos;
using Application.Services.Repositories;

namespace Application.Features.Users.Commands.Create
{
    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, CreatedUserDto>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public CreateUserCommandHandler(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<CreatedUserDto> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            var user = _mapper.Map<User>(request);
            var addedUser = await _userRepository.AddAsync(user);
            var dto = _mapper.Map<CreatedUserDto>(addedUser);
            return dto;
        }
    }
}
