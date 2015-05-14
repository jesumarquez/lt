namespace Tarjetas
{
    partial class PrintView
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PrintView));
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.crystalReportViewer1 = new CrystalDecisions.Windows.Forms.CrystalReportViewer();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.btExportar = new System.Windows.Forms.ToolStripButton();
            this.btCerrar = new System.Windows.Forms.ToolStripButton();
            this.btStart = new System.Windows.Forms.ToolStripButton();
            this.btPrev = new System.Windows.Forms.ToolStripButton();
            this.btNext = new System.Windows.Forms.ToolStripButton();
            this.btEnd = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btExportar,
            this.toolStripSeparator3,
            this.btStart,
            this.btPrev,
            this.btNext,
            this.btEnd,
            this.toolStripSeparator1,
            this.btCerrar,
            this.toolStripSeparator2});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(665, 25);
            this.toolStrip1.TabIndex = 1;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Margin = new System.Windows.Forms.Padding(20, 0, 0, 0);
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // crystalReportViewer1
            // 
            this.crystalReportViewer1.ActiveViewIndex = -1;
            this.crystalReportViewer1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.crystalReportViewer1.DisplayGroupTree = false;
            this.crystalReportViewer1.DisplayToolbar = false;
            this.crystalReportViewer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.crystalReportViewer1.EnableDrillDown = false;
            this.crystalReportViewer1.Location = new System.Drawing.Point(0, 25);
            this.crystalReportViewer1.Name = "crystalReportViewer1";
            this.crystalReportViewer1.SelectionFormula = "";
            this.crystalReportViewer1.ShowCloseButton = false;
            this.crystalReportViewer1.ShowExportButton = false;
            this.crystalReportViewer1.ShowGroupTreeButton = false;
            this.crystalReportViewer1.ShowPrintButton = false;
            this.crystalReportViewer1.ShowRefreshButton = false;
            this.crystalReportViewer1.Size = new System.Drawing.Size(665, 388);
            this.crystalReportViewer1.TabIndex = 2;
            this.crystalReportViewer1.ViewTimeSelectionFormula = "";
            // 
            // saveFileDialog1
            // 
            this.saveFileDialog1.Filter = "Archivo PDF|*.pdf";
            this.saveFileDialog1.Title = "Exportar";
            // 
            // btExportar
            // 
            this.btExportar.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btExportar.Image = global::Tarjetas.Properties.Resources.page_white_acrobat;
            this.btExportar.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btExportar.Name = "btExportar";
            this.btExportar.Size = new System.Drawing.Size(70, 22);
            this.btExportar.Text = "Exportar";
            this.btExportar.Click += new System.EventHandler(this.btExportar_Click);
            // 
            // btCerrar
            // 
            this.btCerrar.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btCerrar.Image = ((System.Drawing.Image)(resources.GetObject("btCerrar.Image")));
            this.btCerrar.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btCerrar.Name = "btCerrar";
            this.btCerrar.Size = new System.Drawing.Size(43, 22);
            this.btCerrar.Text = "Cerrar";
            this.btCerrar.Click += new System.EventHandler(this.btCerrar_Click);
            // 
            // btStart
            // 
            this.btStart.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btStart.Image = global::Tarjetas.Properties.Resources.control_start;
            this.btStart.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btStart.Name = "btStart";
            this.btStart.Size = new System.Drawing.Size(23, 22);
            this.btStart.Click += new System.EventHandler(this.btStart_Click);
            // 
            // btPrev
            // 
            this.btPrev.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btPrev.Image = global::Tarjetas.Properties.Resources.control_back;
            this.btPrev.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btPrev.Name = "btPrev";
            this.btPrev.Size = new System.Drawing.Size(23, 22);
            this.btPrev.Click += new System.EventHandler(this.btPrev_Click);
            // 
            // btNext
            // 
            this.btNext.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btNext.Image = global::Tarjetas.Properties.Resources.control_play;
            this.btNext.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btNext.Name = "btNext";
            this.btNext.Size = new System.Drawing.Size(23, 22);
            this.btNext.Click += new System.EventHandler(this.btNext_Click);
            // 
            // btEnd
            // 
            this.btEnd.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btEnd.Image = global::Tarjetas.Properties.Resources.control_end;
            this.btEnd.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btEnd.Name = "btEnd";
            this.btEnd.Size = new System.Drawing.Size(23, 22);
            this.btEnd.Click += new System.EventHandler(this.btEnd_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Margin = new System.Windows.Forms.Padding(0, 0, 20, 0);
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 25);
            // 
            // PrintView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(665, 413);
            this.ControlBox = false;
            this.Controls.Add(this.crystalReportViewer1);
            this.Controls.Add(this.toolStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "PrintView";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Vista de Impresion";
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton btCerrar;
        private CrystalDecisions.Windows.Forms.CrystalReportViewer crystalReportViewer1;
        private System.Windows.Forms.ToolStripButton btExportar;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.ToolStripButton btStart;
        private System.Windows.Forms.ToolStripButton btPrev;
        private System.Windows.Forms.ToolStripButton btNext;
        private System.Windows.Forms.ToolStripButton btEnd;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;

    }
}