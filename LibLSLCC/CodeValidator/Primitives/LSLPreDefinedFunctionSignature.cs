#region FileInfo

// 
// File: LSLPreDefinedFunctionSignature.cs
// 
// Author/Copyright:  Eric A. Blundell
// 
// Last Compile: 24/09/2015 @ 9:24 PM
// 
// Creation Date: 21/08/2015 @ 12:22 AM
// 
// 
// This file is part of LibLSLCC.
// LibLSLCC is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// LibLSLCC is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// You should have received a copy of the GNU General Public License
// along with LibLSLCC.  If not, see <http://www.gnu.org/licenses/>.
// 

#endregion

#region Imports

using System.Linq;
using LibLSLCC.CodeValidator.Enums;
using LibLSLCC.CodeValidator.ValidatorNodes.ScopeNodes;
using LibLSLCC.CodeValidator.ValidatorNodes.StatementNodes;

#endregion

namespace LibLSLCC.CodeValidator.Primitives
{
    public class LSLPreDefinedFunctionSignature : LSLFunctionSignature
    {
        public LSLPreDefinedFunctionSignature(LSLType returnType, string name, LSLParameterListNode parameters)
            : base(returnType, name, parameters.Parameters.Select(x => new LSLParameter(x.Type, x.Name, false)))
        {
            ParameterListNode = parameters;
        }

        public LSLParameterListNode ParameterListNode { get; private set; }
        internal LSLParser.FunctionDeclarationContext DeclarationContext { get; set; }
        public LSLFunctionDeclarationNode DefinitionNode { get; private set; }

        public void GiveDefinition(LSLFunctionDeclarationNode definition)
        {
            DefinitionNode = definition;
        }
    }
}