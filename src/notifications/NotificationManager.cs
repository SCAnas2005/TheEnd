using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using TheEnd;

public static class NotificationManager
{
    private static List<Notification> notifications = new();
    public static int VerticalSpacing = 30;

    private static ContainerWidget container;

    public static void Init()
    {
        notifications = new();
    }

    private static void BuildWidgets()
    {
        List<Widget> w = [];
        foreach (var notification in notifications)
        {
            float alpha = MathF.Min(1f, notification.TimeRemaining / 0.8f); // fade in last 0.5s
            Color color = Color.White * alpha;
            w.Add(new TextWidget(
                notification.Text,
                font: notification.Font,
                color: color
            ));
            w.Add(new SizedBox(height: VerticalSpacing));
        }

        container = new ContainerWidget(
            rect: new Rectangle(0, 0, (int)(Camera2D.CameraLogicalSize.Width * Globals.TileScale), (int)(Camera2D.CameraLogicalSize.Height * Globals.TileScale)),
            padding: new Padding(50),
            alignItem: Align.Vertical,
            widgets: w.ToArray()
        );
    }

    public static void Add(string text, SpriteFont font = null)
    {
        notifications.Add(new Notification(text, font));
    }

    public static void Add(Notification notification)
    {
        notifications.Add(notification);
    }

    public static void Update(GameTime gameTime)
    {
        for (int i = notifications.Count - 1; i >= 0; i--)
        {
            notifications[i].Update(gameTime);
            if (notifications[i].IsExpired)
            {
                notifications.RemoveAt(i);
            }
        }
    }

    public static void Draw(SpriteBatch _spriteBatch)
    {
        if (notifications.Count == 0) return;
        BuildWidgets();
        container.Load(Globals.Content);
        container.Draw(_spriteBatch);
    }
}
