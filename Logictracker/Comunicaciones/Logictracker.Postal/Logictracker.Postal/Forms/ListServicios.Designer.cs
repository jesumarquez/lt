using System.Drawing;
using System.Windows.Forms;
using C1.Win.C1FlexGrid;
using Urbetrack.Postal.DataSources;

namespace Urbetrack.Postal.Forms
{
    partial class ListServicios
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.MainMenu mainMenu1;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ListServicios));
            this.mainMenu1 = new System.Windows.Forms.MainMenu();
            this.menuItem1 = new System.Windows.Forms.MenuItem();
            this.menuItem2 = new System.Windows.Forms.MenuItem();
            this.gridServicios = new C1.Win.C1FlexGrid.C1FlexGrid();
            this.servicioViewBindingSource = new System.Windows.Forms.BindingSource(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.servicioViewBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // mainMenu1
            // 
            this.mainMenu1.MenuItems.Add(this.menuItem1);
            this.mainMenu1.MenuItems.Add(this.menuItem2);
            // 
            // menuItem1
            // 
            this.menuItem1.Text = "Ver";
            this.menuItem1.Click += new System.EventHandler(this.menuItem1_Click);
            // 
            // menuItem2
            // 
            this.menuItem2.Text = "Tomar...";
            this.menuItem2.Click += new System.EventHandler(this.menuItem2_Click);
            // 
            // gridServicios
            // 
            this.gridServicios.AllowDragging = C1.Win.C1FlexGrid.AllowDraggingEnum.None;
            this.gridServicios.AllowEditing = false;
            this.gridServicios.AutoResize = true;
            this.gridServicios.AutoSearchDelay = 1;
            this.gridServicios.BorderStyle = C1.Win.C1FlexGrid.Util.BaseControls.BorderStyleEnum.None;
            this.gridServicios.Clip = "";
            this.gridServicios.ClipSeparators = "\t\r";
            this.gridServicios.Col = 1;
            this.gridServicios.ColSel = 1;
            this.gridServicios.ComboList = null;
            this.gridServicios.EditMask = null;
            this.gridServicios.ExtendLastCol = true;
            this.gridServicios.LeftCol = 1;
            this.gridServicios.Location = new System.Drawing.Point(0, 0);
            this.gridServicios.Name = "gridServicios";
            this.gridServicios.Redraw = true;
            this.gridServicios.Row = 1;
            this.gridServicios.RowSel = 1;
            this.gridServicios.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.gridServicios.ScrollPosition = new System.Drawing.Point(0, 0);
            this.gridServicios.ScrollTrack = true;
            this.gridServicios.SelectionMode = C1.Win.C1FlexGrid.SelectionModeEnum.Row;
            this.gridServicios.ShowCursor = false;
            this.gridServicios.ShowSort = true;
            this.gridServicios.Size = new System.Drawing.Size(320, 186);
            this.gridServicios.StyleInfo = resources.GetString("gridServicios.StyleInfo");
            this.gridServicios.TabIndex = 1;
            this.gridServicios.Text = "c1FlexGrid1";
            this.gridServicios.TopRow = 1;
            // 
            // servicioViewBindingSource
            // 
            this.servicioViewBindingSource.DataSource = typeof(Urbetrack.Postal.DataSources.ServicioView);
            this.servicioViewBindingSource.Sort = "Direccion";
            // 
            // ListServicios
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(131F, 131F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(192)))), ((int)(((byte)(219)))));
            this.ClientSize = new System.Drawing.Size(320, 186);
            this.Controls.Add(this.gridServicios);
            this.KeyPreview = true;
            this.Menu = this.mainMenu1;
            this.Name = "ListServicios";
            this.Text = "Servicios";
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.OnKeyPress);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ListServicios_KeyDown);
            ((System.ComponentModel.ISupportInitialize)(this.servicioViewBindingSource)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.MenuItem menuItem1;
        private System.Windows.Forms.MenuItem menuItem2;
        private System.Windows.Forms.BindingSource servicioViewBindingSource;
        private C1.Win.C1FlexGrid.C1FlexGrid gridServicios;
    }
}