using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using Avalonia;
using ReactiveUI;
using ABitmap = Avalonia.Media.Imaging.Bitmap;

namespace Geb.Image.Avalonia.Demo
{
    static class Program
    {
        static void Main(string[] args)
        {
            Thread.CurrentThread.TrySetApartmentState(ApartmentState.STA);

            var app = BuildAvaloniaApp();
            //app.Start<MainWindow>();
            app.SetupWithoutStarting();
            //Console.ReadLine();
            MainWindow wd = new MainWindow();
            Console.ReadLine();

            //Thread.CurrentThread.TrySetApartmentState(ApartmentState.STA);
            //var app = BuildAvaloniaApp();
            //app.Start<MainWindow>();
        }

        /// <summary>
        /// This method is needed for IDE previewer infrastructure
        /// </summary>
        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>().UsePlatformDetect();

        static void ConsoleSilencer()
        {
            Console.CursorVisible = false;
            while (true)
                Console.ReadKey(true);
        }
    }
}
