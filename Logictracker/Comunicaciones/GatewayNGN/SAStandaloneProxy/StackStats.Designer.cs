using ListViewEx=Urbetrack.GatewayMovil.ListViewEx;

namespace Urbetrack.GatewayMovil
{
    partial class StackStats
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

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
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
            this.stats = new ListViewEx();
            this.SuspendLayout();
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 1500;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Propiedad";
            this.columnHeader1.Width = 272;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Valor";
            this.columnHeader2.Width = 215;
            // 
            // stats
            // 
            this.stats.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
                                                                                    this.columnHeader1,
                                                                                    this.columnHeader2});
            this.stats.Dock = System.Windows.Forms.DockStyle.Fill;
            this.stats.GridLines = true;
            this.stats.Location = new System.Drawing.Point(0, 0);
            this.stats.Name = "stats";
            this.stats.Size = new System.Drawing.Size(494, 385);
            this.stats.TabIndex = 0;
            this.stats.UseCompatibleStateImageBehavior = false;
            this.stats.View = System.Windows.Forms.View.Details;
            // 
            // StackStats
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(494, 385);
            this.Controls.Add(this.stats);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "StackStats";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Estadisticas del Control XBee";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.StackStats_FormClosing);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private ListViewEx stats;
    }
}