using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Namotion.Reflection;
using NSwag;
using NSwag.Generation.AspNetCore;
using NSwag.Generation.Processors;
using NSwag.Generation.Processors.Contexts;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Infrastructure.OpenApi;

public class SwaggerGlobalAuthProcessor(string scheme) : IOperationProcessor
{
    private readonly string _scheme = scheme;

    public SwaggerGlobalAuthProcessor()
        : this(JwtBearerDefaults.AuthenticationScheme) { }

    /// <summary>
    /// Processes the specified operation context to determine security requirements and allow anonymous access as
    /// appropriate.
    /// </summary>
    /// <remarks>If the endpoint metadata includes an AllowAnonymousAttribute, the operation is considered to
    /// allow anonymous access. If no security requirements are present, a default security requirement is added to the
    /// operation. The method always returns true, indicating the operation is processed successfully.</remarks>
    /// <param name="context">The operation context containing metadata and operation details to be processed. Cannot be null.</param>
    /// <returns>true if the operation is allowed to proceed; otherwise, false.</returns>
    public bool Process(OperationProcessorContext context)
    {
        IList<object> list = ((AspNetCoreOperationProcessorContext)context)
            .ApiDescription.ActionDescriptor.TryGetPropertyValue<IList<object>>("EndpointMetadata");

        if (list is not null)
        {
            if (list.OfType<AllowAnonymousAttribute>().Any())
            {
                return true;
            }

            if (context.OperationDescription.Operation.Security.Count == 0)
            {
                (context.OperationDescription.Operation.Security ??= new List<OpenApiSecurityRequirement>())
                    .Add(new OpenApiSecurityRequirement
                    {
                        { 
                            _scheme, 
                            Array.Empty<string>() 
                        }
                    });
            }
        }

        return true;
    }
}

public static class ObjectExtensions
{
    public static T TryGetPropertyValue<T>(this object obj, string propertyName, T defaultValue = default) =>
        obj.GetType().GetRuntimeProperty(propertyName) is PropertyInfo propertyInfo
            ? (T)propertyInfo.GetValue(obj)
            : defaultValue;
}