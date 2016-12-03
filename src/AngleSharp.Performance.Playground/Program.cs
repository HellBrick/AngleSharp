using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;

namespace AngleSharp.Performance.Playground
{
	class Program
	{
		static void Main( string[] args )
		{
			HtmlParser parser = new HtmlParser();

			for ( int i = 0; i < 1000; i++ )
			{
				using ( IHtmlDocument document = parser.ParseDocument( TestData.TorrentSite ) )
				{
					foreach ( IElement link in document.Links )
					{
					}

					foreach ( IHtmlScriptElement script in document.Scripts )
					{
					}
				}
			}
		}
	}
}
