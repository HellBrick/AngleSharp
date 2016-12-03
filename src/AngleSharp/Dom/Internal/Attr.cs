namespace AngleSharp.Dom
{
    using AngleSharp.Text;
    using System;

    /// <summary>
    /// Represents a generic node attribute.
    /// </summary>
    sealed class Attr : IAttr, IDisposable
    {
        #region Fields

        private LazyString _localName;
        private readonly String _prefix;
        private readonly String _namespace;
        private LazyString _value;

        #endregion

        #region ctor

        internal Attr( LazyString localName )
             : this( localName, LazyString.Empty )
        {
        }

        internal Attr( LazyString localName, LazyString value )
        {
            _localName = localName;
            _value = value;
        }

        internal Attr(String prefix, String localName, String value, String namespaceUri)
        {
            _prefix = prefix;
            _localName = new LazyString( localName );
            _value = new LazyString( value );
            _namespace = namespaceUri;
        }

        #endregion

        #region Internal Properties

        internal NamedNodeMap Container
        {
            get;
            set;
        }

        #endregion

        #region Properties

        public String Prefix
        {
            get { return _prefix; }
        }

        public Boolean IsId
        {
            get { return _prefix == null && _localName.Equals( AttributeNames.Id, StringComparison.OrdinalIgnoreCase ); }
        }

        public Boolean Specified
        {
            get { return !LazyString.IsNullOrEmpty(_value); }
        }

        public String Name
        {
            get { return _prefix == null ? _localName.ToString() : String.Concat( _prefix, ":", _localName.ToString() ); }
        }

        public String Value
        {
            get { return _value.ToString(); }
            set
            {
                var oldValue = _value;
                _value = new LazyString( value );

                if ( Container != null )
                    Container.RaiseChangedEvent( this, value, oldValue.ToString() );
                else
                    oldValue.Dispose();
            }
        }

        public String LocalName
        {
            get { return _localName.ToString(); }
        }

        public String NamespaceUri
        {
            get { return _namespace; }
        }

        #endregion

        #region Methods

        public Boolean Equals(IAttr other)
        {
            return Prefix.Is(other.Prefix) && NamespaceUri.Is(other.NamespaceUri) && Value.Is(other.Value);
        }

        public override Int32 GetHashCode()
        {
            const int prime = 31;
            var result = 1;

            result = result * prime + _localName.GetHashCode();
            result = result * prime + (_value ?? LazyString.Empty).GetHashCode();
            result = result * prime + (_namespace ?? String.Empty).GetHashCode();
            result = result * prime + (_prefix ?? String.Empty).GetHashCode();

            return result;
        }

		public void Dispose()
		{
			_localName?.Dispose();
			_value?.Dispose();
		}

        #endregion
    }
}