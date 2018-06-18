using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Data;
using TheArtOfDev.HtmlRenderer.WPF;

namespace GrandTheftMultiplayer.Launcher.Converters
{
    public class HtmlPanelDisplayConverter : IMultiValueConverter
    {
        private HtmlPanel _htmlPanel;
        
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length != 2)
                throw new ArgumentException("Expected 2 values.", nameof(values));

            var show = values[0] as bool? ?? false;

            if (!show)
            {
                if (_htmlPanel != null)
                    ReleaseHtmlPanelResources();

                return null;
            }

            var text = values[1] as string;
            if (text == null) return null;

            // Sorry, I was drunk. Parsed HTML with RegEx...
            var regex = new Regex(@"(<br\/?>\s*){2,}|(<\w+[^>]*>[^<]{0}<br[^>]*>[^<]{0}<\/\w+>)");
            text = regex.Replace(text, string.Empty);

            _htmlPanel = new HtmlPanel { Text = text };

            return _htmlPanel;
        }

        private void ReleaseHtmlPanelResources()
        {
            _htmlPanel.Text = null;
            _htmlPanel = null;
            DelayGarbageCollect();
        }

        private static async void DelayGarbageCollect()
        {
            await Task.Delay(1000);
            GC.Collect(999, GCCollectionMode.Forced);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}