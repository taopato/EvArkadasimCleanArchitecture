// Yol: Domain/Entities/PersonalExpense.cs
using System;

namespace Domain.Entities
{
    public class PersonalExpense
    {
        public int Id { get; set; }
        public int ExpenseId { get; set; }
        public Expense Expense { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public decimal Tutar { get; set; }
        public DateTime Tarih { get; set; }
    }
}
