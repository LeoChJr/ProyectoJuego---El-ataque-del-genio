using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace ProyectoJuego.Content
{
    // Clase que representa al jugador en el juego
    public class Jugador
    {
        // Campos privados
        private Texture2D _texture;             // Textura que representa al jugador
        private Vector2 _position;             // Posición actual del jugador
        private float _speed;                  // Velocidad de movimiento
        private int _vida;                     // Vida actual del jugador
        private int _screenWidth;              // Ancho de la pantalla del juego
        private int _screenHeight;             // Altura de la pantalla del juego
        private Texture2D _bulletTexture;      // Textura utilizada para las balas del jugador
        private List<DisparoJugador> _balas;   // Lista de balas activas disparadas por el jugador
        public List<DisparoJugador> Balas => _balas;
        // Propiedades para acceder a ciertos campos desde fuera de la clase
        public Texture2D Texture => _texture;  // Textura del jugador
        public Vector2 Position => _position;  // Posición del jugador
        public int Vida => _vida;              // Vida del jugador

        // Constructor
        public Jugador(Texture2D texture, Texture2D bulletTexture, Vector2 position, float speed, int screenWidth, int screenHeight, int vida)
        {
            _texture = texture;
            _bulletTexture = bulletTexture;
            _position = position;
            _speed = speed;
            _vida = vida;
            _screenWidth = screenWidth;
            _screenHeight = screenHeight;
            _balas = new List<DisparoJugador>(); // Inicializa la lista de balas
        }

        // Método que actualiza el estado del jugador
        public void Update(GameTime gameTime)
        {
            var keyboardState = Keyboard.GetState(); // Obtiene el estado actual del teclado
            
            // Movimiento hacia arriba
            if (keyboardState.IsKeyDown(Keys.W))
                _position.Y -= _speed;

            // Movimiento hacia abajo
            if (keyboardState.IsKeyDown(Keys.S))
                _position.Y += _speed;

            // Restringe la posición vertical para que el jugador no salga de la pantalla
            _position.Y = MathHelper.Clamp(_position.Y, 0, _screenHeight - _texture.Height);

            // Movimiento hacia la izquierda
            if (keyboardState.IsKeyDown(Keys.A) && _position.X > 250)
                _position.X -= _speed;

            // Movimiento hacia la derecha
            if (keyboardState.IsKeyDown(Keys.D) && _position.X < 900)
                _position.X += _speed;

            // Restringe la posición horizontal para mantener al jugador dentro de los límites
            _position.X = MathHelper.Clamp(_position.X, 0, _screenWidth - _texture.Width);

            // Acción de disparar
            if (keyboardState.IsKeyDown(Keys.Space))
            {
                Disparar();
            }

            // Actualiza el estado de las balas activas
            for (int i = _balas.Count - 1; i >= 0; i--)
            {
                _balas[i].Update(gameTime);
                // Elimina balas que salgan de la pantalla
                if (_balas[i].Position.Y < 0)
                {
                    _balas.RemoveAt(i);
                }
            }
        }

        // Método para dibujar al jugador y sus balas en la pantalla
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_texture, _position, Color.White); // Dibuja al jugador

            // Dibuja todas las balas activas
            foreach (var bala in _balas)
            {
                bala.Draw(spriteBatch);
            }
        }

        // Método para disparar una bala
        private void Disparar()
        {
            // Calcula la posición inicial de la bala
            Vector2 bulletPosition = new Vector2(
                _position.X + _texture.Width / 2 - _bulletTexture.Width / 2,
                _position.Y + _texture.Height // Para que salga desde debajo del jugador
            );

            // Dirección inicial de la bala (hacia abajo)
            Vector2 direction = new Vector2(0, 1); // Cambiado de (0, -1) a (0, 1)

            // Crea una nueva bala con la textura, posición, dirección y velocidad
            DisparoJugador bala = new DisparoJugador(_bulletTexture, bulletPosition, direction, 5f);
            _balas.Add(bala); // Añade la bala a la lista de balas activas
            //musica
            //_playerShootSound.Play();
        }


        // Método para reducir la vida del jugador
        public void ReducirVida(int cantidad)
        {
            _vida -= cantidad; // Reduce la vida según la cantidad especificada

            // Si la vida llega a 0 o menos, realiza una acción (ej. fin del juego)
            if (_vida <= 0)
            {
                _vida = 0; // Asegura que la vida no sea negativa
                // Aquí puedes añadir lógica adicional (ej. mostrar pantalla de "Game Over").
            }
        }
    }
}
