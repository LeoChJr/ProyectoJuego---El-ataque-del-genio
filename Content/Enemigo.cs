using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;

namespace ProyectoJuego.Content
{
    // Clase que representa un enemigo en el juego
    public class Enemigo
    {
        // Campos privados
        private Texture2D _texture;               // Textura del enemigo
        private Vector2 _position;               // Posición actual del enemigo en la pantalla
        private float _speed;                    // Velocidad de movimiento del enemigo
        private Texture2D _fireballTexture;      // Textura de la bola de fuego que dispara el enemigo
        private List<DisparoEnemigo> _bolasDeFuego; // Lista de disparos activos del enemigo
        private float _fireballCooldown;         // Tiempo de espera entre disparos
        private float _timeSinceLastShot;        // Tiempo transcurrido desde el último disparo

        private float _leftBoundary;             // Límite izquierdo del movimiento del enemigo
        private float _rightBoundary;            // Límite derecho del movimiento del enemigo
        private bool _movingRight;               // Indica si el enemigo se está moviendo hacia la derecha

        private int _vida;                       // Vida restante del enemigo

        // Constructor de la clase Enemigo
        public Enemigo(Texture2D texture, Texture2D fireballTexture, Vector2 position, float speed, float leftBoundary, float rightBoundary)
        {
            _texture = texture;                   // Asigna la textura del enemigo
            _fireballTexture = fireballTexture;   // Asigna la textura de la bola de fuego
            _position = position;                 // Inicializa la posición del enemigo
            _speed = speed;                       // Establece la velocidad de movimiento
            _bolasDeFuego = new List<DisparoEnemigo>(); // Inicializa la lista de disparos
            _fireballCooldown = 2f;               // Tiempo de espera entre disparos (en segundos)
            _timeSinceLastShot = 0f;              // Reinicia el tiempo desde el último disparo
            _leftBoundary = leftBoundary;         // Establece el límite izquierdo
            _rightBoundary = rightBoundary;       // Establece el límite derecho
            _movingRight = true;                  // Inicia moviéndose hacia la derecha
            _vida = 2;                            // Vida inicial del enemigo
        }

        // Actualiza la lógica del enemigo en cada frame
        public void Update(GameTime gameTime, Vector2 jugadorPosition, Jugador jugador)
        {
            // Movimiento del enemigo entre los límites
            if (_movingRight)
            {
                _position.X += _speed;            // Mueve al enemigo hacia la derecha
                if (_position.X >= _rightBoundary) // Cambia de dirección si llega al límite derecho
                {
                    _movingRight = false;
                }
            }
            else
            {
                _position.X -= _speed;            // Mueve al enemigo hacia la izquierda
                if (_position.X <= _leftBoundary) // Cambia de dirección si llega al límite izquierdo
                {
                    _movingRight = true;
                }
            }

            // Manejo de disparos
            _timeSinceLastShot += (float)gameTime.ElapsedGameTime.TotalSeconds; // Incrementa el tiempo desde el último disparo
            if (_timeSinceLastShot >= _fireballCooldown) // Comprueba si ya puede disparar
            {
                Disparar(jugadorPosition);       // Llama al método para disparar
                _timeSinceLastShot = 0f;         // Reinicia el contador
            }

            // Actualiza cada bola de fuego activa
            for (int i = _bolasDeFuego.Count - 1; i >= 0; i--)
            {
                _bolasDeFuego[i].Update(gameTime); // Actualiza la posición de cada disparo

                // Comprueba colisión entre la bola de fuego y el jugador
                if (ColisionaConJugador(_bolasDeFuego[i], jugador))
                {
                    jugador.ReducirVida(10);    // Resta vida al jugador (10 puntos de daño)
                    _bolasDeFuego.RemoveAt(i); // Elimina la bola de fuego tras causar daño
                }

                // Nota: Aquí se puede añadir lógica para eliminar disparos fuera de la pantalla
            }
        }

        // Dibuja al enemigo y sus disparos en la pantalla
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_texture, _position, Color.White); // Dibuja al enemigo

            // Dibuja cada disparo activo
            foreach (var bolaDeFuego in _bolasDeFuego)
            {
                bolaDeFuego.Draw(spriteBatch);
            }
        }

        // Genera un nuevo disparo en dirección al jugador
        private void Disparar(Vector2 jugadorPosition)
        {
            Vector2 direction = jugadorPosition - _position; // Calcula la dirección hacia el jugador
            direction.Normalize();                          // Normaliza la dirección
            DisparoEnemigo nuevaBola = new DisparoEnemigo(_fireballTexture, _position, direction, 10f); // Crea una nueva bola de fuego
            _bolasDeFuego.Add(nuevaBola);                   // Añade el disparo a la lista
        }

        // Comprueba si un disparo colisiona con el jugador
        private bool ColisionaConJugador(DisparoEnemigo bolaDeFuego, Jugador jugador)
        {
            // Escala para ajustar el tamaño del rectángulo de colisión de la bola de fuego
            float escalaBala = 0.1f;
            float escalaAuto = 0.25f; // Escala para el ancho del jugador
            float escalaauto2 = 0.5f; // Escala para el alto del jugador

            // Rectángulo de colisión para la bola de fuego
            Rectangle bolaDeFuegoRect = new Rectangle(
                (int)bolaDeFuego.Position.X,
                (int)bolaDeFuego.Position.Y,
                (int)(bolaDeFuego.Texture.Width * escalaBala),
                (int)(bolaDeFuego.Texture.Height * escalaBala)
            );

            // Rectángulo de colisión para el jugador
            Rectangle jugadorRect = new Rectangle(
                (int)jugador.Position.X,
                (int)jugador.Position.Y,
                (int)(jugador.Texture.Width * escalaAuto),
                (int)(jugador.Texture.Height * escalaauto2)
            );

            // Devuelve true si los rectángulos se intersectan (indica colisión)
            return bolaDeFuegoRect.Intersects(jugadorRect);
        }
    }
}
