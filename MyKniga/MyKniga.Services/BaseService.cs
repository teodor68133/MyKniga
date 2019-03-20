namespace MyKniga.Services
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Data;

    public abstract class BaseService
    {
        protected readonly MyKnigaDbContext Context;

        protected BaseService(MyKnigaDbContext context)
        {
            this.Context = context;
        }

        protected bool IsEntityStateValid(object model)
        {
            var validationContext = new ValidationContext(model);
            var validationResults = new List<ValidationResult>();

            return Validator.TryValidateObject(model, validationContext, validationResults,
                validateAllProperties: true);
        }
    }
}