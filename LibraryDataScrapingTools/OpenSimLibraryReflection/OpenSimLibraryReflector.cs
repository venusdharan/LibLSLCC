﻿#region FileInfo
// File: OpenSimLibraryReflector.cs
// 
// Last Compile: 25/09/2015 @ 5:43 AM
// 
// Creation Date: 21/08/2015 @ 12:22 AM
// 
// This file is part of LibLSLCC.
// 
// LibLSLCC is distributed under the following BSD 3-Clause License
// 
// Copyright (c) 2015, Eric A. Blundell
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:
// 
// 1. Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
// 
// 2. Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.
// 
// 3. Neither the name of the copyright holder nor the names of its contributors may be used to endorse or promote products derived from this software without specific prior written permission.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
// LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT
// HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT
// LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON
// ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
// OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
// 
#endregion
#region Imports

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using LibLSLCC.CodeValidator.Enums;

#endregion

namespace LibraryDataScrapingTools.OpenSimLibraryReflection
{
    public interface IReflectedLibraryData
    {
        /// <summary>
        ///     The base class of the LSL script in generated code
        /// </summary>
        Type ScriptBaseClass { get; }

        /// <summary>
        ///     Interfaces that contain function signatures representing LSL library calls
        ///     all public interface methods will be reflected
        /// </summary>
        IEnumerable<Type> FunctionContainingInterfaces { get; }

        /// <summary>
        ///     Classes with attributed functions and constants.
        ///     if attribute is equal to ScriptModuleFunctionAttribute, then
        ///     the marked method will be visible to the reflector as a public LSL library function.
        ///     if the attribute is equal to ScriptModuleConstantAttribute, then the field is a public LSL
        ///     constant
        /// </summary>
        IEnumerable<Type> AttributedModuleClasses { get; }

        /// <summary>
        ///     Classes in which all public static fields are to be considered public LSL constants
        ///     regardless of attribute notation
        /// </summary>
        IEnumerable<Type> ScriptConstantContainerClasses { get; }

        /// <summary>
        ///     The type of attribute used to mark methods as public lsl functions inside
        ///     AttributedModuleClasses
        /// </summary>
        Type ScriptModuleFunctionAttribute { get; }

        /// <summary>
        ///     The type of attribute used to mark methods as public lsl constants inside
        ///     AttributedModuleClasses
        /// </summary>
        Type ScriptModuleConstantAttribute { get; }

        /// <summary>
        ///     Runtime type for floats inside generated code
        /// </summary>
        Type RuntimeFloat { get; }

        /// <summary>
        ///     Runtime type for integers inside generated code
        /// </summary>
        Type RuntimeInteger { get; }

        /// <summary>
        ///     Runtime type for strings inside generated code
        /// </summary>
        Type RuntimeString { get; }

        /// <summary>
        ///     Runtime type for lists inside generated code
        /// </summary>
        Type RuntimeList { get; }

        /// <summary>
        ///     Runtime type for vectors inside generated code
        /// </summary>
        Type RuntimeVector { get; }

        /// <summary>
        ///     Runtime type for rotations inside generated code
        /// </summary>
        Type RuntimeQuaternion { get; }

        /// <summary>
        ///     Runtime type for keys inside generated code
        /// </summary>
        Type RuntimeKey { get; }

        /// <summary>
        ///     Get the actual type of a function containing interface by its short class name
        /// </summary>
        /// <param name="name">class name</param>
        /// <returns>type</returns>
        Type GetFunctionContainingInterface(string name);

        /// <summary>
        ///     Get the actual type of a function/constant containing attributed module class by its short class name
        /// </summary>
        /// <param name="name">class name</param>
        /// <returns></returns>
        Type GetAttributedModuleClass(string name);

        /// <summary>
        ///     Get the actual type of a constant containing class by its short class name
        /// </summary>
        /// <param name="name">class name</param>
        /// <returns>type</returns>
        Type GetScriptConstantContainer(string name);

        /// <summary>
        ///     Maps native return types/parameters for functions, and field types for constants
        ///     to an LSLType
        /// </summary>
        /// <returns>The LSLType that can represent the native type</returns>
        IReadOnlyDictionary<Type, LSLType> OpenSimToLSLTypeMapping();
    }

    public class OpenSimLibraryReflectedTypeData : IReflectedLibraryData
    {
        private readonly Dictionary<AssemblyName, Assembly> _loaded = new Dictionary<AssemblyName, Assembly>();
        private readonly string _openSimBinDirectory;

        public OpenSimLibraryReflectedTypeData(string openSimBinDirectory)
        {
            _openSimBinDirectory = openSimBinDirectory;

            OptionalScriptModulesAssembly = Assembly.LoadFrom(Path.Combine(openSimBinDirectory,
                "OpenSim.Region.OptionalModules.dll"));

            ScriptApiAssembly = Assembly.LoadFrom(Path.Combine(openSimBinDirectory,
                "OpenSim.Region.ScriptEngine.Shared.dll"));


            ScriptRuntimeAssembly =
                Assembly.LoadFile(Path.Combine(openSimBinDirectory,
                    "OpenSim.Region.ScriptEngine.Shared.Api.Runtime.dll"));

            RegionFrameworkAssembly =
                Assembly.LoadFile(Path.Combine(openSimBinDirectory,
                    "OpenSim.Region.Framework.dll"));


            ScriptModuleFunctionAttribute =
                RegionFrameworkAssembly.GetType("OpenSim.Region.Framework.Interfaces.ScriptInvocationAttribute");

            ScriptModuleConstantAttribute =
                RegionFrameworkAssembly.GetType("OpenSim.Region.Framework.Interfaces.ScriptConstantAttribute");

            AppDomain.CurrentDomain.AssemblyResolve += _currentDomainOnAssemblyResolve;

            AttributedModuleClasses = OptionalScriptModulesAssembly.GetTypes()
                .Where(x => x.GetInterfaces().Any(y => y.Name == "INonSharedRegionModule"))
                .Where(t => t.GetFields().Any(h => h.GetCustomAttributes(true).Any(
                    x =>
                    {
                        var n = x.GetType().Name;
                        return n == "ScriptConstantAttribute" || n == "ScriptInvocationAttribute"
                            ;
                    }))).ToList();


            FunctionContainingInterfaces =
                ScriptApiAssembly.GetTypes()
                    .Where(x => x.IsInterface && x.Namespace == "OpenSim.Region.ScriptEngine.Shared.Api.Interfaces")
                    .ToList();


            ScriptBaseClass =
                ScriptRuntimeAssembly.GetType("OpenSim.Region.ScriptEngine.Shared.ScriptBase.ScriptBaseClass");

            ScriptConstantContainerClasses = new List<Type> {ScriptBaseClass};

            AppDomain.CurrentDomain.AssemblyResolve -= _currentDomainOnAssemblyResolve;
        }

        /// <summary>
        ///     OpenSim.Region.ScriptEngine.Shared.Api.Runtime.dll
        /// </summary>
        public Assembly ScriptRuntimeAssembly { get; }

        /// <summary>
        ///     OpenSim.Region.ScriptEngine.Shared.dll
        /// </summary>
        public Assembly ScriptApiAssembly { get; }

        /// <summary>
        ///     OpenSim.Region.OptionalModules.dll
        /// </summary>
        public Assembly OptionalScriptModulesAssembly { get; }

        /// <summary>
        ///     OpenSim.Region.Framework.dll
        /// </summary>
        public Assembly RegionFrameworkAssembly { get; }

        /// <summary>
        ///     OpenSim.Region.ScriptEngine.Shared.ScriptBase.ScriptBaseClass
        /// </summary>
        public Type ScriptBaseClass { get; }

        /// <summary>
        ///     OpenSim.Region.ScriptEngine.Shared.Api.Interfaces.*
        /// </summary>
        public IEnumerable<Type> FunctionContainingInterfaces { get; }

        public Type GetFunctionContainingInterface(string name)
        {
            return FunctionContainingInterfaces.SingleOrDefault(x => x.Name == name);
        }

        public Type GetAttributedModuleClass(string name)
        {
            return AttributedModuleClasses.SingleOrDefault(x => x.Name == name);
        }

        public Type GetScriptConstantContainer(string name)
        {
            return ScriptConstantContainerClasses.SingleOrDefault(x => x.Name == name);
        }

        /// <summary>
        ///     INonSharedRegionModule that use ScriptConstantAttribute or ScriptInvocationAttribute anywhere in the class
        /// </summary>
        public IEnumerable<Type> AttributedModuleClasses { get; }

        /// <summary>
        ///     ScriptBaseClass
        /// </summary>
        public IEnumerable<Type> ScriptConstantContainerClasses { get; }

        /// <summary>
        ///     OpenSim.Region.Framework.Interfaces.ScriptInvocationAttribute
        /// </summary>
        public Type ScriptModuleFunctionAttribute { get; }

        /// <summary>
        ///     OpenSim.Region.Framework.Interfaces.ScriptConstantAttribute
        /// </summary>
        public Type ScriptModuleConstantAttribute { get; }

        /// <summary>
        ///     OpenSim.Region.ScriptEngine.Shared.LSL_Types.LSLFloat
        /// </summary>
        public Type RuntimeFloat
        {
            get
            {
                var t =
                    ScriptApiAssembly.GetType("OpenSim.Region.ScriptEngine.Shared.LSL_Types")
                        .GetNestedType("LSLFloat");
                return t;
            }
        }

        /// <summary>
        ///     OpenSim.Region.ScriptEngine.Shared.LSL_Types.LSLInteger
        /// </summary>
        public Type RuntimeInteger
        {
            get
            {
                var t =
                    ScriptApiAssembly.GetType("OpenSim.Region.ScriptEngine.Shared.LSL_Types")
                        .GetNestedType("LSLInteger");
                return t;
            }
        }

        /// <summary>
        ///     OpenSim.Region.ScriptEngine.Shared.LSL_Types.LSLString
        /// </summary>
        public Type RuntimeString
        {
            get
            {
                var t =
                    ScriptApiAssembly.GetType("OpenSim.Region.ScriptEngine.Shared.LSL_Types")
                        .GetNestedType("LSLString");
                return t;
            }
        }

        /// <summary>
        ///     OpenSim.Region.ScriptEngine.Shared.LSL_Types.LSLList
        /// </summary>
        public Type RuntimeList
        {
            get
            {
                var t =
                    ScriptApiAssembly.GetType("OpenSim.Region.ScriptEngine.Shared.LSL_Types")
                        .GetNestedType("list");
                return t;
            }
        }

        /// <summary>
        ///     OpenSim.Region.ScriptEngine.Shared.LSL_Types.LSLVector
        /// </summary>
        public Type RuntimeVector
        {
            get
            {
                var t =
                    ScriptApiAssembly.GetType("OpenSim.Region.ScriptEngine.Shared.LSL_Types")
                        .GetNestedType("Vector3");
                return t;
            }
        }

        /// <summary>
        ///     OpenSim.Region.ScriptEngine.Shared.LSL_Types.LSLQuaternion
        /// </summary>
        public Type RuntimeQuaternion
        {
            get
            {
                var t =
                    ScriptApiAssembly.GetType("OpenSim.Region.ScriptEngine.Shared.LSL_Types")
                        .GetNestedType("Quaternion");
                return t;
            }
        }

        /// <summary>
        ///     OpenSim.Region.ScriptEngine.Shared.LSL_Types.key
        /// </summary>
        public Type RuntimeKey
        {
            get
            {
                var t =
                    ScriptApiAssembly.GetType("OpenSim.Region.ScriptEngine.Shared.LSL_Types")
                        .GetNestedType("key");
                return t;
            }
        }

        public IReadOnlyDictionary<Type, LSLType> OpenSimToLSLTypeMapping()
        {
            var r = new Dictionary<Type, LSLType>
            {
                {
                    typeof (void),
                    LSLType.Void
                },

                {
                    typeof (bool),
                    LSLType.Integer
                }
                ,
                {
                    typeof (string),
                    LSLType.String
                }
                ,
                {
                    typeof (int),
                    LSLType.Integer
                }
                ,
                {
                    typeof (long),
                    LSLType.Integer
                }
                ,
                {
                    typeof (float),
                    LSLType.Float
                }
                ,
                {
                    typeof (double),
                    LSLType.Float
                }
                ,
                {
                    RuntimeFloat,
                    LSLType.Float
                }
                ,
                {
                    RuntimeInteger,
                    LSLType.Integer
                }
                ,
                {
                    RuntimeKey,
                    LSLType.Key
                }
                ,
                {
                    RuntimeList,
                    LSLType.List
                }
                ,
                {
                    RuntimeQuaternion,
                    LSLType.Rotation
                }
                ,
                {
                    RuntimeVector,
                    LSLType.Vector
                }
                ,
                {
                    RuntimeString,
                    LSLType.String
                }
            };


            return r;
        }

        private Assembly _currentDomainOnAssemblyResolve(object sender, ResolveEventArgs loadArgs)
        {
            var aname = new AssemblyName(loadArgs.Name);

            if (_loaded.ContainsKey(aname))
            {
                return _loaded[aname];
            }


            if (loadArgs.RequestingAssembly == null)
                return null;


            var path = Path.Combine(_openSimBinDirectory, aname.Name + ".dll");

            if (!File.Exists(path))
                return null;

            var assembly = Assembly.LoadFrom(path);
            _loaded.Add(aname, assembly);
            return assembly;
        }
    }
}