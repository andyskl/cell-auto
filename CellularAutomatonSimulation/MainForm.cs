using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CellularAutomatonSimulation
{
    public partial class MainForm : Form
    {
        Graphics graphics;
        BufferedGraphicsContext bufferedGraphicsContext;
        BufferedGraphics bufferedGraphics;
        bool running;

        public MainForm()
        {
            InitializeComponent();
            InitializeGraphics();
            Randomizer.Initialize();
            Grid.Initialize(200, 100);
            typeSelectComboBox.SelectedIndex = 0;
            mainTimer.Interval = 400;
            mainTimer.Start();
        }

        private void InitializeGraphics()
        {
            this.DoubleBuffered = true;
            graphics = mainPictureBox.CreateGraphics();
            bufferedGraphicsContext = new BufferedGraphicsContext();
            bufferedGraphics = bufferedGraphicsContext.Allocate(graphics, new Rectangle(0, 0, mainPictureBox.Width, mainPictureBox.Height));
        }

        private void Draw()
        {
            lock (bufferedGraphics)
            {
                bufferedGraphics.Graphics.Clear(Color.White);
                Grid.Draw(
                    bufferedGraphics.Graphics, 
                    mainPictureBox.Width,
                    mainPictureBox.Height);
            }
            try
            {
                bufferedGraphics.Render();
            }
            catch { }
        }

        private void mainPictureBox_Resize(object sender, EventArgs e)
        {
            InitializeGraphics();
        }

        private void mainTimer_Tick(object sender, EventArgs e)
        {
            Draw();
            typeSelectComboBox.Enabled = !running;
            if (running)                
                Grid.Step();
            stepLabel.Text = Grid.Generation.ToString();
        }

        private void stepButton_Click(object sender, EventArgs e)
        {
            Grid.Step();
        }

        private void runButton_Click(object sender, EventArgs e)
        {
            running = true;
        }

        private void pauseButton_Click(object sender, EventArgs e)
        {
            running = false;
        }

        private void stopButton_Click(object sender, EventArgs e)
        {
            running = false;
            Grid.Reset();
        }

        private void clearButton_Click(object sender, EventArgs e)
        {
            running = false;
            Grid.Clear();
        }

        private void randomButton_Click(object sender, EventArgs e)
        {
            running = false;
            Grid.Randomize();
        }

        private void mainPictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            running = false;
            Grid.TurnCell(e.X, e.Y, mainPictureBox.Width, mainPictureBox.Height);
        }

        private void typeSelectComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (typeSelectComboBox.SelectedIndex)
            {
                case 0: 
                    Grid.CurrentType = AutomatonType.Conway;
                    break;
                case 1:
                    Grid.CurrentType = AutomatonType.Maze;
                    break;
                case 2:
                    Grid.CurrentType = AutomatonType.Cave;
                    break;
            }
        }

        private void speedTrackBar_Scroll(object sender, EventArgs e)
        {
            mainTimer.Interval = 142 - speedTrackBar.Value * 42;
        }
    }
}
