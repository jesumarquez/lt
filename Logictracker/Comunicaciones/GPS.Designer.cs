namespace urbeGPSTASA65Std
{
    partial class GPS
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
            this.mainMenu1 = new System.Windows.Forms.MainMenu();
            this.btAceptar = new System.Windows.Forms.MenuItem();
            this.btCancelar = new System.Windows.Forms.MenuItem();
            this.pnlFixed = new System.Windows.Forms.Panel();
            this.StatusLabel = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer();
            this.Label8 = new System.Windows.Forms.Label();
            this.StampLabel = new System.Windows.Forms.Label();
            this.FixTypeLabel = new System.Windows.Forms.Label();
            this.Label6 = new System.Windows.Forms.Label();
            this.LatLabel = new System.Windows.Forms.Label();
            this.LongLabel = new System.Windows.Forms.Label();
            this.Label5 = new System.Windows.Forms.Label();
            this.Label4 = new System.Windows.Forms.Label();
            this.SatelitesLabel = new System.Windows.Forms.Label();
            this.HDOPLabel = new System.Windows.Forms.Label();
            this.Label3 = new System.Windows.Forms.Label();
            this.Label1 = new System.Windows.Forms.Label();
            this.pnlFixed.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainMenu1
            // 
            this.mainMenu1.MenuItems.Add(this.btAceptar);
            this.mainMenu1.MenuItems.Add(this.btCancelar);
            // 
            // btAceptar
            // 
            this.btAceptar.Enabled = false;
            this.btAceptar.Text = "Aceptar";
            this.btAceptar.Click += new System.EventHandler(this.btAceptar_Click);
            // 
            // btCancelar
            // 
            this.btCancelar.Text = "Cerrar";
            this.btCancelar.Click += new System.EventHandler(this.btCancelar_Click);
            // 
            // pnlFixed
            // 
            this.pnlFixed.BackColor = System.Drawing.Color.OrangeRed;
            this.pnlFixed.Controls.Add(this.StatusLabel);
            this.pnlFixed.Location = new System.Drawing.Point(0, 0);
            this.pnlFixed.Name = "pnlFixed";
            this.pnlFixed.Size = new System.Drawing.Size(320, 25);
            this.pnlFixed.GotFocus += new System.EventHandler(this.pnlFixed_GotFocus);
            // 
            // StatusLabel
            // 
            this.StatusLabel.BackColor = System.Drawing.Color.OrangeRed;
            this.StatusLabel.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold);
            this.StatusLabel.Location = new System.Drawing.Point(54, 2);
            this.StatusLabel.Name = "StatusLabel";
            this.StatusLabel.Size = new System.Drawing.Size(224, 18);
            this.StatusLabel.Text = "Esperando posición";
            this.StatusLabel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick_1);
            // 
            // Label8
            // 
            this.Label8.Font = new System.Drawing.Font("Segoe Condensed", 9F, System.Drawing.FontStyle.Regular);
            this.Label8.Location = new System.Drawing.Point(6, 64);
            this.Label8.Name = "Label8";
            this.Label8.Size = new System.Drawing.Size(79, 20);
            this.Label8.Text = "Hora GMT:";
            // 
            // StampLabel
            // 
            this.StampLabel.BackColor = System.Drawing.SystemColors.Info;
            this.StampLabel.Font = new System.Drawing.Font("Nina", 8F, System.Drawing.FontStyle.Regular);
            this.StampLabel.Location = new System.Drawing.Point(91, 70);
            this.StampLabel.Name = "StampLabel";
            this.StampLabel.Size = new System.Drawing.Size(128, 14);
            this.StampLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // FixTypeLabel
            // 
            this.FixTypeLabel.BackColor = System.Drawing.SystemColors.Info;
            this.FixTypeLabel.Font = new System.Drawing.Font("Nina", 8F, System.Drawing.FontStyle.Regular);
            this.FixTypeLabel.Location = new System.Drawing.Point(91, 130);
            this.FixTypeLabel.Name = "FixTypeLabel";
            this.FixTypeLabel.Size = new System.Drawing.Size(128, 14);
            this.FixTypeLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // Label6
            // 
            this.Label6.Font = new System.Drawing.Font("Segoe Condensed", 9F, System.Drawing.FontStyle.Regular);
            this.Label6.Location = new System.Drawing.Point(6, 124);
            this.Label6.Name = "Label6";
            this.Label6.Size = new System.Drawing.Size(46, 20);
            this.Label6.Text = "Tipo:";
            // 
            // LatLabel
            // 
            this.LatLabel.BackColor = System.Drawing.SystemColors.Info;
            this.LatLabel.Font = new System.Drawing.Font("Nina", 8F, System.Drawing.FontStyle.Regular);
            this.LatLabel.Location = new System.Drawing.Point(91, 90);
            this.LatLabel.Name = "LatLabel";
            this.LatLabel.Size = new System.Drawing.Size(128, 14);
            this.LatLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // LongLabel
            // 
            this.LongLabel.BackColor = System.Drawing.SystemColors.Info;
            this.LongLabel.Font = new System.Drawing.Font("Nina", 8F, System.Drawing.FontStyle.Regular);
            this.LongLabel.Location = new System.Drawing.Point(91, 110);
            this.LongLabel.Name = "LongLabel";
            this.LongLabel.Size = new System.Drawing.Size(128, 14);
            this.LongLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // Label5
            // 
            this.Label5.Font = new System.Drawing.Font("Segoe Condensed", 9F, System.Drawing.FontStyle.Regular);
            this.Label5.Location = new System.Drawing.Point(6, 104);
            this.Label5.Name = "Label5";
            this.Label5.Size = new System.Drawing.Size(63, 20);
            this.Label5.Text = "Longitud:";
            // 
            // Label4
            // 
            this.Label4.Font = new System.Drawing.Font("Segoe Condensed", 9F, System.Drawing.FontStyle.Regular);
            this.Label4.Location = new System.Drawing.Point(6, 84);
            this.Label4.Name = "Label4";
            this.Label4.Size = new System.Drawing.Size(63, 20);
            this.Label4.Text = "Latitud:";
            // 
            // SatelitesLabel
            // 
            this.SatelitesLabel.BackColor = System.Drawing.SystemColors.Info;
            this.SatelitesLabel.Font = new System.Drawing.Font("Nina", 8F, System.Drawing.FontStyle.Regular);
            this.SatelitesLabel.Location = new System.Drawing.Point(91, 50);
            this.SatelitesLabel.Name = "SatelitesLabel";
            this.SatelitesLabel.Size = new System.Drawing.Size(128, 14);
            this.SatelitesLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // HDOPLabel
            // 
            this.HDOPLabel.BackColor = System.Drawing.SystemColors.Info;
            this.HDOPLabel.Font = new System.Drawing.Font("Nina", 8F, System.Drawing.FontStyle.Regular);
            this.HDOPLabel.Location = new System.Drawing.Point(91, 30);
            this.HDOPLabel.Name = "HDOPLabel";
            this.HDOPLabel.Size = new System.Drawing.Size(128, 14);
            this.HDOPLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // Label3
            // 
            this.Label3.Font = new System.Drawing.Font("Segoe Condensed", 9F, System.Drawing.FontStyle.Regular);
            this.Label3.Location = new System.Drawing.Point(6, 44);
            this.Label3.Name = "Label3";
            this.Label3.Size = new System.Drawing.Size(67, 20);
            this.Label3.Text = "Satelites:";
            // 
            // Label1
            // 
            this.Label1.Font = new System.Drawing.Font("Segoe Condensed", 9F, System.Drawing.FontStyle.Regular);
            this.Label1.Location = new System.Drawing.Point(6, 24);
            this.Label1.Name = "Label1";
            this.Label1.Size = new System.Drawing.Size(46, 20);
            this.Label1.Text = "HDOP:";
            // 
            // GPS
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(131F, 131F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(320, 186);
            this.Controls.Add(this.Label8);
            this.Controls.Add(this.pnlFixed);
            this.Controls.Add(this.StampLabel);
            this.Controls.Add(this.FixTypeLabel);
            this.Controls.Add(this.Label1);
            this.Controls.Add(this.Label6);
            this.Controls.Add(this.LatLabel);
            this.Controls.Add(this.Label3);
            this.Controls.Add(this.LongLabel);
            this.Controls.Add(this.HDOPLabel);
            this.Controls.Add(this.Label5);
            this.Controls.Add(this.Label4);
            this.Controls.Add(this.SatelitesLabel);
            this.Font = new System.Drawing.Font("Nina", 8F, System.Drawing.FontStyle.Regular);
            this.Menu = this.mainMenu1;
            this.Name = "GPS";
            this.Text = "GPS";
            this.Load += new System.EventHandler(this.GPS_Load);
            this.pnlFixed.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        internal System.Windows.Forms.Panel pnlFixed;
        internal System.Windows.Forms.Label StatusLabel;


        private System.Windows.Forms.MenuItem btAceptar;
        private System.Windows.Forms.MenuItem btCancelar;
        private System.Windows.Forms.Timer timer1;
        internal System.Windows.Forms.Label Label8;
        internal System.Windows.Forms.Label StampLabel;
        internal System.Windows.Forms.Label FixTypeLabel;
        internal System.Windows.Forms.Label Label6;
        internal System.Windows.Forms.Label LatLabel;
        internal System.Windows.Forms.Label LongLabel;
        internal System.Windows.Forms.Label Label5;
        internal System.Windows.Forms.Label Label4;
        internal System.Windows.Forms.Label SatelitesLabel;
        internal System.Windows.Forms.Label HDOPLabel;
        internal System.Windows.Forms.Label Label3;
        internal System.Windows.Forms.Label Label1;
    }
}