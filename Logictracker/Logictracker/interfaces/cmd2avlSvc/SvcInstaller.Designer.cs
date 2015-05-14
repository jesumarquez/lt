namespace cmd2avlSvc
{
    partial class SvcInstaller
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

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.cmd2avlProcessInstaller = new System.ServiceProcess.ServiceProcessInstaller();
            this.cmd2avlServiceInstaller = new System.ServiceProcess.ServiceInstaller();
            // 
            // cmd2avlProcessInstaller
            // 
            this.cmd2avlProcessInstaller.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
            this.cmd2avlProcessInstaller.Password = null;
            this.cmd2avlProcessInstaller.Username = null;
            // 
            // cmd2avlServiceInstaller
            // 
            this.cmd2avlServiceInstaller.Description = "Interfaz Command Data a AVL (Lomax)";
            this.cmd2avlServiceInstaller.DisplayName = "CMD2AVL";
            this.cmd2avlServiceInstaller.ServiceName = "CMD2AVL";
            this.cmd2avlServiceInstaller.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
            // 
            // SvcInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.cmd2avlProcessInstaller,
            this.cmd2avlServiceInstaller});
            this.AfterInstall += new System.Configuration.Install.InstallEventHandler(this.SvcInstaller_AfterInstall);

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller cmd2avlProcessInstaller;
        private System.ServiceProcess.ServiceInstaller cmd2avlServiceInstaller;
    }
}