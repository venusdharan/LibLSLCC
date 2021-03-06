﻿#region FileInfo

// 
// File: LSLJumpStatementNode.cs
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
using LibLSLCC.AntlrParser;

#endregion

namespace LibLSLCC.CodeValidator
{
    /// <summary>
    ///     Default <see cref="ILSLJumpStatementNode" /> implementation used by <see cref="LSLCodeValidator" />
    /// </summary>
    public sealed class LSLJumpStatementNode : ILSLJumpStatementNode, ILSLCodeStatement
    {
        /// <summary>
        ///     Creates an <see cref="LSLJumpStatementNode" /> that jumps to a specified <see cref="LSLLabelStatementNode" />, with
        ///     a <see cref="ParentScopeId" /> of zero.
        ///     <paramref name="jumpTarget" /> receives a <see cref="LSLLabelStatementNode.JumpsToHere" /> reference via
        ///     <see cref="LSLLabelStatementNode.AddJumpToHere" />.
        /// </summary>
        /// <param name="jumpTarget">The <see cref="LSLLabelStatementNode" /> to jump to.</param>
        /// <exception cref="ArgumentNullException"><paramref name="jumpTarget" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException"><paramref name="jumpTarget" /> does not have a Parent, is not already in the syntax tree.</exception>
        public LSLJumpStatementNode(LSLLabelStatementNode jumpTarget)
        {
            if (jumpTarget == null)
            {
                throw new ArgumentNullException("jumpTarget");
            }

            if (jumpTarget.Parent == null)
            {
                throw new ArgumentException("jumpTarget.Parent is null, must have a parent (be part of the syntax tree) prior to being jumped to.", "jumpTarget");
            }

            LabelName = jumpTarget.LabelName;

            JumpTarget = jumpTarget;

            JumpTarget.AddJumpToHere(this);
        }


        /// <exception cref="ArgumentNullException"><paramref name="context" /> or <paramref name="jumpTarget" /> is <c>null</c>.</exception>
        internal LSLJumpStatementNode(
            LSLParser.JumpStatementContext context,
            LSLLabelStatementNode jumpTarget
            )
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            if (jumpTarget == null)
            {
                throw new ArgumentNullException("jumpTarget");
            }

            LabelName = context.jump_target.Text;

            JumpTarget = jumpTarget;
            JumpTarget.AddJumpToHere(this);

            SourceRange = new LSLSourceCodeRange(context);

            SourceRangeLabelName = new LSLSourceCodeRange(context.jump_target);
            SourceRangeJumpKeyword = new LSLSourceCodeRange(context.jump_keyword);
            SourceRangeSemicolon = new LSLSourceCodeRange(context.semi_colon);

            SourceRangesAvailable = true;
        }


        /*
        /// <summary>
        ///     Create an <see cref="LSLJumpStatementNode" /> by cloning from another.
        /// </summary>
        /// <param name="other">The other node to clone from.</param>
        /// <exception cref="ArgumentNullException"><paramref name="other" /> is <c>null</c>.</exception>
        private LSLJumpStatementNode(LSLJumpStatementNode other)
        {
            if (other == null) throw new ArgumentNullException("other");


            SourceRangesAvailable = other.SourceRangesAvailable;

            if (SourceRangesAvailable)
            {
                SourceRange = other.SourceRange;
                SourceRangeJumpKeyword = other.SourceRangeJumpKeyword;
                SourceRangeLabelName = other.SourceRangeLabelName;
                SourceRangeSemicolon = other.SourceRangeSemicolon;
            }

            //TODO figure this out

            JumpTarget = other.JumpTarget.Clone();

            LabelName = other.LabelName;

            ConstantJump = other.ConstantJump;

            LSLStatementNodeTools.CopyStatement(this, other);

            HasErrors = other.HasErrors;
        }*/


        /// <summary>
        ///     The label statement node in the syntax tree that this jump statement jumps to.
        /// </summary>
        public LSLLabelStatementNode JumpTarget { get; private set; }

        private ILSLSyntaxTreeNode _parent; // ReSharper disable UnusedParameter.Local


        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "err")]
        private LSLJumpStatementNode(LSLSourceCodeRange sourceRange, Err err)
// ReSharper restore UnusedParameter.Local
        {
            SourceRange = sourceRange;
            HasErrors = true;
        }

        #region ILSLCodeStatement Members

        /// <summary>
        ///     True if this statement belongs to a single statement code scope.
        ///     A single statement code scope is a braceless code scope that can be used in control or loop statements.
        /// </summary>
        /// <seealso cref="ILSLCodeScopeNode.IsSingleStatementScope" />
        public bool InsideSingleStatementScope { get; set; }


        /// <summary>
        ///     The parent node of this syntax tree node.
        /// </summary>
        /// <exception cref="InvalidOperationException" accessor="set">If Parent has already been set.</exception>
        /// <exception cref="ArgumentNullException" accessor="set"><paramref name="value" /> is <c>null</c>.</exception>
        public ILSLSyntaxTreeNode Parent
        {
            get { return _parent; }
            set
            {
                if (_parent != null)
                {
                    throw new InvalidOperationException(GetType().Name +
                                                        ": Parent node already set, it can only be set once.");
                }
                if (value == null)
                {
                    throw new ArgumentNullException("value", GetType().Name + ": Parent cannot be set to null.");
                }

                _parent = value;
            }
        }

        /// <summary>
        ///     True if this jump is guaranteed to occur in a constant manner.
        ///     IE, the jump is always encountered regardless of program control flow.
        /// </summary>
        public bool ConstantJump { get; set; }


        /// <summary>
        ///     Is this statement the last statement in its scope
        /// </summary>
        public bool IsLastStatementInScope { get; set; }


        /// <summary>
        ///     Is this statement dead code
        /// </summary>
        public bool IsDeadCode { get; set; }


        /// <summary>
        ///     The index of this statement in its scope
        /// </summary>
        public int StatementIndex { get; set; }


        /// <summary>
        ///     True if the node represents a return path out of its ILSLCodeScopeNode parent, False otherwise.
        /// </summary>
        public bool HasReturnPath
        {
            get { return false; }
        }

        // ReSharper disable UnusedParameter.Local


        /// <summary>
        ///     True if this syntax tree node contains syntax errors. <para/>
        ///     <see cref="SourceRange"/> should point to a more specific error location when this is <c>true</c>. <para/>
        ///     Other source ranges will not be available.
        /// </summary>
        public bool HasErrors { get; private set; }


        /// <summary>
        ///     The source code range that this syntax tree node occupies.
        /// </summary>
        /// <remarks>
        ///     If <see cref="ILSLReadOnlySyntaxTreeNode.SourceRangesAvailable" /> is <c>false</c> this property will be
        ///     <c>null</c>.
        /// </remarks>
        public LSLSourceCodeRange SourceRange { get; private set; }

        /// <summary>
        ///     Should return true if source code ranges are available/set to meaningful values for this node.
        /// </summary>
        public bool SourceRangesAvailable { get; private set; }


        /// <summary>
        ///     Accept a visit from an implementor of <see cref="ILSLValidatorNodeVisitor{T}" />
        /// </summary>
        /// <typeparam name="T">The visitors return type.</typeparam>
        /// <param name="visitor">The visitor instance.</param>
        /// <returns>The value returned from this method in the visitor used to visit this node.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="visitor"/> is <c>null</c>.</exception>
        public T AcceptVisitor<T>(ILSLValidatorNodeVisitor<T> visitor)
        {
            if (visitor == null) throw new ArgumentNullException("visitor");

            return visitor.VisitJumpStatement(this);
        }

        #endregion

        #region Nested type: Err

        private enum Err
        {
            Err
        }

        #endregion

        ILSLReadOnlySyntaxTreeNode ILSLReadOnlySyntaxTreeNode.Parent
        {
            get { return Parent; }
        }

        /// <summary>
        ///     The type of dead code that this statement is considered to be, if it is dead
        /// </summary>
        public LSLDeadCodeType DeadCodeType { get; set; }


        /// <summary>
        ///     The name of the label that the jump statement jumps to.
        /// </summary>
        public string LabelName { get; private set; }

        ILSLLabelStatementNode ILSLJumpStatementNode.JumpTarget
        {
            get { return JumpTarget; }
        }


        /// <summary>
        ///     Represents an ID number for the scope this code statement is in, they are unique per-function/event handler.
        ///     this is not the scopes level.
        /// </summary>
        public int ParentScopeId { get; set; }


        /// <summary>
        ///     Returns a version of this node type that represents its error state;  in case of a syntax error
        ///     in the node that prevents the node from being even partially built.
        /// </summary>
        /// <param name="sourceRange">The source code range of the error.</param>
        /// <returns>A version of this node type in its undefined/error state.</returns>
        public static
            LSLJumpStatementNode GetError(LSLSourceCodeRange sourceRange)
        {
            return new LSLJumpStatementNode(sourceRange, Err.Err);
        }


        /// <summary>
        ///     The source code range of the 'jump' keyword in the jump statement.
        /// </summary>
        /// <remarks>
        ///     If <see cref="ILSLReadOnlySyntaxTreeNode.SourceRangesAvailable" /> is <c>false</c> this property will be
        ///     <c>null</c>.
        /// </remarks>
        public LSLSourceCodeRange SourceRangeJumpKeyword { get; private set; }


        /// <summary>
        ///     The source code range of the target label name in the jump statement.
        /// </summary>
        /// <remarks>
        ///     If <see cref="ILSLReadOnlySyntaxTreeNode.SourceRangesAvailable" /> is <c>false</c> this property will be
        ///     <c>null</c>.
        /// </remarks>
        public LSLSourceCodeRange SourceRangeLabelName { get; private set; }


        /// <summary>
        ///     The source code range of the semi-colon that follows the jump statement.
        /// </summary>
        /// <remarks>
        ///     If <see cref="ILSLReadOnlySyntaxTreeNode.SourceRangesAvailable" /> is <c>false</c> this property will be
        ///     <c>null</c>.
        /// </remarks>
        public LSLSourceCodeRange SourceRangeSemicolon { get; private set; }
    }
}