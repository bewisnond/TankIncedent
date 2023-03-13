using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.IO;
using TankIncedent;

namespace TankIncedent
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        Point windowSize = new Point(1600, 900);
        Texture2D _spriteParticleTxr, _spriteRocksTxr, _spriteShipTxr, _backgroundTxr, _cityTxr;
        SoundEffect _explosion, _soundtrack, _failure;
        SpriteFont _mainFont, _titleFont;
        Point _screenSize = new Point(800, 800);
        List<Sprite> _spriteList = new List<Sprite>();
        GameStates _currentstate = GameStates.Title;
        
        //
        

        float _shootCounter = 0f;
        float _shootTime = 0.5f;
        bool _isEscapePressed, _isSpacePressed;

        int _numberOfAsteriods = 5;
        float _asteriodSpeed = 1.7f;
        int _numberOfAsteriodFragments = 2;

        int _cityHealth = 10;
        int _currentScore = 0;
        int _hiScore = 0;
        int _baseScore = 10;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        // Applys window size
        protected override void Initialize()
        {
            _graphics.PreferredBackBufferWidth = _screenSize.X;
            _graphics.PreferredBackBufferHeight = _screenSize.Y;
            _graphics.ApplyChanges();

            base.Initialize();
        }
        //Loads in the images and fonts
        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _cityTxr = Content.Load<Texture2D>("city");
            _spriteParticleTxr = Content.Load<Texture2D>("tank shell");
            _spriteRocksTxr = Content.Load<Texture2D>("SpriteDogs");
            _spriteShipTxr = Content.Load<Texture2D>("onetankfull");
            _backgroundTxr = Content.Load<Texture2D>("desert background");

            _titleFont = Content.Load<SpriteFont>("Sampted");
            _mainFont = Content.Load<SpriteFont>("MainFont");

            _explosion = Content.Load<SoundEffect>("explosion");
            _soundtrack = Content.Load<SoundEffect>("soundtrack");
            _failure = Content.Load<SoundEffect>("fail");

            LoadHiScore();
        }


        protected override void Update(GameTime gameTime)
        {
            KeyboardState keyboardState = Keyboard.GetState();

            

            //Updates title, game and gameover screen
            switch (_currentstate)
            {
                case GameStates.Title:
                    Update_Title(gameTime, keyboardState, _spriteList);
                    break;

                case GameStates.Game:
                    Update_Game(gameTime, keyboardState, _spriteList);
                    break;
                case GameStates.Gameover:
                    Update_GameOver(gameTime, keyboardState, _spriteList);
                    break;
            }

            



            if (_spriteList.OfType<AsteriodSprite>().ToList().Count < _numberOfAsteriods)
            {
                SpawnRandomAstroid();
            }

            foreach (Sprite everySprite in _spriteList)
            {
                everySprite.Update(gameTime, _spriteList, _screenSize);
            }

            _spriteList.RemoveAll(deadSprite => deadSprite._spriteDead);

            base.Update(gameTime);
        }

        void Update_Title(GameTime gameTime, KeyboardState keyboardState, List<Sprite> spriteList)
        {
            Random random = new Random();

            

            //to exit game when in title screen
            if (keyboardState.IsKeyDown(Keys.Escape) && !_isEscapePressed)
            {
                Exit();
            }
            else if (!keyboardState.IsKeyDown(Keys.Escape) && _isEscapePressed)
            {
                _isEscapePressed = false;
            }
            if (keyboardState.IsKeyDown(Keys.Space))
            {
                NewGame();
                _currentstate = GameStates.Game;

            }
        }

        void NewGame()
        {

            _spriteList.Clear();

            _spriteList.Add(new CitySprite(_cityTxr,
                new Vector2(_screenSize.X / 2,
                _screenSize.Y / 2), Vector2.Zero,
                (float)Math.PI * 2));
            _soundtrack.Play();
            _currentstate = GameStates.Game;
            _cityHealth = 10;

            _spriteList.Add(new PlayerSprite(_spriteShipTxr,
                new Vector2(_screenSize.X / 2,
                _screenSize.Y / 2), Vector2.Zero,
                (float)Math.PI * 3 / 2));
            _soundtrack.Play();
            _currentstate = GameStates.Game;
            _currentScore = 0;

        }

        void Update_Game(GameTime gameTime, KeyboardState keyboardState, List<Sprite> spriteList)
        {


            //to press escape and go to the title screen then press escape again to leave
            if (keyboardState.IsKeyDown(Keys.Escape) && !_isEscapePressed)
            {
                QuitToTitle();
                _isEscapePressed = true;
            }
            else if (!keyboardState.IsKeyDown(Keys.Escape) && _isEscapePressed)
            {
                _isEscapePressed = false;
            }

           
            if (keyboardState.IsKeyDown(Keys.K))
            {
                _currentstate = GameStates.Gameover;
            }


            if (keyboardState.IsKeyDown(Keys.Space) && !_isSpacePressed)
            {
                _isSpacePressed = true;
                _shootCounter = _shootTime;
                Shoot(spriteList);
            }

            else if (keyboardState.IsKeyDown(Keys.Space) && _isSpacePressed)
            {
                _shootCounter -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (_shootCounter < 0)
                {
                    Shoot(spriteList);
                    _shootCounter = _shootTime;
                }
            }
            else if (!keyboardState.IsKeyDown(Keys.Space))
            {
                _isSpacePressed = false;
            }

           


            PlayerSprite player = _spriteList.OfType<PlayerSprite>().ToList()[0];

            foreach (AsteriodSprite eachAsteriod in _spriteList.OfType<AsteriodSprite>().ToList())
            {
                float distance = (eachAsteriod._spritePos - player._spritePos).Length();
                if (distance < eachAsteriod._collisionRadius + player._collisionRadius)
                {
                    GameOver();
                    break;
                }
            }
            foreach (AsteriodSprite eachAsteriod in _spriteList.OfType<AsteriodSprite>().ToList())
            {
                foreach (BulletParticleSprite eachBullet in _spriteList.OfType<BulletParticleSprite>().ToList())
                {
                    float distance = (eachAsteriod._spritePos - eachBullet._spritePos).Length();
                    if (distance < eachAsteriod._collisionRadius + eachBullet._collisionRadius)
                    {
                        eachBullet._spriteDead = true;
                        SaveHiScore();
                        DestroyAsteriod(eachAsteriod);
                        _explosion.Play();
                    }
                }
            }




            CitySprite City = _spriteList.OfType<CitySprite>().ToList()[0];

            foreach (AsteriodSprite eachAsteriod in _spriteList.OfType<AsteriodSprite>().ToList())
            {
                float distance = (eachAsteriod._spritePos - City._spritePos).Length();
               if (distance < eachAsteriod._collisionRadius + City._collisionRadius)
                {
                    City.Health =- 1;
                    

                    if(City.Health == -1)
                    {
                        _cityHealth = -1;
                    }

                    if(City.Health <= 0)
                    {
                        GameOver();
                    }

                    break;
                }
            }



        }



        void QuitToTitle()
        {
            Random random = new Random();
            PlayerSprite player = _spriteList.OfType<PlayerSprite>().ToList()[0];

            for (int i = 0; i < 64; i++)
            {
                //_spriteList.Add(new ParticleSprite(_spriteParticleTxr,
                    //player._spritePos,
                   //new Vector2(((float)random.NextDouble() * 4f) - 2f,
                    //((float)random.NextDouble() * 2f) - 1f)),0f);
            }
            player._spriteDead = true;
            SaveHiScore();

            _currentstate = GameStates.Title;
        }

        void GameOver()
        {
            Random random = new Random();
            PlayerSprite player = _spriteList.OfType<PlayerSprite>().ToList()[0];

            for (int i = 0; i < 64; i++)
            {
                ///_//spriteList.Add(new ParticleSprite(_spriteParticleTxr,
               //     player._spritePos,
               //     new Vector2(((float)random.NextDouble() * 4f) - 2f,
                //    ((float)random.NextDouble() * 2f) - 1f)));
            }
            player._spriteDead = true;

            _failure.Play();

            _currentstate = GameStates.Gameover;
        }

        void SpawnRandomAstroid()
        {
            Vector2 newPos;
            Vector2 newVel;
            float newAngle;
            Random random = new Random();

            switch (random.Next(4))
            {
                case 0:
                    newPos = new Vector2(-256, (float)random.NextDouble() * _screenSize.Y);
                    break;
                case 1:
                    newPos = new Vector2(_screenSize.X + 256, (float)random.NextDouble() * _screenSize.Y);
                    break;
                case 2:
                    newPos = new Vector2((float)random.NextDouble() * _screenSize.X, -256);
                    break;
                default:
                    newPos = new Vector2((float)random.NextDouble() * _screenSize.X, _screenSize.Y + 256);
                    break;


            }

            newVel = new Vector2((((float)random.NextDouble() * _asteriodSpeed * 2) - 1) * _asteriodSpeed,
                (((float)random.NextDouble() * _asteriodSpeed * 2) - 1) * _asteriodSpeed);

            newAngle = (float)random.NextDouble() * (float)Math.PI * 2;


            _spriteList.Add(new AsteriodSprite(_spriteRocksTxr, newPos, newVel, newAngle));
        }


        void DestroyAsteriod(AsteriodSprite destroyedAsteriod)
        {

            AddScore(destroyedAsteriod._asteriodSize);

            Random random = new Random();

            for (int i = 0; i < 64; i++)
            {
               // _spriteList.Add(new ParticleSprite(_spriteParticleTxr,
                 //   destroyedAsteriod._spritePos,
                 //   new Vector2(((float)random.NextDouble() * 4f) - 2f,
                 //   ((float)random.NextDouble() * 2f) - 1f)));
            }
            destroyedAsteriod._spriteDead = true;

            if (destroyedAsteriod._asteriodSize < 3)
            {
                float newAngle;
                Vector2 newVel;

                for (int i = 0; i < _numberOfAsteriodFragments; i++)
                {
                    newVel = new Vector2((((float)random.NextDouble() * _asteriodSpeed * 2) - 1) * _asteriodSpeed,
                (((float)random.NextDouble() * _asteriodSpeed * 2) - 1) * _asteriodSpeed);

                    newAngle = (float)(random.NextDouble() * Math.PI * 2);

                    _spriteList.Add(new AsteriodSprite(_spriteRocksTxr,
                        destroyedAsteriod._spritePos,
                        newVel,
                        newAngle,
                        destroyedAsteriod._asteriodSize + 1));
                }
            }

        }




        void Shoot(List<Sprite> spriteList)
        {
            
            PlayerSprite player = _spriteList.OfType<PlayerSprite>().ToList()[0];
            Debug.Print("" + player._spriteAngle);
            spriteList.Add(new BulletParticleSprite(_spriteParticleTxr,
                player._spritePos,
                player.Forward() * 10f, player._spriteAngle * (float)Math.PI));
        }

        void AddScore(int asteriodSize)
        {
            _currentScore += (asteriodSize + 1) * _baseScore;
            if (_currentScore > _hiScore)
            {
                _hiScore = _currentScore;
            }
        }


        void Update_GameOver(GameTime gameTime, KeyboardState keyboardState, List<Sprite> spriteList)
        {
            //to press escape and go to the title screen then press escape again to leave
            if (keyboardState.IsKeyDown(Keys.Escape) && !_isEscapePressed)
            {
                _currentstate = GameStates.Title;
                _isEscapePressed = true;
            }
            else if (!keyboardState.IsKeyDown(Keys.Escape) && _isEscapePressed)
            {
                _isEscapePressed = false;
            }


        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin(SpriteSortMode.BackToFront);

            _spriteBatch.Draw(_backgroundTxr, new Rectangle(0, 0, windowSize.X, windowSize.Y), new Rectangle(0, 0, _backgroundTxr.Width, _backgroundTxr.Height), Color.White, 0f, new Vector2(), SpriteEffects.None, 0f);



            foreach (Sprite everySprite in _spriteList)
            {
                everySprite.Draw(_spriteBatch);
            }

            

            //This will draw anything on either the title, game or gameover screen
            switch (_currentstate)
            {
                case GameStates.Title:
                    Draw_Title(gameTime, _spriteBatch, _spriteList);
                    break;

                case GameStates.Game:
                    Draw_Game(gameTime, _spriteBatch, _spriteList);
                    break;
                case GameStates.Gameover:
                    Draw_GameOver(gameTime, _spriteBatch, _spriteList);
                    break;

            }

            

            _spriteBatch.End();

            base.Draw(gameTime);
        }

        void Draw_Title(GameTime gameTime, SpriteBatch _spriteBatch, List<Sprite> spriteList)
        {
            //writes asteriods on title screen
            Vector2 textSize = _titleFont.MeasureString("Tanks!");
            _spriteBatch.DrawString(_titleFont,
                "Tanks!",
                new Vector2(_screenSize.X / 2 - textSize.X/2, _screenSize.Y / 3 - textSize.Y / 2),
                Color.White);

            textSize = _mainFont.MeasureString("Press Space to start" + _hiScore);
            _spriteBatch.DrawString(_mainFont,
                "Press Space to Start",
                new Vector2(_screenSize.X / 2 - textSize.X / 2, _screenSize.Y / 3 * 2 - textSize.Y / 2),
                Color.White);
        }

        void Draw_Game(GameTime gameTime, SpriteBatch _spriteBatch, List<Sprite> spriteList)
        {
            //draws the score indecators and city health
            
            _spriteBatch.DrawString(_mainFont,
                "score:" + _currentScore,
                new Vector2(10, 10),
                Color.White);

            Vector2 textSize = _mainFont.MeasureString("hi score:" + _hiScore);
            _spriteBatch.DrawString(_mainFont,
               "hi score:" + _hiScore,
               new Vector2(_screenSize.X - 10 - textSize.X, 10),
               Color.White);

            _spriteBatch.DrawString(_mainFont,
                "city health:" + _cityHealth,
                new Vector2(10, 40),
                Color.White);


        }

        void Draw_GameOver(GameTime gameTime, SpriteBatch _spriteBatch, List<Sprite> spriteList)
        {
            //writes asteriods on gameover screen
            Vector2 textSize = _titleFont.MeasureString("GameOver:" );
            _spriteBatch.DrawString(_titleFont,
                "Game Over!",
               new Vector2(_screenSize.X / 2 - textSize.X / 2, _screenSize.Y / 3 - textSize.Y / 2),
                Color.White);

            textSize = _mainFont.MeasureString("Press ESCAPE to START OVER" + _hiScore);
            _spriteBatch.DrawString(_mainFont,
                "Press ESCAPE to START OVER",
                new Vector2(_screenSize.X / 2 - textSize.X / 2, _screenSize.Y / 3 * 2 - textSize.Y / 2),
                Color.White);

            textSize = _mainFont.MeasureString("score: " + _currentScore);
            _spriteBatch.DrawString(_mainFont,
                "score: " + _currentScore,
                new Vector2(_screenSize.X / 3 - textSize.X / 2, _screenSize.Y / 2 - textSize.Y / 2),
                Color.White);

            textSize = _mainFont.MeasureString("hi score: " + _hiScore);
            _spriteBatch.DrawString(_mainFont,
                "hi score: " + _hiScore,
                new Vector2(_screenSize.X / 3 * 2 - textSize.X / 2, _screenSize.Y / 2 - textSize.Y / 2),
                Color.White);
        }
        void LoadHiScore()
        {
            string hiScorePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Asteriods";
            string hiScoreFile = hiScorePath + "\\hiscore.sav";

            if (!Directory.Exists(hiScorePath))
            {
                Directory.CreateDirectory(hiScorePath);
            }

            if (!File.Exists(hiScoreFile))
            {
                File.WriteAllText(hiScoreFile, "0");
            }
            else
            {
                int.TryParse(File.ReadAllText(hiScoreFile), out _hiScore);
            }
        }

        void SaveHiScore()
        {
            string hiScorePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Asteriods";
            string hiScoreFile = hiScorePath + "\\hiscore.sav";

            File.WriteAllText(hiScoreFile, _hiScore.ToString());
        }
    }
    enum GameStates
    {
        Title,
        Game,
        Gameover
    }
}

