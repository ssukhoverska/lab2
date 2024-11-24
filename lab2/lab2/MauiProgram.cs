using Microsoft.Extensions.Logging;
using System.Xml;
using System.Xml.Linq;

namespace lab2
{
    public static class MauiProgram
    {
        public class Book
        {
            public string Title { get; set; }
            public string Author { get; set; }
            public string Year { get; set; }
            public string Category { get; set; }
        }

        public interface IStrategy
        {
            List<Book> Search(Book book);
        }

        public class Searcher
        {
            private Book book;
            private IStrategy strategy;

            public Searcher(Book book, IStrategy strategy)
            {
                this.book = book;
                this.strategy = strategy;
            }

            public List<Book> SearchAlgorithm() 
            { 
                return strategy.Search(book);
            }
        }

        public class Sax : IStrategy
        {
            public List<Book> Search(Book book)
            {
                List<Book> results = new List<Book>();
                XmlTextReader xmlReader = new XmlTextReader(@"D:\XMLFile1.xml");

                while (xmlReader.Read())
                {
                    if (xmlReader.HasAttributes)
                    {
                        while (xmlReader.MoveToNextAttribute())
                        {
                            string title = "";
                            string author = "";
                            string year = "";
                            string category = "";

                            if (xmlReader.Name.Equals("Title") && (xmlReader.Value.Equals(book.Title) || book.Title == null))
                            {
                                title = xmlReader.Value;
                                xmlReader.MoveToNextAttribute();

                                if (xmlReader.Name.Equals("Author") && (xmlReader.Value.Equals(book.Author) || book.Author == null))
                                {
                                    author = xmlReader.Value;
                                    xmlReader.MoveToNextAttribute();

                                        if (xmlReader.Name.Equals("Year") && (xmlReader.Value.Equals(book.Year) || book.Year == null))
                                        {
                                            year = xmlReader.Value;
                                            xmlReader.MoveToNextAttribute();

                                            if (xmlReader.Name.Equals("Category") && (xmlReader.Value.Equals(book.Category) || book.Category == null))
                                            {
                                                category = xmlReader.Value;
                                            }
                                        }
                                    
                                }
                            }

                            if (title != "" && author != "" && year != "" && category != "")
                            {
                                Book myBook = new Book();
                                myBook.Title = title;
                                myBook.Author = author;
                                myBook.Year = year;
                                myBook.Category = category;
                                results.Add(myBook);
                            }
                        }
                    }
                }

                xmlReader.Close();
                return results;
            }
        }

        public class Dom : IStrategy
        {
            public List<Book> Search(Book book)
            {
                List<Book> results = new List<Book>();
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(@"D:\XMLFile1.xml");
                XmlNode node = xmlDoc.DocumentElement;

                foreach (XmlNode n in node.ChildNodes)
                {
                    string title = "";
                    string author = "";
                    string year = "";
                    string category = "";

                    foreach (XmlAttribute attribute in n.Attributes)
                    {
                        if (attribute.Name.Equals("Title") && (attribute.Value.Equals(book.Title) || book.Title == null))
                            title = attribute.Value;
                        if (attribute.Name.Equals("Author") && (attribute.Value.Equals(book.Author) || book.Author == null))
                            author = attribute.Value;
                        if (attribute.Name.Equals("Year") && (attribute.Value.Equals(book.Year) || book.Year == null))
                            year = attribute.Value;
                        if (attribute.Name.Equals("Category") && (attribute.Value.Equals(book.Category) || book.Category == null))
                            category = attribute.Value;
                    }

                    if (title != "" && author != "" && year != "" && category != "")
                    {
                        Book myBook = new Book();
                        myBook.Title = title;
                        myBook.Author = author;
                        myBook.Year = year;
                        myBook.Category = category;
                        results.Add(myBook);
                    }
                }
                return results;
            }
        }

        public class Linq : IStrategy
        {
            public List<Book> Search(Book book)
            {
                List<Book> results = new List<Book>();
                var xmlDoc = XDocument.Load(@"D:\XMLFile1.xml");
                var query = from obj in xmlDoc.Descendants("book")
                            where
                            (
                                (book.Title == null || (string)obj.Attribute("Title") == book.Title) &&
                                (book.Author == null || (string)obj.Attribute("Author") == book.Author) &&
                                (book.Year == null || (string)obj.Attribute("Year") == book.Year) &&
                                (book.Category == null || (string)obj.Attribute("Category") == book.Category)
                            )
                            select new Book
                            {
                                Title = (string)obj.Attribute("Title"),
                                Author = (string)obj.Attribute("Author"),
                                Year = (string)obj.Attribute("Year"),
                                Category = (string)obj.Attribute("Category")
                            };

                foreach (var b in query)
                {
                    results.Add(b);
                }

                return results;
            }
        }

        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
		builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}