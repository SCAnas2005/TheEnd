
using Microsoft.Xna.Framework.Graphics;

public class DialogLine
{
    public string Speaker { get; set; }
    public string Text { get; set; }
    public Texture2D Portrait { get; set; }

    public DialogLine(string speaker, string text, Texture2D portrait = null)
    {
        Speaker = speaker;
        Text = text;
        Portrait = portrait;
    }
}