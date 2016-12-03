using System;

namespace AngleSharp.Text
{
	internal struct PooledString : IDisposable, IEquatable<PooledString>, IEquatable<string>
	{
		public static PooledString Null { get; } = default( PooledString );
		public static PooledString Empty { get; } = new PooledString( CharBufferPool.Rent( 0 ), 0 );

		public PooledString( CharBuffer buffer, int length )
		{
			Buffer = buffer;
			Length = length;
		}

		public CharBuffer Buffer { get; }
		public int Length { get; }

		public char this[ int index ] => Buffer[ index ];
		public bool IsNull => Buffer == default( CharBuffer );
		public override string ToString() => IsNull ? null : new string( Buffer.Array, 0, Length );
		public void Dispose() => Buffer?.Dispose();

		public string ToStringAndDispose()
		{
			string value = ToString();
			Dispose();
			return value;
		}

		public static bool operator ==( PooledString pooled, string normal ) => pooled.Equals( normal );
		public static bool operator !=( PooledString pooled, string normal ) => !pooled.Equals( normal );

		public static bool operator ==( PooledString x, PooledString y ) => x.Equals( y );
		public static bool operator !=( PooledString x, PooledString y ) => !x.Equals( y );

		public override bool Equals( object obj )
			=> obj is PooledString
			&& Equals( (PooledString) obj );

		public bool Equals( PooledString other )
			=> Length == other.Length
			&& IsNull == other.IsNull
			&& CharsAreEqual( other );

		private bool CharsAreEqual( PooledString other )
		{
			for ( int i = 0; i < Length; i++ )
			{
				if ( this[ i ] != other[ i ] )
					return false;
			}

			return true;
		}

		public bool Equals( string other )
			=> Length == other.Length
			&& IsNull == ( other == null )
			&& CharsAreEqual( other );

		private bool CharsAreEqual( string other )
		{
			for ( int i = 0; i < Length; i++ )
			{
				if ( this[ i ] != other[ i ] )
					return false;
			}

			return true;
		}

		public override int GetHashCode()
		{
			unchecked
			{
				if ( IsNull )
					return 0;

				const int prime = 19;
				int hash = 23;

				for ( int i = 0; i < Length; i++ )
					hash = hash * prime + this[ i ];

				return hash;
			}
		}
	}
}
