#region FileInfo

// 
// File: FirestormDocumentationScraper.cs
// 
// Author/Copyright:  Eric A. Blundell
// 
// Last Compile: 24/09/2015 @ 9:27 PM
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
using System.Text.RegularExpressions;
using System.Xml;
using LibLSLCC.CodeValidator.Components;
using LibLSLCC.CodeValidator.Primitives;
using LibraryDataScrapingTools.LibraryDataScrapers.FirestormLibraryDataDom;
using LibraryDataScrapingTools.ScraperInterfaces;

#endregion

namespace LibraryDataScrapingTools.LibraryDataScrapers
{
    public class FirestormDocumentationScraper : IDocumentationProvider
    {
        private readonly LSLFunctionSignatureRegex _eventSig = new LSLFunctionSignatureRegex("", ";*");
        private readonly LSLFunctionSignatureRegex _functionSig = new LSLFunctionSignatureRegex("", ";*");
        private readonly ScriptLibrary _scriptLibrary;

        public FirestormDocumentationScraper(XmlReader reader)
        {
            _scriptLibrary = ScriptLibrary.Read(reader);
        }

        public FirestormDocumentationScraper(ScriptLibrary library)
        {
            _scriptLibrary = library;
        }

        public string DocumentFunction(LSLLibraryFunctionSignature function)
        {
            var scriptLibraryHasFunction = _scriptLibrary.Functions.Contains(function.Name);
            if (scriptLibraryHasFunction)
            {
                var doc = "";
                var hasDocSig = false;
                var hasMatchingDocSig = false;

                foreach (var f in _scriptLibrary.Functions.Get(function.Name))
                {
                    var matches = _functionSig.Regex.Matches(f.Desc);


                    foreach (Match match in matches)
                    {
                        if (match.Success)
                        {
                            hasDocSig = true;
                            var sig = LSLLibraryFunctionSignature.Parse(match.ToString());

                            doc = _functionSig.Regex.Replace(f.Desc, "");

                            if (sig.SignatureMatches(function))
                            {
                                hasMatchingDocSig = true;

                                break;
                            }
                        }
                        else
                        {
                            doc = f.Desc;
                            hasDocSig = false;
                        }
                    }
                }

                if (function.Name == "lsSetWindlightScene")
                {
                    Console.WriteLine("test");
                }

                if (hasDocSig)
                {
                    if (hasMatchingDocSig)
                    {
                        Log.WriteLine(
                            "FirestormDocumentationScraper script_library name={0}: Matching docstring signature found for function {1}",
                            _scriptLibrary.Name, function.Name);
                        return doc;
                    }
                    Log.WriteLine(
                        "FirestormDocumentationScraper script_library name={0}: Docstring signature found for function {1} mismatches passed function signature",
                        _scriptLibrary.Name, function.Name);
                    return doc;
                }

                if (string.IsNullOrWhiteSpace(doc))
                {
                    Log.WriteLine(
                        "FirestormDocumentationScraper script_library name={0}: Docstring for function {1} is empty",
                        _scriptLibrary.Name, function.Name);
                    return null;
                }

                Log.WriteLine(
                    "FirestormDocumentationScraper script_library name={0}: Docstring found for function {1} did not contain a signature",
                    _scriptLibrary.Name, function.Name);
                return doc;
            }

            Log.WriteLine(
                "FirestormDocumentationScraper script_library name={0}: Function {1} not defined in firestorm data",
                _scriptLibrary.Name, function.Name);

            return null;
        }

        public string DocumentEvent(LSLLibraryEventSignature eventHandler)
        {
            if (!_scriptLibrary.Keywords.Contains(eventHandler.Name))
            {
                Log.WriteLine(
                    "FirestormDocumentationScraper script_library name={0}: Event {1} not defined in firestorm script library",
                    _scriptLibrary.Name, eventHandler.Name);
                return null;
            }


            var e = _scriptLibrary.Keywords.Get(eventHandler.Name);
            if (string.IsNullOrWhiteSpace(e.Desc))
            {
                Log.WriteLine("FirestormDocumentationScraper script_library name={0}: Docstring for event {1} is empty",
                    _scriptLibrary.Name, eventHandler.Name);
                return null;
            }

            var match = _eventSig.Regex.Match(e.Desc);
            if (match.Success)
            {
                var sig = LSLLibraryEventSignature.Parse(match.ToString());
                if (!sig.SignatureMatches(eventHandler))
                {
                    Log.WriteLine(
                        "FirestormDocumentationScraper script_library name={0}: Docstring signature for event {1} mismatches passed event signature",
                        _scriptLibrary.Name, e.Name);
                }
                else
                {
                    Log.WriteLine(
                        "FirestormDocumentationScraper script_library name={0}: Docstring signature for event {1} matches passed event signature",
                        _scriptLibrary.Name, e.Name);
                }
                return _functionSig.Regex.Replace(e.Desc, "");
            }

            Log.WriteLine(
                "FirestormDocumentationScraper script_library name={0}: Docstring for event {1} does not contain a signature",
                _scriptLibrary.Name, e.Name);


            return e.Desc;
        }

        public string DocumentConstant(LSLLibraryConstantSignature constant)
        {
            if (_scriptLibrary.Keywords.Contains(constant.Name))
            {
                var c = _scriptLibrary.Keywords.Get(constant.Name);
                if (string.IsNullOrWhiteSpace(c.Desc))
                {
                    Log.WriteLine(
                        "FirestormDocumentationScraper script_library name={0}: Docstring for constant {1} is empty",
                        _scriptLibrary.Name, c.Name);
                    return null;
                }

                Log.WriteLine(
                    "FirestormDocumentationScraper script_library name={0}: Docstring for constant {1} found in firestorm data",
                    _scriptLibrary.Name, c.Name);
                return c.Desc;
            }
            Log.WriteLine(
                "FirestormDocumentationScraper script_library name={0}: Constant {1} not defined in firestorm script library",
                _scriptLibrary.Name, constant.Name);
            return null;
        }
    }
}