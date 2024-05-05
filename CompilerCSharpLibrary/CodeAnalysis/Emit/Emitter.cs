using System.Collections.Generic;
using CompilerCSharpLibrary.CodeAnalysis.Binding;
using CompilerCSharpLibrary.CodeAnalysis.Symbols;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace CompilerCSharpLibrary.CodeAnalysis.Emit
{
    public static class Emitter
    {
        private static readonly DiagnosticBag _diagnostics = new DiagnosticBag();
        private static readonly List<AssemblyDefinition> _assemblies = new List<AssemblyDefinition>();
        private static readonly Dictionary<TypeSymbol, TypeReference> _knownTypes = new Dictionary<TypeSymbol, TypeReference>();

        internal static DiagnosticBag Emit(BoundProgram program, string moduleName, string[] references, string outputPath)
        {
            if (program.Diagnostics.Any())
                return program.Diagnostics;

            var assemlies = new List<AssemblyDefinition>();

            var result = new DiagnosticBag();

            foreach (var reference in references)
            {
                try
                {
                    var assembly = AssemblyDefinition.ReadAssembly(reference);
                    assemlies.Add(assembly);
                }
                catch (BadImageFormatException)
                {
                    result.ReportInvalidReference(reference);
                }
            }

            //Resolve types
            //Any    -> System.Object
            //Bool   -> System.Boolean
            //Int    -> System.Int32
            //String -> System.String
            //Void   -> System.Void

            var builtInTypes = new List<(TypeSymbol type, string MetadataName)>(){
                (TypeSymbol.Any, "System.Object"),
                (TypeSymbol.Bool, "System.Boolean"),
                (TypeSymbol.Int, "System.Int32"),
                (TypeSymbol.String, "System.String"),
                (TypeSymbol.Void, "System.Void"),
            };

            var assemblyName = new AssemblyNameDefinition(moduleName, new Version(1, 0));
            var assemblyDefinition = AssemblyDefinition.CreateAssembly(assemblyName, moduleName, ModuleKind.Console);
            var knownTypes = new Dictionary<TypeSymbol, TypeReference>();

            foreach (var (typeSymbol, metadataName) in builtInTypes)
            {
                var typeReference = ResolveType(typeSymbol.Name, metadataName);
                knownTypes.Add(typeSymbol, typeReference);
            }

            TypeReference ResolveType(string typeName, string metadataName)
            {
                var foundTypes = assemlies.SelectMany(a => a.Modules).SelectMany(m => m.Types).Where(t => t.FullName == metadataName).ToArray();
                if (foundTypes.Length == 1)
                {
                    var typeReference = assemblyDefinition.MainModule.ImportReference(foundTypes[0]);
                    return typeReference;
                }
                else if (foundTypes.Length == 0)
                {
                    result.ReportRequiredTypeNotFound(typeName, metadataName);
                }
                else
                {
                    result.ReportRequiredTypeAmbiguous(typeName, metadataName, foundTypes);
                }

                return null;
            }
            
            MethodReference ResolveMethod(string typeName, string methodName, string[] parameterTypeNames)
            {
                var foundTypes = assemlies.SelectMany(a => a.Modules).SelectMany(m => m.Types).Where(t => t.FullName == typeName).ToArray();
                if (foundTypes.Length == 1)
                {
                    var foundType = foundTypes[0];
                    var methods = foundType.Methods.Where(m => m.Name == methodName);

                    foreach (var method in methods)
                    {
                        if (method.Parameters.Count != parameterTypeNames.Length)
                            continue;
                        
                        var allParametersMatch = true;

                        for (int i = 0; i < parameterTypeNames.Length; i++)
                        {
                            if (method.Parameters[i].ParameterType.FullName != parameterTypeNames[i])
                            {
                                allParametersMatch = false;
                                break;
                            }
                        }

                        if (!allParametersMatch)
                            continue;

                        return assemblyDefinition.MainModule.ImportReference(method);
                    }

                    result.ReportRequiredMethodNotFound(typeName, methodName, parameterTypeNames);
                    return null;
                }
                else if (foundTypes.Length == 0)
                {
                    result.ReportRequiredTypeNotFound(null, typeName);
                }
                else
                {
                    result.ReportRequiredTypeAmbiguous(null, typeName, foundTypes);
                }

                return null;
            }

            var consoleWriteLineReference = ResolveMethod("System.Console", "WriteLine", new string[] { "System.String" });

            if (result.Any())
                return result;

            /*
            static class Program {
                void Main() {
                    System.Console.WriteLine("Hello world!");
                }
            }
            */

            var objectType = knownTypes[TypeSymbol.Any];

            var typeDefinition = new TypeDefinition("", "Program", TypeAttributes.Abstract | TypeAttributes.Sealed | TypeAttributes.Public, objectType);
            assemblyDefinition.MainModule.Types.Add(typeDefinition);

            var voidType = knownTypes[TypeSymbol.Void];
            var mainMethod = new MethodDefinition("Main", MethodAttributes.Static | MethodAttributes.Private, voidType);
            typeDefinition.Methods.Add(mainMethod);

            var ilProcessor = mainMethod.Body.GetILProcessor();
            ilProcessor.Emit(OpCodes.Ldstr, "Hello world");
            ilProcessor.Emit(OpCodes.Call, consoleWriteLineReference);
            ilProcessor.Emit(OpCodes.Ret);

            assemblyDefinition.EntryPoint = mainMethod;

            assemblyDefinition.Write(outputPath);

            return result;
        }
    }
}