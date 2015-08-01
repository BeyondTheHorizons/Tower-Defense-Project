using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Duality;

namespace Tower_Defense_Project
{
    public enum CustomCursor { BL_Cursor, BLL_Cursor, GL_Cursor, IT_Cursor, OL_Cursor, RL_Cursor, SS_Cursor, YL_Cursor, }

    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Main : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Level level;

        bool keyPressed, keyDidSomething;
        static CustomCursor cursorType = CustomCursor.GL_Cursor;
        static Texture2D currentCursor, emptyCursor, selectedCursor;
        Texture2D[] cursors = new Texture2D[8];
        int offset;

        public static bool IsCustomMouseVisible
        {
            get { return isCustomMouseVisible; }
            set { isCustomMouseVisible = value; }
        }
        private static bool isCustomMouseVisible = true;

        public static CustomCursor CursorType
        {
            get { return cursorType; }
            set { cursorType = value; }
        }

        public static MouseState CurrentMouse
        {
            get { return mouse; }
            set { mouse = value; }
        }
        private static MouseState mouse;

        private static Texture2D SelectedCursor
        {
            get { return selectedCursor; }
            set { selectedCursor = value; }
        }

        public Main()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            /*graphics.PreferredBackBufferHeight = 900;
            graphics.PreferredBackBufferWidth = 1440;
            graphics.IsFullScreen = true;
            IsMouseVisible = true;*/
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            ErrorHandler.Initialize();

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            emptyCursor = Content.Load<Texture2D>(@"Textures/Null");

            cursors[0] = Content.Load<Texture2D>(@"Textures/Cursors/BLL Cursor");
            cursors[1] = Content.Load<Texture2D>(@"Textures/Cursors/BL Cursor");
            cursors[2] = Content.Load<Texture2D>(@"Textures/Cursors/GL Cursor");
            cursors[3] = Content.Load<Texture2D>(@"Textures/Cursors/IT Cursor");
            cursors[4] = Content.Load<Texture2D>(@"Textures/Cursors/OL Cursor");
            cursors[5] = Content.Load<Texture2D>(@"Textures/Cursors/RL Cursor");
            cursors[6] = Content.Load<Texture2D>(@"Textures/Cursors/SS Cursor");
            cursors[7] = Content.Load<Texture2D>(@"Textures/Cursors/YL Cursor");

            level = new Level(Services, graphics);
            level.LoadLevel(1);
            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (IsCustomMouseVisible)
            {
                currentCursor = cursors[(int)CursorType];
                if (currentCursor == Content.Load<Texture2D>(@"Textures/Cursors/BL Cursor"))
                {
                    offset = 2;
                }
                else
                {
                    offset = 1;
                }
            }
            else
            {
                currentCursor = emptyCursor;
            }

            Input();

            level.Update(gameTime);

            base.Update(gameTime);
        }

        private void Input()
        {
            keyDidSomething = keyPressed && keyDidSomething;

            if(Keyboard.GetState().IsKeyDown(Keys.E))
            {
                keyPressed = true;
                if(!keyDidSomething)
                {
                    CursorType = (CustomCursor)((int)CursorType + 1);
                    keyDidSomething = true;
                }
            }
            else if(Keyboard.GetState().IsKeyDown(Keys.Q))
            {
                keyPressed = true;
                if (!keyDidSomething)
                {
                    CursorType = (CustomCursor)((int)CursorType - 1);
                    keyDidSomething = true;
                }
            }
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();

            // TODO: Add your drawing code here

            level.Draw(gameTime, spriteBatch);

            spriteBatch.Draw(currentCursor, new Vector2(CurrentMouse.X - offset, CurrentMouse.Y - offset), Color.White);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
