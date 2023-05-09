using System;                                   // System contains a lot of default C# libraries 
using GXPEngine;                                // GXPEngine contains the engine
using System.Drawing;                           // System.Drawing contains drawing tools such as Color definitions
using System.Collections.Generic;
using GXPEngine.Core;
using System.Drawing.Drawing2D;
using System.Diagnostics.Tracing;

public class Gravit : Game
{

    bool isRunning;

    Border[] borders;
    Polygon square;

    List<Polygon> polygons;
    List<ForceField> forceFields;

    Turret[] turrets;

    float cameraSpeed = 0.2f;
    Vector2 cameraVel;

    EasyDraw HUD;

    int placing = 0;
    Vector2 placingStart;
    Vector2 placingSize;
    List<Vector2> placingVerts;

    public Gravit() : base(1200, 900, false)
    {
        forceFields = new List<ForceField>();


        Border b2 = new Border(new Vector2(10, 0), new Vector2(0, 450));
        AddChild(b2);

        borders = new Border[] { b2 };


        Turret turret = new Turret(new Vector2(850, -1050));
        AddChild(turret);

        turrets = new Turret[] { turret };


        polygons = new List<Polygon> { turret };


        //BuildLevel();

        cameraVel = new Vector2(0, 0);

        HUD = new EasyDraw(width, height, false);
        AddChild(HUD);

        placingVerts = new List<Vector2>();

        //draw all objects at start of simulation
        foreach (Polygon poly in polygons)
        {
            poly.Step(borders, polygons, forceFields);
        }
    }

    void Update()
    {

        HUD.ClearTransparent();
        HUD.Text("get to the top of the slope!\npress P to start the simulation\n\npress E to place a polygon (clockwise) \npress B to place a block\npress F to place a forcefield\n\nfps: " + currentFps + "\nobjects: " + polygons.Count);
        HUD.SetXY(-x, -y);

        //stepping all objects in the level
        if (isRunning)
        {
            foreach (Polygon poly in polygons)
            {
                poly.Step(borders, polygons, forceFields);
            }
            foreach(Turret turret in turrets)
            {
                turret.Aim(polygons);
            }
        }

        UpdateBuildingInputs();
        UpdateMovement();
        UpdateGeneralInputs();

    }


    void UpdateBuildingInputs()
    {
        if (Input.GetMouseButtonDown(0) && placing >= 1 && placing < 3)
        {
            placingStart = new Vector2(Input.mouseX - x, Input.mouseY - y);
        }
        if (Input.GetMouseButton(0) && placing >= 1)
        {
            PlacingObject(placing, placingStart);
        }
        if (Input.GetMouseButtonUp(0) && placing == 1)
        {
            float size = Mathf.Clamp(new Vector2(Input.mouseX - x, Input.mouseY - y).DistanceTo(placingStart) / 2, 0.1f, 50);
            square = new Polygon(size * size, size, placingStart, true);
            AddChild(square);
            polygons.Add(square);
            placing = 0;
        }
        if (Input.GetMouseButtonUp(0) && placing == 2)
        {
            placingSize = new Vector2(Mathf.Abs(Input.mouseX - placingStart.x - x) * 2, Mathf.Abs(Input.mouseY - placingStart.y - y) * 2);
            placing = 3;
        }
        if (placing == 3)
        {
            PlacingObject(placing, placingStart);
        }
        if (Input.GetMouseButtonDown(0) && placing == 3)
        {
            ForceField ff = new ForceField(placingStart, placingSize, ((new Vector2(Input.mouseX, Input.mouseY) - new Vector2(x, y)) - placingStart) * 0.001f, 1f);
            AddChild(ff);
            forceFields.Add(ff);
            placing = 0;
        }
        if (Input.GetMouseButtonDown(0) && placing == 5)
        {
            placingVerts.Add(new Vector2(Input.mouseX - x, Input.mouseY - y));
        }
        if (Input.GetMouseButtonDown(1) && placing == 5)
        {
            Vector2 averagePosition = new Vector2(0, 0);
            for (int i = 0; i < placingVerts.Count; i++)
            {
                averagePosition += placingVerts[i];
            }
            averagePosition /= placingVerts.Count;
            for (int i = 0; i < placingVerts.Count; i++)
            {
                placingVerts[i] -= averagePosition;
            }

            float furthestVert = 0;
            for (int i = 0; i < placingVerts.Count; i++)
            {
                if (placingVerts[i].Length() > furthestVert)
                {
                    furthestVert = placingVerts[i].Length();
                }
            }

            Polygon polygon = new Polygon(placingVerts.ToArray(), 2, furthestVert, 0, averagePosition, true);
            polygons.Add(polygon);
            AddChild(polygon);

            placing = 0;
        }
        if (placing == 5)
        {
            PlacingObject(placing, placingStart);
        }
        if (Input.GetMouseButtonDown(0) && placing == 4)
        {
            placingVerts.Clear();
            placingVerts.Add(new Vector2(Input.mouseX - x, Input.mouseY - y));
            placing = 5;
        }
    }


    void UpdateMovement()
    {
        bool speedy = false;

        if (Input.GetKey(Key.LEFT_SHIFT) || Input.GetKey(Key.RIGHT_SHIFT))
        {
            cameraSpeed /= 2;
            speedy = true;
        }


        if (Input.GetKey(Key.W)) cameraVel.y += cameraSpeed;
        if (Input.GetKey(Key.A)) cameraVel.x += cameraSpeed;
        if (Input.GetKey(Key.S)) cameraVel.y -= cameraSpeed;
        if (Input.GetKey(Key.D)) cameraVel.x -= cameraSpeed;


        if (speedy == true) cameraSpeed *= 2;

        cameraVel *= 0.97f;

        SetXY(x + cameraVel.x, y + cameraVel.y);
    }
    

    void UpdateGeneralInputs()
    {
        if (Input.GetKeyDown(Key.P)) isRunning = true;

        if (Input.GetKeyDown(Key.B)) placing = 1;
        if (Input.GetKeyDown(Key.F)) placing = 2;
        if (Input.GetKeyDown(Key.E)) placing = 4;

        if (Input.GetKeyDown(Key.F11)) UnitTest.DoTests();
    }

    void PlacingObject(int type, Vector2 placingStart)
    {
        HUD.NoFill();

        switch (type)
        {
            case (1):
                float mouseDistance = new Vector2(Input.mouseX - x, Input.mouseY - y).DistanceTo(placingStart);
                HUD.Rect(placingStart.x + x, placingStart.y + y, mouseDistance, mouseDistance);
                break;
            case (2):
                HUD.Rect(placingStart.x + x, placingStart.y + y, Mathf.Abs(Input.mouseX - placingStart.x - x) * 2, Mathf.Abs(Input.mouseY - placingStart.y - y) * 2);
                break;
            case (3):
                HUD.Rect(placingStart.x + x, placingStart.y + y, placingSize.x, placingSize.y);
                HUD.Line(placingStart.x + x, placingStart.y + y, Input.mouseX, Input.mouseY);
                break;
            case (5):
                for(int i = 0; i < placingVerts.Count - 1; i++)
                {
                    HUD.Line(placingVerts[i].x + x, placingVerts[i].y + y, placingVerts[i + 1].x + x, placingVerts[i + 1].y + y);
                }
                HUD.Line(placingVerts[placingVerts.Count - 1].x + x, placingVerts[placingVerts.Count - 1].y + y, Input.mouseX, Input.mouseY);
                break;
        }
    }

    static void Main()
    {
        new Gravit().Start();
    }

    void BuildLevel()
    {
        Polygon s1 = new Polygon(new Vector2[] { new Vector2(0, 0), new Vector2(300, 0), new Vector2(300, 150), new Vector2(0, 150) }, 1000000, 300, 0, new Vector2(0, 0), false);
        AddChild(s1);
        polygons.Add(s1);

        Polygon s2 = new Polygon(new Vector2[] { new Vector2(-100, 100), new Vector2(200, -300), new Vector2(200, -100), new Vector2(-100, 250) }, 1000000, 200, 0, new Vector2(400, -250), false);
        AddChild(s2);
        polygons.Add(s2);

        Polygon s3 = new Polygon(new Vector2[] { new Vector2(-50, -50), new Vector2(50, -50), new Vector2(50, 50), new Vector2(-50, 50) }, 1000000, 300, 0, new Vector2(650, -400), false);
        AddChild(s3);
        polygons.Add(s3);

        Polygon s4 = new Polygon(new Vector2[] { new Vector2(-50, -150), new Vector2(50, -150), new Vector2(50, 50), new Vector2(-50, 50) }, 1000000, 300, 0, new Vector2(750, -400), false);
        AddChild(s4);
        polygons.Add(s4);

        Polygon s5 = new Polygon(new Vector2[] { new Vector2(-50, 0), new Vector2(100, -200), new Vector2(100, 0), new Vector2(-50, 150) }, 1000000, 300, 0, new Vector2(850, -600), false);
        AddChild(s5);
        polygons.Add(s5);

        Polygon s6 = new Polygon(new Vector2[] { new Vector2(-50, 0), new Vector2(0, -50), new Vector2(0, 50), new Vector2(-50, 100) }, 1000000, 300, 0, new Vector2(1000, -700), false);
        AddChild(s6);
        polygons.Add(s6);

        Polygon s7 = new Polygon(new Vector2[] { new Vector2(-50, -50), new Vector2(100, -200), new Vector2(100, 0), new Vector2(-50, 150) }, 1000000, 300, 0, new Vector2(1050, -800), false);
        AddChild(s7);
        polygons.Add(s7);

        Polygon s8 = new Polygon(new Vector2[] { new Vector2(-50, -50), new Vector2(300, -50), new Vector2(300, 50), new Vector2(-50, 50) }, 1000000, 300, 0, new Vector2(1200, -850), false);
        AddChild(s8);
        polygons.Add(s8);

        Polygon s9 = new Polygon(new Vector2[] { new Vector2(-50, -250), new Vector2(50, -250), new Vector2(50, 250), new Vector2(-50, 250) }, 1000000, 300, 0, new Vector2(1550, -1150), false);
        AddChild(s9);
        polygons.Add(s9);

        Polygon s10 = new Polygon(new Vector2[] { new Vector2(-50, -50), new Vector2(250, -50), new Vector2(250, 50), new Vector2(-50, 50) }, 1000000, 300, 0, new Vector2(1650, -1350), false);
        AddChild(s10);
        polygons.Add(s10);

        Polygon s11 = new Polygon(new Vector2[] { new Vector2(-800, -600), new Vector2(1200, -600), new Vector2(-100, 550), new Vector2(-800, 550) }, 1000000, 3000, 0, new Vector2(300, -1200), false);
        AddChild(s11);
        polygons.Add(s11);

        Polygon s12 = new Polygon(new Vector2[] { new Vector2(-100, -500), new Vector2(100, -500), new Vector2(100, 300), new Vector2(-100, 300) }, 1000000, 300, 0, new Vector2(-600, -150), false);
        AddChild(s12);
        polygons.Add(s12);

        Polygon s13 = new Polygon(new Vector2[] { new Vector2(-100, 50), new Vector2(100, -100), new Vector2(100, -50), new Vector2(-100, 150) }, 1000000, 300, 0, new Vector2(900, -950), false);
        AddChild(s13);
        polygons.Add(s13);

        Polygon s14 = new Polygon(new Vector2[] { new Vector2(-100, 100), new Vector2(0, 100), new Vector2(0, 250), new Vector2(-100, 250) }, 1000000, 200, 0, new Vector2(300, -250), false);
        AddChild(s14);
        polygons.Add(s14);

        ForceField ff = new ForceField(new Vector2(-250, 100), new Vector2(500, 100), new Vector2(0, -0.08f), 0.998f);
        AddChild(ff);
        forceFields.Add(ff);
    }
}
