using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace TankIncedent
{
    internal class Sprite
    {
        protected Texture2D _spriteTxr;
        public Vector2 _spritePos, _spriteVel, _spriteOrigin;
        public float _spriteAngle;
        public bool _spriteDead = false;
        protected Rectangle _spriteSource;
        protected Color _spriteTint = Color.White;
        protected float _spriteScale = 0.35f;
        public float _collisionRadius;

        public Sprite(Texture2D spriteTxr, Vector2 spritePos, Vector2 spriteVel, float spriteAngle)
        {
            _spriteTxr = spriteTxr;
            _spritePos = spritePos;
            _spriteVel = spriteVel;
            _spriteAngle = spriteAngle;
            _spriteSource = new Rectangle(0, 0, _spriteTxr.Width, _spriteTxr.Height);
            _spriteOrigin = new Vector2(_spriteSource.Width / 2, _spriteSource.Height / 2);
            _collisionRadius = _spriteSource.Width * _spriteScale * 0.5f;
        }

        public virtual void Update(GameTime gameTime, List<Sprite> spriteList, Point screenSize) { }


        public void Draw(SpriteBatch spriteBatch)
        {
           
            //This gives the text a colour, size and position
            spriteBatch.Draw(_spriteTxr,
                new Rectangle((int)_spritePos.X,
                    (int)_spritePos.Y,
                    (int)(_spriteSource.Width * _spriteScale),
                    (int)(_spriteSource.Height * _spriteScale)),
                _spriteSource,
                _spriteTint,
                _spriteAngle,
                _spriteOrigin,
                SpriteEffects.None,
                0f
                );
        }
    }
}
