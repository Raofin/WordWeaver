using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using WordWeaver.Data;
using WordWeaver.Data.Entity;
using WordWeaver.Helper;
using WordWeaver.Dtos;
using AutoMapper;

#pragma warning disable CS8604

namespace WordWeaver.Services.Auth;

public class AuthService(IConfiguration config, WordWeaverContext context, IMapper mapper) : IAuthService
{
    #region ### User Login and Registration ###

    public async Task<AuthResponse> Login(LoginDto model)
    {
        // check if user exists
        var user = await context.Users
        .Include(u => u.UserRoles).ThenInclude(ur => ur.Role)
        .FirstOrDefaultAsync(u => u.Username == model.UsernameOrEmail || u.Email == model.UsernameOrEmail);

        if (user == null || !VerifyPassword(user.PasswordHash, user.Salt, model.Password))
        {
            return new AuthResponse {
                Message = "Invalid username or email.",
                StatusCode = HttpStatusCode.BadRequest,
            };
        }

        return new AuthResponse {
            Message = "Login successful.",
            StatusCode = HttpStatusCode.OK,
            Token = GenerateAuthToken(user),
            User = mapper.Map<UserDto>(user),
        };
    }

    public async Task<AuthResponse> Register(RegistrationDto model)
    {
        using (var transaction = await context.Database.BeginTransactionAsync())
        {
            try
            {
                // check if user already exists
                if (await context.Users.AnyAsync(u => u.Username == model.Username || u.Email == model.Email))
                {
                    return new AuthResponse {
                        Message = "Username or Email must be unique.",
                        StatusCode = HttpStatusCode.BadRequest,
                    };
                }

                // hash password and generate salt
                (string hashedPassword, string salt) = HashPasswordWithSalt(model.Password);

                // create user, role and save to database
                var addedUser = await context.Users.AddAsync(new User {
                    Username = model.Username,
                    Email = model.Email,
                    PasswordHash = hashedPassword,
                    Salt = salt,
                });

                await context.SaveChangesAsync();

                await context.UserRoles.AddAsync(new UserRole {
                    RoleId = (int)Roles.User,
                    UserId = addedUser.Entity.UserId,
                });

                await context.SaveChangesAsync();
                await transaction.CommitAsync();

                // login user and return token
                var login = await Login(new LoginDto {
                    UsernameOrEmail = addedUser.Entity.Email,
                    Password = model.Password
                });

                return new AuthResponse {
                    Message = "User created successfully.",
                    StatusCode = HttpStatusCode.OK,
                    Token = login.Token,
                    User = login.User,
                };

            } catch (Exception ex)
            {
                await transaction.RollbackAsync();

                return new AuthResponse {
                    Message = $"Error: {ex.Message}",
                    StatusCode = HttpStatusCode.InternalServerError,
                };
            }
        }
    }

    #endregion ### User Login and Registration ###

    #region ### Token Generation ###
    public string GenerateAuthToken(User user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(config["Jwt:Key"]);
        
        var tokenDescriptor = new SecurityTokenDescriptor {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),  // Subject (user ID)
                new Claim(JwtRegisteredClaimNames.UniqueName, user.Username),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Iss, config["Jwt:Issuer"]),  // Issuer
                new Claim(JwtRegisteredClaimNames.Aud, config["Jwt:Audience"]),  // Audience
                new Claim(JwtRegisteredClaimNames.Exp, new DateTimeOffset(DateTime.UtcNow.AddHours(1)).ToUnixTimeSeconds().ToString()),  // Expiration Time
                new Claim(JwtRegisteredClaimNames.Nbf, new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString()),  // Not Before
                new Claim(JwtRegisteredClaimNames.Iat, new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString()),  // Issued At
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),  // JWT ID
            }
            .Union(user.UserRoles.Select(ur => new Claim(ClaimTypes.Role, ur.Role?.RoleName)))),  // Add roles

            Expires = DateTime.UtcNow.AddSeconds(15),  // Token expiration time
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        var tokenString = tokenHandler.WriteToken(token);

        return tokenString;
    }

    #endregion ### Token Generation ###

    #region ### Password Hashing ###

    public (string hashedPassword, string salt) HashPasswordWithSalt(string password)
    {
        var salt = GenerateSalt();
        var combinedPassword = $"{password}{salt}";
        var passwordHasher = new PasswordHasher<IdentityUser>();
        var hashedPassword = passwordHasher.HashPassword(new IdentityUser(), combinedPassword);

        return (hashedPassword, salt);
    }

    public bool VerifyPassword(string hashedPasswordFromDatabase, string saltFromDatabase, string providedPassword)
    {
        var combinedPassword = $"{providedPassword}{saltFromDatabase}";
        var passwordHasher = new PasswordHasher<IdentityUser>();
        var passwordVerificationResult = passwordHasher.VerifyHashedPassword(new IdentityUser(), hashedPasswordFromDatabase, combinedPassword);

        return passwordVerificationResult == PasswordVerificationResult.Success;
    }

    private string GenerateSalt()
    {
        byte[] saltBytes = new byte[32];

        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(saltBytes);
        }

        return Convert.ToBase64String(saltBytes);
    }

    #endregion ### Password Hashing ###
}