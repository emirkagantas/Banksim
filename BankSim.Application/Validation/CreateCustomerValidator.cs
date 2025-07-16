using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using BankSim.Application.DTOs;

namespace BankSim.Application.Validation
{
    public class CreateCustomerValidator : AbstractValidator<CreateCustomerDto>
    {
        public CreateCustomerValidator()
        {
            RuleFor(x => x.FullName)
                .NotEmpty().WithMessage("Ad soyad alanı zorunludur.")
                .MinimumLength(3).WithMessage("Ad soyad en az 3 karakter olmalı.");

            RuleFor(x => x.IdentityNumber)
                .NotEmpty().WithMessage("TC kimlik numarası zorunludur.")
                .Length(11).WithMessage("TC kimlik numarası 11 haneli olmalı.")
                .Matches(@"^\d{11}$").WithMessage("TC kimlik numarası yalnızca rakamlardan oluşmalı.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("E-posta adresi zorunludur.")
                .EmailAddress().WithMessage("Geçerli bir e-posta adresi giriniz.");

            RuleFor(x => x.Phone)
                .NotEmpty().WithMessage("Telefon numarası zorunludur.")
                .Matches(@"^05\d{9}$").WithMessage("Telefon numarası 05 ile başlamalı ve 11 haneli olmalı.");


        }
    }

}