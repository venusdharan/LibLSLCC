﻿#region FileInfo

// 
// File: CollectionExtensions.cs
// 
// Author/Copyright:  Eric A. Blundell
// 
// Last Compile: 24/09/2015 @ 9:25 PM
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

#endregion

namespace LibLSLCC.Collections
{
    public static class CollectionExtensions
    {
        public static IReadOnlyDictionary<TK, TV> AsReadOnly<TK, TV>(this IDictionary<TK, TV> dict)
        {
            return new ReadOnlyDictionary<TK, TV>(dict);
        }

        public static IReadOnlySet<T> AsReadOnly<T>(this ISet<T> set)
        {
            return new ReadOnlyHashSet<T>(set);
        }
    }
}