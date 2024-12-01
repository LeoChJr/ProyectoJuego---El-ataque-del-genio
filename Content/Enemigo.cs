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

        private int _vida; // Vida del enemigo

        // Propiedad para acceder a la vida desde fuera de la clase
        public int Vida => _vida;
        public Vector2 Position => _position;
        // Constructor de la clase Enemigo
        public Texture2D Texture => _texture;
        public bool DisparoRealizado { get; private set; }
        private Vector2 _size; // Tamaño del enemigo (ancho y alto)
        public Vector2 Size => _size;


        public Enemigo(Texture2D texture, Texture2D fireballTexture, Vector2 position, float speed, float leftBoundary, float rightBoundary, Vector2 size)
        {
            _texture = texture;
            _fireballTexture = fireballTexture;
            _position = position;
            _speed = speed;
            _leftBoundary = leftBoundary;
            _rightBoundary = rightBoundary;
            _movingRight = true;
            _size = size;
            _bolasDeFuego = new List<DisparoEnemigo>(); // Inicializa la lista
        }
        // Método para recibir daño
        public void RecibirDaño(int daño)
        {
            _vida -= daño; // Resta el daño a la vida del enemigo

            // Verifica si el enemigo muere
            if (_vida <= 0)
            {
                _vida = 0; // Asegura que no sea negativa
                           // Aquí puedes agregar lógica para eliminar al enemigo, efectos, etc.
            }
        }

        // Actualiza la lógica del enemigo en cada frame
        public void Update(GameTime gameTime, Vector2 jugadorPosition, Jugador jugador)
        {
            // Movimiento del enemigo entre los límites
            if (_movingRight)
            {
                _position.X += _speed;
                if (_position.X >= _rightBoundary) _movingRight = false;
            }
            else
            {
                _position.X -= _speed;
                if (_position.X <= _leftBoundary) _movingRight = true;
            }

            // Manejo del disparo (limita a 1 bala activa)
            _timeSinceLastShot += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (_timeSinceLastShot >= _fireballCooldown)
            {
                Disparar(jugadorPosition); // Solo dispara si no hay balas activas
                _timeSinceLastShot = 0f;
            }

            // Actualiza cada bala de fuego activa
            for (int i = _bolasDeFuego.Count - 1; i >= 0; i--)
            {
                _bolasDeFuego[i].Update(gameTime);

                // Comprueba colisión con el jugador
                if (ColisionaConJugador(_bolasDeFuego[i], jugador))
                {
                    jugador.ReducirVida(10);    // Resta vida al jugador
                    _bolasDeFuego.RemoveAt(i); // Elimina la bala tras causar daño
                    continue; // Pasa al siguiente elemento después de eliminar
                }

                // Elimina disparos fuera de la pantalla
                else if (_bolasDeFuego[i].Position.Y > 1000 || _bolasDeFuego[i].Position.Y < 0)
                {
                    _bolasDeFuego.RemoveAt(i); // Elimina la bala fuera de pantalla
                    continue; // Pasa al siguiente elemento después de eliminar
                }
            }
        }


        // Dibuja al enemigo y sus disparos en la pantalla
        public void Draw(SpriteBatch spriteBatch)
        {
            // Dibuja al enemigo ajustando su tamaño
            spriteBatch.Draw(
        _texture,
        new Rectangle((int)_position.X, (int)_position.Y, (int)_size.X, (int)_size.Y),
        Color.White
    );

            // Dibuja la única bala activa (si la hay)
            if (_bolasDeFuego.Count > 0)
            {
                foreach (var bolaDeFuego in _bolasDeFuego)
                {
                    bolaDeFuego.Draw(spriteBatch);
                }
            }
        }


        // Genera un nuevo disparo en dirección al jugador
        private void Disparar(Vector2 jugadorPosition)
        {
            if (_bolasDeFuego.Count > 0)
            {
                return; // No dispara si ya hay una bala activa
            }

            // Calcula la dirección hacia el jugador
            Vector2 direction = jugadorPosition - _position;
            direction.Normalize();

            // Crea una nueva bala
            DisparoEnemigo nuevaBola = new DisparoEnemigo(_fireballTexture, _position, direction, 10f);

            // Añade la bala a la lista
            _bolasDeFuego.Add(nuevaBola);
            //musica
            //_enemyShootSound.Play();
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
