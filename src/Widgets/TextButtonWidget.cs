
using System;
using TheEnd;
using Microsoft.Xna.Framework;

public class TextButtonWidget : ButtonWidget
{
    public string Content {get{return _textWidget.Content;} set{_textWidget.Content = value;}}

    public TextButtonWidget(Rectangle rect, string text, Color? color = null, Color? backgroundColor = null, Action OnClick = null, Action OnHover = null, bool debug = false) : base(rect, backgroundAsset: null, text:new TextWidget(text, color: color), backgroundColor: backgroundColor, hover:true,backgroundHoverAsset:null, OnClick:OnClick, OnHover: OnHover, debug: debug) {
    }

    public TextButtonWidget(Size size, string text, Color? color = null, Color? backgroundColor = null, Action OnClick = null, Action OnHover = null, bool debug = false) : this(new Rectangle(0,0,size.Width, size.Height), text:text, color, backgroundColor, OnClick, OnHover, debug) {

    }

    public TextButtonWidget(Rectangle rect, TextWidget text, Color? color = null, Color? backgroundColor = null, Action OnClick = null, Action OnHover = null, bool debug = false) : base(rect, null, text:text, backgroundColor: backgroundColor, hover:true,backgroundHoverAsset:null, OnClick:OnClick, OnHover: OnHover, debug: debug) {
    }

    public TextButtonWidget(Size size, TextWidget text, Color? color = null, Color? backgroundColor = null, Action OnClick = null, Action OnHover = null, bool debug = false) : this(new Rectangle(0,0,size.Width, size.Height), text, color, backgroundColor, OnClick, OnHover, debug) 
    {}

    public TextButtonWidget(TextWidget text, Color? color = null, Color? backgroundColor = null, Action OnClick = null, Action OnHover = null, bool debug = false) : this(Rectangle.Empty, text, color, backgroundColor, OnClick, OnHover, debug) 
    {}
}