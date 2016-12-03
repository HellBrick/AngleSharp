using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace AngleSharp.Text
{
	internal static class CharBufferPool
	{
		private const int _lastSmallSize = 16;
		private const int _largeBucketCount = 12;
		private const int _maxPooledSize = _lastSmallSize << _largeBucketCount;

		private static readonly Bucket[] _smallBuckets
			= Enumerable
			.Range( 1, _lastSmallSize )
			.Select( size => new Bucket( size ) )
			.ToArray();

		private static readonly Bucket[] _largeBuckets
			= Enumerable
			.Range( 1, _largeBucketCount )
			.Select( shift => _lastSmallSize << shift )
			.Select( size => new Bucket( size ) )
			.ToArray();

		public static CharBuffer Rent( int size )
		{
			return size <= 0 ? CharBuffer.Empty
			: size <= _lastSmallSize ? _smallBuckets[ size - 1 ].Rent()
			: size <= _maxPooledSize ? GetLargeBucket( size ).Rent()
			: new CharBuffer( new char[ size ], null );
		}

		private static Bucket GetLargeBucket( int size )
		{
			foreach ( var bucket in _largeBuckets )
			{
				if ( size <= bucket.Length )
					return bucket;
			}

			System.Diagnostics.Debug.Assert( false, $"{nameof( GetLargeBucket )} shouldn't have been called with {nameof( size )} = {size}." );
			return null;
		}

		private class Bucket : ICharBufferOwner
		{
			private ThreadLocal<Stack<CharBuffer>> _tlsBuffers = new ThreadLocal<Stack<CharBuffer>>( () => new Stack<CharBuffer>() );
			private Stack<CharBuffer> LocalBuffers => _tlsBuffers.Value;

			public Bucket( int length )
			{
				Length = length;
			}

			public int Length { get; }

			public CharBuffer Rent()
				=> LocalBuffers.Count == 0
				? new CharBuffer( new char[ Length ], this )
				: LocalBuffers.Pop();

			public void Return( CharBuffer buffer )
				=> LocalBuffers.Push( buffer );
		}
	}
}
