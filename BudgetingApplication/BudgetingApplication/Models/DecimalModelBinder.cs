﻿using System;
using System.Globalization;
using System.Web.Mvc;

public class DecimalModelBinder : IModelBinder
{
    public object BindModel(ControllerContext controllerContext,
                                    ModelBindingContext bindingContext)
    {
        ValueProviderResult valueResult = bindingContext.ValueProvider
            .GetValue(bindingContext.ModelName);
        ModelState modelState = new ModelState { Value = valueResult };
        object actualValue = null;
        try
        {
            string value = valueResult.AttemptedValue.Replace(",", "");
            if(value == "") { value = "0"; }
            actualValue = Convert.ToDecimal(value,
                CultureInfo.CurrentCulture);
        }
        catch (FormatException e)
        {
            modelState.Errors.Add(e);
        }

        bindingContext.ModelState.Add(bindingContext.ModelName, modelState);
        return actualValue;
    }
}