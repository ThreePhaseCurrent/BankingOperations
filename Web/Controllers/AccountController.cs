using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Entity;
using ApplicationCore.Interfaces;
using BankingSystem.ApplicationCore.Interfaces;
using BankingSystem.Infrastructure.Data;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Web.ViewModels;

namespace Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly IBankAccountRepository _bankAccountRepository;
        private UserManager<IdentityUser> _userManager;
        private SignInManager<IdentityUser> _signInManager;
        private ILogger<AccountController> _logger;

        public AccountController(IBankAccountRepository bankAccountRepository, UserManager<IdentityUser> userMgr, 
                                 SignInManager<IdentityUser> signInMgr, ILogger<AccountController> logger)
        {
            _bankAccountRepository = bankAccountRepository;
            _signInManager = signInMgr;
            _userManager = userMgr;
            _logger = logger;
        }

        public IActionResult UserRegistered()
        {
            return View();
        }
    }
}