using IniParser;
using IniParser.Model;
using System.Collections.Specialized;
using System.Configuration;
using System.Runtime.InteropServices;

class Program
{
    [DllImport("kernel32.dll", ExactSpelling = true)]
    private static extern IntPtr GetConsoleWindow();

    [DllImport("user32.dll")]
    static extern bool SetWindowText(IntPtr hWnd, string lpString);

    [DllImport("user32.dll")]
    static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

    [DllImport("kernel32.dll", SetLastError = true)]
    static extern bool SetConsoleTitle(string lpConsoleTitle);

    [DllImport("kernel32.dll", SetLastError = true)]
    static extern bool SetConsoleScreenBufferSize(IntPtr hConsoleOutput, COORD size);

    [DllImport("kernel32.dll", SetLastError = true)]
    static extern bool SetConsoleWindowInfo(IntPtr hConsoleOutput, bool bAbsolute, ref SMALL_RECT lpConsoleWindow);

    [DllImport("kernel32.dll")]
    static extern bool SetConsoleTextAttribute(IntPtr hConsoleOutput, ushort wAttributes);

    [DllImport("kernel32.dll", SetLastError = true)]
    static extern bool GetConsoleScreenBufferInfo(IntPtr hConsoleOutput, out CONSOLE_SCREEN_BUFFER_INFO lpConsoleScreenBufferInfo);

    static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
    static readonly IntPtr HWND_NOT_TOPMOST = new IntPtr(-2);
    static readonly IntPtr HWND_TOP = new IntPtr(0);
    static readonly IntPtr HWND_BOTTOM = new IntPtr(1);

    static readonly uint SWP_NOSIZE = 0x0001;
    static readonly uint SWP_NOMOVE = 0x0002;
    static readonly uint SWP_NOZORDER = 0x0004;
    static readonly uint SWP_NOREDRAW = 0x0008;
    static readonly uint SWP_NOACTIVATE = 0x0010;
    static readonly uint SWP_FRAMECHANGED = 0x0020;
    static readonly uint SWP_SHOWWINDOW = 0x0040;
    static readonly uint SWP_HIDEWINDOW = 0x0080;
    static readonly uint SWP_NOCOPYBITS = 0x0100;
    static readonly uint SWP_NOOWNERZORDER = 0x0200;
    static readonly uint SWP_NOSENDCHANGING = 0x0400;

    static readonly ushort FOREGROUND_BLACK = 0x0000;
    static readonly ushort FOREGROUND_WHITE = 0x0007;

    static void Main()
    {
        if (!File.Exists("config.ini"))
        {
            // Si el archivo no existe, se crea con contenido por defecto
            File.WriteAllText("config.ini", "[Configuracion]" +
                "\nTitulo=Mi Programa" +
                "\nVersion=1.0" +
                "\nAncho=80" +
                "\nAlto=12" +
                "\nColor-Fuente=White" +
                "\nColor-Fondo=Black" +
                "\nMarco=False" +
                "\nDebug-Mode=True" +
                "\nReset=False" 
                );
        }

        // Load configuration from INI file
        var parser = new FileIniDataParser();
        IniData data = parser.ReadFile("config.ini");

        // Get console window handle
        IntPtr consoleHandle = GetConsoleWindow();

        // Set console window title
        SetConsoleTitle(data["Configuracion"]["Titulo"]);

        // Set console window size and buffer size
        int consoleWidth = int.Parse(data["Configuracion"]["Ancho"]);
        int consoleHeight = int.Parse(data["Configuracion"]["Alto"]);
        SetConsoleScreenBufferSize(consoleHandle, new COORD((short)consoleWidth, (short)consoleHeight));
        SMALL_RECT rect = new SMALL_RECT(0, 0, (short)(consoleWidth - 1), (short)(consoleHeight - 1));
        SetConsoleWindowInfo(consoleHandle, true, ref rect);

        // Set console colors
        ConsoleColor foregroundColor;
        ConsoleColor backgroundColor;
        if (Enum.TryParse(data["Configuracion"]["Color-Fuente"], out foregroundColor) &&
            Enum.TryParse(data["Configuracion"]["Color-Fondo"], out backgroundColor))
        {
            Console.ForegroundColor = foregroundColor;
            Console.BackgroundColor = backgroundColor;
        }

        // Add console border
        if (bool.Parse(data["Configuracion"]["Marco"]))
        {
            Console.WriteLine("╔" + new string('═', consoleWidth - 2) + "╗");
            for (int i = 0; i < consoleHeight - 2; i++)
            {
                Console.WriteLine("║" + new string(' ', consoleWidth - 2) + "║");
            }
            Console.WriteLine("╚" + new string('═', consoleWidth - 2) + "╝");
        }

        // Set debug mode
        if (bool.Parse(data["Configuracion"]["Debug-Mode"]))
        {
            Console.Title += " [DEBUG MODE]";
            Console.WindowHeight = consoleHeight * 2;
        }

        // Reset program state
        if (bool.Parse(data["Configuracion"]["Reset"]))
        {
            File.Delete("registro.log");
            File.Delete("config.ini");
            Console.WriteLine("Programa reseteado...");
        }

        // Write program info
        Console.WriteLine($"{data["Configuracion"]["Titulo"]} v{data["Configuracion"]["Version"]}");
        Console.WriteLine("Press any key to continue...");

        // Wait for user input
        Console.ReadKey(true);
    }
}

// Helper structs for console functions
[StructLayout(LayoutKind.Sequential)]
public struct COORD
{
    public short X;
    public short Y;
    public COORD(short x, short y)
    {
        X = x;
        Y = y;
    }
}

[StructLayout(LayoutKind.Sequential)]
public struct SMALL_RECT
{
    public short Left;
    public short Top;
    public short Right;
    public short Bottom;
    public SMALL_RECT(short left, short top, short right, short bottom)
    {
        Left = left;
        Top = top;
        Right = right;
        Bottom = bottom;
    }
}

[StructLayout(LayoutKind.Sequential)]
public struct CONSOLE_SCREEN_BUFFER_INFO
{
    public COORD dwSize;
    public COORD dwCursorPosition;
    public ushort wAttributes;
    public SMALL_RECT srWindow;
    public COORD dwMaximumWindowSize;
}