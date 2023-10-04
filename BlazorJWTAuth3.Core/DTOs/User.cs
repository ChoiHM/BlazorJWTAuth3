
using System.ComponentModel.DataAnnotations.Schema;

namespace BlazorJWTAuth3.DTOs
{
    [Dapper.Contrib.Extensions.Table("users")]
    public class UserDto
    {
        [Dapper.Contrib.Extensions.Key]
        public int Idx { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime DtInserted { get; set; }
    }
}
