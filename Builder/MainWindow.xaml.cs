using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
using System.Windows.Threading;

namespace Builder
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>

    class Hero
    {
        static int count = 0;
        public int horizontal { get; set; }
        public int vertical { get; set; }
        public bool lose_Control { get; set; }
        public (int horizontal, int vertical) step;
        public Hero (UIElement element)
        {
            lose_Control = false;
            horizontal = Grid.GetColumn(element);
            vertical = Grid.GetRow(element);
            count++;
        }
    }

    public partial class MainWindow : Window
    {

        //private static Color hero_Color = Colors.Green;

        int lenght;
        int height;
        bool game_Active = false;
        int game_Speed = 180;
        int game_Level = 0;
        int lines_Num = 0;
        int tick_Num = 0;
        int time_For_Block = 7;// Ticks
        Random random = new Random();
        DispatcherTimer timer;
        //(int horizontal, int vertical) step;
        Hero hero = null;
        Color[] colors = { Colors.OrangeRed, Colors.SlateBlue, Colors.GreenYellow };
        List<Hero> heroes = null;

        public MainWindow()
        {

            InitializeComponent();

            lenght = Sharp.ColumnDefinitions.Count;
            height = Sharp.RowDefinitions.Count;
            KeyDown += MainWindow_KeyDown;

            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 0, 0, game_Speed);
            timer.Tick += Timer_Tick;

            heroes = new List<Hero>();
        }

        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key.ToString() == "P") 
            {
                Start_Button.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            }
            if(e.Key.ToString() == "E" && heroes.Count == 1)
            {
                Rectangle hero_View = Create_Hero();
                heroes.Add(new Hero(hero_View));
                time_For_Block -= 2;
                game_Speed -= 20;
            }
            if (game_Active)
            {
                switch(e.Key.ToString())
                {
                    case "OemPlus":
                        Level_Up();
                        break;
                    case "OemMinus":
                        Level_Down();
                        break;
                }
                if (!heroes[0].lose_Control)
                    switch (e.Key.ToString())
                    {
                        case "Up":
                            if (heroes[0].vertical > 0)
                            {
                                heroes[0].step = (0, -1);
                                Move(heroes[0]);
                                // Move();
                            }
                            break;

                        case "Left":
                            if (heroes[0].horizontal > 0)
                            {
                                heroes[0].step = (-1, 0);
                                // Move(step); 
                                Move(heroes[0]);
                            }
                            break;

                        case "Right":
                            if (heroes[0].horizontal < lenght - 1)
                            {
                                heroes[0].step = (1, 0);
                                //  Move(step);
                                Move(heroes[0]);
                            }
                            break;
                    }
                if (heroes.Count == 2)
                    if (!heroes[1].lose_Control)
                    
                        switch (e.Key.ToString())
                        {
                            case "W":
                                if (heroes[1].vertical > 0)
                                {
                                    heroes[1].step = (0, -1);
                                    Move(heroes[1]);
                                    // Move();
                                }
                                break;

                            case "A":
                                if (heroes[1].horizontal > 0)
                                {
                                    heroes[1].step = (-1, 0);
                                    // Move(step); 
                                    Move(heroes[1]);
                                }
                                break;

                            case "D":
                                if (heroes[1].horizontal < lenght - 1)
                                {
                                    heroes[1].step = (1, 0);
                                    //  Move(step);
                                    Move(heroes[1]);
                                }
                                break;

                        }
                    
            }
        }

        private void Start_Button_Click(object sender, RoutedEventArgs e)
        {
            if (!game_Active)
            {
                game_Active = true;
                //if(hero == null)
                if (heroes.Count == 0)
                {
                    //Rectangle hero_View = Create_Hero();
                    //hero = new Hero(hero_View);
                    Rectangle hero_View = Create_Hero();
                    heroes.Add(new Hero(hero_View));
                    Restart_Button.Visibility = Visibility.Visible;
                    Game_Over_Text.Visibility = Visibility.Hidden;
                    game_Level = 1;
                }
                Level_Text.Text = "Level: " + game_Level.ToString();
                timer.Start();
                Start_Button.Content = "Stop Game";
            }
            else if(game_Active)
            {
                Start_Button.Content = "Start Game";
                game_Active = false;
                timer.Stop();
            }
        }

        private void Restart_Button_Click(object sender, RoutedEventArgs e)
        {
            if(Restart_Button.Visibility == Visibility.Visible)
            {
                timer.Stop();
                game_Speed = 180;
                time_For_Block = 8;
                timer.Interval = new TimeSpan(0, 0, 0, 0, game_Speed);
                tick_Num = 0;
                Sharp.Children.Clear();
                heroes.Clear();
                Rectangle hero_View = Create_Hero();
                heroes.Add(new Hero(hero_View));
                //heroes = new List<Hero>();
                //Rectangle hero_View = Create_Hero();
                //heroes.Add(new Hero(hero_View));
                //hero = new Hero(hero_View);
                game_Level = 1;
                Level_Text.Text = "Level: " + game_Level.ToString();
                timer.Start();
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            tick_Num++;
            if (tick_Num == time_For_Block)
            {
                int block_Horizontal;
                do {
                    block_Horizontal = random.Next(lenght);
                } while (Get_Rectangle(block_Horizontal, 0) != null);
                
                Rectangle block = Create_Block(block_Horizontal);
                //rectangles.Add(block);
                Block_Falling(block);
                tick_Num = 0;
            }
        }

        private Rectangle Create_Hero()
        {
            Rectangle hero_View = new Rectangle();
            hero_View.Width = 50;
            hero_View.Height = 50;
            hero_View.StrokeThickness = 7;
            if (heroes.Count == 0)
            {
                hero_View.Stroke = Brushes.Black;
                hero_View.Fill = Brushes.Cyan;
                Sharp.Children.Add(hero_View);
                Grid.SetRow(hero_View, height - 1);
                Grid.SetColumn(hero_View, lenght / 2);
            }
            else if (heroes.Count == 1)
            {
                hero_View.Stroke = Brushes.Cyan;
                hero_View.Fill = Brushes.Black;
                Sharp.Children.Add(hero_View);
                Grid.SetRow(hero_View, heroes[0].vertical - 1);
                Grid.SetColumn(hero_View, heroes[0].horizontal);
            }
            return hero_View;
        }

        private Rectangle Create_Block(int block_Horizontal)
        {
            Random rand = new Random();
            Color color = colors[rand.Next(colors.Count())];
            SolidColorBrush brush = new SolidColorBrush(color);
            Rectangle block_View = new Rectangle();
            block_View.Width = 50;
            block_View.Height = 50;
            block_View.StrokeThickness = 1;
            block_View.Stroke = Brushes.Black;
            block_View.Fill = brush;
            Sharp.Children.Add(block_View);
            Grid.SetRow(block_View, 0);
            Grid.SetColumn(block_View, block_Horizontal);
            return block_View;
        }

        private Rectangle Get_Rectangle(int horizontal, int vertical)
        {
            Rectangle rectangle;
            rectangle = (Rectangle)Sharp.Children
                .Cast<UIElement>()
                .FirstOrDefault(e => Grid.GetColumn(e) == horizontal && Grid.GetRow(e) == vertical);
            return rectangle;
        }

        private async void Move(Hero hero)
        {
            for(int i = 0; i < heroes.Count; i++)
            {
                if (heroes[i].horizontal != hero.horizontal || heroes[i].vertical != hero.vertical)
                    if (heroes[i].horizontal == hero.horizontal + hero.step.horizontal
                        && heroes[i].vertical == hero.vertical + hero.step.vertical)
                        return;
            }
            Rectangle moving_Block = Get_Rectangle(hero.horizontal + hero.step.horizontal, hero.vertical + hero.step.vertical);

            if (moving_Block == null || Block_Is_Free(moving_Block, hero))
            {
                if(moving_Block != null && Block_Is_Free(moving_Block, hero))
                    if (hero.step.vertical == 0)
                    {
                        Grid.SetColumn(moving_Block, hero.horizontal + (2 * hero.step.horizontal));
                        if(Down_Is_Clear(moving_Block))
                        {
                            Block_Falling(moving_Block);
                        }
                    }

                Rectangle hero_View = Get_Rectangle(hero.horizontal, hero.vertical);

                /*for(int i = 0; i < heroes.Count; i++)
                {
                    if(heroes[i] != hero)
                        if(heroes[i].horizontal == hero.horizontal && heroes[i].vertical == hero.vertical - 1)
                        {
                            Rectangle under_Hero = Get_Rectangle(heroes[i].horizontal, heroes[i].vertical);
                            Hero_Falling(under_Hero, heroes[i]);
                        }
                }*/
                hero.horizontal += hero.step.horizontal;
                if(!Down_Is_Clear(hero_View))
                    hero.vertical += hero.step.vertical;

                Grid.SetRow(hero_View, hero.vertical);
                Grid.SetColumn(hero_View, hero.horizontal);
                
                for(int i = 0; i < heroes.Count; i++)
                {
                    if (heroes[i] != hero)
                    {
                        Rectangle above_Hero = Get_Rectangle(heroes[i].horizontal, heroes[i].vertical);
                        if (Down_Is_Clear(above_Hero))
                        {
                            await Task.Delay(game_Speed);
                            heroes[i].lose_Control = true;
                            Hero_Falling(above_Hero, heroes[i]);
                        }
                    }
                }
                if(hero.step.vertical != 0)
                {
                    await Task.Delay(game_Speed + (game_Speed / 3));
                    if(hero.step.vertical != 0)
                    {
                        hero.lose_Control = true;
                        Hero_Falling(hero_View, hero);
                    }
                }
                if(Down_Is_Clear(hero_View) && hero.step.vertical == 0)
                {
                    hero.lose_Control = true;
                    await Task.Delay(game_Speed);
                    Hero_Falling(hero_View, hero);
                }
            }
        }

        private bool Block_Is_Free(UIElement moving_Block, Hero hero)
        {
            if (Down_Is_Clear(moving_Block))
                return false;
            int horizontal = Grid.GetColumn(moving_Block);
            int vertical = Grid.GetRow(moving_Block);
            if (horizontal == 0 || horizontal == lenght - 1)
                return false;
            Rectangle disturbing_Block = Get_Rectangle(horizontal + hero.step.horizontal, hero.vertical);
            if (disturbing_Block == null)
            {
                if (vertical >= 0)
                {
                    Rectangle block_Over_Block = Get_Rectangle(horizontal, vertical - 1);
                    if (block_Over_Block == null)
                        return true;
                }
                else
                    return true;
            }
            return false;
        }

        private async void Hero_Falling(UIElement hero_View, Hero hero)
        {
            while (Down_Is_Clear(hero_View))
            {
                if (game_Active)
                {
                    hero.vertical++;
                    Grid.SetRow(hero_View, hero.vertical);
                }
                if (Down_Is_Clear(hero_View))
                    await Task.Delay(game_Speed);
            }
            hero.lose_Control = false;
        }

        private async void Block_Falling(UIElement element)
        {
            await Task.Delay(180);
            int vertical = Grid.GetRow(element);
            int horizontal = Grid.GetColumn(element);
            while (Down_Is_Clear(element))
            {
                if (game_Active)
                {
                    vertical++;
                    Grid.SetRow(element, vertical);
                }
                await Task.Delay(game_Speed);
                if (!Down_Is_Clear(element))
                    await Task.Delay(game_Speed);
            }
            if(vertical != height - 1)
            {
                //if (hero.vertical == vertical + 1 && hero.horizontal == horizontal)
                   // Game_Over();
                for(int i = 0; i < heroes.Count; i++)
                {
                    if(heroes[i].vertical == vertical + 1 && heroes[i].horizontal == horizontal)
                    {
                        Game_Over();
                        return;
                    }
                }
            }
            if(vertical == height - 1)
            {
                if (Check_Line())
                {
                    Destroy_Line();
                    lines_Num++;
                    if (lines_Num % 4 == 0)
                        Level_Up();
                }
            }
            
        }

        private bool Down_Is_Clear(UIElement element)
        {
            int horizontal = Grid.GetColumn(element);
            int vertical = Grid.GetRow(element);
            if (vertical == height - 1)
                return false;

            Rectangle block = Get_Rectangle(horizontal, vertical + 1);
                
            if (block != null)
                return false;
            return true;
        }

        private bool Check_Line()
        {
           /* if (hero.vertical == height - 1)
                return false;*/
            for(int i = 0; i < heroes.Count; i++)
            {
                if (heroes[i].vertical == height - 1)
                    return false;
            }
            Rectangle block;

            for (int i = 0; i < lenght; i++)
            {
                block = Get_Rectangle(i, height - 1);
                if (block == null)
                    return false;
            }      
            return true;
        }

        private async void Destroy_Line()//??
        {
            int horizontal;
            int vertical;
            Rectangle block;
            List<Rectangle> falling_Blocks = new List<Rectangle>();
            for (int i = 0; i < lenght; i++)
            {
                block = Get_Rectangle(i, height - 1);
                Sharp.Children.Remove(block);
            }

            await Task.Delay(game_Speed);
            //Rectangle hero_View = Get_Rectangle(hero.horizontal, hero.vertical);
            foreach(Rectangle rectangle in Sharp.Children)
            {
                if (!Down_Is_Clear(rectangle) || Grid.GetRow(rectangle) == height - 2)
                    falling_Blocks.Add(rectangle);
            }
            for (int i = height - 2; i >= 0; i--)
            {
                foreach (Rectangle rectangle in falling_Blocks)
                {
                    horizontal = Grid.GetColumn(rectangle);
                    vertical = Grid.GetRow(rectangle);
                    if (vertical == i)
                    {
                        Grid.SetRow(rectangle, vertical + 1);
                        //if (hero.horizontal == horizontal && hero.vertical == vertical)
                          //  hero.vertical++;
                        for(int j = 0; j < heroes.Count; j++)
                        {
                            if (heroes[j].horizontal == horizontal && heroes[j].vertical == vertical)
                                heroes[j].vertical++;
                        }
                    }
                }
            }
        }

        private void Game_Over()
        {
            Sharp.Children.Clear();
            game_Active = false;
            Game_Over_Text.Visibility = Visibility.Visible;
            Start_Button.Content = "Start Game";
            Restart_Button.Visibility = Visibility.Hidden;
            timer.Stop();
            game_Speed = 180;
            time_For_Block = 8;
            timer.Interval = new TimeSpan(0,0,0,0,game_Speed);
            heroes.Clear();
            game_Level = 1;
        }

        private void Level_Up()
        {
            game_Level++;
            Level_Text.Text = "Level: " + game_Level.ToString();
            if(game_Level % 3 == 0 && time_For_Block > 1)
            {
                time_For_Block--;
            }
            else
            {
                timer.Stop();
                if (game_Speed > 10)
                    game_Speed -= 10;
                else
                    game_Speed = 1;
                timer.Interval = new TimeSpan(0, 0, 0, 0, game_Speed);
                timer.Start();
            }
        }

        private void Level_Down()
        {
            if (game_Level != 1)
            {
                game_Level--;
                Level_Text.Text = "Level: " + game_Level.ToString();
                if ((game_Level - 1) % 3 == 0)
                {
                    time_For_Block++;
                }
                else
                {
                    timer.Stop();
                    game_Speed += 10;
                    timer.Interval = new TimeSpan(0, 0, 0, 0, game_Speed);
                    timer.Start();
                }
            }
        }
    }
}
