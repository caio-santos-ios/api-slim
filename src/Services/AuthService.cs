using api_slim.src.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using api_slim.src.Models.Base;
using api_slim.src.Responses;
using api_slim.src.Interfaces;
using api_slim.src.Shared.DTOs;
using api_slim.src.Handlers;
using api_slim.src.Shared.Templates;
using api_slim.src.Shared.Validators;
using api_slim.src.Shared.Utils;
using MongoDB.Bson;
using System.Text.Json;

namespace api_slim.src.Services
{
    public class AuthService(IUserRepository userRepository, ICustomerRecipientRepository customerRecipientRepository, IPlanRepository planRepository, IServiceModuleRepository serviceModuleRepository, MailHandler mailHandler, ICustomerRepository customerRepository, IPermissionProfileRepository permissionProfileRepository) : IAuthService
    {
        public async Task<ResponseApi<AuthResponse>> LoginAsync(LoginDTO request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.Password)) return new(null, 400, "Senha é obrigatória");
                if (string.IsNullOrEmpty(request.Email)) return new(null, 400, "E-mail é obrigatório");
                
                ResponseApi<User?> res = await userRepository.GetByEmailAsync(request.Email);
                User? user = null;
                string type = "Interno";

                if(res.Data is null) 
                {
                    ResponseApi<Customer?> customer = await customerRepository.GetByEmailAsync(request.Email);
                    if(customer.Data is not null) 
                    {
                        user = new()
                        {
                            Id = customer.Data.Id,
                            Name = customer.Data.CorporateName,
                            Admin = false,
                            Modules = [],
                            Photo = "",
                            Password = customer.Data.Password,
                            Role = Enums.User.RoleEnum.Manager,
                            PermissionProfile = customer.Data.Id,
                            ContractorId = customer.Data.Id
                        };

                        type = "Externo";
                    }
                }
                else
                {
                    user = res.Data!;
                    type = res.Data.Type;
                }

                if(user is null) return new(null, 400, "Dados incorretos");
                bool isValid = BCrypt.Net.BCrypt.Verify(request.Password, user.Password);
                if(!isValid) return new(null, 400, "Dados incorretos");

                ResponseApi<PermissionProfile?> profile = await permissionProfileRepository.GetByIdAsync(user.PermissionProfile);

                AuthResponse auth = new ()
                {
                    Token = GenerateJwtToken(user), 
                    RefreshToken = GenerateJwtToken(user, true), 
                    Name = user.Name, 
                    Id = user.Id, 
                    Admin = user.Admin, 
                    Modules = user.Modules, 
                    Photo = user.Photo,
                    Role = user.Role.ToString(),
                    PermissionProfileName = profile.Data is not null ? profile.Data.Name : "",
                    ContractorId = user.ContractorId,
                    Type = type
                };

                return new(auth);
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");            
            }
        }
        public async Task<ResponseApi<AuthAppResponse>> LoginAppAsync(LoginAppDTO request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.Cpf)) return new(null, 400, "CPF é obrigatório");
                if (string.IsNullOrEmpty(request.Password)) return new(null, 400, "Senha é obrigatória");
                
                ResponseApi<CustomerRecipient?> response = await customerRecipientRepository.GetByDocumentAsync(request.Cpf);
                if(response.Data is null) return new(null, 400, "Dados incorretos");
                CustomerRecipient customer = response.Data!;

                if(customer is null) return new(null, 400, "Dados incorretos");

                ResponseApi<Plan?> plan = await planRepository.GetByIdAsync(customer.PlanId);
                
                if(plan.Data is null) return new(null, 400, "Beneficiário não tem plano ativo");
                
                if(plan.Data.ServiceModuleIds.Count == 0) return new(null, 400, "Plano do beneficiário não possui módulos ativos");

                List<string> listModules = [];

                foreach (string module in plan.Data.ServiceModuleIds)
                {
                    ResponseApi<ServiceModule?> modules = await serviceModuleRepository.GetByIdAsync(module);
                    if(modules.Data is not null)
                    {
                        listModules.Add(modules.Data.Identification);
                    } 
                };

                User user = new()
                {
                    Id = customer.Id,
                    UserName = customer.Name,
                    Email = customer.Email,
                    Password = customer.Password
                };

                if(response.Data.FirstAccess)
                {
                    string document = customer.Cpf.Replace(".", "").Replace("-", "").Substring(0, 6);
                    if(document != request.Password) return new(null, 400, "Dados incorretos");

                    user.Id = customer.Id;
                    user.Email = customer.Email;
                    user.Name = customer.Name;
                    user.Photo = customer.Photo;
                    user.Role = Enums.User.RoleEnum.Manager;
                } 
                else
                {
                    bool isValid = BCrypt.Net.BCrypt.Verify(request.Password, user.Password);
                    if(!isValid) return new(null, 400, "Dados incorretos");
                }

                ResponseApi<Customer?> contractor = await customerRepository.GetByIdAsync(customer.ContractorId);

                AuthAppResponse data = new() 
                {
                    Token = GenerateJwtToken(user, false, true), 
                    RefreshToken = GenerateJwtToken(user, true, true), 
                    Expires = DateTime.UtcNow.AddDays(7),
                    Name = customer.Name, 
                    Photo = customer.Photo, 
                    RapidocId = customer.RapidocId, 
                    FirstAccess = customer.FirstAccess,
                    ModulesIdentifications = listModules,
                    CPF = customer.Cpf,
                    TypeContractor = contractor.Data is null ? "" : contractor.Data.Type
                };

                return new(data);
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");            
            }
        }
        public async Task<ResponseApi<AuthResponse>> RefreshTokenAsync(string token)
        {
            try
            {
                var handler = new JwtSecurityTokenHandler();
                SecurityToken? validatedToken;

                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = Environment.GetEnvironmentVariable("ISSUER"),
                    ValidAudience = Environment.GetEnvironmentVariable("AUDIENCE"),
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("SECRET_KEY") ?? "")),
                    ValidateLifetime = false 
                };

                var principal = handler.ValidateToken(token, validationParameters, out validatedToken);
                var jwtToken = validatedToken as JwtSecurityToken;

                if (jwtToken == null) return new(null, 401, "Token inválido.");

                string? tokenType = jwtToken.Claims.FirstOrDefault(c => c.Type == "type")?.Value;
                if (tokenType != "refresh") return new(null, 401, "O token fornecido não é um refresh token.");

                var userId = principal.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub || c.Type == ClaimTypes.NameIdentifier)?.Value;
                
                if (string.IsNullOrEmpty(userId)) return new(null, 401, "Usuário não encontrado no token.");

                ResponseApi<User?> user = await userRepository.GetByIdAsync(userId);
                if (user.Data is null) return new(null, 401, "Usuário não encontrado.");

                string accessToken = GenerateJwtToken(user.Data);
                string refreshToken = GenerateJwtToken(user.Data, true);

                return new(new AuthResponse
                {
                    Token = accessToken,
                    RefreshToken = refreshToken,
                    Role = user.Data.Role.ToString(),
                    Id = user.Data.Id,
                    Expires = DateTime.UtcNow.AddDays(7)
                });
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");            
            }
        }
        public async Task<ResponseApi<AuthResponse>> RefreshTokenAppAsync(string token)
        {
            try
            {
                var handler = new JwtSecurityTokenHandler();
                SecurityToken? validatedToken;

                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = Environment.GetEnvironmentVariable("ISSUER"),
                    ValidAudience = Environment.GetEnvironmentVariable("AUDIENCE"),
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("SECRET_KEY") ?? "")),
                    ValidateLifetime = false 
                };

                var principal = handler.ValidateToken(token, validationParameters, out validatedToken);
                var jwtToken = validatedToken as JwtSecurityToken;

                if (jwtToken == null) return new(null, 401, "Token inválido.");

                string? tokenType = jwtToken.Claims.FirstOrDefault(c => c.Type == "type")?.Value;
                if (tokenType != "refresh") return new(null, 401, "O token fornecido não é um refresh token.");

                var userId = principal.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub || c.Type == ClaimTypes.NameIdentifier)?.Value;
                
                if (string.IsNullOrEmpty(userId)) return new(null, 401, "Usuário não encontrado no token.");

                ResponseApi<CustomerRecipient?> user = await customerRecipientRepository.GetByIdAsync(userId);
                if (user.Data is null) return new(null, 401, "Usuário não encontrado.");

                User newUser = new ()
                {
                    Id = user.Data.Id,
                    Email = user.Data.Email,
                    UserName = user.Data.Name
                };

                string accessToken = GenerateJwtToken(newUser, false, true);
                string refreshToken = GenerateJwtToken(newUser, true, true);

                return new(new AuthResponse
                {
                    Token = accessToken,
                    RefreshToken = refreshToken,
                    Id = user.Data.Id,
                    Expires = DateTime.UtcNow.AddDays(7)
                });
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");            
            }
        }
        public async Task<ResponseApi<User>> ResetPasswordAsync(ResetPasswordDTO request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.Password)) return new(null, 400, "Senha é obrigatória");
                if (string.IsNullOrEmpty(request.Id)) return new(null, 400, "Falha ao alterar senha");
                
                if(Validator.IsReliable(request.Password).Equals("Ruim")) return new(null, 400, $"Senha é muito fraca");

                ResponseApi<User?> user = await userRepository.GetByIdAsync(request.Id);
                if(!user.IsSuccess || user.Data is null) return new(null, 400, "Falha ao alterar senha");
                
                bool isValid = BCrypt.Net.BCrypt.Verify(request.Password, user.Data.Password);
                if(!isValid) return new(null, 400, "Senha antiga incorreta");

                user.Data.Password = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
                ResponseApi<User?> response = await userRepository.UpdateAsync(user.Data);
                if(!response.IsSuccess) return new(null, 400, "Falha ao alterar senha");

                return new(null, 200, "Senha alterada com sucesso");
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");            
            }
        }
        public async Task<ResponseApi<User>> ResetPasswordAppAsync(ResetPasswordDTO request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.Password)) return new(null, 400, "Senha é obrigatória");
                if (request.Password != request.NewPassword) return new(null, 400, "As senhas precisam ser iguais");
                if (string.IsNullOrEmpty(request.Id)) return new(null, 400, "Falha ao alterar senha");
                
                if(Validator.IsReliable(request.Password).Equals("Ruim")) return new(null, 400, $"Senha é muito fraca");

                ResponseApi<CustomerRecipient?> codeAccess = await customerRecipientRepository.GetByCodeAccessAsync(request.CodeAccess);
                if(!codeAccess.IsSuccess || codeAccess.Data is null) return new(null, 400, "Código expirou, solicite um novo código");

                ResponseApi<CustomerRecipient?> customerRecipient = await customerRecipientRepository.GetByIdAsync(request.Id);
                if(!customerRecipient.IsSuccess || customerRecipient.Data is null) return new(null, 400, "Falha ao alterar senha");

                customerRecipient.Data.Password = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
                customerRecipient.Data.FirstAccess = false;
                ResponseApi<CustomerRecipient?> response = await customerRecipientRepository.UpdateAsync(customerRecipient.Data);
                if(!response.IsSuccess) return new(null, 400, "Falha ao alterar senha");

                return new(null, 200, "Senha alterada com sucesso");
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");            
            }
        }
        public async Task<ResponseApi<User>> ResetPasswordFirstAppAsync(ResetPasswordDTO request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.Password)) return new(null, 400, "Senha é obrigatória");
                if (request.Password != request.NewPassword) return new(null, 400, "As senhas precisam ser iguais");
                if (string.IsNullOrEmpty(request.Id)) return new(null, 400, "Falha ao alterar senha");
                
                if(Validator.IsReliable(request.Password).Equals("Ruim")) return new(null, 400, $"Senha é muito fraca");

                ResponseApi<CustomerRecipient?> customerRecipient = await customerRecipientRepository.GetByIdAsync(request.Id);
                if(!customerRecipient.IsSuccess || customerRecipient.Data is null) return new(null, 400, "Falha ao alterar senha");

                customerRecipient.Data.Password = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
                customerRecipient.Data.FirstAccess = false;
                ResponseApi<CustomerRecipient?> response = await customerRecipientRepository.UpdateAsync(customerRecipient.Data);
                if(!response.IsSuccess) return new(null, 400, "Falha ao alterar senha");

                return new(null, 200, "Senha alterada com sucesso");
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");            
            }
        }
        public async Task<ResponseApi<User>> RequestForgotPasswordAsync(ForgotPasswordDTO request)
        {
            try
            {
                if(request.Device == "app")
                {
                    dynamic access = Util.GenerateCodeAccess();

                    ResponseApi<CustomerRecipient?> customerRecipient = await customerRecipientRepository.GetByEmailAsync(request.Email);
                    if(customerRecipient.Data is null) return new(null, 404, "E-mail inválido");

                    customerRecipient.Data.CodeAccess = access.CodeAccess;
                    customerRecipient.Data.CodeAccessExpiration = access.CodeAccessExpiration;
                    customerRecipient.Data.ValidatedAccess = false;
                    customerRecipient.Data.FirstAccess = false;

                    await customerRecipientRepository.UpdateAsync(customerRecipient.Data);

                    string template = MailTemplate.ForgotPasswordWeb(customerRecipient.Data.Name, access.CodeAccess);
                    await mailHandler.SendMailAsync(request.Email, "Redefinição de Senha", template);

                    return new(new User() {Id = customerRecipient.Data.Id, CodeAccess = access.CodeAccess}, 200, "Código enviado");
                }
                else 
                {
                    ResponseApi<User?> user = await userRepository.GetByEmailAsync(request.Email);

                    Customer? authCustomer = null;
                    if(user.Data is null)
                    {
                        ResponseApi<Customer?> customer = await customerRepository.GetByEmailAsync(request.Email);
                        if(customer.Data is not null) 
                        {
                            user.Data = new()
                            {
                                Id = customer.Data.Id,
                                Name = customer.Data.CorporateName,
                                Admin = false,
                                Modules = [],
                                Photo = "",
                                Role = Enums.User.RoleEnum.Manager
                            };

                            authCustomer = customer.Data;
                        }
                    };

                    if(user.Data is null || !Validator.IsEmail(request.Email)) return new(null, 400, "E-mail inválido.");

                    dynamic access = Util.GenerateCodeAccess();
                    if(user.Data.Role == Enums.User.RoleEnum.Manager)
                    {
                        if(authCustomer is not null)
                        {
                            authCustomer.CodeAccess = access.CodeAccess;
                            authCustomer.CodeAccessExpiration = access.CodeAccessExpiration;
                            authCustomer.ValidatedAccess = false;

                            user.Data.CodeAccess = access.CodeAccess;
                            user.Data.CodeAccessExpiration = access.CodeAccessExpiration;
                            user.Data.ValidatedAccess = false;
                        
                            ResponseApi<Customer?> response = await customerRepository.UpdateAsync(authCustomer);
                            if(!response.IsSuccess) return new(null, 400, "Falha ao redefinir senha");
                        }
                        else
                        {
                            return new(null, 400, "Falha ao redefinir senha");
                        }
                    }
                    else
                    {
                        user.Data.CodeAccess = access.CodeAccess;
                        user.Data.CodeAccessExpiration = access.CodeAccessExpiration;
                        user.Data.ValidatedAccess = false;
                        
                        ResponseApi<User?> response = await userRepository.UpdateAsync(user.Data);
                        if(!response.IsSuccess) return new(null, 400, "Falha ao redefinir senha");
                    }

                    string template = MailTemplate.ForgotPasswordWeb(user.Data.Name, user.Data.CodeAccess);
                    await mailHandler.SendMailAsync(request.Email, "Redefinição de Senha", template);


                    return new(null, 200, "Foi enviado um e-mail para redefinir sua senha");
                }
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");            
            }
        }
        public async Task<ResponseApi<User>> ForgotPasswordAsync(ResetPasswordDTO request)
        {
            try
            {
                ResponseApi<User?> user = await userRepository.GetByCodeAccessAsync(request.CodeAccess);
                DateTime? codeAccessExpiration = null;
                string codeAccess = "";
                string name = "";
                string email = "";
                string role = "";
                Customer? authCustomer = null;

                if(user.Data is null)
                {
                    ResponseApi<Customer?> customer = await customerRepository.GetByCodeAccessAsync(request.CodeAccess);
                    if(customer.Data is null) return new(null, 400, "Código inválido.");

                    codeAccessExpiration = customer.Data.CodeAccessExpiration;
                    codeAccess = customer.Data.CodeAccess;
                    name = customer.Data.CorporateName;
                    email = customer.Data.Email;
                    role = "manager";
                    authCustomer = customer.Data;
                }
                else
                {
                    codeAccessExpiration = user.Data.CodeAccessExpiration;
                    codeAccess = user.Data.CodeAccess;
                    name = user.Data.Name;
                    email = user.Data.Email;
                    role = "user";
                }

                if(codeAccessExpiration < DateTime.UtcNow) return new(null, 400, "Código já expirado");
                
                dynamic access = Util.GenerateCodeAccess();
                string template = "";
                
                if(request.Equals("app"))
                {
                    template = MailTemplate.ForgotPasswordApp($"/api/auth/reset-password?codeAccess={user.Data!.CodeAccess}");
                }
                else
                {
                    template = MailTemplate.ForgotPasswordWeb(name, $"/api/auth/reset-password?codeAccess={codeAccess}");
                };

                if(role == "manager")
                {
                    if(authCustomer is not null) 
                    {
                        authCustomer.Password = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
                        authCustomer.CodeAccess = "";
                        authCustomer.CodeAccessExpiration = null;
                        authCustomer.ValidatedAccess = true;

                        ResponseApi<Customer?> response = await customerRepository.UpdateAsync(authCustomer);
                        if(!response.IsSuccess) return new(null, 400, "Falha ao redefinir senha");
                    }
                }
                else
                {
                    if(user.Data is not null)
                    {
                        user.Data.Password = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
                        user.Data.CodeAccess = "";
                        user.Data.CodeAccessExpiration = null;
                        user.Data.ValidatedAccess = true;

                        ResponseApi<User?> response = await userRepository.UpdateAsync(user.Data);
                        if(!response.IsSuccess) return new(null, 400, "Falha ao redefinir senha");
                    }
                }

                return new(null, 200, "Senha alterada com sucesso");
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");            
            }
        }
        private static string GenerateJwtToken(User user, bool refresh = false, bool app = false)
        {
            string? SecretKey = Environment.GetEnvironmentVariable("SECRET_KEY") ?? "";
            string? Issuer = Environment.GetEnvironmentVariable("ISSUER") ?? "";
            string? Audience = Environment.GetEnvironmentVariable("AUDIENCE") ?? "";

            SymmetricSecurityKey key = new(Encoding.UTF8.GetBytes(SecretKey));

            Claim[] claims =
            [
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Nickname, user.UserName),
                new Claim(ClaimTypes.Role, user.Role.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("type", refresh ? "refresh" : "access"),
                new Claim("photo", user.Photo),
                new Claim("name", user.Name),
                new Claim("master", user.Master.ToString()),
                new Claim("admin", user.Admin.ToString()),
                new Claim("blocked", user.Blocked.ToString()),
                // new Claim("modules", JsonSerializer.Serialize(user.Modules)),
                new Claim("modules", JsonSerializer.Serialize(user.Modules, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                }))
            ];

            SigningCredentials creds = new(key, SecurityAlgorithms.HmacSha256);

            JwtSecurityToken token = new(
                issuer: Issuer,
                audience: Audience,
                claims: claims,
                expires: refresh || app ? DateTime.UtcNow.AddDays(7) : DateTime.UtcNow.AddHours(2),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}