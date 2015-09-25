#region FileInfo

// 
// File: ILSLExpressionListNode.cs
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

using System.Collections.Generic;
using LibLSLCC.CodeValidator.Primitives;
using LibLSLCC.CodeValidator.ValidatorNodes.ExpressionNodes;

#endregion

namespace LibLSLCC.CodeValidator.ValidatorNodes.Interfaces
{
    public interface ILSLExpressionListNode : ILSLSyntaxTreeNode
    {
        LSLExpressionListType ListType { get; }
        IReadOnlyList<ILSLReadOnlyExprNode> ExpressionNodes { get; }
        IReadOnlyList<LSLSourceCodeRange> CommaSourceCodeRanges { get; }
        bool AllExpressionsConstant { get; }
        bool HasExpressionNodes { get; }
    }
}