using System;
using System.Linq;
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
      return r.Contains(new Vector2(ms.X, ms.Y));
   }

   public static bool IsPressed(Keys key)
   {
      return ks.IsKeyDown(key) && oks.IsKeyUp(key);
   }

   public static bool IsHolding(Keys key)
   {
      return ks.IsKeyDown(key);
   }

   public static Vector2 GetMousePosition()
   {
      return new Vector2(ms.X, ms.Y);
   }

   public static Vector2 GetPreviousMousePosition() => new Vector2(oms.X, oms.Y);

    private static char? ConvertKeyToChar(Keys key)
    {
        // Récupère l'état du clavier pour savoir si Shift est pressé
        var ks = Keyboard.GetState();
        bool shift = ks.IsKeyDown(Keys.LeftShift) || ks.IsKeyDown(Keys.RightShift);
        bool caps = Console.CapsLock;

        // Lettres A-Z -> gère Majuscule via Shift ^ CapsLock
        if (key >= Keys.A && key <= Keys.Z)
        {
            char c = (char)('a' + (key - Keys.A));
            if (shift ^ caps) c = char.ToUpperInvariant(c);
            return c;
        }

        // Chiffres de la rangée supérieure (0-9) + symboles shiftés
        if (key >= Keys.D0 && key <= Keys.D9)
        {
            string normal = "0123456789";
            string shifted = ")!@#$%^&*(";
            int idx = key - Keys.D0;
            return shift ? shifted[idx] : normal[idx];
        }

        // Pavé numérique
        if (key >= Keys.NumPad0 && key <= Keys.NumPad9)
            return (char)('0' + (key - Keys.NumPad0));

        // Touches spéciales simples
        if (key == Keys.Space) return ' ';
        if (key == Keys.Enter) return '\n';
        if (key == Keys.Tab) return '\t';
        if (key == Keys.Back) return '\b';         // Backspace
        if (key == Keys.Left) return '\u2190';     // ←
        if (key == Keys.Right) return '\u2192';    // →
        if (key == Keys.Delete) return '\u007F';   // DEL (optionnel : tu peux gérer Delete séparément)

        // Mapping des touches OEM / symboles (approche générale, US/QWERTY-centric)
        switch (key)
        {
            case Keys.OemComma:         return shift ? '<' : ',';
            case Keys.OemPeriod:        return shift ? '>' : '.';
            case Keys.OemSemicolon:     return shift ? ':' : ';';
            case Keys.OemQuotes:        return shift ? '"' : '\'';
            case Keys.OemMinus:         return shift ? '_' : '-';
            case Keys.OemPlus:          return shift ? '+' : '=';
            case Keys.OemOpenBrackets:  return shift ? '{' : '[';
            case Keys.OemCloseBrackets: return shift ? '}' : ']';
            case Keys.OemPipe:          return shift ? '|' : '\\';
            case Keys.OemTilde:         return shift ? '~' : '`';
            case Keys.OemQuestion:      return shift ? '?' : '/';  // si présent dans l'enum
            case Keys.OemBackslash:     return shift ? '|' : '\\';
            // Ajoute d'autres Keys.Oem* si nécessaire selon ta plate-forme/enum
            default:
                return null; // touche non gérée ici
        }
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
   
   public static bool AreKeysPressedTogether(params Keys[] keys)
   {
      return keys.All(k => ks.IsKeyDown(k));
   }

}