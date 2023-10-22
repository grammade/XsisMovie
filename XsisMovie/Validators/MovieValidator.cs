using FluentValidation;
using XsisMovie.Common.Dtos;
using XsisMovie.Entities;

namespace XsisMovie.Validators {
    public class MovieValidator : AbstractValidator<MovieModifyDto> {
        public MovieValidator() {
            RuleFor(m => m.Title).NotEmpty();
        }
    }
}
