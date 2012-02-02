using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Intermediate;
using ResonanceLibrary;
using System.Xml;

namespace LevelEditor
{
    public partial class Form1 : Form
    {
        string[] models = new string[21];
        TextBox levelNameTextBox;
        TextBox levelNumberTextBox;
        Label[] images = new Label[26 * 26];
        Label[] pictures = new Label[21];
        string selectedOption = null;
        System.Drawing.Color selectedColor;
        Boolean goodVibe = false;

        public Form1()
        {
            InitializeComponent();

            models[0] = "";
            models[1] = "tree";
            models[2] = "house";
            models[3] = "goodVibe";
            models[4] = "terrain";
            models[5] = "pickup";
            this.AllowDrop = true;
            this.Size =new Size(1350, 700);
            Label title = new Label();
            Button begin = new Button();
            Button export = new Button();

            this.AutoScroll = true;
            this.Text = "Level Editor - Resonance";
            this.MaximizeBox = false;
            title.Text = "Level Editor";
            title.Location = new Point(600,20);
            title.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            title.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            title.AutoSize = true;
            title.Font = new System.Drawing.Font("Arial", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));

            begin.Text = "Begin";
            begin.Height = 40;
            begin.Width = 100;
            begin.Font = new System.Drawing.Font("Arial", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            begin.Location = new Point(585, 60);

            export.Text = "Export";
            export.Height = 40;
            export.Width = 100;
            export.Font = new System.Drawing.Font("Arial", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            export.Location = new Point(695, 60);

            this.Controls.Add(export);
            this.Controls.Add(title);
            this.Controls.Add(begin);
            Console.WriteLine();
            this.BackgroundImage = Image.FromFile(Path.GetDirectoryName(Path.GetDirectoryName(Application.StartupPath))+@"\Images\splash1.png");

            begin.Click += beginClicked;
            export.Click += exportClicked;
           
           
           
        }
        private void exportClicked(object sender, System.EventArgs e)
        {
            StoredObjects list = new StoredObjects();
            StoredObject obj = new StoredObject();
            obj.identifier = "Ground";
            obj.type = "Ground";
            obj.xWorldCoord = 0;
            obj.yWorldCoord = 0;
            obj.zWorldCoord = 0;
            obj.gameModelNum = 5;
            obj.pickuptype = -1;

            int treeNumber = 0;
            int pickupNumber = 0;
            int houseNumber = 0;

            for (int i = 0; i < 625; i++)
            {
                Console.WriteLine(i);
                if(images[i].Text.Equals("tree"))
                {
                    obj = new StoredObject();
                    obj.identifier = "tree"+treeNumber.ToString();
                    obj.type = "Tree";
                    obj.gameModelNum = 1;
                    obj.xWorldCoord = -125 + ((i * 100) / 250);
                    obj.yWorldCoord = (float)-0.1;
                    obj.zWorldCoord = 125 - ((i * 100) / 250);
                    obj.pickuptype = -1;
                    treeNumber++;
                    list.addObject(obj);
                }
                if (images[i].Text.Equals("house"))
                {
                    obj = new StoredObject();
                    obj.identifier = "house" + houseNumber.ToString();
                    obj.type = "House";
                    obj.gameModelNum = 2;
                    obj.xWorldCoord = -125 + ((i * 100) / 250);
                    obj.yWorldCoord = (float)-0.1;
                    obj.zWorldCoord = 125 - ((i * 100) / 250);
                    obj.pickuptype = -1;
                    houseNumber++;
                    list.addObject(obj);
                }
                if (images[i].Text.Equals("goodVibe"))
                {
                    obj = new StoredObject();
                    obj.identifier = "Player";
                    obj.type = "Good_Vibe";
                    obj.gameModelNum = 4;
                    obj.xWorldCoord = -125 + ((i * 100) / 250);
                    obj.yWorldCoord = (float)-0.1;
                    obj.zWorldCoord = 125 - ((i * 100) / 250);
                    obj.pickuptype = -1;
                    list.addObject(obj);
                }
                if (images[i].Text.Equals("pickup"))
                {
                    obj = new StoredObject();
                    obj.identifier = "pickup"+pickupNumber.ToString();
                    obj.type = "Pickup";
                    obj.gameModelNum = 8;
                    obj.xWorldCoord = -125 + ((i * 100) / 250);
                    obj.yWorldCoord = (float)-0.1;
                    obj.zWorldCoord = 125 - ((i * 100) / 250);
                    obj.pickuptype = 1;
                    pickupNumber++;
                    list.addObject(obj);
                }
            }
            Serialize(list,"Level"+levelNumberTextBox.Text+".xml");

        }

        static void Serialize(StoredObjects obj, string filename)
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            using (XmlWriter writer = XmlWriter.Create(filename, settings))
            {
                IntermediateSerializer.Serialize<StoredObjects>(writer, obj, null);
            }
        }
        static void Deserialize(string filename)
        {
            StoredObjects data = null;
            using (FileStream stream = new FileStream(filename, FileMode.Open))
            {
                using (XmlReader reader = XmlReader.Create(stream))
                {
                    data = IntermediateSerializer.Deserialize<StoredObjects>(reader, null);
                }
            }
            Console.WriteLine(data.list[0].identifier);
        }

        private void confirmClicked(object sender, System.EventArgs e)
        {
            if (levelNameTextBox.Text.Length > 0 && levelNumberTextBox.Text.Length > 0)
            {
                initializeOptionsPanel();
                initializeGraphPanel();
            }
        }

        private void beginClicked(object sender, System.EventArgs e)
        {
            initializeMenuPanel();
        }

        private void initializeGraphPanel()
        {
            Panel graphPanel = new Panel();
            
            int index = 0;
            int x = 0;
            int y = 0;
            for (int i = 0; i < 25; i++)
            {
                for (int j = 0; j < 25; j++)
                {
                    images[index] = new Label();
                    images[index].Width = 40;
                    images[index].Location = new Point(x, y);
                    images[index].BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
                    images[index].BackColor = System.Drawing.Color.Green;
                    images[index].Text = "terrain";
                    images[index].ForeColor = Color.Black;
                    images[index].AllowDrop = true;
                    images[index].Click += new EventHandler(insertObject);

                    graphPanel.Controls.Add(images[index]);
                    x = x + 40;
                    index++;
                }
                x = 0;
                y = y + 20;
            }                
            
            graphPanel.Size = new Size(1010,530);
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

            title.Text = "Game Models";
            title.Location = new Point(70, 0);
            title.AutoSize = true;
            title.Font = new System.Drawing.Font("Arial", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));

            int y = 20;
            for (int i = 1; i <= 5 ; i++)
            {
                options[i] = new Label();
                pictures[i] = new Label();
                options[i].AutoSize = true;
                options[i].Text = i.ToString() + " " + models[i];
                options[i].Location = new Point(0, y);
                options[i].Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));

                pictures[i].Size = new Size(80, 20);
                pictures[i].Location = new Point(150, y);                
                pictures[i].BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
                
              
                


                if (models[i].Equals("terrain") == true)
                    pictures[i].BackColor = System.Drawing.Color.Green;
                else if (models[i].Equals("house") == true)
                    pictures[i].BackColor = System.Drawing.Color.Red;
                else if (models[i].Equals("goodVibe") == true)
                    pictures[i].BackColor = System.Drawing.Color.Blue;
                else if (models[i].Equals("tree") == true)
                    pictures[i].BackColor = System.Drawing.Color.Brown;
                else if (models[i].Equals("pickup") == true)
                    pictures[i].BackColor = System.Drawing.Color.Yellow;
                else
                    pictures[i].BackColor = System.Drawing.Color.Azure;
                pictures[i].Text = models[i];
                pictures[i].ForeColor = System.Drawing.Color.Black;
                pictures[i].Click += new EventHandler(selectObject);

                optionPanel.Controls.Add(pictures[i]);
                optionPanel.Controls.Add(options[i]);
                y = y + 20;

            }

            optionPanel.Size = new Size(270, 410);
            optionPanel.Location = new Point(30, 240);
            optionPanel.AutoScroll = true;
            optionPanel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            optionPanel.BackColor = System.Drawing.Color.GhostWhite;

            optionPanel.Controls.Add(title);
            this.Controls.Add(optionPanel);
        }
        
        private void initializeMenuPanel()
        {
            Panel menuPanel = new Panel();
            Label levelNumberLabel = new Label();
            levelNumberTextBox = new TextBox();
            Label levelNameLabel = new Label();
            levelNameTextBox = new TextBox();
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
            
            confirm.Click += confirmClicked;
            
        }

        void insertObject(object sender, EventArgs e)
        {
            var target = sender as Label;
            if (selectedOption != null && selectedColor != null)
            {
                if (selectedOption.Equals("goodVibe") == true && goodVibe == false)
                {
                    goodVibe = true;
                    target.Text = selectedOption;
                    target.BackColor = selectedColor;
                }
                else if(selectedOption.Equals("goodVibe") == false)
                {
                    if(target.Text.Equals("goodVibe") == true)
                        goodVibe = false;
                    target.Text = selectedOption;
                    target.BackColor = selectedColor;
                }
            }
            
        }
        void selectObject(object sender, EventArgs e)
        {
            var target = sender as Label;
            selectedOption = target.Text;
            selectedColor = target.BackColor;
            this.Text = "Selected object is: " + selectedOption + " and the color is " + selectedColor.ToString();
        }
       

        
       
    }
}
