using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace RetroOS
{
    public partial class Form1 : Form
    {
        private Panel taskbar = null!;
        private Panel startMenu = null!;
        private Label clock = null!;

        // ============================================================
        // VIRTUELLES DATEISYSTEM
        // ============================================================

        private readonly Dictionary<string, Dictionary<string, string>> files =
            new Dictionary<string, Dictionary<string, string>>
            {
                ["C:\\Documents"] = new Dictionary<string, string>
                {
                    ["Schule.txt"] =
                        "Willkommen in RetroOS!\r\n\r\n" +
                        "Das ist ein gespeichertes Dokument.\r\n" +
                        "Du kannst es bearbeiten und wieder speichern.",

                    ["Notizen.txt"] =
                        "Meine Notizen\r\n\r\n" +
                        "- RetroOS bauen\r\n" +
                        "- Startmenü verbessern\r\n" +
                        "- Explorer bauen"
                },

                ["C:\\Music"] = new Dictionary<string, string>
                {
                    ["RetroSong.mp3"] = "",
                    ["CoolMusic.mp3"] = ""
                },

                ["C:\\Pictures"] = new Dictionary<string, string>
                {
                    ["Bild1.png"] = "",
                    ["Bild2.jpg"] = ""
                },

                ["C:\\Downloads"] = new Dictionary<string, string>
                {
                    ["Programm.zip"] = "",
                    ["Datei.txt"] = "Eine heruntergeladene Textdatei."
                },

                ["C:\\Videos"] = new Dictionary<string, string>
                {
                    ["Video1.mp4"] = "",
                    ["RetroVideo.mp4"] = ""
                }
            };

        // Papierkorb
        private readonly Dictionary<string, string> recycleBin =
            new Dictionary<string, string>();

        // ============================================================
        // START
        // ============================================================

        public Form1()
        {
            InitializeComponent();
            SetupRetroOS();
        }

        // ============================================================
        // RETROOS SETUP
        // ============================================================

        private void SetupRetroOS()
        {
            Text = "RetroOS";
            BackColor = Color.SteelBlue;

            ClientSize = new Size(1000, 650);

            StartPosition = FormStartPosition.CenterScreen;

            // --------------------------------------------------------
            // DESKTOP ICONS
            // --------------------------------------------------------

            CreateDesktopIcon(
                "🗑️",
                "Papierkorb",
                25,
                25,
                OpenRecycleBin
            );

            CreateDesktopIcon(
                "💻",
                "Arbeitsplatz",
                25,
                135,
                OpenExplorer
            );

            CreateDesktopIcon(
                "📁",
                "Eigene Dateien",
                25,
                245,
                () => OpenFolder("C:\\Documents")
            );

            CreateDesktopIcon(
                "📝",
                "Notepad",
                25,
                355,
                () => OpenNotepad(null, null)
            );

            // --------------------------------------------------------
            // DESKTOP RECHTSKLICK
            // --------------------------------------------------------

            ContextMenuStrip desktopMenu = new ContextMenuStrip();

            desktopMenu.Items.Add(
                "🔄 Aktualisieren",
                null,
                (s, e) => RefreshDesktop()
            );

            desktopMenu.Items.Add(
                "📁 Neuer Ordner",
                null,
                (s, e) =>
                {
                    MessageBox.Show(
                        "Diese Funktion kommt noch 😎",
                        "RetroOS"
                    );
                }
            );

            desktopMenu.Items.Add(
                "⚙ Einstellungen",
                null,
                (s, e) =>
                {
                    MessageBox.Show(
                        "RetroOS Einstellungen kommen später!",
                        "Einstellungen"
                    );
                }
            );

            ContextMenuStrip = desktopMenu;

            // --------------------------------------------------------
            // TASKLEISTE
            // --------------------------------------------------------

            taskbar = new Panel();

            taskbar.BackColor =
                Color.FromArgb(35, 35, 35);

            taskbar.Dock = DockStyle.Bottom;

            taskbar.Height = 48;

            Controls.Add(taskbar);

            // --------------------------------------------------------
            // START BUTTON
            // --------------------------------------------------------

            Button startButton = new Button();

            startButton.Text = "🪟 Start";

            startButton.Font =
                new Font(
                    "Segoe UI",
                    10,
                    FontStyle.Bold
                );

            startButton.Size =
                new Size(100, 40);

            startButton.Location =
                new Point(5, 4);

            startButton.FlatStyle =
                FlatStyle.Flat;

            startButton.ForeColor =
                Color.White;

            startButton.BackColor =
                Color.FromArgb(50, 100, 180);

            startButton.Click +=
                ToggleStartMenu;

            taskbar.Controls.Add(startButton);

            // --------------------------------------------------------
            // UHR
            // --------------------------------------------------------

            clock = new Label();

            clock.ForeColor = Color.White;

            clock.Font =
                new Font(
                    "Segoe UI",
                    10
                );

            clock.AutoSize = true;

            taskbar.Controls.Add(clock);

            System.Windows.Forms.Timer clockTimer =
                new System.Windows.Forms.Timer();

            clockTimer.Interval = 1000;

            clockTimer.Tick +=
                UpdateClock;

            clockTimer.Start();

            UpdateClock(
                null,
                EventArgs.Empty
            );

            // --------------------------------------------------------
            // STARTMENÜ
            // --------------------------------------------------------

            CreateStartMenu();
        }

        // ============================================================
        // DESKTOP ICON
        // ============================================================

        private void CreateDesktopIcon(
            string icon,
            string name,
            int x,
            int y,
            Action action)
        {
            Panel panel = new Panel();

            panel.Size =
                new Size(130, 95);

            panel.Location =
                new Point(x, y);

            panel.BackColor =
                Color.Transparent;

            panel.Cursor =
                Cursors.Hand;

            // ICON
            Label iconLabel =
                new Label();

            iconLabel.Text =
                icon;

            iconLabel.Font =
                new Font(
                    "Segoe UI Emoji",
                    32
                );

            iconLabel.Size =
                new Size(130, 55);

            iconLabel.Location =
                new Point(0, 0);

            iconLabel.TextAlign =
                ContentAlignment.MiddleCenter;

            iconLabel.BackColor =
                Color.Transparent;

            // TEXT
            Label nameLabel =
                new Label();

            nameLabel.Text =
                name;

            nameLabel.ForeColor =
                Color.White;

            nameLabel.Font =
                new Font(
                    "Segoe UI",
                    9
                );

            nameLabel.Size =
                new Size(130, 35);

            nameLabel.Location =
                new Point(0, 58);

            nameLabel.TextAlign =
                ContentAlignment.TopCenter;

            nameLabel.BackColor =
                Color.Transparent;

            panel.Controls.Add(iconLabel);
            panel.Controls.Add(nameLabel);

            // Klick
            panel.Click +=
                (s, e) => action();

            iconLabel.Click +=
                (s, e) => action();

            nameLabel.Click +=
                (s, e) => action();

            // Doppelklick
            panel.DoubleClick +=
                (s, e) => action();

            Controls.Add(panel);

            panel.BringToFront();
        }

        // ============================================================
        // STARTMENÜ
        // ============================================================

        private void CreateStartMenu()
        {
            startMenu = new Panel();

            startMenu.Size =
                new Size(300, 430);

            startMenu.BackColor =
                Color.FromArgb(
                    35,
                    35,
                    35
                );

            startMenu.Location =
                new Point(
                    5,
                    ClientSize.Height - 478
                );

            startMenu.Visible =
                false;

            Controls.Add(startMenu);

            startMenu.BringToFront();

            Label title =
                new Label();

            title.Text =
                "RetroOS";

            title.ForeColor =
                Color.White;

            title.Font =
                new Font(
                    "Segoe UI",
                    18,
                    FontStyle.Bold
                );

            title.Location =
                new Point(20, 15);

            title.AutoSize = true;

            startMenu.Controls.Add(title);

            AddStartButton(
                "💻 Arbeitsplatz",
                60,
                OpenExplorer
            );

            AddStartButton(
                "📁 Eigene Dateien",
                110,
                () => OpenFolder(
                    "C:\\Documents"
                )
            );

            AddStartButton(
                "📝 Notepad",
                160,
                () => OpenNotepad(
                    null,
                    null
                )
            );

            AddStartButton(
                "🗑️ Papierkorb",
                210,
                OpenRecycleBin
            );

            AddStartButton(
                "💥 Bluescreen",
                260,
                ShowBSOD
            );

            AddStartButton(
                "❌ Herunterfahren",
                330,
                Close
            );
        }

        private void AddStartButton(
            string text,
            int y,
            Action action)
        {
            Button button =
                new Button();

            button.Text =
                text;

            button.Size =
                new Size(
                    260,
                    40
                );

            button.Location =
                new Point(
                    20,
                    y
                );

            button.TextAlign =
                ContentAlignment.MiddleLeft;

            button.FlatStyle =
                FlatStyle.Flat;

            button.ForeColor =
                Color.White;

            button.BackColor =
                Color.FromArgb(
                    55,
                    55,
                    55
                );

            button.Click +=
                (s, e) =>
                {
                    startMenu.Visible =
                        false;

                    action();
                };

            startMenu.Controls.Add(button);
        }

        private void ToggleStartMenu(
            object? sender,
            EventArgs e)
        {
            startMenu.Visible =
                !startMenu.Visible;

            startMenu.BringToFront();
        }

        // ============================================================
        // UHR
        // ============================================================

        private void UpdateClock(
            object? sender,
            EventArgs e)
        {
            clock.Text =
                DateTime.Now.ToString(
                    "HH:mm:ss"
                );

            clock.Location =
                new Point(
                    taskbar.Width - 90,
                    14
                );
        }

        // ============================================================
        // EXPLORER
        // ============================================================

        private void OpenExplorer()
        {
            Form explorer =
                new Form();

            explorer.Text =
                "💻 Arbeitsplatz";

            explorer.Size =
                new Size(
                    700,
                    500
                );

            explorer.StartPosition =
                FormStartPosition.CenterScreen;

            ListBox list =
                new ListBox();

            list.Dock =
                DockStyle.Fill;

            list.Font =
                new Font(
                    "Segoe UI",
                    11
                );

            list.Items.Add(
                "💾 C:\\"
            );

            foreach (
                string folder
                in files.Keys)
            {
                string name =
                    folder.Replace(
                        "C:\\",
                        ""
                    );

                list.Items.Add(
                    "📁 " + name
                );
            }

            list.DoubleClick +=
                (s, e) =>
                {
                    if (
                        list.SelectedItem
                        == null)
                        return;

                    string selected =
                        list.SelectedItem
                        .ToString()!;

                    if (
                        selected
                        == "💾 C:\\")
                        return;

                    string folderName =
                        selected.Replace(
                            "📁 ",
                            ""
                        );

                    OpenFolder(
                        "C:\\" +
                        folderName
                    );
                };

            explorer.Controls.Add(list);

            explorer.Show();
        }

        // ============================================================
        // ORDNER
        // ============================================================

        private void OpenFolder(
            string path)
        {
            if (!files.ContainsKey(path))
                return;

            Form folderWindow =
                new Form();

            folderWindow.Text =
                "📁 " + path;

            folderWindow.Size =
                new Size(
                    650,
                    450
                );

            folderWindow.StartPosition =
                FormStartPosition.CenterScreen;

            ListBox fileList =
                new ListBox();

            fileList.Dock =
                DockStyle.Fill;

            fileList.Font =
                new Font(
                    "Segoe UI",
                    11
                );

            RefreshFileList(
                fileList,
                path
            );

            // --------------------------------------------------------
            // RECHTSKLICK
            // --------------------------------------------------------

            ContextMenuStrip menu =
                new ContextMenuStrip();

            menu.Items.Add(
                "📝 Öffnen",
                null,
                (s, e) =>
                {
                    OpenSelectedFile(
                        fileList,
                        path
                    );
                }
            );

            menu.Items.Add(
                "🗑️ Löschen",
                null,
                (s, e) =>
                {
                    DeleteSelectedFile(
                        fileList,
                        path
                    );
                }
            );

            menu.Items.Add(
                "🔄 Aktualisieren",
                null,
                (s, e) =>
                {
                    RefreshFileList(
                        fileList,
                        path
                    );
                }
            );

            fileList.ContextMenuStrip =
                menu;

            // --------------------------------------------------------
            // DOPPELKLICK
            // --------------------------------------------------------

            fileList.DoubleClick +=
                (s, e) =>
                {
                    OpenSelectedFile(
                        fileList,
                        path
                    );
                };

            folderWindow.Controls.Add(
                fileList
            );

            folderWindow.Show();
        }

        // ============================================================
        // DATEILISTE AKTUALISIEREN
        // ============================================================

        private void RefreshFileList(
            ListBox list,
            string path)
        {
            list.Items.Clear();

            foreach (
                string file
                in files[path].Keys)
            {
                string icon =
                    GetFileIcon(file);

                list.Items.Add(
                    icon + " " + file
                );
            }
        }

        // ============================================================
        // DATEI ÖFFNEN
        // ============================================================

        private void OpenSelectedFile(
            ListBox list,
            string path)
        {
            if (
                list.SelectedItem
                == null)
                return;

            string selected =
                list.SelectedItem
                .ToString()!;

            string fileName =
                RemoveFileIcon(
                    selected
                );

            if (
                !fileName.EndsWith(
                    ".txt",
                    StringComparison.OrdinalIgnoreCase
                ))
            {
                MessageBox.Show(
                    "RetroOS kann diese Datei momentan nicht als Text öffnen.",
                    "RetroOS"
                );

                return;
            }

            OpenNotepad(
                path,
                fileName
            );
        }

        // ============================================================
        // NOTEPAD
        // ============================================================

        private void OpenNotepad(
            string? path,
            string? fileName)
        {
            Form notepad =
                new Form();

            notepad.Text =
                fileName == null
                ? "📝 RetroOS Notepad"
                : "📝 " + fileName;

            notepad.Size =
                new Size(
                    750,
                    550
                );

            notepad.StartPosition =
                FormStartPosition.CenterScreen;

            MenuStrip menu =
                new MenuStrip();

            ToolStripMenuItem fileMenu =
                new ToolStripMenuItem(
                    "Datei"
                );

            ToolStripMenuItem save =
                new ToolStripMenuItem(
                    "💾 Speichern"
                );

            ToolStripMenuItem clear =
                new ToolStripMenuItem(
                    "🗑️ Alles löschen"
                );

            TextBox editor =
                new TextBox();

            editor.Multiline = true;

            editor.Dock =
                DockStyle.Fill;

            editor.ScrollBars =
                ScrollBars.Both;

            editor.Font =
                new Font(
                    "Consolas",
                    11
                );

            // --------------------------------------------------------
            // GESPEICHERTEN INHALT LADEN
            // --------------------------------------------------------

            if (
                path != null &&
                fileName != null &&
                files.ContainsKey(path) &&
                files[path].ContainsKey(fileName))
            {
                editor.Text =
                    files[path][fileName];
            }

            // --------------------------------------------------------
            // SPEICHERN
            // --------------------------------------------------------

            save.Click +=
                (s, e) =>
                {
                    if (
                        path != null &&
                        fileName != null)
                    {
                        files[path][fileName] =
                            editor.Text;

                        MessageBox.Show(
                            "Gespeichert! 💾",
                            "RetroOS"
                        );
                    }
                    else
                    {
                        string newName =
                            "Neue Notiz.txt";

                        int number = 1;

                        while (
                            files["C:\\Documents"]
                            .ContainsKey(newName))
                        {
                            newName =
                                "Neue Notiz " +
                                number +
                                ".txt";

                            number++;
                        }

                        files["C:\\Documents"]
                            [newName] =
                            editor.Text;

                        notepad.Text =
                            "📝 " + newName;

                        path =
                            "C:\\Documents";

                        fileName =
                            newName;

                        MessageBox.Show(
                            "Neue Notiz gespeichert!\r\n\r\n" +
                            "Speicherort: Documents",
                            "RetroOS"
                        );
                    }
                };

            // --------------------------------------------------------
            // ALLES LÖSCHEN
            // --------------------------------------------------------

            clear.Click +=
                (s, e) =>
                {
                    editor.Clear();
                };

            fileMenu.DropDownItems.Add(
                save
            );

            fileMenu.DropDownItems.Add(
                clear
            );

            menu.Items.Add(
                fileMenu
            );

            notepad.MainMenuStrip =
                menu;

            notepad.Controls.Add(
                editor
            );

            notepad.Controls.Add(
                menu
            );

            editor.BringToFront();

            notepad.Show();
        }

        // ============================================================
        // DATEI LÖSCHEN
        // ============================================================

        private void DeleteSelectedFile(
            ListBox list,
            string path)
        {
            if (
                list.SelectedItem
                == null)
                return;

            string selected =
                list.SelectedItem
                .ToString()!;

            string fileName =
                RemoveFileIcon(
                    selected
                );

            if (
                !files[path].ContainsKey(
                    fileName))
                return;

            string content =
                files[path][fileName];

            // In Papierkorb verschieben
            recycleBin[
                path + "\\" + fileName
            ] = content;

            files[path].Remove(
                fileName
            );

            RefreshFileList(
                list,
                path
            );
        }

        // ============================================================
        // PAPIERKORB
        // ============================================================

        private void OpenRecycleBin()
        {
            Form bin =
                new Form();

            bin.Text =
                "🗑️ Papierkorb";

            bin.Size =
                new Size(
                    600,
                    450
                );

            bin.StartPosition =
                FormStartPosition.CenterScreen;

            ListBox list =
                new ListBox();

            list.Dock =
                DockStyle.Fill;

            list.Font =
                new Font(
                    "Segoe UI",
                    10
                );

            RefreshRecycleBin(
                list
            );

            // --------------------------------------------------------
            // RECHTSKLICK
            // --------------------------------------------------------

            ContextMenuStrip menu =
                new ContextMenuStrip();

            menu.Items.Add(
                "♻ Wiederherstellen",
                null,
                (s, e) =>
                {
                    RestoreSelectedFile(
                        list
                    );
                }
            );

            menu.Items.Add(
                "❌ Endgültig löschen",
                null,
                (s, e) =>
                {
                    PermanentlyDeleteSelectedFile(
                        list
                    );
                }
            );

            list.ContextMenuStrip =
                menu;

            // --------------------------------------------------------
            // LEEREN BUTTON
            // --------------------------------------------------------

            Button empty =
                new Button();

            empty.Text =
                "♻ Papierkorb leeren";

            empty.Dock =
                DockStyle.Bottom;

            empty.Height =
                45;

            empty.Click +=
                (s, e) =>
                {
                    if (
                        recycleBin.Count == 0)
                        return;

                    DialogResult result =
                        MessageBox.Show(
                            "Wirklich alles endgültig löschen?",
                            "Papierkorb",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Warning
                        );

                    if (
                        result ==
                        DialogResult.Yes)
                    {
                        recycleBin.Clear();

                        RefreshRecycleBin(
                            list
                        );
                    }
                };

            bin.Controls.Add(
                list
            );

            bin.Controls.Add(
                empty
            );

            bin.Show();
        }

        // ============================================================
        // PAPIERKORB AKTUALISIEREN
        // ============================================================

        private void RefreshRecycleBin(
            ListBox list)
        {
            list.Items.Clear();

            foreach (
                string path
                in recycleBin.Keys)
            {
                list.Items.Add(
                    "📄 " + path
                );
            }
        }

        // ============================================================
        // WIEDERHERSTELLEN
        // ============================================================

        private void RestoreSelectedFile(
            ListBox list)
        {
            if (
                list.SelectedItem
                == null)
                return;

            string selected =
                list.SelectedItem
                .ToString()!;

            string fullPath =
                selected.Replace(
                    "📄 ",
                    ""
                );

            int separator =
                fullPath.LastIndexOf(
                    "\\"
                );

            if (separator < 0)
                return;

            string folder =
                fullPath.Substring(
                    0,
                    separator
                );

            string fileName =
                fullPath.Substring(
                    separator + 1
                );

            if (
                !files.ContainsKey(folder))
                return;

            files[folder][fileName] =
                recycleBin[fullPath];

            recycleBin.Remove(
                fullPath
            );

            RefreshRecycleBin(
                list
            );
        }

        // ============================================================
        // ENDGÜLTIG LÖSCHEN
        // ============================================================

        private void PermanentlyDeleteSelectedFile(
            ListBox list)
        {
            if (
                list.SelectedItem
                == null)
                return;

            string selected =
                list.SelectedItem
                .ToString()!;

            string path =
                selected.Replace(
                    "📄 ",
                    ""
                );

            recycleBin.Remove(
                path
            );

            RefreshRecycleBin(
                list
            );
        }

        // ============================================================
        // DATEI-ICONS
        // ============================================================

        private string GetFileIcon(
            string file)
        {
            if (
                file.EndsWith(
                    ".txt",
                    StringComparison.OrdinalIgnoreCase))
                return "📄";

            if (
                file.EndsWith(
                    ".mp3",
                    StringComparison.OrdinalIgnoreCase))
                return "🎵";

            if (
                file.EndsWith(
                    ".jpg",
                    StringComparison.OrdinalIgnoreCase) ||
                file.EndsWith(
                    ".png",
                    StringComparison.OrdinalIgnoreCase))
                return "🖼️";

            if (
                file.EndsWith(
                    ".mp4",
                    StringComparison.OrdinalIgnoreCase))
                return "🎬";

            if (
                file.EndsWith(
                    ".zip",
                    StringComparison.OrdinalIgnoreCase))
                return "📦";

            return "📄";
        }

        private string RemoveFileIcon(
            string text)
        {
            string[] icons =
            {
                "📄 ",
                "🎵 ",
                "🖼️ ",
                "🎬 ",
                "📦 "
            };

            foreach (
                string icon
                in icons)
            {
                if (
                    text.StartsWith(
                        icon))
                {
                    return text.Substring(
                        icon.Length
                    );
                }
            }

            return text;
        }

        // ============================================================
        // BLUESCREEN
        // ============================================================

        private void ShowBSOD()
        {
            Form bsod =
                new Form();

            bsod.Text =
                "RetroOS";

            bsod.BackColor =
                Color.Blue;

            bsod.ForeColor =
                Color.White;

            bsod.FormBorderStyle =
                FormBorderStyle.None;

            bsod.WindowState =
                FormWindowState.Maximized;

            Label text =
                new Label();

            text.Text =
                ":(\r\n\r\n" +
                "RetroOS ist auf ein Problem gestoßen.\r\n\r\n" +
                "STOP: RETROOS_FAKE_CRASH\r\n\r\n" +
                "Keine Sorge – das ist nur Spaß! 😎\r\n\r\n" +
                "Drücke eine Taste zum Schließen.";

            text.Font =
                new Font(
                    "Consolas",
                    20
                );

            text.ForeColor =
                Color.White;

            text.AutoSize = true;

            text.Location =
                new Point(
                    100,
                    100
                );

            bsod.Controls.Add(
                text
            );

            bsod.KeyPreview = true;

            bsod.KeyDown +=
                (s, e) =>
                {
                    bsod.Close();
                };

            bsod.ShowDialog();
        }

        // ============================================================
        // DESKTOP AKTUALISIEREN
        // ============================================================

        private void RefreshDesktop()
        {
            MessageBox.Show(
                "Desktop aktualisiert! 🔄",
                "RetroOS"
            );
        }
    }
}