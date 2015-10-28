﻿#region FileInfo
// 
// File: LSLDoLoopNode.cs
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
#region Imports

using System;
using System.Diagnostics.CodeAnalysis;
using LibLSLCC.CodeValidator.Enums;
using LibLSLCC.CodeValidator.Nodes.Interfaces;
using LibLSLCC.CodeValidator.Primitives;
using LibLSLCC.CodeValidator.ValidatorNodeVisitor;
using LibLSLCC.Parser;

#endregion

namespace LibLSLCC.CodeValidator.Nodes
{
    public class LSLDoLoopNode : ILSLDoLoopNode, ILSLCodeStatement
    {
// ReSharper disable UnusedParameter.Local
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "err")]
        protected LSLDoLoopNode(LSLSourceCodeRange sourceRange, Err err)
// ReSharper restore UnusedParameter.Local
        {
            SourceCodeRange = sourceRange;
            HasErrors = true;
        }

        internal LSLDoLoopNode(LSLParser.DoLoopContext context, LSLCodeScopeNode code, ILSLExprNode conditionExpression,
            bool isSingleBlockStatement)
        {
            if (code == null)
            {
                throw new ArgumentNullException("code");
            }

            if (conditionExpression == null)
            {
                throw new ArgumentNullException("conditionExpression");
            }

            IsSingleBlockStatement = isSingleBlockStatement;
            ParserContext = context;
            Code = code;
            ConditionExpression = conditionExpression;
            code.Parent = this;
            conditionExpression.Parent = this;

            SourceCodeRange = new LSLSourceCodeRange(context);
            DoKeywordSourceCodeRange = new LSLSourceCodeRange(context.loop_keyword);
            WhileKeywordSourceCodeRange = new LSLSourceCodeRange(context.while_keyword);
            OpenParenthSourceCodeRange = new LSLSourceCodeRange(context.open_parenth);
            CloseParenthSourceCodeRange = new LSLSourceCodeRange(context.close_parenth);
            SemiColonSourceCodeRange = new LSLSourceCodeRange(context.semi_colon);

            SourceCodeRangesAvailable = true;
        }

        internal LSLParser.DoLoopContext ParserContext { get; private set; }


        /// <summary>
        /// The code scope node that represents the code scope of the loop body.
        /// </summary>
        public LSLCodeScopeNode Code { get; private set; }



        /// <summary>
        /// The condition expression that controls the loop, this could be null in the case of for-loops.
        /// </summary>
        public ILSLExprNode ConditionExpression { get; private set; }


        /// <summary>
        /// If the scope has a return path, this is set to the node that causes the function to return.
        /// it may be a return statement, or a control chain node.
        /// </summary>
        public ILSLCodeStatement ReturnPath { get; set; }

        ILSLReadOnlySyntaxTreeNode ILSLReadOnlySyntaxTreeNode.Parent
        {
            get { return Parent; }
        }

        ILSLCodeScopeNode ILSLLoopNode.Code
        {
            get { return Code; }
        }

        /// <summary>
        /// The condition expression that controls the loop, this could be null in the case of for-loops.
        /// </summary>
        ILSLReadOnlyExprNode ILSLLoopNode.ConditionExpression
        {
            get { return ConditionExpression; }
        }


        

        ILSLReadOnlyCodeStatement ILSLReadOnlyCodeStatement.ReturnPath
        {
            get { return ReturnPath; }
        }

        /// <summary>
        /// Represents an ID number for the scope this code statement is in, they are unique per-function/event handler.
        /// this is not the scopes level.
        /// </summary>
        public ulong ScopeId { get; set; }

        /// <summary>
        /// The source code range of the opening parenthesis of the condition expression area.
        /// </summary>
        public LSLSourceCodeRange OpenParenthSourceCodeRange { get; private set; }


        /// <summary>
        /// The source code range of the closing parenthesis of the condition expression area.
        /// </summary>
        public LSLSourceCodeRange CloseParenthSourceCodeRange { get; private set; }


        /// <summary>
        /// The source code range of the 'do' keyword in the statement.
        /// </summary>
        public LSLSourceCodeRange DoKeywordSourceCodeRange { get; private set; }


        /// <summary>
        /// The source code range of the 'while' keyword in the statement.
        /// </summary>
        public LSLSourceCodeRange WhileKeywordSourceCodeRange { get; private set; }


        /// <summary>
        /// The source code range of the semi-colon after the do-while loop statement.
        /// </summary>
        public LSLSourceCodeRange SemiColonSourceCodeRange { get; private set; }



        public static
            LSLDoLoopNode GetError(LSLSourceCodeRange sourceRange)
        {
            return new LSLDoLoopNode(sourceRange, Err.Err);
        }

        #region Nested type: Err

        protected enum Err
        {
            Err
        }

        #endregion

        #region ILSLCodeStatement Members


        /// <summary>
        /// True if this statement belongs to a single statement code scope.
        /// A single statement code scope is a brace-less code scope that can be used in control or loop statements.
        /// </summary>
        public bool IsSingleBlockStatement { get; private set; }


        /// <summary>
        /// The parent node of this syntax tree node.
        /// </summary>
        public ILSLSyntaxTreeNode Parent { get; set; }


        /// <summary>
        /// Is this statement the last statement in its scope
        /// </summary>
        public bool IsLastStatementInScope { get; set; }


        /// <summary>
        ///     The type of dead code that this statement is considered to be, if it is dead
        /// </summary>
        public LSLDeadCodeType DeadCodeType { get; set; }


        /// <summary>
        /// Is this statement dead code
        /// </summary>
        public bool IsDeadCode { get; set; }


        /// <summary>
        ///  The index of this statement in its scope
        /// </summary>
        public int StatementIndex { get; set; }


        /// <summary>
        /// True if this syntax tree node contains syntax errors.
        /// </summary>
        public bool HasErrors { get; set; }


        /// <summary>
        /// The source code range that this syntax tree node occupies.
        /// </summary>
        public LSLSourceCodeRange SourceCodeRange { get; private set; }

        /// <summary>
        /// Should return true if source code ranges are available/set to meaningful values for this node.
        /// </summary>
        public bool SourceCodeRangesAvailable { get; private set; }


        /// <summary>
        /// Accept a visit from an implementor of <see cref="ILSLValidatorNodeVisitor{T}"/>
        /// </summary>
        /// <typeparam name="T">The visitors return type.</typeparam>
        /// <param name="visitor">The visitor instance.</param>
        /// <returns>The value returned from this method in the visitor used to visit this node.</returns>
        public T AcceptVisitor<T>(ILSLValidatorNodeVisitor<T> visitor)
        {
            return visitor.VisitDoLoop(this);
        }


        /// <summary>
        /// True if the node represents a return path out of its ILSLCodeScopeNode parent, False otherwise.
        /// </summary>
        public bool HasReturnPath
        {
            get { return false; }
        }

        #endregion
    }
}