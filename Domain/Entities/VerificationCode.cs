// Domain/Entities/VerificationCode.cs
using System;

namespace Domain.Entities
{
    public class VerificationCode
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Code { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
