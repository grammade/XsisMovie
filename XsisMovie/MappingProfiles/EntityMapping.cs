﻿using AutoMapper;
using System.Reflection;
using XsisMovie.Common.Interfaces;

namespace XsisMovie.MappingProfiles;

public class EntityMapping : Profile {
    public EntityMapping() {
        ApplyMappingsFromAssembly(Assembly.GetExecutingAssembly());
    }

    private void ApplyMappingsFromAssembly(Assembly assembly) {
        var types = assembly.GetExportedTypes()
        .Where(t => t.GetInterfaces()
        .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IMapFrom<>)))
        .ToList();

        foreach (var type in types) {
            var instance = Activator.CreateInstance(type);

            var methodInfo = type.GetMethod("Mapping")
                ?? type.GetInterface("IMapFrom`1").GetMethod("Mapping");

            methodInfo?.Invoke(instance, new object[] { this });

        }
    }
}
