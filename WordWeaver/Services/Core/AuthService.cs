using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Security.Cryptography;
using WordWeaver.Data;
using WordWeaver.Data.Entity;
using WordWeaver.Helper;
using WordWeaver.Dtos;
using AutoMapper;
using WordWeaver.Services.Core.Interfaces;

namespace WordWeaver.Services.Core;

public class AuthService(WordWeaverContext context, IMapper mapper, ITokenService tokenService) : IAuthService
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
            return new AuthResponse
            {
                Message = "Invalid username or email.",
                StatusCode = HttpStatusCode.BadRequest,
            };
        }

        (string token, DateTime expiresAt) = tokenService.GenerateAuthToken(user);

        try
        {
            context.Logins.Add(new Login
            {
                UserId = user.UserId,
                Token = token,
                IpAddress = tokenService.ClientIpAddress,
                ExpiresAt = expiresAt
            });

            context.SaveChanges();

        }
        catch (Exception ex)
        {
            return new AuthResponse
            {
                Message = $"Error: {ex.Message}",
                StatusCode = HttpStatusCode.InternalServerError,
            };
        }

        return new AuthResponse
        {
            Message = "Login successful.",
            StatusCode = HttpStatusCode.OK,
            Token = token,
            ExpiresAt = expiresAt,
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
                    return new AuthResponse
                    {
                        Message = "Username or Email must be unique.",
                        StatusCode = HttpStatusCode.BadRequest,
                    };
                }

                // hash password and generate salt
                (string hashedPassword, string salt) = HashPasswordWithSalt(model.Password);

                // create user, role and save to database
                var addedUser = await context.Users.AddAsync(new User
                {
                    Username = model.Username,
                    Email = model.Email,
                    PasswordHash = hashedPassword,
                    Salt = salt,
                });

                await context.SaveChangesAsync();

                await context.UserRoles.AddAsync(new UserRole
                {
                    RoleId = (int)Roles.User,
                    UserId = addedUser.Entity.UserId,
                });

                await context.SaveChangesAsync();
                await transaction.CommitAsync();

                // login user and return token
                var login = await Login(new LoginDto
                {
                    UsernameOrEmail = addedUser.Entity.Email,
                    Password = model.Password
                });

                return new AuthResponse
                {
                    Message = "User created successfully.",
                    StatusCode = HttpStatusCode.OK,
                    Token = login.Token,
                    ExpiresAt = login.ExpiresAt,
                    User = login.User,
                };

            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();

                return new AuthResponse
                {
                    Message = $"Error: {ex.Message}",
                    StatusCode = HttpStatusCode.InternalServerError,
                };
            }
        }
    }

    public async Task<AuthResponse> SendOtp(string email)
    {
        context.Otps.Add(new Otp
        {
            Email = email,
            OtpValue = GenerateOtp(),
            ExpiresAt = DateTime.UtcNow.AddMinutes(5),
        });

        return null;

    }

    private string GenerateOtp()
    {
        var random = new Random();
        var otp = random.Next(1000, 9999).ToString();

        return otp;
    }

    #endregion ### User Login and Registration ###

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