using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AngleSharp.Text
{
	internal struct LazyString
	{
		public LazyString( StringBuilder builder )
		{
			Builder = builder;
			Value = null;
		}

		public LazyString( String value )
		{
			Builder = null;
			Value = value;
		}

		public StringBuilder Builder { get; }
		public String Value { get; }

		public Boolean HasValue => Value != null;

		public Char this[ int index ] => HasValue ? Value[ index ] : Builder[ index ];
		public int Length => HasValue ? Value.Length : Builder.Length;
		public Boolean IsNullOrEmpty => HasValue ? String.IsNullOrEmpty( Value ) : Builder.Length == 0;

		public LazyString Substring( int offset ) => Substring( offset, Length - offset );
		public LazyString Substring( int offset, int count )
		{
			var newBuilder = StringBuilderPool.Obtain();
			if ( HasValue )
				newBuilder.Append( Value.Substring( offset, count ) );
			else
			{
				for ( int i = offset; i < offset + count; i++ )
					newBuilder.Append( Builder[ i ] );
			}

			return new LazyString( newBuilder );
		}

		public LazyString Insert( int offset, String data )
		{
			if ( HasValue )
			{
				StringBuilder newBuilder = StringBuilderPool.Obtain();
				newBuilder.Append( Value, 0, offset );
				newBuilder.Append( data );
				newBuilder.Append( Value, offset, Value.Length - offset );
				return new LazyString( newBuilder );
			}

			Builder.Insert( offset, data );
			return this;
		}

		public LazyString Remove( int offset, int count )
		{
			if ( count == 0 )
				return this;

			if ( HasValue )
			{
				StringBuilder newBuilder = StringBuilderPool.Obtain();
				newBuilder.Append( Value, 0, offset );
				newBuilder.Append( Value, offset + count, Value.Length - offset - count );
				return new LazyString( newBuilder );
			}

			Builder.Remove( offset, count );
			return this;
		}

		public LazyString EnsureValue()
			=> Builder == null
			? this
			: new LazyString( StringBuilderPool.ToPool( Builder ) );

		public static Boolean operator ==( LazyString lazyString, String ordinaryString )
			=> lazyString.HasValue
			? String.Equals( lazyString.Value, ordinaryString, StringComparison.Ordinal )
			: lazyString.Length == ordinaryString.Length && CharsMatch( lazyString.Builder, ordinaryString );

		public static Boolean operator !=( LazyString lazyString, String ordinaryString ) => !( lazyString == ordinaryString );

		private static Boolean CharsMatch( StringBuilder builder, String str )
		{
			for ( int i = 0; i < str.Length; i++ )
			{
				if ( str[ i ] != builder[ i ] )
					return false;
			}

			return true;
		}
	}
}
