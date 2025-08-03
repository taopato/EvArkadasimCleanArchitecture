// Yol: Domain/Entities/HouseMember.cs
namespace Domain.Entities
{
    public class HouseMember
    {
        public int Id { get; set; }
        public int HouseId { get; set; }
        public int UserId { get; set; }

        public House House { get; set; }
        public User User { get; set; }
    }
}
