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
    internal class AsteriodSprite : Sprite
    {

        public int _asteriodSize = 0;

        public AsteriodSprite(Texture2D spriteTxr,
            Vector2 spritePos,
            Vector2 spriteVel,
            float spriteAngle,
            int asteriodSize = 0) : base(spriteTxr, spritePos, spriteVel, spriteAngle)
            {
            _asteriodSize = asteriodSize;
            _spriteSource = GetSourceRectangle();

            _spriteOrigin = new Vector2(_spriteSource.Width / 2f, _spriteSource.Height / 2f);
            _collisionRadius = _spriteSource.Width * _spriteScale * 0.5f;
        }

        public override void Update(GameTime gameTime, List<Sprite> spriteList, Point ScreenSize)
        {
            _spritePos += _spriteVel;

            if (_spritePos.X < 0 - _spriteTxr.Width)
            {
                _spritePos.X = ScreenSize.X + _spriteTxr.Width;
            }
            else if (_spritePos.X > ScreenSize.X + _spriteTxr.Width)
            {
                _spritePos.X = 0 - _spriteTxr.Width;
            }

            if (_spritePos.Y < 0 - _spriteTxr.Height)
            {
                _spritePos.Y = ScreenSize.X + _spriteTxr.Height;
            }
            else if (_spritePos.Y > ScreenSize.X + _spriteTxr.Height)
            {
                _spritePos.Y = 0 - _spriteTxr.Height;
            }
        }


            Rectangle GetSourceRectangle()
        {
            switch (_asteriodSize)
            {
                case 0:
                    return new Rectangle(0,0,256,256);
                case 1:
                    return new Rectangle(256, 0, 128, 128);
                case 2:
                    return new Rectangle(256, 128, 64, 64);
                default:
                    return new Rectangle(344, 216, 32, 32);
            }
        }

    }
}
