using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

public static class InputManager
{
   public static bool LeftClicked = false;
   public static bool LeftHold = false;

   private static Keys? currentKey = null;
   private static double keyRepeatTimer = 0;
   private static double initialDelay = 500; // en millisecondes
   private static double repeatRate = 50;    // délai entre répétitions



   private static MouseState ms = new MouseState(), oms;
   private static KeyboardState ks = new KeyboardState(), oks;
   private static GamePadState ps = new GamePadState(), ops;


   public static bool IsControllerConnected = false;


   public static void Update()
   {
      oms = ms;
      ms = Mouse.GetState();
      oks = ks;
      ks = Keyboard.GetState();
      ops = ps;
      ps = GamePad.GetState(PlayerIndex.One);


      IsControllerConnected = ps.IsConnected;

      LeftClicked = ms.LeftButton != ButtonState.Pressed && oms.LeftButton == ButtonState.Pressed;
      LeftHold = ms.LeftButton == ButtonState.Pressed && oms.LeftButton == ButtonState.Pressed;

   // true On left release like Windows buttons
   }

   public static bool GamePadButtonPressed(Buttons button)
   {
      return ps.IsButtonDown(button) && ops.IsButtonUp(button);
   }


   public static bool Hover(Rectangle r)
   {
      return r.Contains(new Vector2(ms.X,ms.Y));
   }

   public static bool IsPressed(Keys key){
      return ks.IsKeyDown(key) && oks.IsKeyUp(key);
   }

   public static bool IsHolding(Keys key) {
      return ks.IsKeyDown(key);
   }

   public static Vector2 GetMousePosition()
   {
        return new Vector2(ms.X, ms.Y);
   } 

   private static char? ConvertKeyToChar(Keys key)
   {
      if (key >= Keys.A && key <= Keys.Z)
         return (char)('a' + (key - Keys.A));
      if (key >= Keys.D0 && key <= Keys.D9)
         return (char)('0' + (key - Keys.D0));
      if (key == Keys.Space)
         return ' ';
      if (key == Keys.Back)
         return '\b';
      if (key == Keys.Left)
         return '\u2190'; // ←
      if (key == Keys.Right)
         return '\u2192'; // →
      return null;
   }


   public static char? GetPressedKeyRepeat(GameTime gameTime)
   {
      double elapsed = gameTime.ElapsedGameTime.TotalMilliseconds;

      foreach (Keys key in ks.GetPressedKeys())
      {
         if (oks.IsKeyUp(key)) // Nouvelle pression
         {
               currentKey = key;
               keyRepeatTimer = initialDelay;
               return ConvertKeyToChar(key);
         }
         else if (currentKey.HasValue && key == currentKey.Value)
         {
               keyRepeatTimer -= elapsed;
               if (keyRepeatTimer <= 0)
               {
                  keyRepeatTimer = repeatRate; // Répète la touche
                  return ConvertKeyToChar(key);
               }
         }
      }

      // Si la touche n'est plus maintenue
      if (currentKey.HasValue && !ks.IsKeyDown(currentKey.Value))
      {
         currentKey = null;
         keyRepeatTimer = 0;
      }

      return null;
   }

}