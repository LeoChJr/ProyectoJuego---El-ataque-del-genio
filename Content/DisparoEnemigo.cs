using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoJuego.Content
{
    // Clase que representa una bola de fuego disparada por un enemigo en el juego
    public class DisparoEnemigo
    {
        // Campos privados
        private Texture2D _texture;      // Textura que representa la apariencia de la bola de fuego
        private Vector2 _position;      // Posición actual de la bola de fuego en el mundo del juego
        private Vector2 _direction;     // Dirección en la que se mueve la bola de fuego
        private float _speed;           // Velocidad de movimiento de la bola de fuego

        // Propiedades para acceder a la textura y la posición desde fuera de la clase
        public Texture2D Texture => _texture; // Devuelve la textura de la bola de fuego
        public Vector2 Position => _position; // Devuelve la posición actual de la bola de fuego

        // Constructor para inicializar la bola de fuego
        public DisparoEnemigo(Texture2D texture, Vector2 position, Vector2 direction, float speed)
        {
            _texture = texture;         // Asigna la textura proporcionada
            _position = position;       // Establece la posición inicial
            _direction = direction;     // Configura la dirección de movimiento
            _speed = speed;             // Define la velocidad de la bola de fuego
        }

        // Actualiza la posición de la bola de fuego en cada frame
        public void Update(GameTime gameTime)
        {
            _position += _direction * _speed; // Mueve la bola de fuego según su dirección y velocidad
        }

        // Dibuja la bola de fuego en la pantalla
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_texture, _position, Color.Yellow); // Dibuja la textura de la bola de fuego con un tinte amarillo
        }
    }
}
