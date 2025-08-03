// Yol: Domain/Entities/House.cs
using System.Collections.Generic;

namespace Domain.Entities
{
    public class House
    {
        public int HouseId { get; set; }
        public string HouseName { get; set; }
        public int CreatedBy { get; set; }

        public ICollection<HouseMember> HouseMembers { get; set; }
        public ICollection<Expense> Expenses { get; set; }
        public ICollection<Invitation> Invitations { get; set; }
    }
}
