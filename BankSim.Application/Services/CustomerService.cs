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

        public CustomerService(ICustomerRepository repository, IMapper mapper, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _mapper = mapper;
            _unitOfWork = unitOfWork;

        }

        public async Task<List<CustomerDto>> GetAllAsync()
        {
            var customers = await _repository.GetAllAsync();
            return _mapper.Map<List<CustomerDto>>(customers);
        }

        public async Task<CustomerDto?> GetByIdAsync(int id)
        {
            var customer = await _repository.GetByIdAsync(id);
            return _mapper.Map<CustomerDto?>(customer);
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
        }

        public async Task DeleteAsync(int id)
        {
            var customer = await _repository.GetByIdAsync(id);
            if (customer == null) return;
            await _repository.DeleteAsync(customer);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
