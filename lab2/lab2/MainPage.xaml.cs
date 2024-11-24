using System.Xml;
using System.Xml.Xsl;

namespace lab2
{
    public partial class MainPage : ContentPage
    {
        

        public MainPage()
        {
            InitializeComponent();
            GetAllSongs();
        }

        private void GetAllSongs()
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(@"C:\Users\Admin\Documents\XMLFile1.xml");
            XmlElement el = xmlDoc.DocumentElement;
            XmlNodeList childNodes = el.SelectNodes("book");

            foreach (XmlNode n in childNodes)
            {
                string title = n.SelectSingleNode("@Title")?.Value;
                string author = n.SelectSingleNode("@Author")?.Value;
                string year = n.SelectSingleNode("@Year")?.Value;
                string category = n.SelectSingleNode("@Category")?.Value;

                if (!string.IsNullOrEmpty(title) && !TitlePicker.Items.Contains(title))
                    TitlePicker.Items.Add(title);

                if (!string.IsNullOrEmpty(author) && !AuthorPicker.Items.Contains(author))
                    AuthorPicker.Items.Add(author);

                if (!string.IsNullOrEmpty(year) && !YearPicker.Items.Contains(year))
                    YearPicker.Items.Add(year);
                if (!string.IsNullOrEmpty(category) && !CategoryPicker.Items.Contains(category))
                    CategoryPicker.Items.Add(category);
            }
        }


        private void OnSearchButtonClicked(object sender, EventArgs e)
            {
                editor.Text = "";

                MauiProgram.Book book = GetSelectedParameters();
                MauiProgram.IStrategy analyzer = GetSelectedAnalyzer();
                PerformSearch(book, analyzer);
            }

            private MauiProgram.Book GetSelectedParameters()
            {
                MauiProgram.Book book = new MauiProgram.Book();

                if (TitleCheckBox.IsChecked)
                    book.Title = TitlePicker.SelectedItem.ToString();

                if (AuthorCheckBox.IsChecked)
                    book.Author = AuthorPicker.SelectedItem.ToString();

                if (YearCheckBox.IsChecked)
                    book.Year = YearPicker.SelectedItem.ToString();

                if (CategoryCheckBox.IsChecked)
                    book.Category = CategoryPicker.SelectedItem.ToString();

                return book;
            }

            private MauiProgram.IStrategy GetSelectedAnalyzer()
            {
                MauiProgram.IStrategy analyzer = new MauiProgram.Sax();

                if (DomRadioButton.IsChecked)
                    analyzer = new MauiProgram.Dom();

                if (LinqRadioButton.IsChecked)
                    analyzer = new MauiProgram.Linq();

                return analyzer;
            }

            private void PerformSearch(MauiProgram.Book book, MauiProgram.IStrategy analyzer)
            {
                MauiProgram.Searcher search = new MauiProgram.Searcher(book, analyzer);
                List<MauiProgram.Book> results = search.SearchAlgorithm();

                foreach (MauiProgram.Book s in results)
                {
                    editor.Text += "Назва: " + s.Title + "\n";
                    editor.Text += "Автор: " + s.Author + "\n";
                    editor.Text += "Рік: " + s.Year + "\n";
                    editor.Text += "Категорія: " + s.Category + "\n";
                    editor.Text += "\n";
                }
            }

            private void OnClearButtonClicked(object sender, EventArgs e)
            {
                editor.Text = "";
                SaxRadioButton.IsChecked = false;
                LinqRadioButton.IsChecked = false;
                DomRadioButton.IsChecked = false;
                TitleCheckBox.IsChecked = false;
                AuthorCheckBox.IsChecked = false;
                YearCheckBox.IsChecked = false;
                CategoryCheckBox.IsChecked = false;
                TitlePicker.SelectedItem = null;
                AuthorPicker.SelectedItem = null;
                YearPicker.SelectedItem = null;
                CategoryPicker.SelectedItem = null;
            }

        private void OnTransformToHTMLButtonClicked(object sender, EventArgs e)
        {
            XslCompiledTransform xct = LoadXSLT();
            string xmlPath = @"D:\XMLFile1.xml";
            string htmlPath = @"D:\html.html";

            XsltArgumentList xslArgs = CreateXSLArguments();

            TransformXMLtoHTML(xct, xslArgs, xmlPath, htmlPath);
        }

        private void TransformXMLtoHTML(XslCompiledTransform xct, XsltArgumentList xslArgs, string xmlPath, string htmlPath)
        {
            using (XmlReader xr = XmlReader.Create(xmlPath))
            {
                using (XmlWriter xw = XmlWriter.Create(htmlPath))
                {
                    xct.Transform(xr, xslArgs, xw);
                }
            }
        }

        private XsltArgumentList CreateXSLArguments()
        {
            XsltArgumentList xslArgs = new XsltArgumentList();

            // Ваш код для визначення значень параметрів

            string title = TitleCheckBox.IsChecked ? TitlePicker.SelectedItem.ToString() : null;
            string author = AuthorCheckBox.IsChecked ? AuthorPicker.SelectedItem.ToString() : null;
            string year = YearCheckBox.IsChecked ? YearPicker.SelectedItem.ToString() : null;
            string category = CategoryCheckBox.IsChecked ? CategoryPicker.SelectedItem.ToString() : null;

            if (title != null)
                xslArgs.AddParam("title", "", title);

            if (author != null)
                xslArgs.AddParam("author", "", author);

            if (year != null)
                xslArgs.AddParam("year", "", year);

            if (category != null)
                xslArgs.AddParam("category", "", category);

            return xslArgs;
        }

        private XslCompiledTransform LoadXSLT()
        {
            XslCompiledTransform xct = new XslCompiledTransform();
            xct.Load(@"D:\xslFile.xsl");
            return xct;
        }


        private async void OnExitButtonClicked(object sender, EventArgs e)
        {
            var result = await Application.Current.MainPage.DisplayAlert("Exit", "Are you want exit?", "Yes", "No");
            if (result) 
            {
                System.Diagnostics.Process.GetCurrentProcess().CloseMainWindow();
            }
        }

        private void Editor_TextChanged(object sender, EventArgs e)
        {
            int textLength = editor.Text.Length;
            int fontSize = CalculateFontSize(textLength);
            editor.FontSize = fontSize;
        }

        private int CalculateFontSize(int textLength)
        {
            if (textLength < 100)
                return 20;

            else if (textLength < 500 && textLength > 100)
                return 15;

            else
                return 10;

        }

    }
        
}