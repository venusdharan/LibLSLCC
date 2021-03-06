﻿#region FileInfo

// 
// File: LSLTokenTools.cs
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

using System.Text.RegularExpressions;
using LibLSLCC.AntlrParser;

#endregion

namespace LibLSLCC.Utility
{
    /// <summary>
    ///     Tools for dealing with LSL token strings, mostly symbol names.
    /// </summary>
    public static class LSLTokenTools
    {
        /// <summary>
        ///     A raw string copy of a regex that matches/validates LSL ID Tokens, IE: variable names, state names, label names,
        ///     function names
        /// </summary>
        public static readonly string IDRegexString = LSLLexer.IDRegex;

        /// <summary>
        ///     A raw string copy of a regex that matches/validates LSL Float tokens, the regex string is not anchored.
        /// </summary>
        public static readonly string FloatRegexString =
            "(:?\\-?(?!(:?[0-9]+[fF])|(:?[0-9]+e[-+]?[0-9]+[fF]))(?=\\.?(:?[0-9][eE]?))(:?[0-9]*\\.?[0-9]*)(:?[eE][-+]?[0-9]+)?[fF]?)";


        /// <summary>
        ///     This regex matches/validates LSL Float Tokens, the regex is not anchord.
        /// </summary>
        public static readonly Regex FloatRegex = new Regex(FloatRegexString);


        /// <summary>
        ///     This regex matches/validates LSL Float Tokens.
        /// </summary>
        public static readonly Regex FloatRegexAnchored = new Regex("^" + FloatRegexString + "$");


        /// <summary>
        ///     This regex matches/validates LSL ID Tokens, IE: variable names, state names, label names, function names
        /// </summary>
        public static readonly Regex IDRegex = new Regex(LSLLexer.IDRegex);

        /// <summary>
        ///     This regex matches/validates LSL ID Tokens, IE: variable names, state names, label names, function names
        ///     It is anchored with ^ and $ at the beginning and end respectively.
        /// </summary>
        public static readonly Regex IDRegexAnchored = new Regex("^" + IDRegexString + "$");

        /// <summary>
        ///     A raw string copy of a regex that matches/validates that a character is a valid starting character for an ID Token
        /// </summary>
        public static readonly string IDStartCharRegexString = LSLLexer.IDStartCharRegex;

        /// <summary>
        ///     This regex matches/validates that a character is a valid starting character for an ID Token
        /// </summary>
        public static readonly Regex IDStartCharRegex = new Regex(IDStartCharRegexString);

        /// <summary>
        ///     A raw string copy of a regex matches/validates that a character is a valid trailing character after the first
        ///     character of an ID Token
        /// </summary>
        public static readonly string IDTrailingCharRegexString = LSLLexer.IDTrailingCharRegex;

        /// <summary>
        ///     This regex matches/validates that a character is a valid trailing character after the first character of an ID
        ///     Token
        /// </summary>
        public static readonly Regex IDTrailingCharRegex = new Regex(IDTrailingCharRegexString);

        /// <summary>
        ///     A raw string copy of a regex that matches/validates that a character is either a valid starting OR trailing
        ///     character in an ID token
        /// </summary>
        public static readonly string IDAnyCharRegexString = "(?:" + LSLLexer.IDStartCharRegex + "|" +
                                                             LSLLexer.IDTrailingCharRegex + ")";

        /// <summary>
        ///     This regex matches/validates that a character is either a valid starting OR trailing character in an ID token
        /// </summary>
        public static readonly Regex IDAnyCharRegex = new Regex(IDAnyCharRegexString);
    }
}