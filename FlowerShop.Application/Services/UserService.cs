using AutoMapper;
using AutoMapper.QueryableExtensions;
using FlowerShop.Domain;
using FlowerShop.Infrastructure;
using FlowerShop.Utility;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace FlowerShop.Application
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public UserService(IUnitOfWork unitOfWork, IMapper mapper, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _configuration = configuration;
        }

        public IQueryable<UserDTO> GetUsersOData()
        {
            var userQuery = _unitOfWork.UserRepository.GetQuery();
            var response = userQuery.ProjectTo<UserDTO>(_mapper.ConfigurationProvider);
            return response;
        }
        public async Task<ApiResponse<UserDTO>> RegisterAsync(UserRegisterDTO dto)
        {
            var existingUser = await _unitOfWork.UserRepository.GetByAsync(u => u.Email == dto.Email);
            if (existingUser != null)
            {
                throw new BadRequestException("Email đăng ký đã được sử dụng");
            }

            var user = _mapper.Map<User>(dto);
            user.Password = PasswordHelper.HashPassword(dto.Password);
            await _unitOfWork.UserRepository.AddAsync(user);
            var cart = new Cart { UserID = user.UserID };
            await _unitOfWork.SaveAsync();

            var response = _mapper.Map<UserDTO>(user);
            return new ApiResponse<UserDTO>(response, "Đăng ký tài khoản thành công.");
        }

        public async Task<ApiResponse<UserDTO>> LoginAsync(UserLoginDTO dto)
        {
            var user = await _unitOfWork.UserRepository.GetByAsync(u => u.Email == dto.Email) ?? throw new BadRequestException("Invalid email or password.");

            bool isPasswordValid = PasswordHelper.VerifyPassword(dto.Password, user.Password);
            if (!isPasswordValid)
            {
                throw new BadRequestException("Sai Email hoặc Mật Khẩu.");
            }

            var token = GenerateJwtToken(user);

            var response = _mapper.Map<UserDTO>(user);
            response.Token = token;

            return new ApiResponse<UserDTO>(response, "Đăng nhập thành công.");
        }

        public Task<ApiResponse<bool>> LogoutAsync()
        {
            return Task.FromResult(new ApiResponse<bool>(true, "Đã đăng xuất tài khoản."));
        }
        private string GenerateJwtToken(User user)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var key = Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]!);

            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, user.UserID.ToString()),
                new(JwtRegisteredClaimNames.Email, user.Email),
                new(ClaimTypes.Role, user.Role.ToString()),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(double.Parse(jwtSettings["ExpiryMinutes"]!)),
                Issuer = jwtSettings["Issuer"],
                Audience = jwtSettings["Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
