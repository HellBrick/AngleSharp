using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AngleSharp.Text
{
	internal class LazyString : IEquatable<LazyString>, IEquatable<string>, IDisposable
	{
		public static LazyString Empty { get; } = new LazyString( String.Empty );
		public static bool IsNullOrEmpty( LazyString lazyString ) => lazyString == null || lazyString == LazyString.Empty;

		public LazyString( PooledString value )
		{
			Pooled = value;
			Value = null;
		}

		public LazyString( string value )
		{
			Pooled = PooledString.Null;
			Value = value;
		}

		public PooledString Pooled { get; private set; }
		public string Value { get; private set; }

		public bool HasValue => Pooled.IsNull;

		public char this[ int index ] => HasValue ? Value[ index ] : Pooled[ index ];
		public int Length => HasValue ? Value.Length : Pooled.Length;

		public override string ToString()
		{
			EnsureValue();
			return Value;
		}

		private void EnsureValue()
		{
			if ( !HasValue )
			{
				Value = Pooled.ToStringAndDispose();
				Pooled = PooledString.Null;
			}
		}

		public override int GetHashCode()
			=> HasValue
			? Value.GetHashCode()
			: Pooled.GetHashCode();

		public bool IsOneOf( string str1, string str2, string str3 ) => Equals( str1 ) || Equals( str2 ) || Equals( str3 );

		public override bool Equals( object obj ) => Equals( obj as LazyString );

		public bool Equals( LazyString other )
			=> !ReferenceEquals( other, null )
			&& Length == other.Length
			&&
			(
				other.HasValue && CharsMatch( other.Value, StringComparison.Ordinal )
				|| CharsMatch( other.Pooled, StringComparison.Ordinal )
			);

		public static bool operator ==( LazyString x, LazyString y ) => ReferenceEquals( x, y ) || x?.Equals( y ) == true;
		public static bool operator !=( LazyString x, LazyString y ) => !( x == y );

		public bool Equals( string other ) => Equals( other, StringComparison.Ordinal );

		public bool Equals( string other, StringComparison comparison )
			=> HasValue
			? String.Equals( Value, other, comparison )
			: Length == other.Length && CharsMatch( other, comparison );

		private bool CharsMatch( PooledString other, StringComparison comparison )
		{
			for ( int i = 0; i < other.Length; i++ )
			{
				if ( !AreEqual( this[ i ], other[ i ], comparison ) )
					return false;
			}

			return true;
		}

		private bool CharsMatch( string other, StringComparison comparison )
		{
			for ( int i = 0; i < other.Length; i++ )
			{
				if ( !AreEqual( this[ i ], other[ i ], comparison ) )
					return false;
			}

			return true;
		}

		private bool AreEqual( char x, char y, StringComparison comparison )
			=> comparison == StringComparison.OrdinalIgnoreCase ? Char.ToUpperInvariant( x ) == Char.ToUpperInvariant( y )
			: x == y;

		public void Dispose() => Pooled.Dispose();
	}
}
