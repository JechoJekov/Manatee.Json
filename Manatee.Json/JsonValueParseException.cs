﻿/***************************************************************************************

	Copyright 2012 Greg Dennis

	   Licensed under the Apache License, Version 2.0 (the "License");
	   you may not use this file except in compliance with the License.
	   You may obtain a copy of the License at

		 http://www.apache.org/licenses/LICENSE-2.0

	   Unless required by applicable law or agreed to in writing, software
	   distributed under the License is distributed on an "AS IS" BASIS,
	   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
	   See the License for the specific language governing permissions and
	   limitations under the License.
 
	File Name:		JsonValueParseException.cs
	Namespace:		Manatee.Json
	Class Name:		JsonValueParseException
	Purpose:		Thrown when an input string contains an invalid value.

***************************************************************************************/

using System;

namespace Manatee.Json
{
	/// <summary>
	/// Thrown when an input string contains an invalid value.
	/// </summary>
	[Serializable]
	public class JsonValueParseException : Exception
	{
		/// <summary>
		/// Gets the index at which the error occurred.
		/// </summary>
		public int Index { get; private set; }
		/// <summary>
		/// Gets the input string which caused the error.
		/// </summary>
		public string Input { get; private set; }

		/// <summary>
		/// Creates a new instance of this exception.
		/// </summary>
		internal JsonValueParseException(JsonValueType t, int index, string input)
			: base(string.Format("Parse of type {0} failed at index {1}.", t, index))
		{
			Index = index;
			Input = input;
		}
		/// <summary>
		/// Creates a new instance of this exception.
		/// </summary>
		internal JsonValueParseException(int index, string value, string input)
			: base(string.Format("Parse failed at index {0}: cannot determine type of value '{1}'.", index, value))
		{
			Index = index;
			Input = input;
		}
	}
}