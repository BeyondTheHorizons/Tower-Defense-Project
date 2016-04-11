﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;
using SharpDX.Toolkit.Input;
using Duality;

namespace Tower_Defense_Project
{
    enum TowerType
    {
        Start = 0,
        Point = 1,
        Stop = 2,
        GL = 101,
        RL = 102,
        BLL = 103,
    }

    class Tower
    {
        public bool isPlaced = false, isSelected;
        public Circle range;
        private Color rangeColor = Color.Gray;
        private Designer designer;
        private float attackTimer = 0, minAttackTimer, diameter;
        private int size;
        private Level level;
        private ProjectileType projectileType;
        private RectangleF collision;
        private string spriteSet;
        private Texture2D texture, rangeTex;
        public TowerType type;
        private uint cost;

        public Designer Designer
        {
            get { return designer; }
        }

        public Level Level
        {
            get { return level; }
        }

        public uint Cost
        {
            get { return cost; }
        }

        public Vector2 Position
        {
            get { return collision.Location; }
            set
            {
                Vector2 offset = value - collision.Location;
                range.Offset(offset);
                collision.Location = value;
            }
        }

        public Tower(Level level, TowerType type, MouseState mouse)
        {
            this.level = level;
            this.type = type;

            spriteSet = Level.TowerStats[(int)type][0];
            size = int.Parse(Level.TowerStats[(int)type][1]);
            diameter = float.Parse(Level.TowerStats[(int)type][2]);
            minAttackTimer = float.Parse(Level.TowerStats[(int)type][3]);
            projectileType = (ProjectileType)int.Parse(Level.TowerStats[(int)type][4]);
            cost = uint.Parse(Level.TowerStats[(int)type][5]);

            collision = new RectangleF(mouse.X * Main.Graphics.PreferredBackBufferWidth, mouse.Y * Main.Graphics.PreferredBackBufferHeight, size, size);
            range = new Circle(new Vector2(mouse.X * Main.Graphics.PreferredBackBufferWidth + (collision.Width / 2), mouse.Y * Main.Graphics.PreferredBackBufferHeight + (collision.Height / 2)), diameter);

            LoadContent();
        }

        public Tower(Designer designer, TowerType type, MouseState mouse)
        {
            this.designer = designer;
            this.type = type;

            spriteSet = Designer.TowerStats[(int)type][0];
            size = int.Parse(Designer.TowerStats[(int)type][1]);
            diameter = float.Parse(Designer.TowerStats[(int)type][2]);

            collision = new RectangleF(mouse.X * Main.Graphics.PreferredBackBufferWidth, mouse.Y * Main.Graphics.PreferredBackBufferHeight, size, size);
            range = new Circle(new Vector2(mouse.X * Main.Graphics.PreferredBackBufferWidth + (collision.Width / 2), mouse.Y * Main.Graphics.PreferredBackBufferHeight + (collision.Height / 2)), diameter);

            LoadContent();
        }

        private void LoadContent()
        {
            texture = Main.GameContent.Load<Texture2D>(@"Towers/" + spriteSet);
            rangeTex = Main.GameContent.Load<Texture2D>(@"Towers/" + spriteSet + " Range");
        }

        public void Update(GameTime gameTime, MouseState mouse)
        {
            if (!isPlaced) isPlaced = Placed(mouse);
            else
            {
                attackTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (attackTimer > minAttackTimer)
                {
                    Fire();
                }

                if (mouse.RightButton.Pressed)
                {
                    if (collision.Contains(new Vector2(mouse.X * Main.Graphics.PreferredBackBufferWidth, mouse.Y * Main.Graphics.PreferredBackBufferHeight)))
                    {
                        isSelected = true;
                    }
                    else
                    {
                        isSelected = false;
                    }
                }
            }
        }

        public void UpdateDesigner(GameTime gameTime, MouseState mouse)
        {
            if (!isPlaced) isPlaced = Placed(mouse);
            else if (mouse.RightButton.Pressed)
            {
                if (collision.Contains(new Vector2((mouse.X * Main.Graphics.PreferredBackBufferWidth), mouse.Y * Main.Graphics.PreferredBackBufferHeight)))
                {
                    isSelected = true;
                }
                else
                {
                    isSelected = false;
                }
            }
        }

        private void Fire()
        {
            attackTimer = 0f;
            for (int i = 0; i < Level.enemies.Count; i++)
            {
                if (range.Contains(Level.enemies[i].position))
                {
                    Level.projectiles.Add(new Projectile(this, collision.Location + new Vector2(collision.Width / 2), Level.enemies[i], projectileType, Level));
                    break;
                }
            }
        }

        private bool Placed(MouseState mouse)
        {
            Main.IsCustomMouseVisible = false;
            bool placed = false;
            if (mouse.X * Main.Graphics.PreferredBackBufferWidth >= 0 && mouse.X * Main.Graphics.PreferredBackBufferWidth <= Main.Graphics.PreferredBackBufferWidth && mouse.Y * Main.Graphics.PreferredBackBufferHeight >= 0 && mouse.Y * Main.Graphics.PreferredBackBufferHeight <= Main.Graphics.PreferredBackBufferHeight)
            {
                collision.X = mouse.X * Main.Graphics.PreferredBackBufferWidth;
                collision.Y = mouse.Y * Main.Graphics.PreferredBackBufferHeight;
                range.Center = new Vector2(mouse.X * Main.Graphics.PreferredBackBufferWidth + (collision.Width / 2), mouse.Y * Main.Graphics.PreferredBackBufferHeight + (collision.Height / 2));
                if (CanPlace() || CanPlaceDesigner())
                {
                    rangeColor = Color.Gray;
                    if (mouse.LeftButton.Pressed)
                    {
                        placed = true;
                        isSelected = false;
                        Main.IsCustomMouseVisible = true;
                    }
                }
                else
                {
                    rangeColor = Color.Red;
                    placed = false;
                }
                return placed;
            }
            #region Mouse Off-Screen Handling
            else if (mouse.X * Main.Graphics.PreferredBackBufferWidth <= 0 && mouse.Y * Main.Graphics.PreferredBackBufferHeight >= 0 && mouse.Y * Main.Graphics.PreferredBackBufferHeight <= Main.Graphics.PreferredBackBufferHeight)
            {
                collision.X = 0;
                collision.Y = mouse.Y * Main.Graphics.PreferredBackBufferHeight;
                range.Center = new Vector2(0 + (collision.Width / 2), mouse.Y * Main.Graphics.PreferredBackBufferHeight + (collision.Height / 2));
                return placed;
            }
            else if (mouse.X * Main.Graphics.PreferredBackBufferWidth >= Main.Graphics.PreferredBackBufferWidth && mouse.Y * Main.Graphics.PreferredBackBufferHeight >= 0 && mouse.Y * Main.Graphics.PreferredBackBufferHeight <= Main.Graphics.PreferredBackBufferHeight)
            {
                collision.X = Main.Graphics.PreferredBackBufferWidth;
                collision.Y = mouse.Y * Main.Graphics.PreferredBackBufferHeight;
                range.Center = new Vector2(Main.Graphics.PreferredBackBufferWidth + (collision.Width / 2), mouse.Y * Main.Graphics.PreferredBackBufferHeight + (collision.Height / 2));
                return placed;
            }
            else if (mouse.Y * Main.Graphics.PreferredBackBufferHeight <= 0 && mouse.X * Main.Graphics.PreferredBackBufferWidth >= 0 && mouse.X * Main.Graphics.PreferredBackBufferWidth <= Main.Graphics.PreferredBackBufferWidth)
            {
                collision.X = mouse.X * Main.Graphics.PreferredBackBufferWidth;
                collision.Y = 0;
                range.Center = new Vector2(mouse.X * Main.Graphics.PreferredBackBufferWidth + (collision.Width / 2), 0 + (collision.Height / 2));
                return placed;
            }
            else if (mouse.Y * Main.Graphics.PreferredBackBufferHeight >= Main.Graphics.PreferredBackBufferHeight && mouse.X * Main.Graphics.PreferredBackBufferWidth >= 0 && mouse.X * Main.Graphics.PreferredBackBufferWidth <= Main.Graphics.PreferredBackBufferWidth)
            {
                collision.X = mouse.X * Main.Graphics.PreferredBackBufferWidth;
                collision.Y = Main.Graphics.PreferredBackBufferHeight;
                range.Center = new Vector2(mouse.X * Main.Graphics.PreferredBackBufferWidth + (collision.Width / 2), Main.Graphics.PreferredBackBufferHeight + (collision.Height / 2));
                return placed;
            }
            else if (mouse.X * Main.Graphics.PreferredBackBufferWidth <= 0 && mouse.Y * Main.Graphics.PreferredBackBufferHeight <= 0)
            {
                collision.X = 0;
                collision.Y = 0;
                range.Center = new Vector2(0 + (collision.Width / 2), 0 + (collision.Height / 2));
                return placed;
            }
            else if (mouse.X * Main.Graphics.PreferredBackBufferWidth <= 0 && mouse.Y * Main.Graphics.PreferredBackBufferHeight >= Main.Graphics.PreferredBackBufferHeight)
            {
                collision.X = 0;
                collision.Y = Main.Graphics.PreferredBackBufferHeight;
                range.Center = new Vector2(0 + (collision.Width / 2), Main.Graphics.PreferredBackBufferHeight + (collision.Height / 2));
                return placed;
            }
            else if (mouse.X * Main.Graphics.PreferredBackBufferWidth >= Main.Graphics.PreferredBackBufferWidth && mouse.Y * Main.Graphics.PreferredBackBufferHeight <= 0)
            {
                collision.X = Main.Graphics.PreferredBackBufferWidth;
                collision.Y = 0;
                range.Center = new Vector2(Main.Graphics.PreferredBackBufferWidth + (collision.Width / 2), 0 + (collision.Height / 2));
                return placed;
            }
            else if (mouse.X * Main.Graphics.PreferredBackBufferWidth >= Main.Graphics.PreferredBackBufferWidth && mouse.Y * Main.Graphics.PreferredBackBufferHeight >= Main.Graphics.PreferredBackBufferHeight)
            {
                collision.X = Main.Graphics.PreferredBackBufferWidth;
                collision.Y = Main.Graphics.PreferredBackBufferHeight;
                range.Center = new Vector2(Main.Graphics.PreferredBackBufferWidth + (collision.Width / 2), Main.Graphics.PreferredBackBufferHeight + (collision.Height / 2));
                return placed;
            }
            #endregion
            else
            {
                return placed;
            }
        }

        private bool CanPlace()
        {
            try { return !Level.Path.Intersects(collision) && TowerCheck() && !Level.storeSection.Intersects(collision); }
            catch { return false; }
        }

        private bool CanPlaceDesigner()
        {
            try { return !Designer.Path.Intersects(collision) && PointCheck() && !Designer.storeSection.Intersects(collision); }
            catch { return false; }
        }

        private bool TowerCheck()
        {
            bool check = true;
            for (int i = 0; i < Level.towers.Count - 1; i++)
            {
                if (collision.Intersects(Level.towers[i].collision))
                {
                    check = false;
                }
            }
            return check;
        }

        private bool PointCheck()
        {
            bool check = true;
            for (int i = 0; i < Designer.towers.Count - 1; i++)
            {
                if (collision.Intersects(Designer.towers[i].collision))
                {
                    check = false;
                }
            }
            return check;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (!isPlaced || isSelected)
            {
                spriteBatch.Draw(rangeTex, range.Location, rangeColor);
            }
            spriteBatch.Draw(texture, collision, Color.White);
        }
    }
}