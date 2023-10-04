using System.ComponentModel.DataAnnotations;

namespace BlazorJWTAuth3.DTOs
{
    public class UserRegister
    {
        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required, StringLength(14, MinimumLength = 8, ErrorMessage = "비밀번호는 8~14자 이내로 입력해주세요")]
        public string Password { get; set; } = string.Empty;
        
        [Compare("Password", ErrorMessage = "비밀번호 불일치")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
