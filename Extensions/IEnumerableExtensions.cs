// 
//  IEnumerableExtensions.cs
// 
//  Author:
//    Robert Kozak (rkozak@gmail.com / Twitter:@robertkozak)
// 
//  Copyright 2011 - 2012, Nowcom Corporation.
// 
//  Code licensed under the MIT X11 license
// 
//  Permission is hereby granted, free of charge, to any person obtaining
//  a copy of this software and associated documentation files (the
//  "Software"), to deal in the Software without restriction, including
//  without limitation the rights to use, copy, modify, merge, publish,
//  distribute, sublicense, and/or sell copies of the Software, and to
//  permit persons to whom the Software is furnished to do so, subject to
//  the following conditions:
// 
//  The above copyright notice and this permission notice shall be
//  included in all copies or substantial portions of the Software.
// 
//  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
//  EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
//  MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
//  NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
//  LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
//  OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
//  WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// 
namespace MonoMobile.Views
{
	using System;
	using System.Collections.Generic;
	using System.Collections.ObjectModel;

	public static class IEnumerableExtensions
	{
		public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> items)
		{
			var collection = new ObservableCollection<T>();
			foreach (T item in items)
			{
				collection.Add(item);
			}
			return collection;

//			return new ObservableCollection<T>(collection);
		}


	#region License and Terms
	//
	// MoreLINQ - Extensions to LINQ to Objects
	// Copyright (c) 2008-2011 Jonathan Skeet. All rights reserved.
	//
	// Licensed under the Apache License, Version 2.0 (the "License");
	// you may not use this file except in compliance with the License.
	// You may obtain a copy of the License at
	//
	//    http://www.apache.org/licenses/LICENSE-2.0
	//
	// Unless required by applicable law or agreed to in writing, software
	// distributed under the License is distributed on an "AS IS" BASIS,
	// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
	// See the License for the specific language governing permissions and
	// limitations under the License.
	//
	#endregion
	
	    /// <summary>
	    /// Skips items from the input sequence until the given predicate returns true
	    /// when applied to the current source item; that item will be the last skipped.
	    /// </summary>
	    /// <remarks>
	    /// <para>
	    /// SkipUntil differs from Enumerable.SkipWhile in two respects. Firstly, the sense
	    /// of the predicate is reversed: it is expected that the predicate will return false
	    /// to start with, and then return true - for example, when trying to find a matching
	    /// item in a sequence.
	    /// </para>
	    /// <para>
	    /// Secondly, SkipUntil skips the element which causes the predicate to return true. For
	    /// example, in a sequence <code>{ 1, 2, 3, 4, 5 }</code> and with a predicate of
	    /// <code>x => x == 3</code>, the result would be <code>{ 4, 5 }</code>.
	    /// </para>
	    /// <para>
	    /// SkipUntil is as lazy as possible: it will not iterate over the source sequence
	    /// until it has to, it won't iterate further than it has to, and it won't evaluate
	    /// the predicate until it has to. (This means that an item may be returned which would
	    /// actually cause the predicate to throw an exception if it were evaluated, so long as
	    /// it comes after the first item causing the predicate to return true.)
	    /// </para>
	    /// </remarks>
	    /// <typeparam name="TSource">Type of the source sequence</typeparam>
	    /// <param name="source">Source sequence</param>
	    /// <param name="predicate">Predicate used to determine when to stop yielding results from the source.</param>
	    /// <returns>Items from the source sequence after the predicate first returns true when applied to the item.</returns>
	    /// <exception cref="ArgumentNullException"><paramref name="source"/> or <paramref name="predicate"/> is null</exception>
	
	    public static IEnumerable<TSource> SkipUntil<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
	    {
	        using (var iterator = source.GetEnumerator())
	        {
	            while (iterator.MoveNext())
	            {
	                if (predicate(iterator.Current))
	                {
	                    break;
	                }
	            }
	            while (iterator.MoveNext())
	            {
	                yield return iterator.Current;
	            }
	        }
	    }
	}
}