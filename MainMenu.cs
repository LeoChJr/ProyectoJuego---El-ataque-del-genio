using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ProyectoJuego
{
    // Clase que gestiona el Menú Principal del juego
    public class MainMenu
    {
        // Variables privadas
        private Texture2D buttonTexture; // Textura del botón (aspecto visual del botón)
        private SpriteFont font;         // Fuente que se utilizará para dibujar el texto en los botones
        private Rectangle playButton;    // Área del botón "Jugar" en la pantalla
        private Rectangle exitButton;    // Área del botón "Salir" en la pantalla
        private Action startGameAction;  // Acción que se ejecutará cuando el jugador haga clic en "Jugar"
        private Action exitAction;       // Acción que se ejecutará cuando el jugador haga clic en "Salir"
        private Texture2D backgroundTexture; // Textura del fondo del menú principal

        // Constructor del menú principal
        public MainMenu(Texture2D buttonTexture, SpriteFont font, Texture2D backgroundTexture, Action startGameAction, Action exitAction)
        {
            this.buttonTexture = buttonTexture;          // Asigna la textura de los botones
            this.font = font;                            // Asigna la fuente
            this.backgroundTexture = backgroundTexture;  // Asigna la textura de fondo del menú
            this.startGameAction = startGameAction;      // Asigna la acción para empezar el juego
            this.exitAction = exitAction;                // Asigna la acción para salir del juego

            // Define las posiciones y tamaños de los botones en la pantalla
            playButton = new Rectangle(525, 550, 200, 50); // Botón "Jugar" (x, y, ancho, alto)
            exitButton = new Rectangle(525, 650, 200, 50); // Botón "Salir" (x, y, ancho, alto)
        }

        // Método que actualiza la lógica del menú en cada frame
        public void Update(GameTime gameTime)
        {
            // Obtiene el estado actual del mouse
            MouseState mouseState = Mouse.GetState();

            // Verifica si el botón izquierdo del mouse está presionado
            if (mouseState.LeftButton == ButtonState.Pressed)
            {
                // Si el mouse está sobre el botón "Jugar" y es presionado
                if (playButton.Contains(mouseState.Position))
                {
                    startGameAction?.Invoke(); // Ejecuta la acción para iniciar el juego
                }
                // Si el mouse está sobre el botón "Salir" y es presionado
                else if (exitButton.Contains(mouseState.Position))
                {
                    exitAction?.Invoke(); // Ejecuta la acción para salir del juego
                }
            }
        }

        // Método que dibuja el menú en la pantalla
        public void Draw(SpriteBatch spriteBatch)
        {
            // Dibuja la textura de fondo del menú principal
            spriteBatch.Draw(backgroundTexture, new Rectangle(0, 0, 1280, 1000), Color.White);

            // Dibuja los botones en la pantalla con su respectiva textura
            spriteBatch.Draw(buttonTexture, playButton, Color.White); // Dibuja el botón "Jugar"
            spriteBatch.Draw(buttonTexture, exitButton, Color.White); // Dibuja el botón "Salir"

            // Posiciones para el texto "Jugar" y "Salir" dentro de los botones
            Vector2 playTextPosition = new Vector2(playButton.X + 95, playButton.Y + 15);
            Vector2 exitTextPosition = new Vector2(exitButton.X + 95, exitButton.Y + 15);

            try
            {
                // Dibuja el texto "Jugar" y "Salir" en los botones
                spriteBatch.DrawString(font, "Jugar", playTextPosition, Color.Black);
                spriteBatch.DrawString(font, "Salir", exitTextPosition, Color.Black);
            }
            catch (ArgumentException)
            {
                // Si ocurre una excepción, usa texto en mayúsculas como alternativa
                spriteBatch.DrawString(font, "JUGAR", playTextPosition, Color.Black);
                spriteBatch.DrawString(font, "SALIR", exitTextPosition, Color.Black);
            }
        }
    }
}
