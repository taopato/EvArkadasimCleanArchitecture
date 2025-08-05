using AutoMapper;
using MediatR;
using Application.Features.Houses.Dtos;
using Application.Services.Repositories;

namespace Application.Features.Houses.Queries.GetHouseList
{
    public class GetHouseListQueryHandler
        : IRequestHandler<GetHouseListQuery, List<HouseDto>>
    {
        private readonly IHouseRepository _houseRepository;
        private readonly IMapper _mapper;

        public GetHouseListQueryHandler(IHouseRepository houseRepository, IMapper mapper)
        {
            _houseRepository = houseRepository;
            _mapper = mapper;
        }

        public async Task<List<HouseDto>> Handle(GetHouseListQuery request, CancellationToken cancellationToken)
        {
            var list = await _houseRepository.GetAllAsync();
            return _mapper.Map<List<HouseDto>>(list);
        }
    }
}
