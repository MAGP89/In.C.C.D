using IniParser;
using System.Drawing;

namespace ExampleApp
{
    public partial class MainForm : Form
    {
        private string iniFilePath = "config.ini";
        private Color defaultBackgroundColor = Color.White;
        private Color backgroundColor;

        public MainForm()
        {
            InitializeComponent();

            // Lee el archivo .ini
            if (File.Exists(iniFilePath))
            {
                var parser = new FileIniDataParser();
                IniData data = parser.ReadFile(iniFilePath);
                string colorString = data["General"]["BackgroundColor"];
                if (Color.TryParse(colorString, out backgroundColor))
                {
                    this.BackColor = backgroundColor;
                }
                else
                {
                    this.BackColor = defaultBackgroundColor;
                }
            }
            else
            {
                // Crea el archivo .ini con el color predeterminado
                IniData data = new IniData();
                data.Sections.AddSection("General");
                data["General"].AddKey("BackgroundColor", defaultBackgroundColor.ToString());
                var parser = new FileIniDataParser();
                parser.WriteFile(iniFilePath, data);

                this.BackColor = defaultBackgroundColor;
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Guarda el color de fondo en el archivo .ini
            var parser = new FileIniDataParser();
            IniData data = parser.ReadFile(iniFilePath);
            data["General"]["BackgroundColor"] = this.BackColor.ToString();
            parser.WriteFile(iniFilePath, data);
        }
    }
}
