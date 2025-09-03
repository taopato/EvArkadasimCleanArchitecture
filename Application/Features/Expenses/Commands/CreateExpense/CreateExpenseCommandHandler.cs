// Application/Features/Expenses/Commands/CreateExpense/CreateExpenseCommandHandler.cs
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Features.Expenses.Dtos;
using Application.Services.Repositories;
using AutoMapper;
using Domain.Entities;
using MediatR;

namespace Application.Features.Expenses.Commands.CreateExpense
{
    public class CreateExpenseCommandHandler
        : IRequestHandler<CreateExpenseCommand, CreatedExpenseResponseDto>
    {
        private readonly IExpenseRepository _expenseRepository;
        private readonly IMapper _mapper;

        public CreateExpenseCommandHandler(IExpenseRepository expenseRepository, IMapper mapper)
        {
            _expenseRepository = expenseRepository;
            _mapper = mapper;
        }

        public async Task<CreatedExpenseResponseDto> Handle(CreateExpenseCommand request, CancellationToken cancellationToken)
        {
            // Ortak harcama tutarı gönderilmediyse, kişisel toplamı düşerek hesapla
            var sahsiToplam = request.SahsiHarcamalar?.Sum(x => x.Tutar) ?? 0m;
            var ortak = request.OrtakHarcamaTutari > 0 ? request.OrtakHarcamaTutari
                                                       : Math.Max(0m, request.Tutar - sahsiToplam);

            var entity = new Expense
            {
                // alias sayesinde Description'a yazıyoruz
                Tur = request.Tur,
                Tutar = request.Tutar,
                OrtakHarcamaTutari = ortak,
                HouseId = request.HouseId,
                OdeyenUserId = request.OdeyenUserId,
                KaydedenUserId = request.KaydedenUserId,
                KayitTarihi = request.Date == default ? DateTime.UtcNow : request.Date,
                IsActive = true
            };

            // Kişisel satırlar
            if (request.SahsiHarcamalar != null)
            {
                foreach (var p in request.SahsiHarcamalar.Where(x => x.Tutar > 0))
                {
                    entity.PersonalExpenses.Add(new PersonalExpense
                    {
                        UserId = p.UserId,
                        Tutar = p.Tutar
                    });
                }
            }

            // NOT: Mevcut projede Shares oluşturma mantığınız var ise bozmamak adına
            // burada otomatik dağıtım yapmıyorum. (HouseMembers, eşit bölme vb.)
            // Var olan kodunuz zaten Shares’i dolduruyorsa çalışmaya devam eder.

            var created = await _expenseRepository.AddAsync(entity);
            // bazı repo implementasyonlarınız SaveChanges içeriyor olabilir, yoksa:
            await _expenseRepository.SaveChangesAsync();

            // Response dto’yu doldur
            var response = new CreatedExpenseResponseDto
            {
                Id = created.Id,
                HouseId = created.HouseId,
                OdeyenUserId = created.OdeyenUserId,
                KaydedenUserId = created.KaydedenUserId,
                Tutar = created.Tutar,
                OrtakHarcamaTutari = created.OrtakHarcamaTutari,
                Tur = created.Tur,
                KayitTarihi = created.KayitTarihi
            };
            return response;
        }
    }
}
