using Microsoft.AspNetCore.Mvc;

using System.ComponentModel.DataAnnotations;

namespace BlazorJWTAuth3.DTO
{
    public class UserRegister
    {
        [Required, EmailAddress(ErrorMessage ="E-mail 주소를 올바르게 입력해주세요.")]
        public string Email { get; set; } = string.Empty;

        [Required, StringLength(14, MinimumLength = 8, ErrorMessage = "비밀번호는 8~14자 이내로 입력해주세요")]
        public string Password { get; set; } = string.Empty;
        
        [Compare("Password", ErrorMessage = "비밀번호 불일치")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
