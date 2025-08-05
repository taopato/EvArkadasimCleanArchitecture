using AutoMapper;
using MediatR;
using Application.Features.Houses.Dtos;
using Application.Services.Repositories;

namespace Application.Features.Houses.Queries.GetHouseDetail
{
    public class GetHouseDetailQueryHandler
        : IRequestHandler<GetHouseDetailQuery, HouseDetailDto>
    {
        private readonly IHouseRepository _houseRepository;
        private readonly IMapper _mapper;

        public GetHouseDetailQueryHandler(IHouseRepository houseRepository, IMapper mapper)
        {
            _houseRepository = houseRepository;
            _mapper = mapper;
        }

        public async Task<HouseDetailDto> Handle(GetHouseDetailQuery request, CancellationToken cancellationToken)
        {
            var entity = await _houseRepository.GetByIdAsync(request.Id);
            return _mapper.Map<HouseDetailDto>(entity);
        }
    }
}
