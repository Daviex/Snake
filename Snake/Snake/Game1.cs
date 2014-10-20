using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Snake
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Texture2D snakeTex;

        List<Vector2> snakeStruct;
        Rectangle snakeBlocks;

        TextWriter text = new StreamWriter("cordinates.txt");

        int cw = 10;
        int score = 0;

        string dir;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            createSnake();



            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            snakeTex = Content.Load<Texture2D>("Snake");

            spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            Vector2 headPos = (Vector2)snakeStruct[snakeStruct.Count - 1];

            if (Keyboard.GetState().IsKeyDown(Keys.Right) && dir != "left")
                dir = "right";
            else if (Keyboard.GetState().IsKeyDown(Keys.Left) && dir != "right")
                dir = "left";
            else if (Keyboard.GetState().IsKeyDown(Keys.Up) && dir != "down")
                dir = "up";
            else if (Keyboard.GetState().IsKeyDown(Keys.Down) && dir != "up")
                dir = "down";

            if (dir == "right")
                headPos.X++;
            else if (dir == "left")
                headPos.X--;
            else if (dir == "up")
                headPos.Y--;
            else if (dir == "down")
                headPos.Y++;

            snakeStruct[snakeStruct.Count - 1] = headPos;
            text.WriteLine("HeadPos.X: " + headPos.X + "  HeadPos.Y: " + headPos.Y);

            for (int i = snakeStruct.Count - 2; i >= 0; i--)
            {
                text.WriteLine("First Was: " + i + "° Body.X: " + snakeStruct[i].X + "  " + i + "° Body.Y: " + snakeStruct[i].Y);

                snakeStruct[i] = new Vector2(snakeStruct[i].X + (headPos.X - snakeStruct[i].X) - 10, snakeStruct[i].Y + (headPos.Y - snakeStruct[i].Y) - 10);

                text.WriteLine("Later Was: " + i + "° Body.X: " + snakeStruct[i].X + "  " + i + "° Body.Y: " + snakeStruct[i].Y);
                text.Flush();
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);

            spriteBatch.Begin();

            foreach (Vector2 body in snakeStruct)
                spriteBatch.Draw(snakeTex, body, Color.White);

            spriteBatch.End();

            base.Draw(gameTime);
        }

        protected void createSnake()
        {
            int length = 5;
            dir = "right";

            snakeStruct = new List<Vector2>();

            for (int i = 0; i < length; i++)
            {
                snakeStruct.Add(new Vector2(i * cw + 2, 2));
            }
        }
    }
}
