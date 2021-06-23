using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Drawing;

namespace DisplayDrawer
{
    public class Drawer
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool SetConsoleMode(IntPtr hConsoleHandle, int mode);
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool GetConsoleMode(IntPtr handle, out int mode);
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr GetStdHandle(int handle);

        Dictionary<string, int[]> pixelInfo = new Dictionary<string, int[]>();
        Dictionary<string, string[]> textInfo = new Dictionary<string, string[]>();
        uint length, height;
        Dictionary<string, Dictionary<string, int[]>> layoutPixelInfo = new Dictionary<string, Dictionary<string, int[]>>();
        Dictionary<string, Dictionary<string, string[]>> layoutTextInfo = new Dictionary<string, Dictionary<string, string[]>>();

        public Drawer(uint length, uint height)
        {
            Console.ResetColor();

            this.length = length;
            this.height = height;

            var handle = GetStdHandle(-11);
            int mode;
            GetConsoleMode(handle, out mode);
            SetConsoleMode(handle, mode | 0x4);
        }
        public void DrawPixel(int x, int y, int red, int green, int blue)
        {
            if (x <= length && y <= height)
            {
                if (red < 256 && green < 256 && blue < 256)
                {
                    string cords = $"{x},{y}";
                    int[] color = { red, green, blue };

                    if (pixelInfo.ContainsKey(cords)) pixelInfo.Remove(cords);
                    pixelInfo.Add(cords, color);
                }
            }
        }
        private void DrawLine(int Ax, int Ay, int Bx, int By, int red, int green, int blue)
        {
            if (Ax <= length && Ay <= height)
            {
                bool lineAtX = false;
                bool lineAtY = false;

                if (Ax != Bx && Ay == By) lineAtX = true;
                if (Ax == Bx && Ay != By) lineAtY = true;

                if (lineAtX && !lineAtY)
                {
                    bool increase = false;
                    bool decrease = false;

                    if (Ax > Bx) decrease = true;
                    else if (Ax < Bx) increase = true;

                    if (increase)
                    {
                        while (Ax != Bx)
                        {
                            Ax++;
                            DrawPixel(Ax, Ay, red, green, blue);
                        }
                    }
                    else if (decrease)
                    {
                        while (Ax != Bx)
                        {
                            Ax--;
                            DrawPixel(Ax, Ay, red, green, blue);
                        }
                    }
                }
                else if (!lineAtX && lineAtY)
                {
                    bool increase = false;
                    bool decrease = false;

                    if (Ay > By) decrease = true;
                    else if (Ay < By) increase = true;

                    if (increase)
                    {
                        while (Ay != By)
                        {
                            Ay++;
                            DrawPixel(Ax, Ay, red, green, blue);
                        }
                    }
                    else if (decrease)
                    {
                        while (Ay != By)
                        {
                            Ay--;
                            DrawPixel(Ax, Ay, red, green, blue);
                        }
                    }
                }
            }
        }
        public void DrawRectangle(int Ax, int Ay, int Dx, int Dy, int red, int green, int blue, bool fill)
        {
            int Bx = Dx;
            int By = Ay;
            int Cx = Ax;
            int Cy = Dy; 

            if (!fill)
            {
                DrawLine(Ax, Ay, Bx, By, red, green, blue);
                DrawLine(Bx, By, Dx, Dy, red, green, blue);
                DrawLine(Dx, Dy, Cx, Cy, red, green, blue);
                DrawLine(Cx, Cy, Ax, Ay, red, green, blue);
            }
            else
            {
                DrawLine(Ax, Ay, Bx, By, red, green, blue);
                DrawLine(Bx, By, Dx, Dy, red, green, blue);
                DrawLine(Dx, Dy, Cx, Cy, red, green, blue);
                DrawLine(Cx, Cy, Ax, Ay, red, green, blue);

                List<int> list = new List<int>();

                bool increase = false;
                bool decrease = false;

                if (Ay > Cy) decrease = true;
                if (Ay < Cy) increase = true;

                if (increase)
                {
                    while (Ay < Cy)
                    {
                        Ay++;
                        DrawLine(Ax, Ay, Dx, Ay, red, green, blue);
                    }
                }
                else if (decrease)
                {
                    while (Ay > Cy)
                    {
                        Ay--;
                        DrawLine(Ax, Ay, Dx, Ay, red, green, blue);
                    }

                }
            }
        }
        public void DrawText(string text, int x, int y, int red=255, int green=255, int blue=255)
        {
            List<string> list = new List<string>();

            int i = 0;

            while (true)
            {
                if (text.Length > 1)
                {
                    string[] textToAdd = { text.Substring(0, 2), $"{red},{green},{blue}" };
                    if (textInfo.ContainsKey($"{x + i},{y}")) textInfo.Remove($"{x + i},{y}");
                    textInfo.Add($"{x + i},{y}", textToAdd);
                    text = text.Substring(2);
                }
                else if (text.Length == 1)
                {
                    string[] textToAdd = { text.Substring(0, 1) + " ", $"{red},{green},{blue}" };
                    if (textInfo.ContainsKey($"{x + i},{y}")) textInfo.Remove($"{x + i},{y}");
                    textInfo.Add($"{x + i},{y}", textToAdd);
                    break;
                }
                else
                {
                    break;
                }
                i++;
            }
        }
        public void MoveDrawing(int x, int y)
        {
            var dictToAcess = new Dictionary<string, int[]>(pixelInfo);
            pixelInfo.Clear();

            foreach (var key in dictToAcess.Keys)
            {
                var color = dictToAcess[key];

                int newX = Convert.ToInt32(key.Split(",")[0]);
                int newY = Convert.ToInt32(key.Split(",")[1]);

                string cords = $"{newX + x},{newY + y}";

                pixelInfo.Add(cords, color);
            }

            var dictToAcess2 = new Dictionary<string, string[]>(textInfo);
            textInfo.Clear();

            foreach (var key in dictToAcess2.Keys)
            {
                var textToAdd = dictToAcess2[key];

                int newX = Convert.ToInt32(key.Split(",")[0]);
                int newY = Convert.ToInt32(key.Split(",")[1]);

                string cords = $"{newX + x},{newY + y}";

                textInfo.Add(cords, textToAdd);
            }
        }
        public void Render()
        {
            int i = 1;
            int c = 1;

            while (true)
            {
                if (i > length) { i = 1; c++; Console.Write("\n"); }
                if (c > height) { break; }

                string key = $"{i},{c}";

                if (pixelInfo.ContainsKey(key))
                {
                    if (textInfo.ContainsKey(key))
                    {
                        string text = textInfo[key][0];
                        int rText = Convert.ToInt32(textInfo[key][1].Split(",")[0]);
                        int gText = Convert.ToInt32(textInfo[key][1].Split(",")[1]);
                        int bText = Convert.ToInt32(textInfo[key][1].Split(",")[2]);

                        int[] color = pixelInfo[key];
                        int r = color[0];
                        int g = color[1];
                        int b = color[2];
                        Console.Write($"\x1b[48;2;{r};{g};{b}m" + $"\x1b[38;2;{rText};{gText};{bText}m" + text);
                        Console.ResetColor();
                    }
                    else
                    {
                        int[] color = pixelInfo[key];
                        int r = color[0];
                        int g = color[1];
                        int b = color[2];
                        Console.Write($"\x1b[48;2;{r};{g};{b}m  ");
                        Console.ResetColor();
                    }
                }
                else
                {
                    if (textInfo.ContainsKey(key))
                    {
                        string text = textInfo[key][0];
                        int r = Convert.ToInt32(textInfo[key][1].Split(",")[0]);
                        int g = Convert.ToInt32(textInfo[key][1].Split(",")[1]);
                        int b = Convert.ToInt32(textInfo[key][1].Split(",")[2]);

                        Console.Write($"\x1b[38;2;{r};{g};{b}m" + text);
                    }
                    else
                    {
                        Console.Write("  ");
                    }
                }
                i++;
            }
        }
        public void ClearBuffer()
        {
            pixelInfo.Clear();
            textInfo.Clear();
        }
        public void SetCanvasSize(uint length, uint height)
        {
            this.length = length;
            this.height = height;
        }
        public void SaveLayout(string name)
        {
            if (layoutPixelInfo.ContainsKey(name)) layoutPixelInfo.Remove(name);
            if (layoutTextInfo.ContainsKey(name)) layoutTextInfo.Remove(name);

            layoutPixelInfo.Add(name,new Dictionary<string, int[]>(pixelInfo));
            layoutTextInfo.Add(name, new Dictionary<string, string[]>(textInfo));
        }
        public void LoadLayout(string name)
        {
            if (layoutPixelInfo.ContainsKey(name) && layoutTextInfo.ContainsKey(name))
            {
                pixelInfo = layoutPixelInfo[name];
                textInfo = layoutTextInfo[name];
            }
        }
        public void DeleteLayout(string name)
        {
            if (layoutPixelInfo.ContainsKey(name)) layoutPixelInfo.Remove(name);
            if (layoutTextInfo.ContainsKey(name)) layoutTextInfo.Remove(name);
        }
        public void SaveScheme(string path)
        {
            Bitmap bmp = new Bitmap(Convert.ToInt32(length), Convert.ToInt32(height));

            for (int y = 0; y<bmp.Height; y++)
            {
                for (int x = 0; x < bmp.Width; x++)
                {
                    if (pixelInfo.ContainsKey($"{x + 1},{y + 1}"))
                    {
                        var colors = pixelInfo[$"{x + 1},{y + 1}"];
                        Color color = Color.FromArgb(colors[0], colors[1], colors[2]);
                        bmp.SetPixel(x, y, color);
                    }
                }
            }
            bmp.Save(path);
            bmp.Dispose();
        }
        public void LoadScheme(string path)
        {
            Bitmap bmp = new Bitmap(path);

            pixelInfo.Clear();
            textInfo.Clear();

            for (int y = 0; y < bmp.Height; y++)
            {
                for (int x = 0; x < bmp.Width; x++)
                {
                    Color color = bmp.GetPixel(x, y);
                    DrawPixel(x+1,y+1,color.R,color.G,color.B);
                }
            }
            bmp.Dispose();
        }
    }
}