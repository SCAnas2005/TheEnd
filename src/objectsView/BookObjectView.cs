
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class BookObjectView
{
    public static BookObjectView Instance;

    public BookStruct Book = null;

    private bool show = false;

    private ContainerWidget _container;
    private ContainerWidget _title;
    private ContainerWidget _body;
    private ContainerWidget _foot;


    public void Show()
    {
        show = true;
        Build();
    }

    public void Close()
    {
        show = false;
    }


    public void Build()
    {
        BookStructPage page = Book.GetCurrentPage();
        int firstPageIndex = 1;
        int lastPageIndex = Book.GetLastPageNumber();


        _title = new ContainerWidget(
            size: new Size(460, 50),
            mainAxisAlignment: MainAxisAlignment.Start,
            crossAxisAlignment: CrossAxisAlignment.Start,
            widgets: [
                new SizedBox(
                    new Size(460, 50),
                    child: page != null ? new TextWidget(page.Title) : new SizedBox()
                ),
            ]
        );

        _body = new ContainerWidget(
            size: new Size(460, 580),
            mainAxisAlignment: MainAxisAlignment.Start,
            crossAxisAlignment: CrossAxisAlignment.Start,
            widgets: [
                new SizedBox(
                    new Size(460, 580),
                    child: page != null ? new TextWidget(page.Content) : new SizedBox()
                ),
            ]
        );

        _foot = new ContainerWidget(
            size: new Size(460, 50),
            mainAxisAlignment: MainAxisAlignment.Start,
            crossAxisAlignment: CrossAxisAlignment.Start,
            widgets: [
                new SizedBox(
                    new Size(460, 50),
                    child: page != null ? new TextWidget(page.Foot) : new SizedBox()
                ),
                
            ]
        );
        _container = new ContainerWidget(
            rect: new Rectangle(500, 150, 500, 700),
            backgroundColor: Color.Gray,
            widgets: [
                new ContainerWidget(
                    size: new Size(20, 700),
                    widgets: [
                        new IconButtonWidget(
                            size: new Size(20),
                            src: "Icons/leftarrow",
                            color: Book.CurrentPageIndex == firstPageIndex ? Color.Gray : null,
                            OnClick: () => {
                                if (Book.CurrentPageIndex > firstPageIndex)
                                {
                                    Book.LastPage();
                                    Build();
                                }
                            }
                        ),
                    ]
                ),
                new ContainerWidget(
                    size: new Size(460, 700),
                    alignItem: Align.Vertical,
                    widgets: [
                        _title,
                        _body,
                        _foot,
                        new ContainerWidget(
                            size: new Size(460, 20),
                            mainAxisAlignment: MainAxisAlignment.Center,
                            widgets: [
                                new TextWidget($"{Book.CurrentPageIndex}/{lastPageIndex}"),
                            ]
                        ),
                    ]
                ),
                new ContainerWidget(
                    size: new Size(20, 700),
                    widgets: [
                        new IconButtonWidget(
                            size: new Size(20),
                            src: "Icons/rightarrow",
                            color: Book.CurrentPageIndex == lastPageIndex ? Color.Gray : null,
                            OnClick: () => {
                                if (Book.CurrentPageIndex < lastPageIndex)
                                {
                                    Book.NextPage();
                                    Build();
                                }
                            }
                        ),
                    ]
                ),
            ]
        );

        _container.Load(Globals.Content);
    }


    public void Update(GameTime gameTime)
    {
        if (!show) return;
        _container.Update(gameTime);
    }

    public void Draw(SpriteBatch _spriteBatch)
    {
        if (!show) return;

        _spriteBatch.End();
        _spriteBatch.Begin();

        _container.Draw(_spriteBatch);

        _spriteBatch.End();
        SpriteBatchContext.ApplyToContext(_spriteBatch, SpriteBatchContext.Top);
    }
}