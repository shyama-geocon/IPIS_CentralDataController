using IpisCentralDisplayController.models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace IpisCentralDisplayController.views
{
    /// <summary>
    /// Interaction logic for AddTextSlideWindow.xaml
    /// </summary>
    public partial class AddTextSlideWindow : Window
    {
        public TextSlideFile TextSlide { get; private set; }
        private readonly string _workspacePath;
        public AddTextSlideWindow(string workspacePath, int pxW, int pxH)
        {
            InitializeComponent();
            this.DataContext = new ColorViewModel();
            LoadFonts();
            _workspacePath = workspacePath;
            ud_height.Value = pxH;
            ud_width.Value = pxW;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            // Generate a unique file name for the text slide image
            string fileName = $"{Guid.NewGuid()}.png";

            // Save the canvas as a PNG file and get the file path
            string filePath = SaveCanvasAsPng(lb_canvas_bmp, fileName);

            // Create the TextSlideFile with the file path
            TextSlide = new TextSlideFile
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Text Slide",
                TextContent = tb_text_bmp.Text,
                FilePath = filePath, // Set the file path to the saved PNG file
                Created = DateTime.Now,
                Updated = DateTime.Now
            };

            DialogResult = true;
            Close();
        }

        private string SaveCanvasAsPng(Canvas canvas, string fileName)
        {
            canvas.Measure(new Size(canvas.Width, canvas.Height));
            canvas.Arrange(new Rect(new Size(canvas.Width, canvas.Height)));

            RenderTargetBitmap rtb = new RenderTargetBitmap((int)canvas.ActualWidth, (int)canvas.ActualHeight, 96, 96, PixelFormats.Pbgra32);
            rtb.Render(canvas);

            PngBitmapEncoder pngEncoder = new PngBitmapEncoder();
            pngEncoder.Frames.Add(BitmapFrame.Create(rtb));

            string filePath = System.IO.Path.Combine(_workspacePath, "Media", fileName);

            using (var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                pngEncoder.Save(fs);
            }

            return filePath;
        }

        private void LoadFonts()
        {
            foreach (var fontFamily in Fonts.SystemFontFamilies)
            {
                cb_font_bmp.Items.Add(fontFamily);
            }

            cb_fontstyle_bmp.Items.Add(FontStyles.Italic);
            cb_fontstyle_bmp.Items.Add(FontStyles.Normal);
            cb_fontstyle_bmp.Items.Add(FontStyles.Oblique);
        }

        private void cb_font_selectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            tb_canvas_bmp.FontFamily = (FontFamily)cb_font_bmp.SelectedItem;
        }

        private void cb_fontStyle_selectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            tb_canvas_bmp.FontStyle = (FontStyle)cb_fontstyle_bmp.SelectedItem;
        }

        private void cb_halign_selectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (tb_canvas_bmp != null)
            {
                tb_canvas_bmp.TextAlignment = (TextAlignment)cb_halign_bmp.SelectedItem;
            }
        }

        private void cb_valign_selectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (tb_canvas_bmp != null)
            {
                tb_canvas_bmp.VerticalAlignment = (VerticalAlignment)cb_valign_bmp.SelectedItem;
            }
        }
    }
}
