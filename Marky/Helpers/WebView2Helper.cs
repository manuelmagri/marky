using Microsoft.Web.WebView2.Wpf;
using System.Windows;

namespace Marky.Helpers
{
    public static class WebView2Helper
    {
        public static readonly DependencyProperty HtmlProperty = DependencyProperty.RegisterAttached(
            "Html",
            typeof(string),
            typeof(WebView2Helper),
            new PropertyMetadata(OnHtmlChanged)
        );


        public static string GetHtml(DependencyObject obj)
        {
            return (string)obj.GetValue(HtmlProperty);
        }

        public static void SetHtml(DependencyObject obj, string value)
        {
            obj.SetValue(HtmlProperty, value);
        }

        private static async void OnHtmlChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (obj is WebView2 webView)
            {
                await webView.EnsureCoreWebView2Async();

                string html = e.NewValue as string ?? "";

                webView.NavigateToString($@"
                    <html>
                    <head>
                        <meta charset='UTF-8'>
                    </head>
                    <body>
                        {html}
                    </body>
                    </html>");
            }
        }
    }
}
