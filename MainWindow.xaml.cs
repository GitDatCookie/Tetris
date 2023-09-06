using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Tetris.BlockLogic;
using Block = Tetris.BlockLogic.Block;

namespace Tetris
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly ImageSource[] tileImages = new ImageSource[]
        {
            new BitmapImage(new Uri("Assets/TileEmpty.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/TileCyan.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/TileBlue.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/TileOrange.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/TileYellow.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/TileGreen.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/TilePurple.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/TileRed.png", UriKind.Relative))
        };

        private readonly ImageSource[] blockImages = new ImageSource[]
        {
            new BitmapImage(new Uri("Assets/BlockEmpty.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/Block-I.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/Block-J.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/Block-L.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/Block-O.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/Block-S.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/Block-T.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/Block-Z.png", UriKind.Relative))
        };

        private readonly Image[,] imageControls;
        private readonly int maxDelay = 1000;
        private readonly int minDelay = 75;
        private readonly int delayDecrease = 25;

        private GameState gameState = new();

        public MainWindow()
        {
            InitializeComponent();
            imageControls = SetupGameCanvas(gameState.GameGrid);
        }

        private Image[,] SetupGameCanvas(Grid grid)
        {
            Image[,] imageControls = new Image [grid.Rows, grid.Columns];
            int cellSize = 25;

            for(int row = 0; row < grid.Rows; row++)
            {
                for(int col = 0; col < grid.Columns; col++)
                {
                    Image imageControl = new Image
                    {
                        Width = cellSize,
                        Height = cellSize
                    };

                    Canvas.SetTop(imageControl, (row - 2) * cellSize +10);
                    Canvas.SetLeft(imageControl, col * cellSize);
                    GameCanvas.Children.Add(imageControl);
                    imageControls[row, col] = imageControl;
                }
            }
            return imageControls;
        }
        private void DrawGrid(Grid grid)
        {
            for (int row = 0; row < grid.Rows; row++)
            {
                for (int col = 0; col < grid.Columns; col++)
                {
                    int id = grid[row, col];
                    imageControls[row, col].Opacity = 1;
                    imageControls[row, col].Source = tileImages[id];
                }
            }
        }

        private void DrawBlock(Block block)
        {
            foreach(Position pos in block.TilePositions())
            {
                imageControls[pos.Row, pos.Column].Opacity = 1;
                imageControls[pos.Row, pos.Column].Source = tileImages[block.ID];
            }
        }

        private void DrawHeldBlock(Block heldBlock)
        {
            if(heldBlock == null)
            {
                HoldImage.Source = blockImages[0];
            }
            else
            {
                HoldImage.Source = blockImages[heldBlock.ID];
            }
        }
        private void DrawNextBlock(BlockQueue blockQueue)
        {
            Block next = blockQueue.NextBlock;
            NextImage.Source = blockImages[next.ID];
        }

        private void DrawGhostBlock(Block ghostBlock)
        {
            int dropDistance = gameState.BlockDropDistance();
            foreach(Position pos in ghostBlock.TilePositions())
            {
                imageControls[pos.Row + dropDistance, pos.Column].Opacity = 0.25;
                imageControls[pos.Row + dropDistance, pos.Column].Source = tileImages[ghostBlock.ID];
            }
        }
        private void Draw(GameState gamestate)
        {
            DrawGrid(gamestate.GameGrid);
            DrawGhostBlock(gamestate.CurrentBlock);
            DrawBlock(gamestate.CurrentBlock);
            DrawNextBlock(gameState.BlockQueue);
            DrawHeldBlock(gamestate.HeldBlock);
            ScoreText.Text = $"Score: {gamestate.Score}";
        }
        private async Task GameLoop()
        {
            Draw(gameState);
            while (!gameState.GameOver)
            {
                int delay = Math.Max(minDelay, maxDelay - (gameState.Score * delayDecrease));
                await Task.Delay(500);
                gameState.MoveBlockDown();
                Draw(gameState);
            }

            GameOverMenu.Visibility = Visibility.Visible;
            FinalScoreText.Text = $"Final Score: {gameState.Score}";
        }
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (gameState.GameOver)
            {
                return;
            }

            switch (e.Key)
            {
                case Key.Left:
                    gameState.MoveBlockLeft();
                    break;
                case Key.Right:
                    gameState.MoveBlockRight();
                    break;
                case Key.Down:
                    gameState.DropBlock();
                    //gameState.MoveBlockDown();
                    break;
                case Key.Back:
                    gameState.RotateBlockCounterClockWise();
                    break;
                case Key.Enter:
                    gameState.RotateBlockClockWise();
                    break;
                case Key.C:
                    gameState.HoldBlock();
                    break;
                //case Key.Space:
                //    gameState.DropBlock();
                //    break;
                default:
                    return;
            }
            Draw(gameState);
        }

        private async void PlayAgain_Click(object sender, RoutedEventArgs e)
        {
            gameState = new GameState();
            GameOverMenu.Visibility = Visibility.Hidden;
            await GameLoop();
        }

        private async void GameCanvas_Loaded(object sender, RoutedEventArgs e)
        {
            await GameLoop();
        }
    }
}
