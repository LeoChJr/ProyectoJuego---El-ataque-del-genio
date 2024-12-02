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
        private bool _enemigoMuerto = false;     // Campo para verificar si el enemigo ha muerto
        public bool EnemigoMuerto => _enemigoMuerto;  // Propiedad pública para acceder al estado

        private int _vida; // Vida del enemigo

        // Propiedad para acceder a la vida desde fuera de la clase
        public int Vida => _vida;
        public Vector2 Position => _position;
        public Texture2D Texture => _texture;  // Propiedad para acceder a la textura

        public bool DisparoRealizado { get; private set; }

        // Constructor de la clase Enemigo
        public Enemigo(Texture2D texture, Texture2D fireballTexture, Vector2 position, float speed, float leftBoundary, float rightBoundary)
        {
            _texture = texture;
            _fireballTexture = fireballTexture;
            _position = position;
            _speed = speed;
            _leftBoundary = leftBoundary;
            _rightBoundary = rightBoundary;
            _movingRight = true;
            _bolasDeFuego = new List<DisparoEnemigo>(); // Inicializa la lista
            _vida = 100; // Inicializa la vida del enemigo
        }

        // Método para recibir daño
        public void RecibirDaño(int daño)
        {
            _vida -= daño; // Resta el daño a la vida del enemigo

            // Verifica si el enemigo muere
            if (_vida <= 0)
            {
                _vida = 0; // Asegura que no sea negativa
                _enemigoMuerto = true; // Marca al enemigo como muerto
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
            // Dibuja al enemigo usando su textura y posición
            spriteBatch.Draw(_texture, _position, Color.White);

            // Dibuja las balas activas si existen
            foreach (var bolaDeFuego in _bolasDeFuego)
            {
                bolaDeFuego.Draw(spriteBatch);
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
        }

        // Comprueba si un disparo colisiona con el jugador
        private bool ColisionaConJugador(DisparoEnemigo bolaDeFuego, Jugador jugador)
        {
            // Escala para reducir el tamaño de la bola de fuego
            float escalaBolaDeFuego = 0.01f;  // Ajusta el valor para hacer la colisión más pequeña (0.5 = 50% del tamaño original)

            // Rectángulo de colisión para la bola de fuego con escala aplicada
            Rectangle balaRect = new Rectangle(
                (int)bolaDeFuego.Position.X,
                (int)bolaDeFuego.Position.Y,
                (int)(bolaDeFuego.Texture.Width * escalaBolaDeFuego),  // Aplica la escala en el ancho
                (int)(bolaDeFuego.Texture.Height * escalaBolaDeFuego)  // Aplica la escala en la altura
            );

            // Rectángulo de colisión para el jugador, con escala aplicada si es necesario
            Rectangle jugadorRect = new Rectangle(
                (int)jugador.Position.X,
                (int)jugador.Position.Y,
                (int)(jugador.Texture.Width * 0.25f),  // Ajuste del ancho del jugador
                (int)(jugador.Texture.Height * 0.5f)   // Ajuste de la altura del jugador
            );

            // Devuelve true si los rectángulos se intersectan (indica colisión)
            return balaRect.Intersects(jugadorRect);
        }


        public void DrawHealthBar(SpriteBatch spriteBatch)
        {
            // Dimensiones de la barra de vida
            int barWidth = 100;// Ancho de la barra
            int barHeight = 30; // Alto de la barra
            int healthWidth = (int)((_vida / 100f) * barWidth); // Proporción de vida restante

            // Dibuja la barra de fondo (gris)
            spriteBatch.Draw(
                _texture,
                new Rectangle((int)_position.X, (int)_position.Y - 20, barWidth, barHeight),
                Color.Yellow
            );

            // Dibuja la barra de vida (roja)
            spriteBatch.Draw(
                _texture,
                new Rectangle((int)_position.X, (int)_position.Y - 20, healthWidth, barHeight),
                Color.Red
            );
        }

    }
}
