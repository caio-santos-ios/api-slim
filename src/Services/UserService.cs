using api_slim.src.Handlers;
using api_slim.src.Interfaces;
using api_slim.src.Models;
using api_slim.src.Models.Base;
using api_slim.src.Shared.DTOs;
using api_slim.src.Shared.Templates;
using api_slim.src.Shared.Utils;
using api_slim.src.Shared.Validators;

namespace api_slim.src.Services
{
    public class UserService(IUserRepository userRepository, SmsHandler smsHandler, MailHandler mailHandler, CloudinaryHandler cloudinaryHandler) : IUserService
    {
        #region CREATE
        public async Task<ResponseApi<User?>> CreateAsync(CreateUserDTO request)
        {
            try
            {
                ResponseApi<User?> isEmail = await userRepository.GetByEmailAsync(request.Email);
                if(isEmail.Data is not null || !Validator.IsEmail(request.Email)) return new(null, 400, "E-mail inválido.");

                if(Validator.IsReliable(request.Password).Equals("Ruim")) return new(null, 400, $"Senha é muito fraca");

                dynamic access = Util.GenerateCodeAccess();

                List<api_slim.src.Models.Module> modules = [];
                foreach (var module in request.Modules)
                {
                    List<api_slim.src.Models.Routine> routines = [];

                    foreach (var routine in module.Routines)
                    {
                        routines.Add(new () 
                        {
                            Code = routine.Code,
                            Description = routine.Description,
                            Permissions = new ()
                            {
                                Create = routine.Permissions.Create,
                                Read = routine.Permissions.Read,
                                Update = routine.Permissions.Update,
                                Delete = routine.Permissions.Delete
                            }
                        });
                    }
                    
                    modules.Add(new () 
                    {
                        Code = module.Code,
                        Description = module.Description,
                        Routines = routines
                    });
                };

                User user = new()
                {
                    UserName = $"usuário{access.CodeAccess}",
                    Email = request.Email,
                    Phone = request.Phone,
                    Name = request.Name,
                    Password = BCrypt.Net.BCrypt.HashPassword(access.CodeAccess),
                    CodeAccess = "",
                    CodeAccessExpiration = null,
                    ValidatedAccess = true,
                    Modules = modules,
                    Admin = request.Admin,
                    Blocked = request.Blocked,
                    PermissionProfile = request.PermissionProfile
                };

                ResponseApi<User?> response = await userRepository.CreateAsync(user);
                if(response.Data is null) return new(null, 400, "Falha ao criar conta.");
                
                string messageCode = $"Seu código de verificação é: {access.CodeAccess}";
                
                await smsHandler.SendMessageAsync(request.Phone, messageCode);
                
                await mailHandler.SendMailAsync(request.Email, "Primeiro acesso", MailTemplate.FirstAccess(request.Name, request.Email, access.CodeAccess, "/erp"));

                return new(null, 201, "Conta criada com sucesso, foi enviado o código de verificação para seu celular e e-mail.");
            }
            catch
            {                
                return new(null, 500, $"Ocorreu um erro inesperado. Por favor, tente novamente mais tarde");
            }
        }
        #endregion
        
        #region READ
        public async Task<PaginationApi<List<dynamic>>> GetAllAsync(GetAllDTO request, string userId)
        {
            try
            {
                PaginationUtil<User> pagination = new(request.QueryParams);
                ResponseApi<List<dynamic>> users = await userRepository.GetAllAsync(pagination);
                int count = await userRepository.GetCountDocumentsAsync(pagination);
                return new(users.Data, count, pagination.PageNumber, pagination.PageSize);
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
            }
        }
        public async Task<ResponseApi<dynamic?>> GetByIdAggregateAsync(string id)
        {
            try
            {
                ResponseApi<dynamic?> user = await userRepository.GetByIdAggregateAsync(id);
                if(user.Data is null) return new(null, 404, "Usuário não encontrado");
                return new(user.Data);
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
            }
        }
        public async Task<ResponseApi<List<dynamic>>> GetSelectBarberAsync(GetAllDTO request)
        {
            try
            {
                ResponseApi<List<dynamic>> users = await userRepository.GetSelectBarberAsync(new(request.QueryParams));
                return new(users.Data);
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
            }
        }
        #endregion
        
        #region UPDATE
        public async Task<ResponseApi<User?>> ValidatedAccessAsync(string codeAccess)
        {
            try
            {
                ResponseApi<User?> user = await userRepository.ValidatedAccessAsync(codeAccess);
                if(!user.IsSuccess) return new(null, 400, "Código inválido");
                return new(user.Data, 201, "Código de acesso confirmado");
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
            }
        }
        public async Task<ResponseApi<User?>> UpdateAsync(UpdateUserDTO request)
        {
            try
            {
                ResponseApi<User?> user = await userRepository.GetByIdAsync(request.Id);
                if(user.Data is null || Validator.IsEmail(request.Email)) return new(null, 404, "Falha ao atualizar");
                if(!Validator.IsEmail(request.Email)) return new(null, 404, "E-mail inválido.");
                
                user.Data.UpdatedAt = DateTime.UtcNow;
                user.Data.UserName = request.UserName;
                user.Data.Email = request.Email;
                user.Data.Phone = request.Phone;
                user.Data.Name = request.Name;
                user.Data.PermissionProfile = request.PermissionProfile;

                ResponseApi<User?> response = await userRepository.UpdateAsync(user.Data);
                if(!response.IsSuccess) return new(null, 400, "Falha ao atualizar");
                return new(response.Data, 201, "Atualizado com sucesso");
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
            }
        }
        public async Task<ResponseApi<User?>> UpdateModuleAsync(UpdateUserDTO request)
        {
            try
            {
                ResponseApi<User?> user = await userRepository.GetByIdAsync(request.Id);
                if(user.Data is null) return new(null, 404, "Falha ao atualizar");
                
                user.Data.UpdatedAt = DateTime.UtcNow;
                List<api_slim.src.Models.Module> modules = [];
                foreach (var module in request.Modules)
                {
                    List<api_slim.src.Models.Routine> routines = [];

                    foreach (var routine in module.Routines)
                    {
                        routines.Add(new () 
                        {
                            Code = routine.Code,
                            Description = routine.Description,
                            Permissions = new ()
                            {
                                Create = routine.Permissions.Create,
                                Read = routine.Permissions.Read,
                                Update = routine.Permissions.Update,
                                Delete = routine.Permissions.Delete
                            }
                        });
                    }
                    
                    modules.Add(new () 
                    {
                        Code = module.Code,
                        Description = module.Description,
                        Routines = routines
                    });
                };

                user.Data.Modules = modules;
                user.Data.Name = request.Name;
                user.Data.Email = request.Email;
                user.Data.Blocked = request.Blocked;
                user.Data.Admin = request.Admin;
                user.Data.PermissionProfile = request.PermissionProfile;

                ResponseApi<User?> response = await userRepository.UpdateAsync(user.Data);
                if(!response.IsSuccess) return new(null, 400, "Falha ao atualizar");
                return new(response.Data, 201, "Atualizado com sucesso");
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
            }
        }
        public async Task<ResponseApi<User?>> ResendCodeAccessAsync(UpdateUserDTO request)
        {
            try
            {
                if(string.IsNullOrEmpty(request.Email) && string.IsNullOrEmpty(request.Phone)) return new(null, 400, "E-mail ou celular é obrigatório.");                    

                User? user = new();

                dynamic access = Util.GenerateCodeAccess();
                string messageCode = $"Seu código de verificação é: {access.CodeAccess}";
                
                if (!string.IsNullOrEmpty(request.Email))
                {
                    ResponseApi<User?> isEmail = await userRepository.GetByEmailAsync(request.Email);
                    if(isEmail.Data is null && !Validator.IsEmail(request.Email)) return new(null, 400, "E-mail inválido.");                    
                    user = isEmail.Data;
                    await mailHandler.SendMailAsync(request.Email, "Código de verificação", messageCode);
                };

                if (!string.IsNullOrEmpty(request.Phone))
                {
                    ResponseApi<User?> isPhone = await userRepository.GetByPhoneAsync(request.Phone);
                    if(isPhone.Data is null) return new(null, 400, "Celular inválido.");
                    user = isPhone.Data;
                    await smsHandler.SendMessageAsync(request.Phone, messageCode);
                };
                                             
                if(user is null) return new(null, 400, "Falha ao reenviar código de acesso");

                user.UpdatedAt = DateTime.UtcNow;
                user.CodeAccess = access.CodeAccess;
                user.CodeAccessExpiration = access.CodeAccessExpiration;
                user.ValidatedAccess = false;

                ResponseApi<User?> response = await userRepository.UpdateAsync(user);
                if(!response.IsSuccess) return new(null, 400, "Falha ao reenviar código de acesso");
                return new(response.Data, 201, "Novo código de acesso enviado");
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
            }
        }
        public async Task<ResponseApi<User?>> SavePhotoProfileAsync(SaveUserPhotoDTO request)
        {
            try
            {
                if (request.Photo == null || request.Photo.Length == 0) return new(null, 400, "Falha ao salvar foto de perfil");

                ResponseApi<User?> user = await userRepository.GetByIdAsync(request.Id);
                if(user.Data is null) return new(null, 404, "Falha ao salvar foto de perfil");

                var tempPath = Path.GetTempFileName();

                using (var stream = new FileStream(tempPath, FileMode.Create))
                {
                    request.Photo.CopyTo(stream);
                }

                string uriPhoto = await cloudinaryHandler.UploadAttachment("user", request.Photo);
                user.Data.UpdatedAt = DateTime.UtcNow;
                user.Data.Photo = uriPhoto;

                ResponseApi<User?> response = await userRepository.UpdateAsync(user.Data);
                if(!response.IsSuccess) return new(null, 400, "Falha ao salvar foto de perfil");
                return new(response.Data, 201, "Foto de perfil salva com sucesso");
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
            }
        }
        public async Task<ResponseApi<User?>> RemovePhotoProfileAsync(string id)
        {
            try
            {
                ResponseApi<User?> user = await userRepository.GetByIdAsync(id);
                if(user.Data is null) return new(null, 404, "Falha ao remover foto de perfil");
                string photo = user.Data.Photo.Split("/").Last();
                string publicId = photo.Split(".")[0];

                bool isRemoved = await cloudinaryHandler.Delete(publicId, "api-barber", "users");
                if(!isRemoved) return new(null, 400, "Falha ao remover foto de perfil");
                user.Data.UpdatedAt = DateTime.UtcNow;
                user.Data.Photo = "";

                ResponseApi<User?> response = await userRepository.UpdateAsync(user.Data);
                if(!response.IsSuccess) return new(null, 400, "Falha ao remover foto de perfil");
                return new(response.Data, 201, "Foto de perfil removida com sucesso");
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
            }
        }
        #endregion
        
        #region DELETE
        public async Task<ResponseApi<User>> DeleteAsync(string userId)
        {
            try
            {
                ResponseApi<User> user = await userRepository.DeleteAsync(userId);
                if(!user.IsSuccess) return new(null, 400, user.Message);
                return user;
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
            }
        }
        #endregion        
    }
}