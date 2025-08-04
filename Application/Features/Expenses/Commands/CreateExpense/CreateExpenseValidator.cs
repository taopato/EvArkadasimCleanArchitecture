using FluentValidation;
using Application.Features.Expenses.Commands.CreateExpense;
using Application.Services.Repositories;
using System.Linq;

namespace Application.Features.Expenses.Commands.CreateExpense
{
    public class CreateExpenseValidator : AbstractValidator<CreateExpenseCommand>
    {
        public CreateExpenseValidator(IHouseRepository houseRepo)
        {
            RuleFor(x => x.Tur).NotEmpty();
            RuleFor(x => x.Tutar).GreaterThan(0);
            RuleFor(x => x.OrtakHarcamaTutari)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Ortak harcama tutarı negatif olamaz.");
            RuleForEach(x => x.SahsiHarcamalar).ChildRules(p =>
            {
                p.RuleFor(y => y.UserId).GreaterThan(0);
                p.RuleFor(y => y.Tutar)
                    .GreaterThanOrEqualTo(0)
                    .WithMessage("Şahsi harcama tutarı negatif olamaz.");
            });

            RuleFor(x => x).CustomAsync(async (cmd, ctx, ct) =>
            {
                // toplam kontrolü
                var sumPersonal = cmd.SahsiHarcamalar.Sum(pe => pe.Tutar);
                if (sumPersonal + cmd.OrtakHarcamaTutari != cmd.Tutar)
                    ctx.AddFailure("Tutar doğrulaması",
                      "Şahsi harcamalar + ortak harcama tutarı toplam Tutar ile eşit olmalı.");

                // üye kontrolü
                var house = await houseRepo.GetByIdAsync(cmd.HouseId);
                var memberIds = house.HouseMembers.Select(m => m.UserId).ToHashSet();
                foreach (var pe in cmd.SahsiHarcamalar)
                {
                    if (!memberIds.Contains(pe.UserId))
                        ctx.AddFailure($"User {pe.UserId}",
                          "Bu ev grubunun üyesi değil.");
                }
            });
        }
    }
}
