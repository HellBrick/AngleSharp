namespace AngleSharp.Dom
{
    using System;
    using System.IO;
    using Text;

    /// <summary>
    /// The base class for all characterdata implementations.
    /// </summary>
    abstract class CharacterData : Node, ICharacterData
    {
        #region Fields

        private LazyString _content;

        #endregion

        #region ctor

        internal CharacterData(Document owner, String name, NodeType type)
            : this(owner, name, type, String.Empty)
        {
        }

        internal CharacterData(Document owner, String name, NodeType type, String content)
            : this(owner, name, type, new LazyString(content))
        {
        }

        internal CharacterData(Document owner, String name, NodeType type, LazyString content)
            : base(owner, name, type)
        {
            _content = content;
        }

        #endregion

        #region Properties

        private String ContentValue => (_content = _content.EnsureValue()).Value;

        public IElement PreviousElementSibling
        {
            get
            {
                var parent = Parent;

                if (parent != null)
                {
                    var found = false;

                    for (var i = parent.ChildNodes.Length - 1; i >= 0; i--)
                    {
                        if (Object.ReferenceEquals(parent.ChildNodes[i], this))
                        {
                            found = true;
                        }
                        else if (found && parent.ChildNodes[i] is IElement)
                        {
                            return (IElement)parent.ChildNodes[i];
                        }
                    }
                }

                return null;
            }
        }

        public IElement NextElementSibling
        {
            get
            {
                var parent = Parent;
                
                if (parent != null)
                {
                    var n = parent.ChildNodes.Length;
                    var found = false;

                    for (var i = 0; i < n; i++)
                    {
                        if (Object.ReferenceEquals(parent.ChildNodes[i], this))
                        {
                            found = true;
                        }
                        else if (found && parent.ChildNodes[i] is IElement)
                        {
                            return (IElement)parent.ChildNodes[i];
                        }
                    }
                }

                return null;
            }
        }

        internal Char this[Int32 index]
        {
            get { return _content[index]; }
            set 
            {
                if (index >= 0)
                {
                    if (index >= Length)
                    {
                        if (_content.HasValue)
                        {
                            _content = new LazyString(_content.Value.PadRight(index) + value.ToString());
                        }
                        else
                        {
                            _content.Builder.Append( ' ', index - Length - 1 );
                            _content.Builder.Append( value );
                        }
                    }
                    else
                    {
                        if (_content.HasValue)
                        {
                            var chrs = _content.Value.ToCharArray();
                            chrs[index] = value;
                            _content = new LazyString(new String(chrs));
                        }
                        else
                        {
                            _content.Builder[ index ] = value;
                        }
                    }
                }
            }
        }

        public Int32 Length 
        { 
            get { return _content.Length; } 
        }

        public sealed override String NodeValue
        {
            get { return Data; }
            set { Data = value; }
        }

        public sealed override String TextContent
        {
            get { return Data; }
            set { Data = value; }
        }

        public String Data
        {
            get { return ContentValue; }
            set { Replace(0, Length, value); }
        }

        #endregion

        #region Methods

        public String Substring(Int32 offset, Int32 count)
        {
            var length = _content.Length;

            if (offset > length)
                throw new DomException(DomError.IndexSizeError);

            if (offset + count > length)
            {
                return ContentValue.Substring(offset);
            }

            return ContentValue.Substring(offset, count);
        }

        public void Append(String value)
        {
            Replace(_content.Length, 0, value);
        }

        public void Insert(Int32 offset, String data)
        {
            Replace(offset, 0, data);
        }

        public void Delete(Int32 offset, Int32 count)
        {
            Replace(offset, count, String.Empty);
        }

        public void Replace(Int32 offset, Int32 count, String data)
        {
            var owner = Owner;
            var length = _content.Length;

            if (offset > length)
                throw new DomException(DomError.IndexSizeError);

            if (offset + count > length)
            {
                count = length - offset;
            }
            
            owner.QueueMutation(self => MutationRecord.CharacterData(target: self, previousValue: self.ContentValue), this);

            var deleteOffset = offset + data.Length;
            _content = _content.Insert(offset, data);

            if (count > 0)
            {
                _content = _content.Remove(deleteOffset, count);
            }

            owner.ForEachRange(m => m.Head == this && m.Start > offset && m.Start <= offset + count, m => m.StartWith(this, offset));
            owner.ForEachRange(m => m.Tail == this && m.End > offset && m.End <= offset + count, m => m.EndWith(this, offset));
            owner.ForEachRange(m => m.Head == this && m.Start > offset + count, m => m.StartWith(this, m.Start + data.Length - count));
            owner.ForEachRange(m => m.Tail == this && m.End > offset + count, m => m.EndWith(this, m.End + data.Length - count));
        }
        
        public override void ToHtml(TextWriter writer, IMarkupFormatter formatter)
        {
            writer.Write(formatter.Text(ContentValue));
        }

        public void Before(params INode[] nodes)
        {
            this.InsertBefore(nodes);
        }

        public void After(params INode[] nodes)
        {
            this.InsertAfter(nodes);
        }

        public void Replace(params INode[] nodes)
        {
            this.ReplaceWith(nodes);
        }

        public void Remove()
        {
            this.RemoveFromParent();
        }

        #endregion
    }
}
