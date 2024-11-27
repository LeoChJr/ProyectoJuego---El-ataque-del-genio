using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ProyectoJuego.Content;

using Color = Microsoft.Xna.Framework.Color;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace ProyectoJuego
{
    // Enum que define los posibles estados del juego
    public enum GameState
    {
        MainMenu,  // Menú principal
        Playing,   // Jugando
        Paused,    // Pausado
        GameOver   // Fin del juego
    }

    public class Game1 : Game
    {
        private float _backgroundOffsetY; // Desplazamiento vertical del fondo
        private float _backgroundSpeed;   // Velocidad de desplazamiento del fondo

        // Variables principales del juego
        private GraphicsDeviceManager _graphics;  // Configuración gráfica
        private SpriteBatch _spriteBatch;         // Dibujo de texturas
        private MainMenu _mainMenu;               // Menú principal
        private Jugador _jugador;                 // Jugador
        private GameState _currentState;          // Estado actual del juego
        private Texture2D _gameBackgroundTexture; // Textura de fondo del juego
        private Enemigo _enemigo;                 // Enemigo
        private SpriteFont font;                  // Fuente para texto

        // Constructor del juego
        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this); // Inicializa la configuración gráfica
            Content.RootDirectory = "Content";           // Directorio del contenido
            IsMouseVisible = true;                       // Muestra el cursor
            _graphics.PreferredBackBufferWidth = 1280;   // Ancho de pantalla
            _graphics.PreferredBackBufferHeight = 1000;  // Alto de pantalla
            _graphics.ApplyChanges();                   // Aplica los cambios
        }

        // Método para cargar el contenido (texturas, fuentes, etc.)
        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice); // Inicializa SpriteBatch
            _backgroundOffsetY = 0f;  // Comienza en la parte superior
            _backgroundSpeed = 2f;    // Velocidad inicial del fondo

            // Carga de texturas
            Texture2D enemyTexture = Content.Load<Texture2D>("autoPolicia2");
            Texture2D fireballTexture = Content.Load<Texture2D>("balaPolicia");
            Texture2D playerTexture = Content.Load<Texture2D>("autoJugador2");
            Texture2D buttonTexture = Content.Load<Texture2D>("button");
            font = Content.Load<SpriteFont>("font");
            Texture2D bulletTexture = Content.Load<Texture2D>("balaJugador2");
            Texture2D backgroundTexture = Content.Load<Texture2D>("menu");
            _gameBackgroundTexture = Content.Load<Texture2D>("fondoCiudad");

            // Inicializa el menú principal
            _mainMenu = new MainMenu(buttonTexture, font, backgroundTexture, StartGame, Exit);

            // Inicializa al jugador
            _jugador = new Jugador(
                playerTexture,
                bulletTexture,
                new Vector2(600, 100),
                8f,
                _graphics.PreferredBackBufferWidth,
                _graphics.PreferredBackBufferHeight,
                10
            );

            // Inicializa al enemigo
            _enemigo = new Enemigo(
                enemyTexture,
                fireballTexture,
                new Vector2(400, 700),
                6f,
                100f,
                775f
            );
        }

        // Actualización lógica en cada frame
        protected override void Update(GameTime gameTime)
        {
            // Salir del juego con Escape o botón de retroceso
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
                Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            switch (_currentState)
            {
                case GameState.MainMenu:
                    _mainMenu.Update(gameTime); // Actualiza el menú principal
                    break;

                case GameState.Playing:
                    // Mueve el fondo
                    _backgroundOffsetY += _backgroundSpeed;

                    // Reinicia el fondo cuando supera la altura de la pantalla
                    if (_backgroundOffsetY >= _graphics.PreferredBackBufferHeight)
                    {
                        _backgroundOffsetY = 0f;
                    }

                    // Actualiza al jugador y enemigo
                    _jugador.Update(gameTime);
                    _enemigo.Update(gameTime, _jugador.Position, _jugador);

                    // Cambia el estado si el jugador pierde
                    if (_jugador.Vida <= 0)
                    {
                        _currentState = GameState.GameOver;
                    }
                    break;


                case GameState.GameOver:
                    var keyboardState = Keyboard.GetState();
                    if (keyboardState.IsKeyDown(Keys.R))
                    {
                        ResetGame(); // Reinicia el juego
                    }
                    else if (keyboardState.IsKeyDown(Keys.Escape))
                    {
                        Exit(); // Sale del juego
                    }
                    break;
            }

            base.Update(gameTime);
        }

        // Reinicia el juego
        private void ResetGame()
        {
            _currentState = GameState.Playing;

            // Reinicia el jugador
            _jugador = new Jugador(
                Content.Load<Texture2D>("autoJugador2"),
                Content.Load<Texture2D>("balaJugador2"),
                new Vector2(600, 100),
                8f,
                _graphics.PreferredBackBufferWidth,
                _graphics.PreferredBackBufferHeight,
                10
            );

            // Reinicia el enemigo
            _enemigo = new Enemigo(
                Content.Load<Texture2D>("autoPolicia2"),
                Content.Load<Texture2D>("balaPolicia"),
                new Vector2(400, 700),
                6f,
                100f,
                775f
            );
        }

        // Dibuja el juego
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();

            switch (_currentState)
            {
                case GameState.MainMenu:
                    _mainMenu.Draw(_spriteBatch); // Dibuja el menú principal
                    break;

                case GameState.Playing:
                    // Dibuja dos fondos para el desplazamiento continuo
                    _spriteBatch.Draw(
                        _gameBackgroundTexture,
                        new Rectangle(0, (int)-_backgroundOffsetY, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight),
                        Color.White
                    );

                    _spriteBatch.Draw(
                        _gameBackgroundTexture,
                        new Rectangle(0, (int)(-_backgroundOffsetY + _graphics.PreferredBackBufferHeight), _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight),
                        Color.White
                    );

                    // Dibuja al jugador y al enemigo
                    _jugador.Draw(_spriteBatch);
                    _enemigo.Draw(_spriteBatch);
                    break;


                case GameState.GameOver:
                    // Dibuja la pantalla de Game Over
                    _spriteBatch.DrawString(font, "Game Over",
                        new Vector2(_graphics.PreferredBackBufferWidth / 2 - 100,
                                    _graphics.PreferredBackBufferHeight / 2 - 50), Color.Red);
                    _spriteBatch.DrawString(font, "Presiona R para reiniciar o Esc para salir",
                        new Vector2(_graphics.PreferredBackBufferWidth / 2 - 200,
                                    _graphics.PreferredBackBufferHeight / 2 + 50), Color.White);
                    break;
            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }

        // Cambia el estado del juego a "Jugando"
        private void StartGame()
        {
            _currentState = GameState.Playing;
        }
    }
}
