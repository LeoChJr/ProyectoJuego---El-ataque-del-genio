using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ProyectoJuego.Content
{
    // Clase que representa una bala disparada por el jugador en el juego
    public class DisparoJugador
    {
        // Campos privados
        private Texture2D _texture;      // Textura que representa la apariencia de la bala
        private Vector2 _position;      // Posición actual de la bala en el mundo del juego
        private Vector2 _direction;     // Dirección en la que se mueve la bala
        private float _speed;           // Velocidad de movimiento de la bala
        public Texture2D Texture => _texture;
        // Propiedad para acceder a la posición de la bala desde fuera de la clase
        public Vector2 Position => _position; // Devuelve la posición actual de la bala

        // Constructor para inicializar la bala disparada por el jugador
        public DisparoJugador(Texture2D texture, Vector2 position, Vector2 direction, float speed)
        {
            _texture = texture;         // Asigna la textura proporcionada
            _position = position;       // Establece la posición inicial
            _direction = direction;     // Configura la dirección de movimiento
            _speed = speed;             // Define la velocidad de la bala
        }

        // Método que actualiza la posición de la bala en cada frame
        public void Update(GameTime gameTime)
        {
            _position += _direction * _speed; // Mueve la bala según su dirección y velocidad
        }

        // Método que dibuja la bala en la pantalla
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_texture, _position, Color.White); // Dibuja la textura de la bala con su color original
        }
    }
}
