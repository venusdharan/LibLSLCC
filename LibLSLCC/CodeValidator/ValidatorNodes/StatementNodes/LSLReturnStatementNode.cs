﻿#region FileInfo

// 
// File: LSLReturnStatementNode.cs
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

using System;
using System.Diagnostics.CodeAnalysis;
using LibLSLCC.CodeValidator.Enums;
using LibLSLCC.CodeValidator.Primitives;
using LibLSLCC.CodeValidator.ValidatorNodes.Interfaces;
using LibLSLCC.CodeValidator.ValidatorNodeVisitor;

#endregion

namespace LibLSLCC.CodeValidator.ValidatorNodes.StatementNodes
{
    public class LSLReturnStatementNode : ILSLReturnStatementNode, ILSLCodeStatement
    {
// ReSharper disable UnusedParameter.Local
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "err")]
        protected LSLReturnStatementNode(LSLSourceCodeRange sourceRange, Err err)
// ReSharper restore UnusedParameter.Local
        {
            SourceCodeRange = sourceRange;
            HasErrors = true;
        }

        internal LSLReturnStatementNode(LSLParser.ReturnStatementContext context, ILSLExprNode returnExpression,
            bool isSingleBlockStatement)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            if (returnExpression == null)
            {
                throw new ArgumentNullException("returnExpression");
            }

            IsSingleBlockStatement = isSingleBlockStatement;
            ParserContext = context;
            ReturnExpression = returnExpression;

            ReturnExpression.Parent = this;
            SourceCodeRange = new LSLSourceCodeRange(context);
            ReturnKeywordSourceCodeRange = new LSLSourceCodeRange(context.return_keyword);
            SemiColonSourceCodeRange = new LSLSourceCodeRange(context.semi_colon);
        }

        internal LSLReturnStatementNode(LSLParser.ReturnStatementContext context, bool isSingleBlockStatement)
        {
            IsSingleBlockStatement = isSingleBlockStatement;
            ParserContext = context;
            ReturnExpression = null;
            SourceCodeRange = new LSLSourceCodeRange(context);
            ReturnKeywordSourceCodeRange = new LSLSourceCodeRange(context.return_keyword);
            SemiColonSourceCodeRange = new LSLSourceCodeRange(context.semi_colon);
        }

        internal LSLParser.ReturnStatementContext ParserContext { get; private set; }
        public ILSLExprNode ReturnExpression { get; }
        public ILSLCodeStatement ReturnPath { get; set; }

        ILSLReadOnlySyntaxTreeNode ILSLReadOnlySyntaxTreeNode.Parent
        {
            get { return Parent; }
        }

        public LSLDeadCodeType DeadCodeType { get; set; }

        ILSLReadOnlyExprNode ILSLReturnStatementNode.ReturnExpression
        {
            get { return ReturnExpression; }
        }

        public bool HasReturnExpression
        {
            get { return ReturnExpression != null; }
        }

        public ulong ScopeId { get; set; }
        public LSLSourceCodeRange ReturnKeywordSourceCodeRange { get; }
        public LSLSourceCodeRange SemiColonSourceCodeRange { get; }

        public static
            LSLReturnStatementNode GetError(LSLSourceCodeRange sourceRange)
        {
            return new LSLReturnStatementNode(sourceRange, Err.Err);
        }

        protected enum Err
        {
            Err
        }

        #region ILSLCodeStatement Members

        public bool IsSingleBlockStatement { get; private set; }
        public ILSLSyntaxTreeNode Parent { get; set; }
        public bool HasErrors { get; set; }

        public LSLSourceCodeRange SourceCodeRange { get; }


        public T AcceptVisitor<T>(ILSLValidatorNodeVisitor<T> visitor)
        {
            return visitor.VisitReturnStatement(this);
        }


        public int StatementIndex { get; set; }
        public bool IsLastStatementInScope { get; set; }

        public bool IsDeadCode { get; set; }

        ILSLReadOnlyCodeStatement ILSLReadOnlyCodeStatement.ReturnPath
        {
            get { return ReturnPath; }
        }


        public bool HasReturnPath
        {
            get { return true; }
        }

        #endregion

        #region Nested type: Err

        #endregion
    }
}