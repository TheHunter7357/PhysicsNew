using GXPEngine.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GXPEngine
{
    internal class Border : EasyDraw
    {

        public Vector2 direction;
        public Vector2 offset;

        public Border(Vector2 direction, Vector2 offset) : base (2400, 900)
        { 
            this.direction = direction;
            this.offset = offset;

            Line(offset.x + 1000 * direction.x, offset.y + 1000 * direction.y, offset.x - 1000 * direction.x, offset.y - 1000 * direction.y);
        }
    }
}
