using SplashKitSDK;
using System;
using System.Collections.Generic;

public struct Bird
{
    public float x;
    public float y;
    public float velocity;
}

public struct Pipe
{
    public float x;
    public float gap_y;
    public float width;
    public float gap_height;
    public bool scored;
}

public struct Cloud
{
    public float x;
    public float y;
    public float speed;
}

class Program
{
    const int WINDOW_W = 800;
    const int WINDOW_H = 600;

    //----------------------------------
    // CREATE OBJECTS
    //----------------------------------

    static Bird MakeBird()
    {
        Bird b = new Bird();

        b.x = 150;
        b.y = WINDOW_H / 2;
        b.velocity = 0;

        return b;
    }

    static Pipe MakePipe(float start_x)
    {
        Pipe p = new Pipe();

        p.x = start_x;
        p.gap_y = (float)SplashKit.Rnd(150, WINDOW_H - 150);

        p.width = 90;
        p.gap_height = 180;
        p.scored = false;

        return p;
    }

    static Cloud MakeCloud(float start_x)
    {
        Cloud c = new Cloud();

        c.x = start_x;
        c.y = (float)SplashKit.Rnd(40, 200);
        c.speed = (float)SplashKit.Rnd(5/10, 15/10);

        return c;
    }

    //----------------------------------
    // DRAW FUNCTIONS
    //----------------------------------

    static void DrawBirdShape(Bird b)
    {
        float r = 18;

        // Body
        SplashKit.FillCircle(
            Color.Yellow,
            b.x,
            b.y,
            r
        );

        // Wing
        SplashKit.FillCircle(
            Color.White,
            b.x - 10,
            b.y,
            r * 0.6f
        );

        // Eye
        SplashKit.FillCircle(
            Color.White,
            b.x + 10,
            b.y - 8,
            r * 0.45f
        );

        // Pupil
        SplashKit.FillCircle(
            Color.Black,
            b.x + 12,
            b.y - 8,
            r * 0.2f
        );

        // Beak
        SplashKit.FillRectangle(
            Color.Orange,
            b.x + r,
            b.y - 3,
            12,
            6
        );
    }

    static void DrawCloud(Cloud c)
    {
        SplashKit.FillCircle(Color.White, c.x, c.y, 25);

        SplashKit.FillCircle(
            Color.White,
            c.x + 30,
            c.y + 10,
            30
        );

        SplashKit.FillCircle(
            Color.White,
            c.x - 30,
            c.y + 10,
            28
        );

        SplashKit.FillCircle(
            Color.White,
            c.x + 15,
            c.y - 10,
            20
        );
    }

    static void DrawPipe(Pipe p)
    {
        SplashKit.FillRectangle(
            Color.Green,
            p.x,
            0,
            p.width,
            p.gap_y - p.gap_height / 2
        );

        SplashKit.FillRectangle(
            Color.Green,
            p.x,
            p.gap_y + p.gap_height / 2,
            p.width,
            WINDOW_H - (p.gap_y + p.gap_height / 2)
        );
    }

    //----------------------------------
    // UPDATE FUNCTIONS
    //----------------------------------

    static void UpdateBird(ref Bird b)
    {
        b.velocity += 0.4f;
        b.y += b.velocity;

        if (SplashKit.KeyTyped(KeyCode.SpaceKey))
        {
            b.velocity = -7;
        }
    }

    static void UpdatePipes(List<Pipe> pipes)
    {
        for (int i = 0; i < pipes.Count; i++)
        {
            Pipe p = pipes[i];

            p.x -= 3;

            if (p.x < -120)
            {
                p.x = WINDOW_W + 200;

                p.gap_y = (float)SplashKit.Rnd(
                    150,
                    WINDOW_H - 150
                );

                p.scored = false;
            }

            pipes[i] = p;
        }
    }

    static void UpdateClouds(List<Cloud> clouds)
    {
        for (int i = 0; i < clouds.Count; i++)
        {
            Cloud c = clouds[i];

            c.x -= c.speed;

            if (c.x < -150)
            {
                c.x = WINDOW_W + (float)SplashKit.Rnd(100, 300);

                c.y = (float)SplashKit.Rnd(40, 200);

                c.speed = (float)SplashKit.Rnd(05/10, 15/10);
            }

            clouds[i] = c;
        }
    }

    //----------------------------------
    // COLLISION CHECK
    //----------------------------------

    static bool Collision(Bird b, Pipe p)
    {
        float r = 18;

        bool hit_pipe =
            (b.x + r > p.x &&
             b.x - r < p.x + p.width) &&

            (
                b.y - r < p.gap_y - p.gap_height / 2 ||

                b.y + r > p.gap_y + p.gap_height / 2
            );

        bool hit_edge =
            (b.y - r < 0 ||
             b.y + r > WINDOW_H);

        return hit_pipe || hit_edge;
    }

    //----------------------------------
    // BUTTON CLICK
    //----------------------------------

    static bool ButtonClicked(
        float x,
        float y,
        float w,
        float h
    )
    {
        return
            SplashKit.MouseClicked(MouseButton.LeftButton) &&

            SplashKit.MouseX() > x &&
            SplashKit.MouseX() < x + w &&

            SplashKit.MouseY() > y &&
            SplashKit.MouseY() < y + h;
    }

    //----------------------------------
    // MAIN
    //----------------------------------

    public static void Main()
    {
        Window window = new Window(
            "Flappy Bird",
            WINDOW_W,
            WINDOW_H
        );

    start_game:

        Bird bird = MakeBird();

        List<Pipe> pipes = new List<Pipe>()
        {
            MakePipe(500),
            MakePipe(800),
            MakePipe(1100)
        };

        List<Cloud> clouds = new List<Cloud>()
        {
            MakeCloud(200),
            MakeCloud(550),
            MakeCloud(900)
        };

        int score = 0;

        bool game_over = false;

        while (!window.CloseRequested)
        {
            SplashKit.ProcessEvents();

            SplashKit.ClearScreen(Color.Cyan);

            //----------------------------------
            // CLOUDS
            //----------------------------------

            UpdateClouds(clouds);

            foreach (Cloud c in clouds)
            {
                DrawCloud(c);
            }

            //----------------------------------
            // GAME UPDATE
            //----------------------------------

            if (!game_over)
            {
                UpdateBird(ref bird);

                UpdatePipes(pipes);
            }

            //----------------------------------
            // DRAW PIPES
            //----------------------------------

            foreach (Pipe p in pipes)
            {
                DrawPipe(p);
            }

            //----------------------------------
            // DRAW BIRD
            //----------------------------------

            DrawBirdShape(bird);

            //----------------------------------
            // SCORE + COLLISION
            //----------------------------------

            for (int i = 0; i < pipes.Count; i++)
            {
                Pipe p = pipes[i];

                if (!p.scored &&
                    bird.x > p.x + p.width)
                {
                    score++;

                    p.scored = true;

                    pipes[i] = p;
                }

                if (!game_over &&
                    Collision(bird, p))
                {
                    game_over = true;
                }
            }

            //----------------------------------
            // SCORE TEXT
            //----------------------------------

            SplashKit.DrawText(
                "Score: " + score,
                Color.Black,
                (int)20,
                (int)20
            );

            //----------------------------------
            // GAME OVER
            //----------------------------------

            if (game_over)
            {
                SplashKit.DrawText(
                    "GAME OVER!",
                    Color.Red,
                    (int)300,
                    (int)230
                );

                SplashKit.FillRectangle(
                    Color.White,
                    330,
                    300,
                    150,
                    50
                );

                SplashKit.DrawText(
                    "Restart",
                    Color.Black,
                    (int)365,
                    (int)315
                );

                if (ButtonClicked(
                    330,
                    300,
                    150,
                    50
                ))
                {
                    goto start_game;
                }
            }

            SplashKit.RefreshScreen(60);
        }
    }
}