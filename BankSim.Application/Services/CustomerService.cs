using AutoMapper;
using BankSim.Application.DTOs;
using BankSim.Domain.Entities;
using BankSim.Domain.Interfaces;

namespace BankSim.Application.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _repository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRedisCacheService _cacheService;

        public CustomerService(ICustomerRepository repository, IMapper mapper, IUnitOfWork unitOfWork, IRedisCacheService cacheService)
        {
            _repository = repository;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _cacheService = cacheService;
        }

        public async Task<List<CustomerDto>> GetAllAsync()
        {
            var customers = await _repository.GetAllAsync();
            return _mapper.Map<List<CustomerDto>>(customers);
        }

        public async Task<CustomerDto?> GetByIdAsync(int id)
        {
            string cacheKey = $"customer:{id}";


            var cached = await _cacheService.GetAsync<CustomerDto>(cacheKey);
            if (cached is not null)
                return cached;


            var customer = await _repository.GetByIdAsync(id);
            if (customer is null) return null;

            var dto = _mapper.Map<CustomerDto>(customer);


            await _cacheService.SetAsync(cacheKey, dto, TimeSpan.FromMinutes(15));

            return dto;
        }

        public async Task AddAsync(CreateCustomerDto dto)
        {
            var entity = _mapper.Map<Customer>(dto);
            await _repository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task UpdateAsync(int id, UpdateCustomerDto dto)
        {
            var customer = await _repository.GetByIdAsync(id);
            if (customer == null) return;
            _mapper.Map(dto, customer);
            await _repository.UpdateAsync(customer);
            await _unitOfWork.SaveChangesAsync();
            await _cacheService.RemoveAsync($"customer:{id}");
        }

        public async Task DeleteAsync(int id)
        {
            var customer = await _repository.GetByIdAsync(id);
            if (customer == null) return;
            await _repository.DeleteAsync(customer);
            await _unitOfWork.SaveChangesAsync();
            await _cacheService.RemoveAsync($"customer:{id}");
        }
    }
}
