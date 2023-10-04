using BlazorJWTAuth3.Core;
using BlazorJWTAuth3.DataAccess;
using BlazorJWTAuth3.DataAccess.Interface;
using BlazorJWTAuth3.DTOs;

using Dapper;
using Dapper.Contrib.Extensions;

using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace BlazorDapper.DataAccess.Repositories
{
    public class AuthService : IAuthService
    {
        private readonly DapperContext _context;
        private readonly IConfiguration _configuration;

        public AuthService(DapperContext context, IConfiguration configuration)
        {
            _context = context;
            this._configuration = configuration;
        }

        public async Task<ServiceResponse<int>> Register(UserDto user, string password)
        {
            try
            {
                if (await UserExists(user.UserId))
                {
                    return new ServiceResponse<int>
                    {
                        Success = false,
                        Message = "Already exists."
                    };
                }

                CreatePasswordHash(password, out byte[] passwordHash, out byte[] passwordSalt);
                user.PasswordHash = passwordHash;
                user.PasswordSalt = passwordSalt;

                using (var conn = _context.CreateConnection())
                {
                    var cnt = await conn.InsertAsync(user);
                }

                return new ServiceResponse<int>
                {
                    Success = true,
                    Data = user.Idx,
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<int>
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<ServiceResponse<string>> Login(string userId, string password)
        {
            var response = new ServiceResponse<string>();
            UserDto user;
            using (var conn = _context.CreateConnection())
            {
                var sql = "SELECT * FROM users where UserId=@UserId";
                user = await conn.QueryFirstOrDefaultAsync<UserDto>(sql, new { UserId = userId });
            }
            if (user == null)
            {
                return new ServiceResponse<string>() { Success = false, Message = "User not found." };
            }
            else if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
            {
                return new ServiceResponse<string>() { Success = false, Message = "Wrong password." };
            }
            else
            {
                response.Data = CreateToken(user);
            }

            return response;
        }

        public async Task<bool> UserExists(string userId)
        {
            try
            {
                using (var conn = _context.CreateConnection())
                {
                    var sql = "SELECT * FROM users where UserId=@UserId";
                    var cnt = await conn.QueryAsync<UserDto>(sql, new { UserId = userId });
                    return cnt.Count() > 0;
                }
            }
            catch
            {
                return false;
            }
        }

        public async Task<ServiceResponse<bool>> ChangePassword(int userIdx, string newPassword)
        {
            var response = new ServiceResponse<bool>();
            UserDto user;
            using (var conn = _context.CreateConnection())
            {
                var sql = "SELECT * FROM users where Idx=@userIdx";
                user = await conn.QueryFirstOrDefaultAsync<UserDto>(sql, new { userIdx = userIdx });
            }
            if (user == null)
            {
                return new ServiceResponse<bool>() { Success = false, Message = "User not found." };
            }

            CreatePasswordHash(newPassword, out byte[] passwordHash, out byte[] passwordSalt);
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            using (var conn = _context.CreateConnection())
            {
                var cnt = await conn.UpdateAsync(user);
            }

            return response;
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));

            }
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using(var hmac = new HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(passwordHash);
            }
        }

        private string CreateToken(UserDto user)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Idx.ToString()),
                new Claim(ClaimTypes.Name, user.UserId),
            };

            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8
                .GetBytes(_configuration.GetSection("AppSettings:Token").Value));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            var token = new JwtSecurityToken(
                    claims: claims,
                    expires: DateTime.Now.AddHours(3),
                    signingCredentials: creds);

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);
            return jwt;
        }

        public async Task<IEnumerable<UserDto>?> GetAll()
        {
            try
            {
                using (var conn = _context.CreateConnection())
                {
                    var sql = "SELECT * FROM tb_user";
                    var books = await conn.QueryAsync<UserDto>(sql);
                    return books;
                }
            }
            catch
            {
                return null;
            }
        }

        public async Task<bool> AddAsync(UserDto product)
        {
            try
            {
                using (var conn = _context.CreateConnection())
                {
                    var result = await conn.InsertAsync(product);
                    return result > 0;
                }
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UpdateAsync(UserDto product)
        {
            try
            {
                using (var conn = _context.CreateConnection())
                {
                    var result = await conn.UpdateAsync(product);
                    return result;
                }
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteAsync(UserDto product)
        {
            try
            {
                using (var conn = _context.CreateConnection())
                {
                    var result = await conn.UpdateAsync(product);
                    return result;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}