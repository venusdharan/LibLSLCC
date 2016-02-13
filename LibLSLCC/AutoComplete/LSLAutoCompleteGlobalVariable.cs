#region FileInfo
// 
// File: LSLAutoCompleteGlobalVariable.cs
// 
// 
// ============================================================
// ============================================================
// 
// 
// Copyright (c) 2015, Eric A. Blundell
// 
// All rights reserved.
// 
// 
// This file is part of LibLSLCC.
// 
// LibLSLCC is distributed under the following BSD 3-Clause License
// 
// 
// Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:
// 
// 1. Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
// 
// 2. Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer
//     in the documentation and/or other materials provided with the distribution.
// 
// 3. Neither the name of the copyright holder nor the names of its contributors may be used to endorse or promote products derived
//     from this software without specific prior written permission.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
// LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT
// HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT
// LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON
// ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
// OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
// 
// 
// ============================================================
// ============================================================
// 
// 
#endregion
using LibLSLCC.CodeValidator.Primitives;

namespace LibLSLCC.AutoComplete
{
    /// <summary>
    /// Represents a global variable parsed by the auto complete parser
    /// </summary>
    public class LSLAutoCompleteGlobalVariable
    {
        internal LSLAutoCompleteGlobalVariable(string name, string typeString, LSLSourceCodeRange range, LSLSourceCodeRange typeRange,
            LSLSourceCodeRange nameRange)
        {
            Name = name;
            TypeString = typeString;
            SourceCodeRange = range;

            NameSourceCodeRange = nameRange;

            TypeSourceCodeRange = typeRange;
        }

        /// <summary>
        /// Gets the name of the global variable.
        /// </summary>
        /// <value>
        /// The name of the global variable.
        /// </value>
        public string Name { get; private set; }

        /// <summary>
        /// Gets a string representing the type that the global variable was defined as.
        /// </summary>
        /// <value>
        /// The type string representing the type the global variable was defined as.
        /// </value>
        public string TypeString { get; private set; }


        /// <summary>
        /// Get the <see cref="LSLSourceCodeRange"/> that encompasses the entire variable declaration.
        /// </summary>
        public LSLSourceCodeRange SourceCodeRange { get; private set; }

        /// <summary>
        /// Gets the <see cref="LSLSourceCodeRange"/> of the global variable name.
        /// </summary>
        /// <value>
        /// The <see cref="LSLSourceCodeRange"/> of the global variable name.
        /// </value>
        public LSLSourceCodeRange NameSourceCodeRange { get; private set; }


        /// <summary>
        /// Gets the <see cref="LSLSourceCodeRange"/> of the global variable type specifier.
        /// </summary>
        /// <value>
        /// The <see cref="LSLSourceCodeRange"/> of the global variable type specifier.
        /// </value>
        public LSLSourceCodeRange TypeSourceCodeRange { get; private set; }
    }
}