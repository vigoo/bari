using System.Diagnostics.Contracts;
using Bari.Core.Model;
using Ninject;
using Ninject.Parameters;
using Ninject.Syntax;

namespace Bari.Core.Build
{    	
    public static class ReferenceBuilderResolver
    {
         /// <summary>
       /// Creates a new builder instance 
       /// </summary>
       /// <typeparam name="T">Builder key to be created</typeparam>
       /// <param name="root">Interface to create new instances</param>
       /// <param name="reference">The reference (type is encoded as the scheme part of the reference URI)</param>
       /// <param name="parameters">Optional parameters for the inner builder</param>
       /// <returns>Returns a cached builder instance</returns>
       public static IBuilder GetReferenceBuilder<T>(this IResolutionRoot root, Reference reference, params IParameter[] parameters)
            where T : class, IReferenceBuilder
       {
           Contract.Requires(root != null);
           Contract.Requires(reference != null);

           var builder = root.TryGet<T>(reference.Uri.Scheme, parameters);
           if (builder != null)
           {
               builder.Reference = reference;

               return builder;
           }
           else
           {
               return null;
           }
       }

    }
}