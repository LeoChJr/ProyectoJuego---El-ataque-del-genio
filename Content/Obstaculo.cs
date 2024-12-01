using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoJuego.Content
{
    public class Obstaculo
    {
        
            private Texture2D _texture; // Textura del obstáculo
            private Vector2 _position; // Posición actual del obstáculo
            private float _speed; // Velocidad de movimiento vertical del obstáculo

            public Vector2 Position => _position; // Exponer la posición para colisiones

            // Constructor
            public Obstaculo(Texture2D texture, Vector2 position, float speed)
            {
                _texture = texture;
                _position = position;
                _speed = speed;
            }

            // Actualizar posición
            public void Update(GameTime gameTime)
            {
                _position.Y += _speed; // Mueve el obstáculo hacia abajo
            }

            // Dibujar el obstáculo
            public void Draw(SpriteBatch spriteBatch)
            {
                spriteBatch.Draw(_texture, _position, Color.White);
            }
        }

    
}
