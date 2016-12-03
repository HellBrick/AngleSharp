namespace AngleSharp.Html.Parser.Tokens
{
    using AngleSharp.Text;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Class for StartTagToken and EndTagToken.
    /// </summary>
    sealed class HtmlTagToken : HtmlToken
    {
        #region Fields

        private readonly List<KeyValuePair<LazyString, LazyString>> _attributes;

        private Boolean _selfClosing;

        #endregion

        #region ctor

        /// <summary>
        /// Sets the default values.
        /// </summary>
        /// <param name="type">The type of the tag token.</param>
        /// <param name="position">The token's position.</param>
        public HtmlTagToken(HtmlTokenType type, TextPosition position)
            : this(type, position, String.Empty)
        {
        }

        /// <summary>
        /// Creates a new HTML TagToken with the defined name.
        /// </summary>
        /// <param name="type">The type of the tag token.</param>
        /// <param name="position">The token's position.</param>
        /// <param name="name">The name of the tag.</param>
        public HtmlTagToken(HtmlTokenType type, TextPosition position, String name)
            : base(type, position, name)
        {
            _attributes = new List<KeyValuePair<LazyString, LazyString>>();
        }

        #endregion

        #region Creators

        /// <summary>
        /// Creates a new opening HtmlTagToken for the given name.
        /// </summary>
        /// <param name="name">The name of the tag.</param>
        /// <returns>The new HTML tag token.</returns>
        public static HtmlTagToken Open(String name)
        {
            return new HtmlTagToken(HtmlTokenType.StartTag, TextPosition.Empty, name);
        }

        /// <summary>
        /// Creates a new closing HtmlTagToken for the given name.
        /// </summary>
        /// <param name="name">The name of the tag.</param>
        /// <returns>The new HTML tag token.</returns>
        public static HtmlTagToken Close(String name)
        {
            return new HtmlTagToken(HtmlTokenType.EndTag, TextPosition.Empty, name);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the state of the self-closing flag.
        /// </summary>
        public Boolean IsSelfClosing
        {
            get { return _selfClosing; }
            set { _selfClosing = value; }
        }

        /// <summary>
        /// Gets the list of attributes.
        /// </summary>
        public List<KeyValuePair<LazyString, LazyString>> Attributes
        {
            get { return _attributes; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Adds a new attribute to the list of attributes. The value will
        /// be set to an empty string.
        /// </summary>
        /// <param name="name">The name of the attribute.</param>
        public void AddAttribute(LazyString name)
        {
            _attributes.Add(new KeyValuePair<LazyString, LazyString>(name, LazyString.Empty));
        }

        /// <summary>
        /// Adds a new attribute to the list of attributes.
        /// </summary>
        /// <param name="name">The name of the attribute.</param>
        /// <param name="value">The value of the attribute.</param>
        public void AddAttribute(LazyString name, LazyString value)
        {
            _attributes.Add(new KeyValuePair<LazyString, LazyString>(name, value));
        }

        /// <summary>
        /// Sets the value of the last added attribute.
        /// </summary>
        /// <param name="value">The value to set.</param>
        public void SetAttributeValue(LazyString value)
        {
            _attributes[_attributes.Count - 1] = new KeyValuePair<LazyString, LazyString>(_attributes[_attributes.Count - 1].Key, value);
        }

        /// <summary>
        /// Gets the value of the attribute with the given name or an empty
        /// string if the attribute is not available.
        /// </summary>
        /// <param name="name">The name of the attribute.</param>
        /// <returns>The value of the attribute.</returns>
        public LazyString GetAttribute(String name)
        {
            for (var i = 0; i != _attributes.Count; i++)
            {
                if (_attributes[i].Key.Equals(name))
                    return _attributes[i].Value;
            }

            return LazyString.Empty;
        }

        #endregion
    }
}
