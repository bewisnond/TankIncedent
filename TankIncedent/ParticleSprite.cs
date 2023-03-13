using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace TankIncedent
{
    internal class ParticleSprite : Sprite
    {

        protected float _shrinkSpeed = 0.95f;

        public ParticleSprite(Texture2D spriteTxr,
            Vector2 spritePos,
            Vector2 spriteVel,
            float spriteAngle) : base(spriteTxr, spritePos, spriteVel, 0f)
        {
            _spriteTint = Color.SeaGreen;
        }

        public override void Update(GameTime gameTime, List<Sprite> spriteList, Point ScreenSize)
        {
            _spritePos += _spriteVel;
            _spriteScale *= _shrinkSpeed;

            if (_spriteScale < 0.1f)
            {
                _spriteDead = true;
            }

            if (_spritePos.X < 0 - _spriteTxr.Width)
            {
                _spriteDead = true;
            }
            else if (_spritePos.X > ScreenSize.X + _spriteTxr.Width)
            {
                _spriteDead = true;
            }

            if (_spritePos.Y < 0 - _spriteTxr.Height)
            {
                _spriteDead = true;
            }
            else if (_spritePos.Y > ScreenSize.X + _spriteTxr.Height)
            {
                _spriteDead = true;
            }
        }
    }
}
