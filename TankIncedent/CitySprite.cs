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
    internal class CitySprite : Sprite
    {
        public float Health = 10f;

        public CitySprite(Texture2D spriteTxr, Vector2 spritePos, Vector2 spriteVel, float spriteAngle) : base(spriteTxr, spritePos, spriteVel, spriteAngle)
        {
            _spriteScale = 0.25f;
            _collisionRadius /= 2f;

        }
    }
}