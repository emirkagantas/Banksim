using BankSim.Application.DTOs;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankSim.Application.Validation
{
    public class RegisterValidator : AbstractValidator<RegisterDto>
    {
        public RegisterValidator()
        {
            RuleFor(x => x.FullName)
                .NotEmpty().WithMessage("Ad soyad boş olamaz.")
                .MinimumLength(3).WithMessage("En az 3 karakter olmalı.");

            RuleFor(x => x.IdentityNumber)
                .NotEmpty().WithMessage("TC Kimlik numarası boş olamaz.")
                .Length(11).WithMessage("TC Kimlik 11 haneli olmalı.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email boş olamaz.")
                .EmailAddress().WithMessage("Geçerli bir email girin.");

            RuleFor(x => x.Phone)
                .NotEmpty().WithMessage("Telefon numarası boş olamaz.")
                .Matches(@"^05\d{9}$").WithMessage("Telefon 05 ile başlamalı ve 11 haneli olmalı.");
        }
    }
}