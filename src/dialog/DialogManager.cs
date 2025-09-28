
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

public class DialogManager
{
    public static DialogManager Instance = new DialogManager();
    private Queue<DialogLine> _dialogQueue;
    private bool _active = false;

    private Widget _dialogWidget = null;
    private TimerHandle _handler;

    public bool IsActive => _active;

    private DialogManager()
    {
        _dialogQueue = new Queue<DialogLine>();
    }

    public void StartDialog(List<DialogLine> lines)
    {
        Console.WriteLine("Dialog started with : " + lines.Count + " lines");
        _active = true;
        _dialogQueue = new Queue<DialogLine>(lines);
        ShowNextDialog();
    }

    public void ShowNextDialog()
    {
        if (_dialogQueue.Count == 0)
        {
            _active = false;
            _dialogWidget = null;
            return;
        }


        var line = _dialogQueue.Dequeue();
        var width = Globals.FullScreenRect.Width;
        var height = Globals.FullScreenRect.Height;

        _handler = TimerManager.Wait(3);

        _dialogWidget = new DialogWidget(
            rect: new Rectangle(50, height - 50 - 250, width - 100, 250),
            name: line.Speaker,
            text: line.Text,
            profilePicture: line.Portrait
        );

        _dialogWidget.Load(Globals.Content);
    }


    public void Update(GameTime gameTime)
    {
        if (!_active) return;
        _dialogWidget.Update(gameTime);
        if (InputManager.IsPressed(Keys.Enter) || _handler.IsDone)
        {
            ShowNextDialog();
        }
    }

    public void Draw(SpriteBatch _spriteBatch)
    {
        if (!_active) return;
        _dialogWidget.Draw(_spriteBatch);
    }

}