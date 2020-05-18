using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Web.ViewModels
{
    public class AccountOperationViewModel
    {
        /// <summary>
        ///   Номер аккаунта
        /// </summary>
        public int IdAccount { get; set; }

        /// <summary>
        ///   Все операции которые производились на аккаунте
        /// </summary>
        public IList<ApplicationCore.Entity.Operation> Operations { get; set; }

        /// <summary>
        ///   Фильтр начала операций
        /// </summary>
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name                   = "Начало периода")]
        public DateTime? StartPeriod { get; set; }

        /// <summary>
        ///   Фильтр конца операций
        /// </summary>
        [Display(Name = "Конец периода")]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime? EndPeriod { get; set; }

        /// <summary>
        ///   Текущий баланс
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        ///   Можно ли менять валюту аккаунта
        /// </summary>
        public bool CanChangeCurrency { get; set; }

        /// <summary>
        ///   Закрытый ли в текущий момент аккаунт
        /// </summary>
        public bool IsClosed { get; set; }

        public override string ToString() =>
            $"{nameof(IdAccount)}: {IdAccount}, {nameof(StartPeriod)}: {StartPeriod}, {nameof(EndPeriod)}: {EndPeriod}, {nameof(Amount)}: {Amount}, {nameof(CanChangeCurrency)}: {CanChangeCurrency}, {nameof(IsClosed)}: {IsClosed}";
    }
}