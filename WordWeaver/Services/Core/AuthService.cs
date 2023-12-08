using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Security.Cryptography;
using WordWeaver.Data;
using WordWeaver.Data.Entity;
using WordWeaver.Helpers;
using WordWeaver.Dtos;
using AutoMapper;
using WordWeaver.Services.Core.Interfaces;

namespace WordWeaver.Services.Core;

public class AuthService(WordWeaverContext context, IMapper mapper, ITokenService tokenService, IAuthenticatedUser authUser, IMailService mailService) : IAuthService
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
                IpAddress = authUser.IpAddress,
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
                if(!await IsUsernameUnique(model.Username))
                {
                    return new AuthResponse {
                        Message = "Username must be unique.",
                        StatusCode = HttpStatusCode.BadRequest,
                    };
                }

                // verify email
                if (!await VerifyEmail(model.Email, model.Otp))
                {
                    return new AuthResponse {
                        Message = "Email it not verified.",
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

                // update otp record
                var otpRecord = context.Otps.FirstOrDefault(o => o.Email == model.Email);

                if (otpRecord != null)
                {
                    otpRecord.IsUsed = true;
                    otpRecord.ExpiresAt = DateTime.UtcNow;
                }

                // save and commit changes
                await context.SaveChangesAsync();
                await transaction.CommitAsync();

                // send welcome email
                await mailService.SendEmail(new EmailDto {
                    To = model.Email,
                    Subject = "WordWeaver Registration Successful",
                    Body = @$"
                        <body style='font-family: Arial, sans-serif;'>
                            <div style='max-width: 600px; margin: 0 auto; padding: 20px; border: 1px solid #ddd; border-radius: 5px;'>
                                <h2 style='color: #333; text-align: center;'>Registration Successful!</h2>
                                <p>Hello {model.Username},</p>
                                <p>Congratulations! Your registration on WordWeaver was successful.</p>
                                <p>Thank you for becoming a part of our community. If you have any questions, feel free to contact our support team at <a href='mailto:support@wordweaver.com'>support@wordweaver.com</a></p>
                                <p>Happy blogging!<br>WordWeaver</p>
                            </div>
                        </body>"}, 0, false);

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

    #endregion ### User Login and Registration ###

    #region ### OTP ###
    public async Task<bool> SendOtp(string email)
    {
        try
        {
            if (await context.Users.AnyAsync(u => u.Email == email))
            {
                return false;
            }

            var otp = GenerateOtp();

            // send otp to email
            await mailService.SendEmail(new EmailDto {
                To = email,
                Subject = "WordWeaver Email Verification",
                Body = @$"
                    <body style='font-family: Arial, sans-serif;'>
                        <div style='max-width: 600px; margin: 0 auto; padding: 20px; border: 1px solid #ddd; border-radius: 5px;'>
                            <p>Thanks for starting the new WordWeaver account creation process. We want to make sure it's really you. Please enter the following verification code.</p>
                            <div style='text-align: center; line-height: 25px;'>
                                <div style='margin-bottom: 5px;'>
                                    <span>Verification Code</span><br>
                                    <span style='font-size: 20px; font-weight: 700;'>{otp}</span><br>
                                    <span>(This code is valid for 5 minutes)</span>
                                </div>
                            </div>
                            <p>If you did not request this OTP, please ignore this email.</p>
                            <p>Thank you,<br> WordWeaver Team</p>
                        </div>
                    </body>"}, 0, false);

            // save otp to database
            context.Otps.Add(new Otp {
                Email = email,
                OtpValue = otp,
                ExpiresAt = DateTime.UtcNow.AddMinutes(5),
            });

            await context.SaveChangesAsync();

            return true;

        } catch (Exception ex)
        {
            throw new Exception($"Error: {ex.Message}");
        }
    }

    public async Task<bool> VerifyEmail(string email, string otp)
    {
        return await context.Otps.AnyAsync(o => o.Email == email && o.IsUsed == false && o.OtpValue == otp && o.ExpiresAt > DateTime.UtcNow);
    }

    public async Task<bool> IsUsernameUnique(string username)
    {
        return !await context.Users.AnyAsync(u => u.Username == username);
    }

    private string GenerateOtp()
    {
        var random = new Random();
        var otp = random.Next(1000, 9999).ToString();

        return otp;
    }

    #endregion ### OTP ###

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