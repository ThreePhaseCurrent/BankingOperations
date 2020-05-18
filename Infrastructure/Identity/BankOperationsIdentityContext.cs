using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Infrastructure.Identity
{
    public partial class BankOperationsIdentityContext : IdentityDbContext<IdentityUser>
    {
        public BankOperationsIdentityContext(DbContextOptions<BankOperationsIdentityContext> options)
                        : base(options) { }
    }
}
