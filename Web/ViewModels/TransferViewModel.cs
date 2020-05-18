using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace Web.ViewModels
{
    /// <summary>
    ///   Модель данных необходимых для перевода денег
    /// </summary>
    public class TransferViewModel
    {
        public int IdFrom { get; set; }

        [Required]
        [Remote("AccountExist", "Operation", ErrorMessage = "Такой карточки не существует!")]
        public int IdTo { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        public decimal Amount { get; set; }

        public override string ToString() =>
            $"{nameof(IdFrom)}: {IdFrom}, {nameof(IdTo)}: {IdTo}, {nameof(Amount)}: {Amount}";
    }

    // public class TransferViewModelValidator : AbstractValidator<TransferViewModel>
    // {
    //     public TransferViewModelValidator()
    //     {
    //         RuleFor(it => it.IdFrom).NotNull();
    //         RuleFor(it => it.Amount).NotNull().GreaterThan(valueToCompare: 0)
    //             .WithMessage("Не корректная сумма!");
    //     }
    // }
}