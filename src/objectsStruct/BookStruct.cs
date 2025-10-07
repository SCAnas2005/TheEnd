
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

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

    public static BookStruct LoadFromJson(string path)
    {
        var jsonFile = File.ReadAllText(path);
        var data = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(jsonFile);
        var bookstruct = new BookStruct(data["pagesNumber"].GetInt32());

        if (!data.TryGetValue("pages", out var pagesElem))
        {
            throw new Exception("Le JSON ne contient pas la clé 'pages' !");
        }

        foreach (var pageElem in pagesElem.EnumerateArray())
        {
            var pageDict = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(pageElem.GetRawText());

            string title = pageDict.ContainsKey("title:") ? pageDict["title:"].GetString() : "";
            string content = pageDict["content"].GetString();
            string foot = pageDict["foot"].GetString();
            int index = pageDict["pageIndex"].GetInt32();

            bookstruct.EditPage(index, title, content, foot);
        }

        return bookstruct;
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
        if (CurrentPageIndex - 1 < 1) return null;
        CurrentPageIndex--;
        return GetPage(CurrentPageIndex);
    }

    public BookStructPage EditPage(int index, string title = null, string content = null, string foot = null, int? pageIndex = null)
    {
        var page = GetPage(index);
        if (page == null) return null;

        page.Title = title ?? page.Title;
        page.Content = content ?? page.Content;
        page.Foot = foot ?? page.Foot;
        page.PageNumber = pageIndex ?? page.PageNumber;

        return page;
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