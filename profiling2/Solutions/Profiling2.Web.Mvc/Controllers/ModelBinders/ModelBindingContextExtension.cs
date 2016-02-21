using System.Web.Mvc;

namespace Profiling2.Web.Mvc.Controllers.ModelBinders
{
    // Extension method for ModelBindingContext according to http://blogs.taiga.nl/martijn/2011/09/29/custom-model-binders-and-request-validation/
    public static class ModelBindingContextExtension
    {
        public static ValueProviderResult GetValueFromValueProvider(this ModelBindingContext bindingContext, bool performRequestValidation)
        {
            var unvalidatedValueProvider = bindingContext.ValueProvider as IUnvalidatedValueProvider;
            return (unvalidatedValueProvider != null)
                       ? unvalidatedValueProvider.GetValue(bindingContext.ModelName, !performRequestValidation)
                       : bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
        }
    }
}