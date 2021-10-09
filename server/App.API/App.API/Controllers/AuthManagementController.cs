using App.API.Models;
using App.API.Models.Helpers;
using App.API.Models.Security;
using App.API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace App.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class AuthManagementController : ControllerBase
    {
        private readonly TokenValidationParameters _tokenValidationParameters;
        private readonly RoleManager<IdentityRole> _rolManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _userRole;
        private readonly JwtConfig _jwtConfig;
        private readonly ICrudService<Office> _serviceOffice;
        private readonly ICrudService<RefreshToken> _serviceRefreshToken;
        private readonly ILogger<AuthManagementController> _logger;

        public AuthManagementController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> rolManager,
            IOptionsMonitor<JwtConfig> optionsMonitor,
            TokenValidationParameters tokenValidationParameters, ICrudService<RefreshToken> serviceRefreshToken,
            ILogger<AuthManagementController> logger, RoleManager<IdentityRole> userRole, ICrudService<Office> serviceOffice)
        {
            _rolManager = rolManager;
            _userManager = userManager;
            _jwtConfig = optionsMonitor.CurrentValue;
            _tokenValidationParameters = tokenValidationParameters;
            _serviceRefreshToken = serviceRefreshToken;
            _logger = logger;
            _userRole = userRole;
            _serviceOffice = serviceOffice;
        }

        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register([FromBody] UserRegistration user)
        {
            // Check if the incoming request is valid
            if (ModelState.IsValid)
            {
                // check i the user with the same email exist
                var existingUser = await _userManager.FindByEmailAsync(user.Email);

                if (existingUser != null)
                {
                    return BadRequest(new AuthResponse()
                    {
                        Success = false,
                        Errors = new List<string>() { "Invalid User" }
                    });
                }

                var newUser = new ApplicationUser() { Email = user.Email, UserName = user.Name };
                var isCreated = await _userManager.CreateAsync(newUser, user.Password);
                if (isCreated.Succeeded)
                {
                    await _userManager.AddToRoleAsync(newUser, user.rol);
                    var jwtToken = await GenerateJwtToken(newUser);
                    return Ok(new AuthResponse()
                    {
                        Success = true,
                        Token = jwtToken.Token,
                        RefreshToken = jwtToken.RefreshToken,
                    });
                }

                return new JsonResult(
                    new AuthResponse()
                    { Errors = isCreated.Errors.Select(x => x.Description).ToList() })
                { StatusCode = 500 };
            }

            return BadRequest(new AuthResponse()
            {
                Success = false,
                Errors = new List<string>() { "Invalid payload" }
            });
        }


        [HttpPost]
        [Route("Login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] UsuarioLogin user)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // check if the user with the same email exist
                    var existingUser = await _userManager.FindByNameAsync(user.UserName);
                    var role = (await _userManager.GetRolesAsync(existingUser)).FirstOrDefault();

                    if (existingUser == null)
                    {
                        // We dont want to give to much information on why the request has failed for security reasons
                        return BadRequest(new AuthResponse()
                        {
                            Success = false,
                            Errors = new List<string>() { "Invalid authentication request" }
                        });
                    }

                    // Now we need to check if the user has inputed the right password
                    var isCorrect = await _userManager.CheckPasswordAsync(existingUser, user.Password);

                    if (isCorrect)
                    {
                        var jwtToken = await GenerateJwtToken(existingUser);

                        return Ok(new AuthResponse()
                        {
                            id = existingUser.Id,
                            userEmail = existingUser.NormalizedEmail,
                            validate = true,
                            Success = true,
                            Token = jwtToken.Token,
                            RefreshToken = jwtToken.RefreshToken,
                            userRol = new Rol() { name = role },
                            expiration = jwtToken.expiration,
                            userName = existingUser.NormalizedUserName
                        });
                    }
                    else
                    {
                        // We dont want to give to much information on why the request has failed for security reasons
                        return BadRequest(new AuthResponse()
                        {
                            Success = false,
                            Errors = new List<string>() { "Invalid authentication request" }
                        });
                    }

                }
                catch (Exception)
                {
                    throw;
                }
            }

            return BadRequest(new AuthResponse()
            {
                Success = false,
                Errors = new List<string>() { "Invalid payload" }
            });
        }


        private async Task<AuthResult> GenerateJwtToken(IdentityUser user)
        {
            _tokenValidationParameters.ValidateLifetime = true;

            var jwtTokenHandler = new JwtSecurityTokenHandler();

            var key = Encoding.ASCII.GetBytes(_jwtConfig.Secret);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("Id", user.Id),
                    new Claim(JwtRegisteredClaimNames.Email, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                }),
                Expires = DateTime.Now.AddHours(2),
                NotBefore = DateTime.Now,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = jwtTokenHandler.CreateToken(tokenDescriptor);
            var jwtToken = jwtTokenHandler.WriteToken(token);

            var refreshToken = new RefreshToken()
            {
                JwtId = token.Id,
                IsUsed = false,
                UserId = user.Id,
                AddedDate = DateTime.Now,
                ExpiryDate = DateTime.Now.AddMinutes(60),
                IsRevoked = false,
                Token = AuthHelper.RandomString(25) + Guid.NewGuid(),
                DateCreated = DateTime.Now
            };

            await _serviceRefreshToken.CreateAsync(refreshToken);

            return new AuthResult()
            {
                Token = jwtToken,
                Success = true,
                RefreshToken = refreshToken.Token,
                expiration = tokenDescriptor.Expires.Value
            };
        }

        [HttpPost]
        [Route("RefreshToken")]
        public async Task<IActionResult> RefreshToken([FromBody] UsuarioToken tokenRequest)
        {
            if (ModelState.IsValid)
            {
                var res = await VerifyToken(tokenRequest);

                if (res == null)
                {
                    return BadRequest(new AuthResult()
                    {
                        Errors = new List<string>() { "Invalid tokens" },
                        Success = false
                    });
                }

                var existingUser = await _userManager.FindByNameAsync(tokenRequest.username);
                var role = (await _userManager.GetRolesAsync(existingUser)).FirstOrDefault();


                return Ok(new AuthResponse()
                {
                    id = existingUser.Id,
                    userEmail = existingUser.NormalizedEmail,
                    validate = true,
                    Success = true,
                    Token = res.Token,
                    RefreshToken = res.RefreshToken,
                    userRol = new Rol() { name = role },
                    expiration = res.expiration,
                    userName = existingUser.NormalizedUserName
                });

            }

            return BadRequest(new AuthResult()
            {
                Errors = new List<string>() {
                "Invalid payload"
            },
                Success = false
            });
        }

        private async Task<AuthResult> VerifyToken(UsuarioToken tokenRequest)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();

            try
            {
                // This validation function will make sure that the token meets the validation parameters
                // and its an actual jwt token not just a random string
                // and not validate lifetime because is already expired
                _tokenValidationParameters.ValidateLifetime = false;
                var principal = jwtTokenHandler.ValidateToken(tokenRequest.Token, _tokenValidationParameters, out var validatedToken);

                // Now we need to check if the token has a valid security algorithm
                if (validatedToken is JwtSecurityToken jwtSecurityToken)
                {
                    var result = jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase);

                    if (result == false)
                    {
                        return null;
                    }
                }

                // Will get the time stamp in unix time
                var utcExpiryDate = long.Parse(principal.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Exp).Value);

                // we convert the expiry date from seconds to the date
                var expDate = AuthHelper.UnixTimeStampToDateTime(utcExpiryDate);

                if (expDate > DateTime.Now)
                {
                    return new AuthResult()
                    {
                        Errors = new List<string>() { "We cannot refresh this since the token has not expired" },
                        Success = false
                    };
                }

                // Check the token we got if its saved in the db
                var storedRefreshToken = _serviceRefreshToken.Where(x => x.Token == tokenRequest.RefreshToken).FirstOrDefault();

                if (storedRefreshToken == null)
                {
                    return new AuthResult()
                    {
                        Errors = new List<string>() { "refresh token doesnt exist" },
                        Success = false
                    };
                }

                // Check the date of the saved token if it has expired
                if (DateTime.Now > storedRefreshToken.ExpiryDate)
                {
                    return new AuthResult()
                    {
                        Errors = new List<string>() { "token has expired, user needs to relogin" },
                        Success = false
                    };
                }

                // check if the refresh token has been used
                if (storedRefreshToken.IsUsed)
                {
                    return new AuthResult()
                    {
                        Errors = new List<string>() { "token has been used" },
                        Success = false
                    };
                }

                // Check if the token is revoked
                if (storedRefreshToken.IsRevoked)
                {
                    return new AuthResult()
                    {
                        Errors = new List<string>() { "token has been revoked" },
                        Success = false
                    };
                }

                // we are getting here the jwt token id
                var jti = principal.Claims.SingleOrDefault(x => x.Type == JwtRegisteredClaimNames.Jti).Value;

                // check the id that the recieved token has against the id saved in the db
                if (storedRefreshToken.JwtId != jti)
                {
                    return new AuthResult()
                    {
                        Errors = new List<string>() { "the token doenst mateched the saved token" },
                        Success = false
                    };
                }

                storedRefreshToken.IsUsed = true;
                _serviceRefreshToken.Update(storedRefreshToken);

                var dbUser = await _userManager.FindByIdAsync(storedRefreshToken.UserId);
                return await GenerateJwtToken(dbUser);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fail generating token!");
                return new AuthResult()
                {
                    Errors = new List<string>() { "Fail generating token!" },
                    Success = false
                };
            }
        }

    }
}
