namespace Urbetrack.Postal
{
    partial class Sincronizacion
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Sincronizacion));
            this.mainMenu1 = new System.Windows.Forms.MainMenu();
            this.btSincronizar = new System.Windows.Forms.MenuItem();
            this.btCancelar = new System.Windows.Forms.MenuItem();
            this.lblSincronizar = new System.Windows.Forms.Label();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.SuspendLayout();
            // 
            // mainMenu1
            // 
            this.mainMenu1.MenuItems.Add(this.btSincronizar);
            this.mainMenu1.MenuItems.Add(this.btCancelar);
            // 
            // btSincronizar
            // 
            this.btSincronizar.Text = "Iniciar";
            this.btSincronizar.Click += new System.EventHandler(this.btSincronizar_Click);
            // 
            // btCancelar
            // 
            this.btCancelar.Text = "Cancelar";
            this.btCancelar.Click += new System.EventHandler(this.btCancelar_Click);
            // 
            // lblSincronizar
            // 
            this.lblSincronizar.Location = new System.Drawing.Point(118, 80);
            this.lblSincronizar.Name = "lblSincronizar";
            this.lblSincronizar.Size = new System.Drawing.Size(138, 30);
            this.lblSincronizar.Text = "Sincronizando";
            this.lblSincronizar.Visible = false;
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(14, 124);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(289, 30);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(14, 3);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(186, 57);
            // 
            // Sincronizacion
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(131F, 131F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(170)))), ((int)(((byte)(0)))));
            this.ClientSize = new System.Drawing.Size(320, 186);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.lblSincronizar);
            this.Menu = this.mainMenu1;
            this.Name = "Sincronizacion";
            this.Text = "Sincronizacion";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lblSincronizar;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.MenuItem btSincronizar;
        private System.Windows.Forms.MenuItem btCancelar;
        private System.Windows.Forms.PictureBox pictureBox1;
    }
}