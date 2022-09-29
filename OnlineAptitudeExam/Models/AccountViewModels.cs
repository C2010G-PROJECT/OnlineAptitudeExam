using System.ComponentModel.DataAnnotations;

namespace OnlineAptitudeExam.Models
{

    public class LoginViewModel
    {
        [Required(ErrorMessage = "Enter your username")]
        [Display(Name = "Username")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Enter your password")]
        [Display(Name = "Password")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,32}$", ErrorMessage = "Password must contain at least a number, a lowercase character, a uppercase character and between [8, 32] characters")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }

}


