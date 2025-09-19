using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Application.Features.Expenses.Dtos;
using Application.Services.Repositories;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using MediatR;

namespace Application.Features.Expenses.Commands.CreateExpense
{
    public class CreateExpenseCommandHandler
        : IRequestHandler<CreateExpenseCommand, CreatedExpenseResponseDto>
    {
        private const int RECURRING_HORIZON_MONTHS = 12;

        private readonly IExpenseRepository _expenseRepository;
        private readonly IMapper _mapper;
        private readonly IHouseMemberRepository _houseMemberRepo;
        private readonly ILedgerLineRepository _ledgerRepo;

        public CreateExpenseCommandHandler(
            IExpenseRepository expenseRepository,
            IMapper mapper,
            IHouseMemberRepository houseMemberRepo,
            ILedgerLineRepository ledgerRepo)
        {
            _expenseRepository = expenseRepository;
            _mapper = mapper;
            _houseMemberRepo = houseMemberRepo;
            _ledgerRepo = ledgerRepo;
        }

        public async Task<CreatedExpenseResponseDto> Handle(CreateExpenseCommand request, CancellationToken cancellationToken)
        {
            var mode = (request.Mode ?? "").Trim().ToLowerInvariant();

            if (mode == "installment" || (request.InstallmentCount ?? 0) > 1)
                return await HandleInstallmentAsync(request, cancellationToken);

            if (mode == "recurring")
                return await HandleRecurringAsync(request, cancellationToken);

            // --- SINGLE ---
            var sahsiToplam = request.SahsiHarcamalar?.Sum(x => x.Tutar) ?? 0m;
            var ortak = request.OrtakHarcamaTutari > 0 ? request.OrtakHarcamaTutari
                                                       : Math.Max(0m, request.Tutar - sahsiToplam);

            var description = request.Aciklama ?? request.Note ?? request.Description;
            var whenUtc = request.Date == default ? DateTime.UtcNow : request.Date.ToUniversalTime();

            var cat = ResolveCategoryNonNull(request);       // enum garanti
            var title = ResolveTitle(request);               // TUR asla boş kalmasın

            var entity = new Expense
            {
                Tur = title,
                Tutar = request.Tutar,
                OrtakHarcamaTutari = ortak,
                HouseId = request.HouseId,
                OdeyenUserId = request.OdeyenUserId,
                KaydedenUserId = request.KaydedenUserId,
                CreatedDate = whenUtc,
                IsActive = true,
                Description = description,

                // listeleme alanları
                PostDate = whenUtc,
                DueDate = whenUtc,
                PeriodMonth = $"{whenUtc:yyyy-MM}",
                VisibilityMode = VisibilityMode.OnBillDate,

                // kategori (non-null)
                Category = cat
            };

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

            var created = await _expenseRepository.AddAsync(entity);
            await _expenseRepository.SaveChangesAsync();

            return new CreatedExpenseResponseDto
            {
                Id = created.Id,
                HouseId = created.HouseId,
                OdeyenUserId = created.OdeyenUserId,
                KaydedenUserId = created.KaydedenUserId,
                Tutar = created.Tutar,
                OrtakHarcamaTutari = created.OrtakHarcamaTutari,
                Tur = created.Tur,
                KayitTarihi = created.CreatedDate
            };
        }

        // --- Helpers ---
        private static DateTime FirstDayUtc(DateTime dtUtc)
            => new DateTime(dtUtc.Year, dtUtc.Month, 1, 0, 0, 0, DateTimeKind.Utc);

        private static DateTime MonthWithDueDayUtc(DateTime monthFirstUtc, byte dueDay)
        {
            var day = Math.Clamp((int)dueDay, 1, 28);
            return new DateTime(monthFirstUtc.Year, monthFirstUtc.Month, day, 0, 0, 0, DateTimeKind.Utc);
        }

        // Category/CategoryId -> ExpenseCategory (fallback'lı)
        private static ExpenseCategory ResolveCategoryNonNull(CreateExpenseCommand req)
        {
            if (req.Category.HasValue) return req.Category.Value;

            if (req.CategoryId.HasValue)
            {
                try { return (ExpenseCategory)req.CategoryId.Value; }
                catch { /* geçersiz ise alttan devam */ }
            }

            // Tur metnine göre kaba tahmin (son çare)
            var text = (req.Tur ?? string.Empty).ToLowerInvariant();

            if (text.Contains("kira") || text.Contains("rent"))
                if (TryParseAny(out var rent, "Rent", "Kira")) return rent;

            if (text.Contains("elektrik") || text.Contains("electric"))
                if (TryParseAny(out var elec, "Electricity", "Elektrik", "Electric")) return elec;

            if (text.Contains(" water") || text.Contains(" su") || text.StartsWith("su") || text == "su" || text.Contains("water"))
                if (TryParseAny(out var water, "Water", "Su")) return water;

            if (text.Contains("internet"))
                if (TryParseAny(out var net, "Internet", "İnternet")) return net;

            if (text.Contains("market"))
                if (TryParseAny(out var market, "Market")) return market;

            if (text.Contains("yemek") || text.Contains("food"))
                if (TryParseAny(out var food, "Food", "Yemek")) return food;

            // fallback
            if (TryParseAny(out var other, "Other", "Diger", "Diğer")) return other;

            return default; // enum 0 değeri (Rent) - pratikte yukarıdakiler dönecek
        }

        private static bool TryParseAny(out ExpenseCategory value, params string[] names)
        {
            foreach (var n in names)
            {
                if (Enum.TryParse<ExpenseCategory>(n, true, out value))
                    return true;
            }
            value = default;
            return false;
        }

        // Category/CategoryId -> Türkçe başlık (tur)
        private static string ResolveTitle(CreateExpenseCommand req)
        {
            if (!string.IsNullOrWhiteSpace(req.Tur))
                return req.Tur.Trim();

            byte? catId = null;
            if (req.Category.HasValue) catId = (byte)req.Category.Value;
            else if (req.CategoryId.HasValue) catId = (byte)req.CategoryId.Value;

            return catId switch
            {
                0 => "Kira",
                1 => "İnternet",
                2 => "Elektrik",
                3 => "Su",
                4 => "Market",
                5 => "Yemek",
                99 => "Diğer",
                _ => "Diğer"
            };
        }

        // --- INSTALLMENT ---
        private async Task<CreatedExpenseResponseDto> HandleInstallmentAsync(CreateExpenseCommand request, CancellationToken ct)
        {
            var total = request.Tutar;
            var count = Math.Max(1, request.InstallmentCount ?? 1);
            var dueDay = (byte)Math.Clamp((int)(request.DueDay ?? 1), 1, 28);

            var startBase = request.StartMonth?.ToUniversalTime()
                         ?? (request.Date == default ? DateTime.UtcNow : request.Date.ToUniversalTime());
            var startMonthUtc = FirstDayUtc(startBase);
            var cardholderId = request.CardholderUserId ?? request.OdeyenUserId;

            var cat = ResolveCategoryNonNull(request);
            var title = ResolveTitle(request);

            // Katılımcılar
            List<int> participants;
            if (request.Participants != null && request.Participants.Count > 0)
                participants = request.Participants.Distinct().ToList();
            else
                participants = (await _houseMemberRepo.GetActiveUserIdsAsync(request.HouseId, ct))?.Distinct().ToList() ?? new();

            if (participants.Count == 0)
                throw new InvalidOperationException("Aktif ev üyesi bulunamadı.");

            var monthly = Math.Round(total / count, 2, MidpointRounding.AwayFromZero);

            // 1) Parent
            var parentPostDate = MonthWithDueDayUtc(startMonthUtc, dueDay);
            var parent = new Expense
            {
                Tur = string.IsNullOrWhiteSpace(title) ? "Taksit Planı" : $"{title} (Taksit Planı)",
                Tutar = total,
                OrtakHarcamaTutari = 0m,
                HouseId = request.HouseId,
                OdeyenUserId = cardholderId,
                KaydedenUserId = request.KaydedenUserId,
                CreatedDate = parentPostDate,
                IsActive = true,
                DueDay = dueDay,
                PlanStartMonth = startMonthUtc,
                Description = request.Aciklama ?? request.Note ?? request.Description,

                PostDate = parentPostDate,
                DueDate = parentPostDate,
                PeriodMonth = $"{parentPostDate:yyyy-MM}",
                VisibilityMode = VisibilityMode.OnBillDate,

                Category = cat
            };
            parent = await _expenseRepository.AddAsync(parent);
            await _expenseRepository.SaveChangesAsync();

            // 2) Child taksitler
            var childExpenses = new List<Expense>(count);
            for (int i = 0; i < count; i++)
            {
                var monthFirst = FirstDayUtc(startMonthUtc.AddMonths(i));
                var postDate = MonthWithDueDayUtc(monthFirst, dueDay);

                var child = new Expense
                {
                    ParentExpenseId = parent.Id,
                    Tur = $"{title} taksit {i + 1}/{count}",
                    Tutar = monthly,
                    OrtakHarcamaTutari = monthly,
                    HouseId = request.HouseId,
                    OdeyenUserId = cardholderId,
                    KaydedenUserId = request.KaydedenUserId,
                    CreatedDate = postDate,
                    IsActive = true,
                    DueDay = dueDay,
                    PlanStartMonth = startMonthUtc,
                    InstallmentIndex = i + 1,
                    InstallmentCount = count,
                    Description = request.Aciklama ?? request.Note ?? request.Description,

                    PostDate = postDate,
                    DueDate = postDate,
                    PeriodMonth = $"{postDate:yyyy-MM}",
                    VisibilityMode = VisibilityMode.OnBillDate,

                    Category = cat
                };
                child = await _expenseRepository.AddAsync(child);
                childExpenses.Add(child);
            }
            await _expenseRepository.SaveChangesAsync();

            // 3) Ledger
            await CreateEqualLedgersForMaturedAsync(request.HouseId, childExpenses, participants, cardholderId, monthly, ct);

            return new CreatedExpenseResponseDto
            {
                Id = parent.Id,
                HouseId = parent.HouseId,
                OdeyenUserId = parent.OdeyenUserId,
                KaydedenUserId = parent.KaydedenUserId,
                Tutar = parent.Tutar,
                OrtakHarcamaTutari = parent.OrtakHarcamaTutari,
                Tur = parent.Tur,
                KayitTarihi = parent.CreatedDate,
                ParentExpenseId = null,
                InstallmentIndex = null,
                InstallmentCount = parent.InstallmentCount,
                PlanStartMonth = parent.PlanStartMonth,
                DueDay = parent.DueDay
            };
        }

        // --- RECURRING ---
        private async Task<CreatedExpenseResponseDto> HandleRecurringAsync(CreateExpenseCommand request, CancellationToken ct)
        {
            var monthlyAmount = request.Tutar;
            var dueDay = (byte)Math.Clamp((int)(request.DueDay ?? 1), 1, 28);

            var startBase = request.StartMonth?.ToUniversalTime()
                         ?? (request.Date == default ? DateTime.UtcNow : request.Date.ToUniversalTime());
            var startMonthUtc = FirstDayUtc(startBase);

            var cat = ResolveCategoryNonNull(request);
            var title = ResolveTitle(request);

            var participants = (await _houseMemberRepo.GetActiveUserIdsAsync(request.HouseId, ct))?.Distinct().ToList() ?? new();
            if (participants.Count == 0)
                throw new InvalidOperationException("Aktif ev üyesi bulunamadı.");

            // 1) Parent
            var parentPostDate = MonthWithDueDayUtc(startMonthUtc, dueDay);
            var parent = new Expense
            {
                Tur = string.IsNullOrWhiteSpace(title) ? "Düzenli Gider Planı" : $"{title} (Plan)",
                Tutar = monthlyAmount * RECURRING_HORIZON_MONTHS,
                OrtakHarcamaTutari = 0m,
                HouseId = request.HouseId,
                OdeyenUserId = request.OdeyenUserId,
                KaydedenUserId = request.KaydedenUserId,
                CreatedDate = parentPostDate,
                IsActive = true,
                DueDay = dueDay,
                PlanStartMonth = startMonthUtc,
                Description = request.Aciklama ?? request.Note ?? request.Description,

                PostDate = parentPostDate,
                DueDate = parentPostDate,
                PeriodMonth = $"{parentPostDate:yyyy-MM}",
                VisibilityMode = VisibilityMode.OnBillDate,

                Category = cat
            };
            parent = await _expenseRepository.AddAsync(parent);
            await _expenseRepository.SaveChangesAsync();

            // 2) 12 çocuk
            var childExpenses = new List<Expense>(RECURRING_HORIZON_MONTHS);
            for (int i = 0; i < RECURRING_HORIZON_MONTHS; i++)
            {
                var monthFirst = FirstDayUtc(startMonthUtc.AddMonths(i));
                var postDate = MonthWithDueDayUtc(monthFirst, dueDay);

                var child = new Expense
                {
                    ParentExpenseId = parent.Id,
                    Tur = title,
                    Tutar = monthlyAmount,
                    OrtakHarcamaTutari = monthlyAmount,
                    HouseId = request.HouseId,
                    OdeyenUserId = request.OdeyenUserId,
                    KaydedenUserId = request.KaydedenUserId,
                    CreatedDate = postDate,
                    IsActive = true,
                    DueDay = dueDay,
                    PlanStartMonth = startMonthUtc,
                    Description = request.Aciklama ?? request.Note ?? request.Description,

                    PostDate = postDate,
                    DueDate = postDate,
                    PeriodMonth = $"{postDate:yyyy-MM}",
                    VisibilityMode = VisibilityMode.OnBillDate,

                    Category = cat
                };
                child = await _expenseRepository.AddAsync(child);
                childExpenses.Add(child);
            }
            await _expenseRepository.SaveChangesAsync();

            // 3) Ledger
            await CreateEqualLedgersForMaturedAsync(request.HouseId, childExpenses, participants, request.OdeyenUserId, monthlyAmount, ct);

            return new CreatedExpenseResponseDto
            {
                Id = parent.Id,
                HouseId = parent.HouseId,
                OdeyenUserId = parent.OdeyenUserId,
                KaydedenUserId = parent.KaydedenUserId,
                Tutar = parent.Tutar,
                OrtakHarcamaTutari = parent.OrtakHarcamaTutari,
                Tur = parent.Tur,
                KayitTarihi = parent.CreatedDate,
                ParentExpenseId = null,
                InstallmentIndex = null,
                InstallmentCount = null,
                PlanStartMonth = parent.PlanStartMonth,
                DueDay = parent.DueDay
            };
        }

        private async Task CreateEqualLedgersForMaturedAsync(
            int houseId,
            List<Expense> children,
            List<int> participants,
            int collectorUserId,
            decimal monthlyAmount,
            CancellationToken ct)
        {
            var nowUtc = DateTime.UtcNow;
            var matured = children.Where(c => c.CreatedDate <= nowUtc).ToList();
            if (matured.Count == 0) return;

            var n = Math.Max(1, participants.Count);
            var baseShare = Math.Round(monthlyAmount / n, 2, MidpointRounding.AwayFromZero);
            var totalBase = baseShare * n;
            var diff = Math.Round(monthlyAmount - totalBase, 2, MidpointRounding.AwayFromZero);

            var adjustUserId = participants.FirstOrDefault(uid => uid != collectorUserId, participants.First());

            foreach (var exp in matured)
            {
                foreach (var uid in participants)
                {
                    if (uid == collectorUserId) continue;

                    var share = uid == adjustUserId
                        ? Math.Round(baseShare + diff, 2, MidpointRounding.AwayFromZero)
                        : baseShare;

                    if (share <= 0) continue;

                    await _ledgerRepo.AddAsync(new LedgerLine
                    {
                        HouseId = houseId,
                        ExpenseId = exp.Id,
                        FromUserId = uid,
                        ToUserId = collectorUserId,
                        Amount = share,
                        PostDate = exp.CreatedDate,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    }, ct);
                }
            }
            await _ledgerRepo.SaveChangesAsync(ct);
        }
    }
}
