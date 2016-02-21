using System.Web.Mvc;

namespace Profiling2.Web.Mvc.Controllers.ModelBinders
{
    /*
     * Added via Global.asax via
     * ModelBinders.Binders.Add(typeof(string),new TrimModelBinder());
     * 
     * Trims all strings.
     */
    public class TrimModelBinder : IModelBinder
    {
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            // Obey ValidateInput decorator used in Controller actions.
            // Attribution: http://blogs.taiga.nl/martijn/2011/09/29/custom-model-binders-and-request-validation/
            bool shouldPerformRequestValidation = controllerContext.Controller.ValidateRequest && bindingContext.ModelMetadata.RequestValidationEnabled;
            var valueResult = bindingContext.GetValueFromValueProvider(shouldPerformRequestValidation);

            //ValueProviderResult valueResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            if (valueResult == null || string.IsNullOrEmpty(valueResult.AttemptedValue))
                return null;
            
            // Perform trim.
            string value = valueResult.AttemptedValue.Trim();

            // Check for single comma value - some old browser version does this, couldn't figure out which.
            if (string.Equals(value, ","))
                return null;

            return value;
        }
    }
}