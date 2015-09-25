﻿#region FileInfo

// 
// File: LSLFloatLiteralNode.cs
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

using System.Diagnostics.CodeAnalysis;
using LibLSLCC.CodeValidator.Enums;
using LibLSLCC.CodeValidator.Primitives;
using LibLSLCC.CodeValidator.ValidatorNodes.Interfaces;
using LibLSLCC.CodeValidator.ValidatorNodeVisitor;

#endregion

namespace LibLSLCC.CodeValidator.ValidatorNodes.ExpressionNodes
{
    public class LSLFloatLiteralNode : LSLConstantLiteralNode, ILSLFloatLiteralNode
    {
        // ReSharper disable UnusedParameter.Local
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "err")]
        protected LSLFloatLiteralNode(LSLSourceCodeRange sourceRange, Err err)
            : base(sourceRange, Err.Err)
            // ReSharper restore UnusedParameter.Local
        {
        }

        internal LSLFloatLiteralNode(LSLParser.Expr_AtomContext context)
            : base(context, LSLType.Float)
        {
        }

        ILSLReadOnlySyntaxTreeNode ILSLReadOnlySyntaxTreeNode.Parent
        {
            get { return Parent; }
        }

        public override T AcceptVisitor<T>(ILSLValidatorNodeVisitor<T> visitor)
        {
            return visitor.VisitFloatLiteral(this);
        }

        ILSLReadOnlyExprNode ILSLReadOnlyExprNode.Clone()
        {
            return Clone();
        }

        public static LSLFloatLiteralNode GetError(LSLSourceCodeRange sourceRange)
        {
            return new LSLFloatLiteralNode(sourceRange, Err.Err);
        }

        public override ILSLExprNode Clone()
        {
            if (HasErrors)
            {
                return GetError(SourceCodeRange);
            }

            var x = new LSLFloatLiteralNode(ParserContext)
            {
                HasErrors = HasErrors,
                Parent = Parent
            };

            return x;
        }
    }
}