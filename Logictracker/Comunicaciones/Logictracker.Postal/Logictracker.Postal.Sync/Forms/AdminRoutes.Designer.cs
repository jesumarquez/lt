namespace Urbetrack.Postal.Sync.Forms
{
    partial class AdminRoutes
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
            this.gbRoutes = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.txtDays = new System.Windows.Forms.NumericUpDown();
            this.radEliminadas = new System.Windows.Forms.RadioButton();
            this.radActivas = new System.Windows.Forms.RadioButton();
            this.radAsignadas = new System.Windows.Forms.RadioButton();
            this.btnUnasignRoutes = new System.Windows.Forms.Button();
            this.lbRoutes = new System.Windows.Forms.ListBox();
            this.gbRoutes.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtDays)).BeginInit();
            this.SuspendLayout();
            // 
            // gbRoutes
            // 
            this.gbRoutes.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.gbRoutes.Controls.Add(this.label2);
            this.gbRoutes.Controls.Add(this.label1);
            this.gbRoutes.Controls.Add(this.txtDays);
            this.gbRoutes.Controls.Add(this.radEliminadas);
            this.gbRoutes.Controls.Add(this.radActivas);
            this.gbRoutes.Controls.Add(this.radAsignadas);
            this.gbRoutes.Controls.Add(this.btnUnasignRoutes);
            this.gbRoutes.Controls.Add(this.lbRoutes);
            this.gbRoutes.Location = new System.Drawing.Point(12, 12);
            this.gbRoutes.Name = "gbRoutes";
            this.gbRoutes.Size = new System.Drawing.Size(263, 272);
            this.gbRoutes.TabIndex = 3;
            this.gbRoutes.TabStop = false;
            this.gbRoutes.Text = "Rutas";
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(229, 244);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(28, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "días";
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(135, 244);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Ultimos";
            // 
            // txtDays
            // 
            this.txtDays.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDays.Location = new System.Drawing.Point(182, 242);
            this.txtDays.Maximum = new decimal(new int[] {
            365,
            0,
            0,
            0});
            this.txtDays.Name = "txtDays";
            this.txtDays.Size = new System.Drawing.Size(44, 20);
            this.txtDays.TabIndex = 4;
            this.txtDays.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.txtDays.ValueChanged += new System.EventHandler(this.txtDays_ValueChanged);
            // 
            // radEliminadas
            // 
            this.radEliminadas.AutoSize = true;
            this.radEliminadas.Location = new System.Drawing.Point(182, 19);
            this.radEliminadas.Name = "radEliminadas";
            this.radEliminadas.Size = new System.Drawing.Size(75, 17);
            this.radEliminadas.TabIndex = 3;
            this.radEliminadas.TabStop = true;
            this.radEliminadas.Text = "Eliminadas";
            this.radEliminadas.UseVisualStyleBackColor = true;
            this.radEliminadas.CheckedChanged += new System.EventHandler(this.RadioChackedChanged);
            // 
            // radActivas
            // 
            this.radActivas.AutoSize = true;
            this.radActivas.Location = new System.Drawing.Point(98, 20);
            this.radActivas.Name = "radActivas";
            this.radActivas.Size = new System.Drawing.Size(60, 17);
            this.radActivas.TabIndex = 2;
            this.radActivas.Text = "Activas";
            this.radActivas.UseVisualStyleBackColor = true;
            this.radActivas.CheckedChanged += new System.EventHandler(this.RadioChackedChanged);
            // 
            // radAsignadas
            // 
            this.radAsignadas.AutoSize = true;
            this.radAsignadas.Checked = true;
            this.radAsignadas.Location = new System.Drawing.Point(7, 20);
            this.radAsignadas.Name = "radAsignadas";
            this.radAsignadas.Size = new System.Drawing.Size(74, 17);
            this.radAsignadas.TabIndex = 2;
            this.radAsignadas.TabStop = true;
            this.radAsignadas.Text = "Asignadas";
            this.radAsignadas.UseVisualStyleBackColor = true;
            this.radAsignadas.CheckedChanged += new System.EventHandler(this.RadioChackedChanged);
            // 
            // btnUnasignRoutes
            // 
            this.btnUnasignRoutes.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnUnasignRoutes.Enabled = false;
            this.btnUnasignRoutes.Location = new System.Drawing.Point(7, 239);
            this.btnUnasignRoutes.Name = "btnUnasignRoutes";
            this.btnUnasignRoutes.Size = new System.Drawing.Size(82, 23);
            this.btnUnasignRoutes.TabIndex = 1;
            this.btnUnasignRoutes.Text = "Desasignar";
            this.btnUnasignRoutes.UseVisualStyleBackColor = true;
            this.btnUnasignRoutes.Click += new System.EventHandler(this.BtnUnasingRoutesClick);
            // 
            // lbRoutes
            // 
            this.lbRoutes.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lbRoutes.DisplayMember = "ShowName";
            this.lbRoutes.FormattingEnabled = true;
            this.lbRoutes.Location = new System.Drawing.Point(7, 43);
            this.lbRoutes.Name = "lbRoutes";
            this.lbRoutes.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lbRoutes.Size = new System.Drawing.Size(250, 186);
            this.lbRoutes.TabIndex = 0;
            this.lbRoutes.SelectedIndexChanged += new System.EventHandler(this.LbRoutesSelectedIndexChanged);
            // 
            // AdminRoutes
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(287, 298);
            this.Controls.Add(this.gbRoutes);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AdminRoutes";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Administrar Rutas";
            this.Shown += new System.EventHandler(this.AdminRoutes_Shown);
            this.gbRoutes.ResumeLayout(false);
            this.gbRoutes.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtDays)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox gbRoutes;
        private System.Windows.Forms.Button btnUnasignRoutes;
        private System.Windows.Forms.ListBox lbRoutes;
        private System.Windows.Forms.RadioButton radEliminadas;
        private System.Windows.Forms.RadioButton radActivas;
        private System.Windows.Forms.RadioButton radAsignadas;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown txtDays;
        private System.Windows.Forms.Label label2;

    }
}