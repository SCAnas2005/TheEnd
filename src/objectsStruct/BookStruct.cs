
using System.Collections.Generic;
using System.Linq;

public class BookStruct
{
    public List<BookStructPage> Pages;
    public int Count { get => Pages.Count; }
    public int CurrentPageIndex = 1;


    public BookStruct(int n = 0)
    {
        Pages = [];
        CurrentPageIndex = 1;
        for (int i = 1; i <= n; i++)
        {
            Pages.Add(new BookStructPage(i));
        }

        if (Count > 1) CurrentPageIndex = 1;
    }

    public BookStructPage GetPage(int n)
    {
        if (n - 1 < 0 || n - 1 > Count) return null;
        return Pages[n - 1];
    }

    public void AddPage(string title = "", string content = "", string foot = "")
    {
        Pages.Add(new BookStructPage(Count + 1, title, content, foot));
    }


    public int GetLastPageNumber()
    {
        return Pages.Last().PageNumber;
    }

    public BookStructPage GetCurrentPage()
    {
        return GetPage(CurrentPageIndex);
    }

    public BookStructPage NextPage()
    {
        if (CurrentPageIndex >= Count) return null;
        CurrentPageIndex++;
        return GetPage(CurrentPageIndex);
    }

    public BookStructPage LastPage()
    {
        if (CurrentPageIndex-1 < 1) return null;
        CurrentPageIndex--;
        return GetPage(CurrentPageIndex);
    }
}

public class BookStructPage
{
    public int PageNumber;
    public string Title;
    public string Content;
    public string Foot;

    public BookStructPage(int n, string title = "", string content = "", string foot = "")
    {
        PageNumber = n;
        Title = title;
        Content = content;
        Foot = foot;
    }

}