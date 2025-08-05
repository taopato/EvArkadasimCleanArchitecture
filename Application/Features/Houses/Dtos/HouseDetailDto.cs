using System;
using System.Collections.Generic;

namespace Application.Features.Houses.Dtos
{
    public class HouseDetailDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public List<MemberDto> Members { get; set; } = new();
    }
}
