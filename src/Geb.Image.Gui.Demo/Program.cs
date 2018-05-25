using System;
using System.Threading;
using Avalonia;
using Avalonia.Media;
using Avalonia.Controls;

namespace Geb.Image.Gui.Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            //Thread.CurrentThread.TrySetApartmentState(ApartmentState.STA);

            {
                Thread thread = new Thread(new ThreadStart(ShowWindow));
                thread.Start();
            }
            
            
            for(int i = 0; i < 10000; i ++)
            {
                Thread.Sleep(1000);
                Console.WriteLine(i);
                if(i == 10)
                {
                    {
                        Thread thread = new Thread(new ThreadStart(ShowWindow));
                        thread.Start();
                    }
                }
            }

            //Thread.CurrentThread.TrySetApartmentState(ApartmentState.STA);
            //var app = BuildAvaloniaApp();
            //app.Start<MainWindow>();
        }

        static void ShowWindow()
        {
            var app = BuildAvaloniaApp();
            MainWindow wd = new MainWindow();
            app.Start(wd);
            app.Instance.Run()
            app.Instance.Exit();

        }

        /// <summary>
        /// This method is needed for IDE previewer infrastructure
        /// </summary>
        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>().UseWin32().UseDirect2D1();
    }

    public class App : Application
    {
        public override void Initialize()
        {
            //AvaloniaXamlLoader.Load(this);
        }
    }

    public class MainWindow : Window
    {
        public int Level;

        public MainWindow()
        {
            this.InitializeComponent();
           // this.AttachDevTools();
        }

        private void InitializeComponent()
        {
            this.Width = 600;
            this.Height = 400;
            //this.Background = new SolidColorBrush(Colors.Blue);
            this.Activated += MainWindow_Activated;
            this.Initialized += MainWindow_Initialized;
        }

        private void MainWindow_Initialized(object sender, EventArgs e)
        {
            if (Level < 3)
            {
                MainWindow wd = new MainWindow();
                wd.Level = Level + 1;
                wd.ShowDialog();
            }
        }

        private void MainWindow_Activated(object sender, EventArgs e)
        {
           
        }

        public override void Render(DrawingContext context)
        {
            base.Render(context);
            context.FillRectangle(new SolidColorBrush(Colors.Blue), new Avalonia.Rect(0, 0, (int)ClientSize.Width, (int)ClientSize.Height));

        }
    }
}
