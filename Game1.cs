using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using ProyectoJuego.Content;
using System;
using System.Collections.Generic;
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

        // Música de fondo
        //private Song _backgroundMusic;

        // Sonidos
        //private SoundEffect _playerShootSound;
        //private SoundEffect _enemyShootSound;
        //private SoundEffect _collisionSound;

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
        private List<Obstaculo> _obstaculos; // Lista de obstáculos activos
        private Texture2D _obstaculoTexture; // Textura de los obstáculos
        private float _spawnCooldown; // Tiempo entre la generación de obstáculos
        private float _timeSinceLastSpawn; // Tiempo transcurrido desde el último obstáculo

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
            _obstaculoTexture = Content.Load<Texture2D>("enemigo"); // Cambia "piedra" al nombre del archivo
            _obstaculos = new List<Obstaculo>();
            _spawnCooldown = 2f; // Generar un obstáculo cada 2 segundos
            _timeSinceLastSpawn = 0f;
            //falta poner el rango en donde sale los obstaculos osea (que no ocupe todo la pantalla sino que solo una parte)
            //_________________________________________________________________________________________
            // Cargar música de fondo
            //_backgroundMusic = Content.Load<Song>("musicaFondo"); // Archivo .mp3 o .wma

            // Cargar efectos de sonido
            //_playerShootSound = Content.Load<SoundEffect>("disparoJugador"); // Archivo .wav
            //_enemyShootSound = Content.Load<SoundEffect>("disparoEnemigo"); // Archivo .wav
            //_collisionSound = Content.Load<SoundEffect>("colision"); // Archivo .wav

            // Inicia la música de fondo
            //MediaPlayer.IsRepeating = true; // Repetir música en bucle
            //MediaPlayer.Volume = 0.5f;     // Ajustar volumen de la música
            //MediaPlayer.Play(_backgroundMusic);
            //________________________________________________________________________________________
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
            // Crear un enemigo con tamaño personalizado
            _enemigo = new Enemigo(
                Content.Load<Texture2D>("autoPolicia2"), // Textura del enemigo
                Content.Load<Texture2D>("balaPolicia"), // Textura de las balas
                new Vector2(400, 700), // Posición inicial
                6f,                    // Velocidad
                100f,                  // Límite izquierdo
                775f,                  // Límite derecho
                new Vector2(64, 32)    // Tamaño personalizado (ancho: 64, alto: 32)
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
                    MediaPlayer.Pause(); // Pausa la música en el menú principal
                    break;

                case GameState.Playing:
                    if (MediaPlayer.State == MediaState.Paused)
                    {
                        MediaPlayer.Resume(); // Reanuda la música al empezar a jugar
                    }

                    // Mueve el fondo
                    _backgroundOffsetY += _backgroundSpeed;

                    // Reinicia el fondo cuando supera la altura de la pantalla
                    if (_backgroundOffsetY >= _graphics.PreferredBackBufferHeight)
                    {
                        _backgroundOffsetY = 0f;
                    }

                    // Actualiza al jugador
                    _jugador.Update(gameTime);

                    // Actualiza al enemigo
                    if (_enemigo != null)
                    {
                        _enemigo.Update(gameTime, _jugador.Position, _jugador);

                        // Reproduce el sonido de disparo del enemigo si dispara
                        if (_enemigo.DisparoRealizado) // Supón que existe esta propiedad en el enemigo
                        {
                            //_enemyShootSound.Play();
                        }
                    }

                    // Lógica para las balas del jugador
                    for (int i = _jugador.Balas.Count - 1; i >= 0; i--)
                    {
                        _jugador.Balas[i].Update(gameTime);

                        // Comprueba colisión entre la bala y el enemigo
                        if (_enemigo != null && ColisionaConEnemigo(_jugador.Balas[i], _enemigo))
                        {
                            //_collisionSound.Play(); // Sonido de colisión al golpear al enemigo
                            _enemigo.RecibirDaño(1); // Resta 1 punto de vida al enemigo
                            _jugador.Balas.RemoveAt(i); // Elimina la bala tras el impacto

                            // Si el enemigo muere, se elimina
                            if (_enemigo.Vida <= 0)
                            {
                                _enemigo = null; // Elimina al enemigo
                            }
                        }
                    }

                    // Lógica de obstáculos
                    _timeSinceLastSpawn += (float)gameTime.ElapsedGameTime.TotalSeconds;

                    if (_timeSinceLastSpawn >= _spawnCooldown)
                    {
                        // Define el rango donde aparecerán los obstáculos
                        int minX = 300; // Margen izquierdo
                        int maxX = _graphics.PreferredBackBufferWidth - 300; // Margen derecho

                        // Genera una posición X aleatoria dentro del rango definido
                        Random random = new Random();
                        float randomX = random.Next(minX, maxX);

                        // Genera un nuevo obstáculo
                        Obstaculo nuevoObstaculo = new Obstaculo(
                            _obstaculoTexture,
                            new Vector2(randomX, -_obstaculoTexture.Height), // Aparece fuera de la pantalla
                            5f // Velocidad del obstáculo
                        );

                        _obstaculos.Add(nuevoObstaculo);
                        _timeSinceLastSpawn = 0f; // Reinicia el temporizador
                    }

                    // Actualiza la posición de los obstáculos
                    for (int i = _obstaculos.Count - 1; i >= 0; i--)
                    {
                        _obstaculos[i].Update(gameTime);

                        // Comprueba colisión con el jugador
                        if (ColisionaConJugador(_obstaculos[i]))
                        {
                            //_collisionSound.Play(); // Sonido al chocar con un obstáculo
                            _jugador.ReducirVida(1); // Resta 1 punto de vida
                            _obstaculos.RemoveAt(i); // Elimina el obstáculo tras la colisión
                        }
                        else if (_obstaculos[i].Position.Y > _graphics.PreferredBackBufferHeight)
                        {
                            // Elimina obstáculos fuera de la pantalla
                            _obstaculos.RemoveAt(i);
                        }
                    }

                    // Cambia el estado si el jugador pierde
                    if (_jugador.Vida <= 0)
                    {
                        MediaPlayer.Pause(); // Pausa la música al terminar el juego
                        _currentState = GameState.GameOver;
                    }
                    break;

                case GameState.GameOver:
                    MediaPlayer.Pause(); // Pausa la música si está en Game Over
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
            // Crear un enemigo con tamaño personalizado
            _enemigo = new Enemigo(
                Content.Load<Texture2D>("autoPolicia2"), // Textura del enemigo
                Content.Load<Texture2D>("balaPolicia"), // Textura de las balas
                new Vector2(400, 700), // Posición inicial
                6f,                    // Velocidad
                100f,                  // Límite izquierdo
                775f,                  // Límite derecho
                new Vector2(64, 32)    // Tamaño personalizado (ancho: 64, alto: 32)
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

                    // Dibuja todos los obstáculos
                    foreach (var obstaculo in _obstaculos)
                    {
                        obstaculo.Draw(_spriteBatch);
                    }

                    // Dibuja al jugador
                    _jugador.Draw(_spriteBatch);

                    // Verifica si _enemigo no es null antes de dibujarlo
                    if (_enemigo != null)
                    {
                        _enemigo.Draw(_spriteBatch); // Dibuja al enemigo si no es null
                    }
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
        private bool ColisionaConJugador(Obstaculo obstaculo)//falta poner que quita vida
        {
            Rectangle obstaculoRect = new Rectangle(
                (int)obstaculo.Position.X,
                (int)obstaculo.Position.Y,
                _obstaculoTexture.Width,
                _obstaculoTexture.Height
            );

            Rectangle jugadorRect = new Rectangle(
                (int)_jugador.Position.X,
                (int)_jugador.Position.Y,
                _jugador.Texture.Width,
                _jugador.Texture.Height
            );

            if (obstaculoRect.Intersects(jugadorRect))
            {
                // Reproducir el sonido de colisión
                //_collisionSound.Play();
                _jugador.ReducirVida(10); // Ejemplo: reduce vida
                return true;
            }

            return false;
        }
        private bool ColisionaConEnemigo(DisparoJugador bala, Enemigo enemigo)
        {
            Rectangle balaRect = new Rectangle(
                (int)bala.Position.X,
                (int)bala.Position.Y,
                bala.Texture.Width,
                bala.Texture.Height
            );

            Rectangle enemigoRect = new Rectangle(
                (int)enemigo.Position.X,
                (int)enemigo.Position.Y,
                (int)enemigo.Size.X, // Usa el ancho ajustado
                (int)enemigo.Size.Y  // Usa el alto ajustado
            );

            return balaRect.Intersects(enemigoRect);
        }



    }
}
