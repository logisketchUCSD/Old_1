using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;

using Sketch;
using ConverterXML;
using ConverterJnt;

namespace Toolkit
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class MainForm : System.Windows.Forms.Form
	{
		private Sketch.Sketch sketch;

		private FragmentPanel fragmenterPanel;
		private System.Windows.Forms.ToolBar fragmenterToolBar;

		private LabelerPanel labelerPanel;
		private System.Windows.Forms.ToolBar labelerToolBar;


		
		private System.Windows.Forms.StatusBar statusBar;
		private System.Windows.Forms.TabControl tabControl;
		private System.Windows.Forms.MainMenu mainMenu;
		private System.Windows.Forms.MenuItem fileMenuItem;
		private System.Windows.Forms.TabPage labelerTabPage;
		private System.Windows.Forms.MenuItem loadSketchMenuItem;
		private System.Windows.Forms.TabPage fragmentTabPage;
		private System.Windows.Forms.Panel viewingPanel;
		private System.Windows.Forms.ToolBar viewingToolBar;
		private System.Windows.Forms.ToolBar mainToolBar;
		private System.Windows.Forms.MenuItem saveSketchMenuItem;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public MainForm()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			// Double-buffering code
			this.SetStyle(ControlStyles.DoubleBuffer | ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint, true);
			this.UpdateStyles();

			// Give the current tab access to the viewing panel and scrollbars
			this.tabControl.SelectedTab = this.labelerTabPage;
			
			// Resize the main window
			MainFormResize();
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.statusBar = new System.Windows.Forms.StatusBar();
			this.mainMenu = new System.Windows.Forms.MainMenu();
			this.fileMenuItem = new System.Windows.Forms.MenuItem();
			this.loadSketchMenuItem = new System.Windows.Forms.MenuItem();
			this.saveSketchMenuItem = new System.Windows.Forms.MenuItem();
			this.mainToolBar = new System.Windows.Forms.ToolBar();
			this.tabControl = new System.Windows.Forms.TabControl();
			this.labelerTabPage = new System.Windows.Forms.TabPage();
			this.viewingPanel = new System.Windows.Forms.Panel();
			this.viewingToolBar = new System.Windows.Forms.ToolBar();
			this.fragmentTabPage = new System.Windows.Forms.TabPage();
			this.tabControl.SuspendLayout();
			this.labelerTabPage.SuspendLayout();
			this.SuspendLayout();
			// 
			// statusBar
			// 
			this.statusBar.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.statusBar.Location = new System.Drawing.Point(0, 546);
			this.statusBar.Name = "statusBar";
			this.statusBar.Size = new System.Drawing.Size(792, 20);
			this.statusBar.TabIndex = 0;
			// 
			// mainMenu
			// 
			this.mainMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					 this.fileMenuItem});
			// 
			// fileMenuItem
			// 
			this.fileMenuItem.Index = 0;
			this.fileMenuItem.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																						 this.loadSketchMenuItem,
																						 this.saveSketchMenuItem});
			this.fileMenuItem.Text = "File";
			// 
			// loadSketchMenuItem
			// 
			this.loadSketchMenuItem.Index = 0;
			this.loadSketchMenuItem.Text = "Load Sketch";
			this.loadSketchMenuItem.Click += new System.EventHandler(this.loadSketchMenuItem_Click);
			// 
			// saveSketchMenuItem
			// 
			this.saveSketchMenuItem.Index = 1;
			this.saveSketchMenuItem.Text = "Save Sketch";
			this.saveSketchMenuItem.Click += new System.EventHandler(this.saveSketchMenuItem_Click);
			// 
			// mainToolBar
			// 
			this.mainToolBar.ButtonSize = new System.Drawing.Size(47, 43);
			this.mainToolBar.Divider = false;
			this.mainToolBar.Dock = System.Windows.Forms.DockStyle.Right;
			this.mainToolBar.DropDownArrows = true;
			this.mainToolBar.Location = new System.Drawing.Point(745, 0);
			this.mainToolBar.Name = "mainToolBar";
			this.mainToolBar.ShowToolTips = true;
			this.mainToolBar.Size = new System.Drawing.Size(47, 546);
			this.mainToolBar.TabIndex = 1;
			// 
			// tabControl
			// 
			this.tabControl.Controls.Add(this.labelerTabPage);
			this.tabControl.Controls.Add(this.fragmentTabPage);
			this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tabControl.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.tabControl.Location = new System.Drawing.Point(0, 0);
			this.tabControl.Name = "tabControl";
			this.tabControl.SelectedIndex = 0;
			this.tabControl.Size = new System.Drawing.Size(745, 546);
			this.tabControl.TabIndex = 2;
			this.tabControl.SelectedIndexChanged += new System.EventHandler(this.tabControl_SelectedIndexChanged);
			// 
			// labelerTabPage
			// 
			this.labelerTabPage.Controls.Add(this.viewingPanel);
			this.labelerTabPage.Controls.Add(this.viewingToolBar);
			this.labelerTabPage.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.labelerTabPage.Location = new System.Drawing.Point(4, 29);
			this.labelerTabPage.Name = "labelerTabPage";
			this.labelerTabPage.Size = new System.Drawing.Size(737, 513);
			this.labelerTabPage.TabIndex = 0;
			this.labelerTabPage.Text = "Labeler";
			// 
			// viewingPanel
			// 
			this.viewingPanel.AutoScroll = true;
			this.viewingPanel.BackColor = System.Drawing.Color.White;
			this.viewingPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.viewingPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.viewingPanel.Location = new System.Drawing.Point(0, 51);
			this.viewingPanel.Name = "viewingPanel";
			this.viewingPanel.Size = new System.Drawing.Size(737, 462);
			this.viewingPanel.TabIndex = 3;
			// 
			// viewingToolBar
			// 
			this.viewingToolBar.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.viewingToolBar.ButtonSize = new System.Drawing.Size(47, 44);
			this.viewingToolBar.DropDownArrows = true;
			this.viewingToolBar.Location = new System.Drawing.Point(0, 0);
			this.viewingToolBar.Name = "viewingToolBar";
			this.viewingToolBar.ShowToolTips = true;
			this.viewingToolBar.Size = new System.Drawing.Size(737, 51);
			this.viewingToolBar.TabIndex = 2;
			// 
			// fragmentTabPage
			// 
			this.fragmentTabPage.Location = new System.Drawing.Point(4, 29);
			this.fragmentTabPage.Name = "fragmentTabPage";
			this.fragmentTabPage.Size = new System.Drawing.Size(737, 510);
			this.fragmentTabPage.TabIndex = 1;
			this.fragmentTabPage.Text = "Fragmenter";
			// 
			// MainForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(792, 566);
			this.Controls.Add(this.tabControl);
			this.Controls.Add(this.mainToolBar);
			this.Controls.Add(this.statusBar);
			this.Menu = this.mainMenu;
			this.MinimumSize = new System.Drawing.Size(800, 600);
			this.Name = "MainForm";
			this.Text = "Sketch Toolkit";
			this.Resize += new System.EventHandler(this.MainForm_Resize);
			this.tabControl.ResumeLayout(false);
			this.labelerTabPage.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() 
		{
			Application.Run(new MainForm());
		}


		private void SetPanelAttributes(Panel panel)
		{
			panel.AutoScroll = true;
			panel.BackColor = System.Drawing.Color.White;
			panel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			panel.Dock = System.Windows.Forms.DockStyle.Fill;
		}

		private void SetToolBarAttributes(ToolBar toolBar)
		{
			toolBar.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			toolBar.ButtonSize = new System.Drawing.Size(47, 44);
			toolBar.DropDownArrows = true;
			toolBar.ShowToolTips = true;
		}
		
		private void InitLabelerPanel()
		{
			this.labelerPanel = new LabelerPanel();
			SetPanelAttributes(this.labelerPanel);
			
			this.labelerToolBar = new ToolBar();
			SetToolBarAttributes(this.labelerToolBar);
		}
		
		private void InitFragmenterPanel()
		{
			this.fragmenterPanel = new FragmentPanel(this.sketch);
			SetPanelAttributes(this.fragmenterPanel);

			this.fragmenterToolBar = new ToolBar();
			SetToolBarAttributes(this.fragmenterToolBar);
		}


		/// <summary>
		/// Starts the "Open" dialog allowing the user to open an MIT XML file.
		/// </summary>
		/// <param name="sender">Reference to the object that raised the event</param>
		/// <param name="e">Passes an object specific to the event that is being handled</param>
		private void loadSketchMenuItem_Click(object sender, System.EventArgs e)
		{
			LoadSketch();
		}

		private void LoadSketch()
		{
			System.Windows.Forms.OpenFileDialog openFileDialog = new OpenFileDialog();
			
			openFileDialog.Title  = "Load a Sketch";
			openFileDialog.Filter = "MIT XML Files (*.xml)|*.xml|" +
				"Microsoft Windows Journal Files (*.jnt)|*.jnt";
			
			if (openFileDialog.ShowDialog() == DialogResult.OK)
			{
				string extension = System.IO.Path.GetExtension(openFileDialog.FileName.ToLower());
				
				// Load the Sketch
				if (extension.Equals(".jnt"))
				{
					this.statusBar.Text = "Loading Sketch...";
					this.sketch = (new ReadJnt(openFileDialog.FileName)).Sketch;
				}
				else if (extension.Equals(".xml"))
				{
					this.sketch = (new ReadXML(openFileDialog.FileName)).Sketch;
				}
				
				this.Text = System.IO.Path.GetFileNameWithoutExtension(openFileDialog.FileName);

				// Initialize the new panels
				InitLabelerPanel();
				InitFragmenterPanel();

				this.Refresh();

				this.statusBar.Text = "";
			}
		}


		
		private void saveSketchMenuItem_Click(object sender, System.EventArgs e)
		{
			SaveSketch();
		}
		
		
		/// <summary>
		/// NOTE: Want to later add flags for saving seperately:
		/// - original
		/// - labeled
		/// - fragged
		/// - combination of the above
		/// </summary>
		private void SaveSketch()
		{
			System.Windows.Forms.SaveFileDialog saveFileDialog = new SaveFileDialog();
			
			saveFileDialog.Filter = "MIT XML Files (*.xml)|*.xml";
			saveFileDialog.AddExtension = true;

			// Write the XML to a file
			if (saveFileDialog.ShowDialog() == DialogResult.OK)
			{
				if (this.sketch != null)
				{
					ConverterXML.MakeXML xmlHolder = new MakeXML(this.sketch);
					xmlHolder.WriteXML(saveFileDialog.FileName);
				}
				else 
					MessageBox.Show("No data to save", "Error");
			}
		}




		private void ChangePanelView(Panel newPanel, ToolBar newToolBar)
		{
			this.viewingPanel = newPanel;
			this.viewingToolBar = newToolBar;

			this.tabControl.SelectedTab.Controls.Clear();
			this.tabControl.SelectedTab.Controls.Add(this.viewingPanel);
			this.tabControl.SelectedTab.Controls.Add(this.viewingToolBar);

			this.viewingPanel.Refresh();
		}

		private void MainForm_Resize(object sender, System.EventArgs e)
		{
			MainFormResize();
		}

		private void MainFormResize()
		{
		}


		private void tabControl_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			// 0 = Labeler
			// 1 = Fragmenter
			
			switch (tabControl.SelectedIndex)
			{
				case 0:
					if (this.labelerPanel != null)
						ChangePanelView(this.labelerPanel, this.labelerToolBar);
					else
						ChangePanelView(this.viewingPanel, this.viewingToolBar);
					break;
				case 1:
					if (this.fragmenterPanel != null)
						ChangePanelView(this.fragmenterPanel, this.fragmenterToolBar);
					else
						ChangePanelView(this.viewingPanel, this.viewingToolBar);
					break;
				default:
					ChangePanelView(this.viewingPanel, this.viewingToolBar);
					break;
			}
		}

	}
}
