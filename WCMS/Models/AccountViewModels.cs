using System.ComponentModel.DataAnnotations;

namespace WCMS.Web.Models
{
    public class ExternalLoginConfirmationViewModel
    {
        [Required]
        [Display(Name = "사용자 이름")]
        public string UserName { get; set; }
    }

    public class ManageUserViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "현재 암호")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "{0}은(는) {2}자 이상이어야 합니다.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "새 암호")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "새 암호 확인")]
        [Compare("NewPassword", ErrorMessage = "새 암호와 확인 암호가 일치하지 않습니다.")]
        public string ConfirmPassword { get; set; }
    }

    public class LoginViewModel
    {
        [Required]
        [Display(Name = "사용자 ID")]
        public string memberId { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "암호")]
        public string memberPw { get; set; }

    }

    public class RegisterViewModel
    {
        [Required]
        [Display(Name = "사용자 ID")]
        public string memberId { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "암호")]
        public string memberPw { get; set; }

        [Required]
        [Display(Name = "사용자 이름")]
        public string memberName { get; set; }

        [Required]
        [Display(Name = "사용자 연락처")]
        public string memberPhone { get; set; }
    }
}
