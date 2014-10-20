using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;

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
        #region Dependencies

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Texture2D snakeTex;
        Texture2D border;

        List<Vector2> snakeStruct;
        List<Rectangle> bordersRect;

        Rectangle headSnake;

        int cw = 10;
        int score = 0;        

        string dir;

        private enum GameState
        {
            Pause,
            Playing,
            Win,
            Lose
        }

        GameState stateOfGame;

        #endregion

        #region Initial Functions

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferMultiSampling = true;
            graphics.PreferredBackBufferWidth = 800;
            graphics.PreferredBackBufferHeight = 600;
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
            stateOfGame = GameState.Playing;

            createBorders();
            createSnake();

            base.Initialize();
        }

        #endregion

        #region Content

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            snakeTex = Content.Load<Texture2D>("Snake");
            border = new Texture2D(GraphicsDevice, 1, 1);
            border.SetData<Color>(new Color[] { Color.White });
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
        }

        #endregion

        #region Update

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            switch (stateOfGame)
            {
                case GameState.Playing:
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

                        headSnake.X = (int)headPos.X;
                        headSnake.Y = (int)headPos.Y;

                        snakeStruct[snakeStruct.Count - 1] = headPos;
                        //Console.WriteLine("HeadPos.X: " + headPos.X + "  HeadPos.Y: " + headPos.Y);

                        for (int i = snakeStruct.Count - 2; i >= 0; i--)
                        {
                            //Console.WriteLine("First Was: " + i + "° Body.X: " + snakeStruct[i].X + "  " + i + "° Body.Y: " + snakeStruct[i].Y);

                            snakeStruct[i] = new Vector2(snakeStruct[i].X + (headPos.X - snakeStruct[i].X), snakeStruct[i].Y + (headPos.Y - snakeStruct[i].Y));

                            //Console.WriteLine("Later Was: " + i + "° Body.X: " + snakeStruct[i].X + "  " + i + "° Body.Y: " + snakeStruct[i].Y);
                        }

                        #region Collisions

                        foreach (Rectangle borderRect in bordersRect)
                        {
                            if (borderRect.Intersects(headSnake))
                                stateOfGame = GameState.Lose;

                        }

                        #endregion
                    }
                    break;

                case GameState.Lose:
                    {
                        Initialize();
                    }
                    break;
            }

            base.Update(gameTime);
        }

        #endregion

        #region Draw

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);

            spriteBatch.Begin();

            //Draw snake
            foreach (Vector2 body in snakeStruct)
                spriteBatch.Draw(snakeTex, body, Color.White);

            //Draw borders of the game
            DrawLine();

            spriteBatch.End();

            base.Draw(gameTime);
        }

        protected void DrawLine()
        {
            foreach (Rectangle borderRect in bordersRect)
            {
                spriteBatch.Draw(
                    border, //Texture2D
                    borderRect,
                    null, //Source Rectangle 
                    Color.Black, //Color of line
                    0, //Rotation
                    Vector2.Zero, //Origin
                    SpriteEffects.None, //Sprite Effect
                    0); //LayerDepth
            }

        }

        #endregion

        #region My Functions

        protected void createBorders()
        {
            Rectangle temp = new Rectangle();
            bordersRect = new List<Rectangle>();

            Vector2 edge, startPoint, endPoint;

            //Upper Border
            startPoint = new Vector2(5, 5);
            endPoint = new Vector2(795, 5);
            edge = endPoint - startPoint;

            temp.X = (int)startPoint.X;
            temp.Y = (int)startPoint.Y;
            temp.Width = (int)edge.Length();
            temp.Height = 2;

            bordersRect.Add(temp);
            //End Upper Border

            //Bottom Border
            startPoint = new Vector2(5, 595);
            endPoint = new Vector2(795, 595);
            edge = endPoint - startPoint;

            temp.X = (int)startPoint.X;
            temp.Y = (int)startPoint.Y;
            temp.Width = (int)edge.Length();
            temp.Height = 2;

            bordersRect.Add(temp);
            //End Bottom Border

            //Left Border
            startPoint = new Vector2(5, 5);
            endPoint = new Vector2(5, 595);
            edge = endPoint - startPoint;

            temp.X = (int)startPoint.X;
            temp.Y = (int)startPoint.Y;
            temp.Width = 2;
            temp.Height = (int)edge.Length();

            bordersRect.Add(temp);
            //End Left Border

            //Right Border
            startPoint = new Vector2(793, 5);
            endPoint = new Vector2(793, 597);
            edge = endPoint - startPoint;

            temp.X = (int)startPoint.X;
            temp.Y = (int)startPoint.Y;
            temp.Width = 2;
            temp.Height = (int)edge.Length();

            bordersRect.Add(temp);
            //End Right Border
        }

        protected void createSnake()
        {
            int length = 5;
            dir = "right";

            snakeStruct = new List<Vector2>();

            for (int i = 0; i < length; i++)
            {
                snakeStruct.Add(new Vector2(i * cw + 15, 18));
            }

            headSnake = new Rectangle((int)snakeStruct[snakeStruct.Count - 1].X, (int)snakeStruct[snakeStruct.Count - 1].Y, 10, 10);
        }

        #endregion
    }
}
