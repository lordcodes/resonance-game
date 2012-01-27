using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace LevelEditor
{
    public partial class Form1 : Form
    {
        string[] models = new string[21];
        
        public Form1()
        {
            InitializeComponent();

            models[0] = "";
            models[1] = "tree";
            models[2] = "house";
            models[3] = "virus";
            models[4] = "goodVibe";
            models[5] = "terrain";
            models[6] = "house";
            models[7] = "wave";
            models[8] = "wave";
            models[9] = "wave";
            models[10] = "wave";
            models[11] = "wave";
            models[12] = "virus";
            models[13] = "virus";
            models[14] = "testAnim";
            models[15] = "virus";
            models[16] = "virus";
            models[17] = "pickup";
            models[18] = "ShieldGoodVibe";
            models[19] = "";
            models[20] = "";

            this.Size =new Size(1024, 700);
            Label title = new Label();
            Button begin = new Button();
            Button export = new Button();

            this.AutoScroll = true;
            this.Text = "Level Editor - Resonance";
            this.MaximizeBox = false;
            title.Text = "Level Editor";
            title.Location = new Point(450,20);
            title.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            title.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            title.AutoSize = true;
            title.Font = new System.Drawing.Font("Arial", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));

            begin.Text = "Begin";
            begin.Height = 40;
            begin.Width = 100;
            begin.Font = new System.Drawing.Font("Arial", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            begin.Location = new Point(435, 60);

            export.Text = "Export";
            export.Height = 40;
            export.Width = 100;
            export.Font = new System.Drawing.Font("Arial", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            export.Location = new Point(545, 60);

            this.Controls.Add(export);
            this.Controls.Add(title);
            this.Controls.Add(begin);
            this.BackColor = System.Drawing.Color.White;
            initializeMenuPanel();
            initializeOptionsPanel();
            initializeGraphPanel();
        }

        private void initializeGraphPanel()
        {
            Panel graphPanel = new Panel();
            PictureBox[] images = new PictureBox[26*26];
            int index = 0;
            int x = 0;
            int y = 0;
            for (int i = 0; i < 25; i++)
            {
                for (int j = 0; j < 25; j++)
                {
                    images[index] = new PictureBox();
                    images[index].Size = new Size(40, 40);
                    images[index].Location = new Point(x, y);
                    images[index].BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;

                    graphPanel.Controls.Add(images[index]);
                    x = x + 40;
                    index++;
                }
                x = 0;
                y = y + 40;
            }
                
            
            graphPanel.Size = new Size(690,530);
            graphPanel.Location = new Point(310, 120);
            graphPanel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            graphPanel.AutoScroll = true;


           
            this.Controls.Add(graphPanel);
        }
        private void initializeOptionsPanel()
        {
            Panel optionPanel = new Panel();
            Label title = new Label();
            Label[] options = new Label[21];
            PictureBox[] pictures = new PictureBox[21];

            title.Text = "Game Models";
            title.Location = new Point(70, 0);
            title.AutoSize = true;
            title.Font = new System.Drawing.Font("Arial", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));

            int y = 20;
            for (int i = 1; i < 21; i++)
            {
                options[i] = new Label();
                pictures[i] = new PictureBox();
                options[i].AutoSize = true;
                options[i].Text = i.ToString() + " " + models[i];
                options[i].Location = new Point(0, y);
                options[i].Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));

                pictures[i].Size = new Size(80, 20);
                pictures[i].Location = new Point(150, y);
                pictures[i].BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;

                optionPanel.Controls.Add(pictures[i]);
                optionPanel.Controls.Add(options[i]);
                y = y + 20;

            }

            optionPanel.Size = new Size(270, 410);
            optionPanel.Location = new Point(30, 240);
            optionPanel.AutoScroll = true;
            optionPanel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;

            optionPanel.Controls.Add(title);
            this.Controls.Add(optionPanel);
        }
        private void initializeMenuPanel()
        {
            Panel menuPanel = new Panel();
            Label levelNumberLabel = new Label();
            TextBox levelNumberTextBox = new TextBox();
            Label levelNameLabel = new Label();
            TextBox levelNameTextBox = new TextBox();
            Button confirm = new Button();

            levelNumberLabel.Text = "Level Number:";
            levelNumberLabel.Font = new System.Drawing.Font("Arial", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            levelNumberLabel.AutoSize = true;
            levelNumberLabel.Location = new Point(0, 0);

            levelNumberTextBox.Width = 100;
            levelNumberTextBox.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            levelNumberTextBox.BackColor = System.Drawing.Color.Beige;
            levelNumberTextBox.Location = new Point(150, 0);

            menuPanel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            menuPanel.Size = new Size(270, 100);
            menuPanel.Location = new Point(30, 120);           
            menuPanel.AutoScroll = true;
            menuPanel.AutoSize = true;
            menuPanel.BackColor = System.Drawing.Color.GhostWhite;

            levelNameLabel.Text = "Level Name:";
            levelNameLabel.AutoSize = true;
            levelNameLabel.Location = new Point(0, 30);
            levelNameLabel.Font = new System.Drawing.Font("Arial", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));

            levelNameTextBox.Width = 100;
            levelNameTextBox.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            levelNameTextBox.BackColor = System.Drawing.Color.Beige;
            levelNameTextBox.Location = new Point(150, 30);

            confirm.Width = 100;
            confirm.Text = "Confirm";
            confirm.Font = new System.Drawing.Font("Arial", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            confirm.Location = new Point(150, 60);
            confirm.Height = 40;

            menuPanel.Controls.Add(levelNumberLabel);
            menuPanel.Controls.Add(levelNumberTextBox);
            menuPanel.Controls.Add(levelNameLabel);
            menuPanel.Controls.Add(levelNameTextBox);
            menuPanel.Controls.Add(confirm);

            this.Controls.Add(menuPanel);
            
        }
    }
}
