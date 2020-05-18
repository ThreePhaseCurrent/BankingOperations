using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ApplicationCore.Entity;
using ApplicationCore.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data
{
    public class CreditRepository : ICreditRepository
    {
        private readonly BankOperationsContext _context;

        public CreditRepository(BankOperationsContext context)
        {
            _context = context;
        }

        public IQueryable<Credit> Credits => _context.Credit;

        /// <summary>
        ///   Добавление кредита
        /// </summary>
        /// <param name="credit"></param>
        /// <returns></returns>
        public async Task AddCredit(Credit credit)
        {
            await _context.Credit.AddAsync(credit);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        ///   Закрытие кредита
        /// </summary>
        /// <param name="credit"></param>
        /// <returns></returns>
        public async Task CloseCredit(Credit credit)
        {
            var userCredit = await _context.Credit.FindAsync(credit);

            userCredit.Status = false;
            await _context.SaveChangesAsync();
        }
    }
}
