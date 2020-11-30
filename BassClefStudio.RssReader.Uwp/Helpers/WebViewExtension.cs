using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace BassClefStudio.RssReader.Uwp.Helpers
{
    public class WebViewExtention
    {
        public static readonly DependencyProperty HtmlSourceProperty =
               DependencyProperty.RegisterAttached("HtmlSource", typeof(string), typeof(WebViewExtention), new PropertyMetadata("", OnHtmlSourceChanged));
        public static string GetHtmlSource(DependencyObject obj) { return (string)obj.GetValue(HtmlSourceProperty); }
        public static void SetHtmlSource(DependencyObject obj, string value) { obj.SetValue(HtmlSourceProperty, value); }
        private static void OnHtmlSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is WebView webView)
            {
                if (e.NewValue is string content)
                {
                    webView.NavigateToString(content);
                }
            }
        }
    }
}
