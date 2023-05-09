using GXPEngine.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GXPEngine
{
    internal class Shell : EasyDraw
    {

        Vector2 vel;
        Vector2 pos;

        public bool isGone = false;

        public Shell(Vector2 velocity, Vector2 spawnPosition) : base(5, 5, false)
        {
            vel = velocity;
            pos = spawnPosition;

            Clear(255);
        }

        public void Step(List<Polygon> targets)
        {
            pos += vel;

            SetXY(pos.x, pos.y);

            for (int i = 0; i < targets.Count; i++)
            {

                //the same collision detection that is used for a single vertice on polygons
                if (pos.DistanceTo(targets[i].pos) < targets[i].size)
                {

                    bool insideCube = true;

                    int closestEdge = 0;
                    float closestEdgeDistance = 10000;

                    for (int k = 0; k < targets[i].verts.Length; k++)
                    {

                        Vector2 targetEdge;

                        if (k == targets[i].verts.Length - 1)
                        {
                            targetEdge = (targets[i].verts[0] - targets[i].verts[k]);
                        }
                        else
                        {
                            targetEdge = (targets[i].verts[k + 1] - targets[i].verts[k]);
                        }

                        float vertDistanceToEdge = targetEdge.Normal().Dot(pos - targets[i].pos + vel - targets[i].vel - targets[i].verts[k]);

                        if (vertDistanceToEdge < 1)
                        {
                            insideCube = false;
                        }

                        if (vertDistanceToEdge < closestEdgeDistance)
                        {
                            closestEdge = k;
                            closestEdgeDistance = vertDistanceToEdge;
                        }

                    }

                    if (insideCube)
                    {
                        if (targets[i].isActive) targets[i].vel += vel;
                        LateDestroy();
                        isGone = true;
                    }
                }
            }
        }
    }
}
