using api_slim.src.Interfaces;
using api_slim.src.Models;
using api_slim.src.Models.Base;
using api_slim.src.Shared.DTOs;
using api_slim.src.Shared.Utils;
using AutoMapper;

namespace api_slim.src.Services
{
    public class SellerRepresentativeService(ISellerRepresentativeRepository sellerRepresentativeRepository, IAddressRepository addressRepository, IMapper _mapper) : ISellerRepresentativeService
{
    #region READ
    public async Task<PaginationApi<List<dynamic>>> GetAllAsync(GetAllDTO request)
    {
        try
        {
            PaginationUtil<SellerRepresentative> pagination = new(request.QueryParams);
            ResponseApi<List<dynamic>> sellerRepresentatives = await sellerRepresentativeRepository.GetAllAsync(pagination);
            int count = await sellerRepresentativeRepository.GetCountDocumentsAsync(pagination);
            return new(sellerRepresentatives.Data, count, pagination.PageNumber, pagination.PageSize);
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
            ResponseApi<dynamic?> sellerRepresentative = await sellerRepresentativeRepository.GetByIdAggregateAsync(id);
            if(sellerRepresentative.Data is null) return new(null, 404, "Item não encontrado");
            return new(sellerRepresentative.Data);
        }
        catch
        {
            return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
        }
    }
    #endregion
    
    #region CREATE
    public async Task<ResponseApi<SellerRepresentative?>> CreateAsync(CreateSellerRepresentativeDTO request)
    {
        try
        {
            SellerRepresentative sellerRepresentative = _mapper.Map<SellerRepresentative>(request);
            sellerRepresentative.Deleted = false;
            sellerRepresentative.DeletedAt = null;
            sellerRepresentative.CreatedAt = DateTime.UtcNow;
            sellerRepresentative.UpdatedAt = DateTime.UtcNow;
            
            ResponseApi<SellerRepresentative?> response = await sellerRepresentativeRepository.CreateAsync(sellerRepresentative);

            if(response.Data is null) return new(null, 400, "Falha ao criar Representante.");

            Address address = _mapper.Map<Address>(request.Address);
            address.Parent = "seller-representative";
            address.ParentId = response.Data!.Id;
            ResponseApi<Address?> addressResponse = await addressRepository.CreateAsync(address);
            if(!addressResponse.IsSuccess) return new(null, 400, "Falha ao criar Representante.");

            return new(response.Data, 201, "Representante criado com sucesso.");
        }
        catch
        { 
            return new(null, 500, $"Ocorreu um erro inesperado. Por favor, tente novamente mais tarde");
        }
    }
    #endregion
    
    #region UPDATE
    public async Task<ResponseApi<SellerRepresentative?>> UpdateAsync(UpdateSellerRepresentativeDTO request)
    {
        try
        {
            ResponseApi<SellerRepresentative?> sellerRepresentativeResponse = await sellerRepresentativeRepository.GetByIdAsync(request.Id);
            if(sellerRepresentativeResponse.Data is null) return new(null, 404, "Falha ao atualizar");
            
            SellerRepresentative sellerRepresentative = _mapper.Map<SellerRepresentative>(request);
            sellerRepresentative.UpdatedAt = DateTime.UtcNow;

            ResponseApi<SellerRepresentative?> response = await sellerRepresentativeRepository.UpdateAsync(sellerRepresentative);
            if(!response.IsSuccess) return new(null, 400, "Falha ao atualizar");
            
            if(!string.IsNullOrEmpty(request.Address.Id))
            {
                ResponseApi<Address?> findAddress = await addressRepository.GetByParentIdAsync(request.Address.ParentId, request.Address.Parent);
                if(!findAddress.IsSuccess || findAddress.Data is null) return new(null, 400, "Falha ao atualizar.");
                
                request.Address.Id = findAddress.Data.Id;
            
                ResponseApi<Address?> addressResponse = await addressRepository.UpdateAsync(request.Address);
                if(!addressResponse.IsSuccess) return new(null, 400, "Falha ao atualizar.");
            }
            else
            {
                Address address = _mapper.Map<Address>(request.Address);
                address.Parent = "seller-representative";
                address.ParentId = response.Data!.Id;
                ResponseApi<Address?> addressResponse = await addressRepository.CreateAsync(address);
                if(!addressResponse.IsSuccess) return new(null, 400, "Falha ao criar Item.");
            };            

            return new(response.Data, 200, "Atualizado com sucesso");
        }
        catch
        {
            return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
        }
    }
    public async Task<ResponseApi<SellerRepresentative?>> UpdateResponsibleAsync(UpdateSellerRepresentativeDTO request)
    {
        try
        {
            ResponseApi<SellerRepresentative?> sellerRepresentativeResponse = await sellerRepresentativeRepository.GetByIdAsync(request.Id);
            if(sellerRepresentativeResponse.Data is null) return new(null, 404, "Falha ao atualizar");
            
            sellerRepresentativeResponse.Data.Responsible = new ()
            {
                DateOfBirth = request.Responsible.DateOfBirth,
                Gender = request.Responsible.Gender,
                Name = request.Responsible.Name,
                Cpf = request.Responsible.Cpf,
                Rg = request.Responsible.Rg,
                Phone = request.Responsible.Phone,
                Whatsapp = request.Responsible.Whatsapp,
                Notes = request.Responsible.Notes,
                Email = request.Responsible.Email,
            };

            sellerRepresentativeResponse.Data.UpdatedAt = DateTime.UtcNow;

            ResponseApi<SellerRepresentative?> response = await sellerRepresentativeRepository.UpdateAsync(sellerRepresentativeResponse.Data!);
            if(!response.IsSuccess) return new(null, 400, "Falha ao atualizar");

            if(string.IsNullOrEmpty(request.Responsible.Address.Id))
            {
                Address address = _mapper.Map<Address>(request.Responsible.Address);
                address.Parent = "seller-representative-responsible";
                address.ParentId = request.Id;
                ResponseApi<Address?> addressResponse = await addressRepository.CreateAsync(address);
                if(!addressResponse.IsSuccess) return new(null, 400, "Falha ao criar Item.");
            } 
            else
            {
                ResponseApi<Address?> addressResponse = await addressRepository.UpdateAsync(request.Responsible.Address);
                if(!addressResponse.IsSuccess) return new(null, 400, "Falha ao atualizar.");
            }

            return new(response.Data, 201, "Atualizado com sucesso");
        }
        catch
        {
            return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
        }
    }
    public async Task<ResponseApi<SellerRepresentative?>> UpdateSellerAsync(UpdateSellerRepresentativeDTO request)
    {
        try
        {
            ResponseApi<SellerRepresentative?> sellerRepresentativeResponse = await sellerRepresentativeRepository.GetByIdAsync(request.Id);
            if(sellerRepresentativeResponse.Data is null) return new(null, 404, "Falha ao atualizar");
            
            SellerRepresentative sellerRepresentative = _mapper.Map<SellerRepresentative>(request);
            sellerRepresentative.UpdatedAt = DateTime.UtcNow;

            ResponseApi<SellerRepresentative?> response = await sellerRepresentativeRepository.UpdateAsync(sellerRepresentative);
            if(!response.IsSuccess) return new(null, 400, "Falha ao atualizar");

            if(string.IsNullOrEmpty(request.Address.Id))
            {
                Address address = _mapper.Map<Address>(request.Address);
                address.Parent = "seller-representative-seller";
                address.ParentId = response.Data!.Id;
                ResponseApi<Address?> addressResponse = await addressRepository.CreateAsync(address);
                if(!addressResponse.IsSuccess) return new(null, 400, "Falha ao criar Item.");
            } 
            else
            {
                ResponseApi<Address?> addressResponse = await addressRepository.UpdateAsync(request.Address);
                if(!addressResponse.IsSuccess) return new(null, 400, "Falha ao atualizar.");
            }

            return new(response.Data, 201, "Atualizado com sucesso");
        }
        catch
        {
            return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
        }
    }
    #endregion
    
    #region DELETE
    public async Task<ResponseApi<SellerRepresentative>> DeleteAsync(string id)
    {
        try
        {
            ResponseApi<SellerRepresentative> sellerRepresentative = await sellerRepresentativeRepository.DeleteAsync(id);
            if(!sellerRepresentative.IsSuccess) return new(null, 400, sellerRepresentative.Message);
            return new(null, 204, "Excluído com sucesso");
        }
        catch
        {
            return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
        }
    }
    #endregion 
}
}