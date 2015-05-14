namespace Urbetrack.Postal.Forms
{
    partial class LateralesReferencia
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
            this.menuItem1 = new System.Windows.Forms.MenuItem();
            this.menuItem2 = new System.Windows.Forms.MenuItem();
            this.tbReferencia = new System.Windows.Forms.TextBox();
            this.lbReferencia = new System.Windows.Forms.Label();
            this.tbLateral1 = new System.Windows.Forms.TextBox();
            this.lbLateral1 = new System.Windows.Forms.Label();
            this.tbLateral2 = new System.Windows.Forms.TextBox();
            this.lbLateral2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // mainMenu1
            // 
            this.mainMenu1.MenuItems.Add(this.menuItem1);
            this.mainMenu1.MenuItems.Add(this.menuItem2);
            // 
            // menuItem1
            // 
            this.menuItem1.Text = "Aceptar";
            this.menuItem1.Click += new System.EventHandler(this.menuItem1_Click);
            // 
            // menuItem2
            // 
            this.menuItem2.Text = "Después";
            this.menuItem2.Click += new System.EventHandler(this.menuItem2_Click);
            // 
            // tbReferencia
            // 
            this.tbReferencia.BackColor = System.Drawing.Color.White;
            this.tbReferencia.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(75)))), ((int)(((byte)(148)))));
            this.tbReferencia.Location = new System.Drawing.Point(98, 114);
            this.tbReferencia.Name = "tbReferencia";
            this.tbReferencia.Size = new System.Drawing.Size(211, 30);
            this.tbReferencia.TabIndex = 36;
            // 
            // lbReferencia
            // 
            this.lbReferencia.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(75)))), ((int)(((byte)(148)))));
            this.lbReferencia.Location = new System.Drawing.Point(1, 114);
            this.lbReferencia.Name = "lbReferencia";
            this.lbReferencia.Size = new System.Drawing.Size(108, 30);
            this.lbReferencia.Text = "Referencia:";
            // 
            // tbLateral1
            // 
            this.tbLateral1.BackColor = System.Drawing.Color.White;
            this.tbLateral1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(75)))), ((int)(((byte)(148)))));
            this.tbLateral1.Location = new System.Drawing.Point(98, 42);
            this.tbLateral1.Name = "tbLateral1";
            this.tbLateral1.Size = new System.Drawing.Size(211, 30);
            this.tbLateral1.TabIndex = 34;
            // 
            // lbLateral1
            // 
            this.lbLateral1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(75)))), ((int)(((byte)(148)))));
            this.lbLateral1.Location = new System.Drawing.Point(1, 42);
            this.lbLateral1.Name = "lbLateral1";
            this.lbLateral1.Size = new System.Drawing.Size(108, 30);
            this.lbLateral1.Text = "Lateral 1:";
            // 
            // tbLateral2
            // 
            this.tbLateral2.BackColor = System.Drawing.Color.White;
            this.tbLateral2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(75)))), ((int)(((byte)(148)))));
            this.tbLateral2.Location = new System.Drawing.Point(98, 78);
            this.tbLateral2.Name = "tbLateral2";
            this.tbLateral2.Size = new System.Drawing.Size(211, 30);
            this.tbLateral2.TabIndex = 35;
            // 
            // lbLateral2
            // 
            this.lbLateral2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(75)))), ((int)(((byte)(148)))));
            this.lbLateral2.Location = new System.Drawing.Point(1, 78);
            this.lbLateral2.Name = "lbLateral2";
            this.lbLateral2.Size = new System.Drawing.Size(108, 30);
            this.lbLateral2.Text = "Lateral 2:";
            // 
            // LateralesReferencia
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(131F, 131F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(320, 186);
            this.Controls.Add(this.tbReferencia);
            this.Controls.Add(this.lbReferencia);
            this.Controls.Add(this.tbLateral1);
            this.Controls.Add(this.lbLateral1);
            this.Controls.Add(this.tbLateral2);
            this.Controls.Add(this.lbLateral2);
            this.Menu = this.mainMenu1;
            this.Name = "LateralesReferencia";
            this.Text = "Más datos";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.MenuItem menuItem1;
        private System.Windows.Forms.MenuItem menuItem2;
        private System.Windows.Forms.TextBox tbReferencia;
        private System.Windows.Forms.Label lbReferencia;
        private System.Windows.Forms.TextBox tbLateral1;
        private System.Windows.Forms.Label lbLateral1;
        private System.Windows.Forms.TextBox tbLateral2;
        private System.Windows.Forms.Label lbLateral2;
    }
}