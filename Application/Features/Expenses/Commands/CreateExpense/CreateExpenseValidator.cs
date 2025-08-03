using FluentValidation;

namespace Application.Features.Expenses.Commands.CreateExpense
{
    public class CreateExpenseValidator : AbstractValidator<CreateExpenseCommand>
    {
        public CreateExpenseValidator()
        {
            RuleFor(x => x.Description).NotEmpty().WithMessage("Açıklama boş olamaz.");
            RuleFor(x => x.Tutar).GreaterThan(0).WithMessage("Tutar sıfırdan büyük olmalı.");
            RuleFor(x => x.KaydedenUserId).GreaterThan(0);
            RuleFor(x => x.OdeyenUserId).GreaterThan(0);
        }
    }
}
