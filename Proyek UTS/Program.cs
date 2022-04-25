using OpenTK.Windowing.Desktop;
using System;

namespace Proyek_UTS
{
    class Program
    {
        static void Main(string[] args)
        {
            // Console.WriteLine("Hello World!");
            var nativeWindowSettings = new NativeWindowSettings()
            {
                Size = new OpenTK.Mathematics.Vector2i(800, 800),
                Title = "Pertemuan1"

            };
            using (var window = new Window(GameWindowSettings.Default, nativeWindowSettings))
            {

                window.Run();
            }
        }
    }
}