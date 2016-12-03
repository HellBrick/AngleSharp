using System;
using System.Collections;
using System.Collections.Generic;

namespace AngleSharp.Text
{
	internal class CharBuffer : IDisposable, IReadOnlyList<char>
	{
		private readonly ICharBufferOwner _owner;

		public static CharBuffer Empty { get; } = new CharBuffer( new char[ 0 ], null );

		public CharBuffer( char[] array, ICharBufferOwner owner )
		{
			_owner = owner;
			Array = array;
		}

		public char[] Array { get; }
		public int Count => Array.Length;

		public char this[ int index ]
		{
			get { return Array[ index ]; }
			set { Array[ index ] = value; }
		}

		public void Dispose() => _owner?.Return( this );

		public IEnumerator<char> GetEnumerator() => ( (IReadOnlyList<char>) Array ).GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}
}
