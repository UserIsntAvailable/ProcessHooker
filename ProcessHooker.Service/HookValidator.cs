using System.IO;
using FluentValidation;

namespace ProcessHooker.Service {
    public class HookValidator : AbstractValidator<Hook> {

        public HookValidator() {
            this.RuleFor(hook => hook.HookedProcessName)
                .NotEmpty()
                .NotNull();

            this.RuleFor(hook => hook.FilePath)
                .NotEmpty()
                .NotNull()
                .Must(File.Exists)
                .WithMessage("{PropertyName} ( {PropertyValue} ) doesn't exists.");
        }
    }
}
