using System;
using System.Collections.Generic;
using System.Linq;
using Bari.Core.Exceptions;

namespace Bari.Core.Model.Validator
{
    public class DefaultSuiteValidator: ISuiteValidator
    {
        public void Validate(Suite suite)
        {
            ModuleAndProductNamesAreUnique(suite);
        }

        private void ModuleAndProductNamesAreUnique(Suite suite)
        {
            var moduleNames = new HashSet<string>(suite.Modules.Select(module => module.Name));
            foreach (var product in suite.Products)
            {
                if (moduleNames.Contains(product.Name))
                {
                    throw new SuiteValidatorException(String.Format("Product name `{0}` is already used as a module name", product.Name));
                }
            }
        }
    }
}