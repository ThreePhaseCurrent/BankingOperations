using System.ComponentModel.DataAnnotations;

namespace Web.ViewModels
{
    public class ChangePasswordViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Старый пароль")]
        public string OldPassword { get; set; }

        [Required]
        [Display(Name = "Новый пароль")]
        public string NewPassword { get; set; }

        [Required]
        [Display(Name = "Подтверждение нового пароля")]
        [Compare("NewPassword", ErrorMessage = "Пароли не совпадают!")]
        public string ConfirmNewPassword { get; set; }
    }
}