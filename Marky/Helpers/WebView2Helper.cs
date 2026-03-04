using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Web.WebView2.Wpf;
using System.Windows;
using System.Security.Policy;

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



        public static string GetHtml(DependencyObject obj) { }

        public static void SetHtml(DependencyObject obj, string value) { }

        private static async void OnHtmlChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) { }
    }
}
