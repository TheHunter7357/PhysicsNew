using GXPEngine.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace GXPEngine
{
    internal class Polygon : Pivot
    {

        public bool isActive;

        float mass;
        public float size;

        EasyDraw edges;
        public EasyDraw[] points;

        public Vector2[] verts;
        float[] vertRot;
        public Vector2 pos;
        public Vector2 vel;
        public Vector2 acc;

        float rot = 0;
        float rotVel;
        float rotAcc;

        Vector2 gravity = new Vector2(0, 0.05f);

        bool grounded;

        float bounciness = 0.8f;
        float friction = 1.8f;
        float rotationalFriction = 0.8f;
        float inertia = 0.8f;
        float inverseMomentOfInertia;

        float airResistance = 0.998f;


        //for making a polygon from vertices
        public Polygon(Vector2[] verts, float mass, float size, float rotation, Vector2 spawnPos, bool isActive)
        {

            this.isActive = isActive;

            this.mass = mass;
            this.size = size;
            rot = rotation;

            pos = spawnPos;

            this.verts = verts;
            vertRot = new float[verts.Length];

            for (int i = 0; i < verts.Length; i++)
            {
                vertRot[i] = verts[i].GetAngleDegrees();
            }

            edges = new EasyDraw((int)size * 3, (int)size * 3);
            AddChild(edges);

            points = new EasyDraw[verts.Length];
            for (int i = 0; i < points.Length; i++)
            {
                points[i] = new EasyDraw(3, 3);
                points[i].SetOrigin(points[i].width / 2, points[i].height / 2);
                points[i].Clear(Color.White);
                AddChild(points[i]);
            }

            // Deduced from general triangle / polygon formula:
            float inertia = mass * (size * 2 * size * 2 + size * 2 * size * 2) / 12;
            inverseMomentOfInertia = 1 / inertia;

            DrawPolygon();
        }


        //for making a square from a set size
        public Polygon(float mass, float size, Vector2 spawnPos, bool isActive)
        {

            this.isActive = isActive;

            this.mass = mass;
            this.size = size;

            pos = spawnPos;

            //tr,tl,bl,br
            verts = new Vector2[4];
            verts[0] = new Vector2(size, size);
            verts[1] = new Vector2(-size, size);
            verts[2] = new Vector2(-size, -size);
            verts[3] = new Vector2(size, -size);


            edges = new EasyDraw((int)size * 3, (int)size * 3);
            AddChild(edges);

            points = new EasyDraw[4];
            for (int i = 0; i < points.Length; i++)
            {
                points[i] = new EasyDraw(3, 3);
                points[i].SetOrigin(points[i].width / 2, points[i].height / 2);
                points[i].Clear(Color.White);
                AddChild(points[i]);
            }

            vertRot = new float[verts.Length];

            for (int i = 0; i < verts.Length; i++)
            {
                vertRot[i] = verts[i].GetAngleDegrees();
            }

            // Deduced from general triangle / polygon formula:
            float inertia = mass * (size * 2 * size * 2 + size * 2 * size * 2) / 12;
            inverseMomentOfInertia = 1 / inertia;

            DrawPolygon();
        }

        public void Step(Border[] borders, List<Polygon> polygons, List<ForceField> forceFields)
        {
            if (isActive)
                DoCollisionCheckBorders(borders);
                DoCollisionCheckpolygons(polygons);
                DoCollisionCheckForceFields(forceFields);




            if (isActive)
            {
                acc = gravity;
            }
            else
            {
                acc = new Vector2(0, 0);
            }

            if (isActive)
            {
                vel += acc;
                pos += vel;

                rotVel += rotAcc;
                rot += rotVel;

                vel *= Mathf.Pow(airResistance, vel.Length() / 10);
                rotVel *= Mathf.Clamp(Mathf.Pow(airResistance, rotVel / 10), 0, 1);
            }

            //running this every frame for non-active large easydraws was eating up performance, thus this is only
            //called for large inactive polygons at the beginning of the program
            if (isActive) DrawPolygon();
        }

        void DrawPolygon()
        {
            edges.SetXY(pos.x - size * 1.5f, pos.y - size * 1.5f);
            edges.ClearTransparent();

            edges.Line(size * 1.5f, size * 1.5f, size * 1.5f + acc.x * 300, size * 1.5f + acc.y * 300);

            for (int i = 0; i < points.Length; i++)
            {
                verts[i].SetAngleDegrees(rot + vertRot[i]);
                points[i].SetXY(pos.x + verts[i].x, pos.y + verts[i].y);

                if (i == points.Length - 1)
                {
                    edges.Line(verts[i].x + size * 1.5f, verts[i].y + size * 1.5f, verts[0].x + size * 1.5f, verts[0].y + size * 1.5f);
                }
                else
                {
                    edges.Line(verts[i].x + size * 1.5f, verts[i].y + size * 1.5f, verts[i + 1].x + size * 1.5f, verts[i + 1].y + size * 1.5f);
                }
            }
        }

        void DoCollisionCheckBorders(Border[] borders)
        {
            for (int i = 0; i < verts.Length; i++)
            {
                foreach (Border b in borders)
                {
                    if (b.direction.Normal().Dot(pos + verts[i] + vel - b.offset) > 0)
                    {
                        ResolveStaticCollision(b.direction, b.offset, verts[i]);
                    }
                }
            }
        }

        public void DoCollisionCheckpolygons(List<Polygon> polygons)
        {
            for (int i = 0; i < polygons.Count; i++)
            {

                //if both objects are inactive, no need to run collision checks
                if(isActive != false || polygons[i].isActive != false) {

                    //check if not this object and if it is close enough to collide
                    if (pos.DistanceTo(polygons[i].pos) > 0.001f && pos.DistanceTo(polygons[i].pos) < size * 3 + polygons[i].size * 3)
                    {

                        //checks if a vert is behind all edges, if so it is colliding

                        for (int j = 0; j < verts.Length; j++)
                        {
                            bool insideCube = true;

                            int closestEdge = 0;
                            float closestEdgeDistance = 10000;

                            for (int k = 0; k < polygons[i].verts.Length; k++)
                            {

                                Vector2 targetEdge;

                                if (k == polygons[i].verts.Length - 1)
                                {
                                    targetEdge = (polygons[i].verts[0] - polygons[i].verts[k]);
                                }
                                else
                                {
                                    targetEdge = (polygons[i].verts[k + 1] - polygons[i].verts[k]);
                                }

                                float vertDistanceToEdge = targetEdge.Normal().Dot(pos - polygons[i].pos + verts[j] + vel - polygons[i].vel - polygons[i].verts[k]);

                                if (vertDistanceToEdge < 0)
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

                                Vector2 impactEdge;
                                Vector2 impactNormal;
                                Vector2 pointOfImpact;

                                if (closestEdge == polygons[i].verts.Length - 1)
                                {
                                    impactEdge = polygons[i].verts[0] - polygons[i].verts[closestEdge];
                                    impactNormal = impactEdge.Normal().Normalized();
                                    //POI = edge normalized * dot product of relative position of colliding vert to edge
                                    pointOfImpact = impactEdge.Normalized() * ((verts[j] + pos) - (polygons[i].verts[0] + polygons[i].pos)).Dot(impactEdge.Normalized()) + polygons[i].verts[closestEdge] + polygons[i].pos;

                                    polygons[i].points[0].color = 10;
                                    polygons[i].points[closestEdge].color = 10;
                                }
                                else
                                {
                                    impactEdge = polygons[i].verts[closestEdge + 1] - polygons[i].verts[closestEdge];
                                    impactNormal = impactEdge.Normal().Normalized();
                                    //POI = edge normalized * dot product of relative position of colliding vert to edge
                                    pointOfImpact = impactEdge.Normalized() * ((verts[j] + pos) - (polygons[i].verts[closestEdge] + polygons[i].pos)).Dot(impactEdge.Normalized()) + polygons[i].verts[closestEdge] + polygons[i].pos;

                                    polygons[i].points[closestEdge + 1].color = 10;
                                    polygons[i].points[closestEdge].color = 10;
                                }

                                if (isActive && !polygons[i].isActive)
                                {
                                    ResolveStaticCollision(impactEdge, pointOfImpact, verts[j]);
                                    break;
                                }
                                else if (!isActive && polygons[i].isActive)
                                {
                                    ResolveDynamicCollision(impactEdge, impactNormal, pointOfImpact, polygons[i], verts[j], closestEdge, false);
                                }
                                else if (isActive && polygons[i].isActive)
                                {
                                    ResolveDynamicCollision(impactEdge, impactNormal, pointOfImpact, polygons[i], verts[j], closestEdge, true);
                                }
                            }
                        }
                    }
                }
            }
        }

        public void DoCollisionCheckForceFields(List<ForceField> forceFields)
        {
            for (int i = 0; i < forceFields.Count; i++)
            {
                for (int j = 0; j < verts.Length; j++)
                {
                    if (pos.x + verts[j].x > forceFields[i].position.x - forceFields[i].Dimensions.x / 2 && pos.x + verts[j].x < forceFields[i].position.x + forceFields[i].Dimensions.x / 2 &&
                        pos.y + verts[j].y > forceFields[i].position.y - forceFields[i].Dimensions.y / 2 && pos.y + verts[j].y < forceFields[i].position.y + forceFields[i].Dimensions.y / 2)
                    {
                        vel += forceFields[i].appliedForce / verts.Length;
                        vel *= forceFields[i].addedFriction;
                        rotVel *= forceFields[i].addedFriction;
                    }
                }
            }
        }

        public void ResolveStaticCollision(Vector2 edgeDir, Vector2 edgeOffset, Vector2 currentVert)
        {
            Vector2 edgeDirNorm = edgeDir.Normalize();

            pos -= edgeDir.Normal().Normalized() * (pos + currentVert + vel - edgeOffset).Dot(edgeDir.Normal().Normalized());


            Vector2 totalVel = vel + rotVel * currentVert.Normal().Normalize();

            //applying rotation impulse
            rotVel -= ((currentVert.Normal().Normalize().Dot(edgeDir.Normal().Normalize()) * totalVel.Length())) * inertia;

            //friction
            float frictionLength = edgeDirNorm.Normal().Dot(gravity) * friction;

            Vector2 frictionForce = edgeDirNorm * totalVel.Dot(edgeDirNorm) * -frictionLength;

            if (frictionForce.Length() >= vel.Length())
            {
                vel = new Vector2(0, 0);
            }
            else
            {
                vel += frictionForce;
            }

            //mirroring the velocity on the impact edge
            vel = vel - 2 * (vel.Dot(edgeDir.Normal().Normalize())) * edgeDir.Normal().Normalize() * bounciness;
        }



        public void ResolveDynamicCollision(Vector2 impactEdge, Vector2 impactNormal, Vector2 pointOfImpact, Polygon other, Vector2 currentVert, int closestEdge, bool selfActive)
        {
            //snap the colliding vertice to the colliding edge
            if (selfActive) pos -= impactEdge.Normal().Normalized() * ((currentVert + pos) - (other.verts[closestEdge] + other.pos)).Dot(impactEdge.Normal().Normalized());

            other.pos += impactEdge.Normal().Normalized() * ((currentVert + pos) - (other.verts[closestEdge] + other.pos)).Dot(impactEdge.Normal().Normalized());


            Vector2 relativeVel = ((vel + Vector2.Deg2Rad(rotVel) * (pointOfImpact - pos).Normal().Normalized()) - (other.vel + Vector2.Deg2Rad(other.rotVel) * (pointOfImpact - other.pos).Normal().Normalized()));


            //calculate the impulse
            float impulse = (-(1 + bounciness) * relativeVel.Dot(impactNormal)) /
                                (impactNormal.Dot(impactNormal) * (1 / mass + 1 / other.mass));


            //add velocity
            if (selfActive) vel += (impulse / mass) * impactNormal;

            other.vel -= (impulse / other.mass) * impactNormal;


            //applying rotation impulse
            if (isActive)
                rotVel -= (((pointOfImpact - pos).Normal().Normalize().Dot(impactNormal.Normalize()) * relativeVel.Length())) * inertia;

            if (other.isActive)
                other.rotVel += (((pointOfImpact - other.pos).Normal().Normalize().Dot(impactNormal.Normalize()) * relativeVel.Length())) * other.inertia;


            //add friction force
            float frictionLength = impactNormal.Dot(gravity) * friction;

            Vector2 frictionForce = impactNormal.Normal().Normalize() * relativeVel.Dot(impactNormal.Normal().Normalize()) * -frictionLength;

            if (frictionForce.Length() >= vel.Length())
            {
                vel = new Vector2(0, 0);
            }
            else
            {
                vel += frictionForce;
            }
        }
    }
}