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

        private enum GameState
        {
            Pause,
            Playing,
            Win,
            Lose
        }

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Texture2D snakeTex;
        Texture2D border;
        SpriteFont scoreFont;
        SpriteFont endFont;

        List<Vector2> snakeStruct;
        List<Rectangle> bordersRect;

        Rectangle foodPos;
        Rectangle headSnake;

        int cw;
        int score;        

        string dir;
        string oldDir;

        bool isPaused;
        KeyboardState oldState;

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
            TargetElapsedTime = new TimeSpan(0, 0, 0, 0, 50);
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

            cw = 10;
            score = 0;
            isPaused = false;

            createBorders();
            createSnake();
            createFood();

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
            scoreFont = Content.Load<SpriteFont>("ScoreFont");
            endFont = Content.Load<SpriteFont>("endFont");

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
                        var newState = Keyboard.GetState();

                        if (newState.IsKeyDown(Keys.Right) && oldState.IsKeyDown(Keys.Right) && dir != "left")
                            dir = "right";
                        else if (newState.IsKeyDown(Keys.Left) && oldState.IsKeyDown(Keys.Left) && dir != "right")
                            dir = "left";
                        else if (newState.IsKeyDown(Keys.Up) && oldState.IsKeyDown(Keys.Up) && dir != "down")
                            dir = "up";
                        else if (newState.IsKeyDown(Keys.Down) && oldState.IsKeyDown(Keys.Down) && dir != "up")
                            dir = "down";
                        else if (newState.IsKeyDown(Keys.Space) && !oldState.IsKeyDown(Keys.Space) && !isPaused)
                        {
                            isPaused = true;
                            oldDir = dir;
                            dir = "stop";
                            stateOfGame = GameState.Pause;
                        }

                        oldState = newState;

                        if (dir != "stop")
                        {
                            Vector2 headPos = snakeStruct[0];

                            if (dir == "right")
                                headPos.X += 10;
                            else if (dir == "left")
                                headPos.X -= 10;
                            else if (dir == "up")
                                headPos.Y -= 10;
                            else if (dir == "down")
                                headPos.Y += 10;

                            headSnake.X = (int)headPos.X;
                            headSnake.Y = (int)headPos.Y;

                            for (int i = snakeStruct.Count - 1; i > 0; i--)
                            {
                                snakeStruct[i] = snakeStruct[i - 1];
                            }
                            snakeStruct[0] = headPos;     
                        }

                        #region Collisions

                        foreach (Rectangle borderRect in bordersRect)
                        {
                            if (borderRect.Intersects(headSnake))
                                stateOfGame = GameState.Lose;
                        }

                        if (headSnake.Intersects(foodPos))
                        {
                            snakeStruct.Add(snakeStruct[snakeStruct.Count - 1]);
                            foodPos.X = -500;
                            foodPos.Y = -500;

                            score++;

                            createFood();
                        }

                        for (int i = 1; i < snakeStruct.Count - 1; i++)
                        {
                            if (headSnake.Contains((int)snakeStruct[i].X, (int)snakeStruct[i].Y))
                                Initialize();
                        }

                        #endregion    

                        if (score >= 500)
                            stateOfGame = GameState.Win;
                    }
                    break;

                case GameState.Pause:
                    {
                        var newState = Keyboard.GetState();

                        if (newState.IsKeyDown(Keys.Space) && !oldState.IsKeyDown(Keys.Space) && isPaused)
                        {
                            isPaused = false;
                            dir = oldDir;
                            stateOfGame = GameState.Playing;
                        }

                        oldState = newState;
                    }
                    break;

                case GameState.Lose:
                    {
                        if(Keyboard.GetState().IsKeyDown(Keys.Enter))
                            Initialize();
                    }
                    break;

                case GameState.Win:
                    {
                        if (Keyboard.GetState().IsKeyDown(Keys.Enter))
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
        /// <param name="gameTime">Provides xa snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);

            switch(stateOfGame)
            {
                case GameState.Pause:
                case GameState.Playing:
                    {
                        spriteBatch.Begin();

                        //Draw snake
                        foreach (Vector2 body in snakeStruct)
                            spriteBatch.Draw(snakeTex, body, Color.White);

                        //Draw borders of the game
                        DrawLine();

                        //Draw Food
                        DrawFood();

                        //Draw Score
                        spriteBatch.DrawString(scoreFont, "Score: " + score, new Vector2(10, 573), Color.Blue);

                        if(stateOfGame == GameState.Pause)
                        {
                            spriteBatch.Draw(snakeTex, new Rectangle(0, 0, 800, 600), new Color(255, 255, 255, 100));
                            spriteBatch.DrawString(endFont, "   In pause!\nPress  space\n to resume!", new Vector2(150, 100), Color.Black);
                        }


                        spriteBatch.End();
                    }
                    break;

                case GameState.Lose:
                    {                         
                        spriteBatch.Begin();
                        spriteBatch.DrawString(endFont, "        You lose\n        Score: " + score + "\nPress Enter to start again", new Vector2(20, 100), Color.Black);
                        spriteBatch.End();
                    }
                    break;
                    
                case GameState.Win:
                    {
                        spriteBatch.Begin();
                        spriteBatch.DrawString(endFont, "You Win! Congrats!\n        Score: " + score + "\nPress Enter to start again", new Vector2(15, 100), Color.Black);
                        spriteBatch.End();
                    }
                    break;
            }

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

        protected void DrawFood()
        {
            spriteBatch.Draw(
                    snakeTex, //Texture2D
                    foodPos,
                    null, //Source Rectangle 
                    Color.White, //Color of line
                    0, //Rotation
                    Vector2.Zero, //Origin
                    SpriteEffects.None, //Sprite Effect
                    0); //LayerDepth
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

            for (int i = length - 1; i >= 0; i--)
            {
                snakeStruct.Add(new Vector2(i * cw + 15, 18));
            }

            headSnake = new Rectangle((int)snakeStruct[0].X, (int)snakeStruct[0].Y, 10, 10);
        }

        protected void createFood()
        {
            Random rand = new Random(Environment.TickCount);

            int x = rand.Next(10, 780);
            int y = rand.Next(10, 580);

            foodPos = new Rectangle(x, y, 10, 10);
        }

        #endregion
    }
}
