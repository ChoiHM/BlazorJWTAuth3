using BlazorJWTAuth3.Core;
using BlazorJWTAuth3.DTOs;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorJWTAuth3.DataAccess.Interface
{
    public interface IAuthService
    {
        Task<ServiceResponse<int>> Register(UserDto user, string password);
        Task<bool> UserExists(string userId);
        Task<ServiceResponse<string>> Login(string userId, string password);
        Task<ServiceResponse<bool>> ChangePassword(int userIdx, string newPassword);


        Task<IEnumerable<UserDto>?> GetAll();
        Task<bool> AddAsync(UserDto user);
        Task<bool> DeleteAsync(UserDto user);
        Task<bool> UpdateAsync(UserDto user);
    }
}
