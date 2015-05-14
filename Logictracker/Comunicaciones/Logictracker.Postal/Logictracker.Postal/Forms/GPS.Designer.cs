namespace Urbetrack.Postal.Forms
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
            this.btCancelar.Text = "Después";
            this.btCancelar.Click += new System.EventHandler(this.btCancelar_Click);
            // 
            // pnlFixed
            // 
            this.pnlFixed.BackColor = System.Drawing.Color.OrangeRed;
            this.pnlFixed.Controls.Add(this.StatusLabel);
            this.pnlFixed.Location = new System.Drawing.Point(0, 0);
            this.pnlFixed.Name = "pnlFixed";
            this.pnlFixed.Size = new System.Drawing.Size(320, 186);
            this.pnlFixed.GotFocus += new System.EventHandler(this.pnlFixed_GotFocus);
            // 
            // StatusLabel
            // 
            this.StatusLabel.BackColor = System.Drawing.Color.OrangeRed;
            this.StatusLabel.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold);
            this.StatusLabel.Location = new System.Drawing.Point(54, 75);
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
            // GPS
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(131F, 131F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(320, 186);
            this.Controls.Add(this.pnlFixed);
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
    }
}