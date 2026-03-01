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
        public async Task<ApiResponse<UserDTO>> RegisterAsync(UserRegisterDTO model)
        {
            var existingUser = await _unitOfWork.UserRepository.GetByAsync(u => u.Email == model.Email);
            if (existingUser != null)
            {
                throw new BadRequestException("Email is already registered.");
            }

            var user = _mapper.Map<User>(model);
            user.Password = PasswordHelper.HashPassword(model.Password);
            await _unitOfWork.UserRepository.AddAsync(user);
            await _unitOfWork.SaveAsync();

            var response = _mapper.Map<UserDTO>(user);
            return new ApiResponse<UserDTO>(response, "Registration successful.");
        }

        public async Task<ApiResponse<UserDTO>> LoginAsync(UserLoginDTO model)
        {
            var user = await _unitOfWork.UserRepository.GetByAsync(u => u.Email == model.Email) ?? throw new BadRequestException("Invalid email or password.");

            bool isPasswordValid = PasswordHelper.VerifyPassword(model.Password, user.Password);
            if (!isPasswordValid)
            {
                throw new BadRequestException("Invalid email or password.");
            }

            var token = GenerateJwtToken(user);

            var response = _mapper.Map<UserDTO>(user);
            response.Token = token;

            return new ApiResponse<UserDTO>(response, "Login successful.");
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
