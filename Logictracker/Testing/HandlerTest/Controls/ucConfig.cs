using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using HandlerTest.Classes;

namespace HandlerTest.Controls
{
    public partial class ucConfig : UserControl, IConfig
    {
        public ITestApp TestApp { get { return ParentForm as ITestApp; } }

        public string Queue { get { return txtQueueName.Text; } set { txtQueueName.Text = value; } }

        public ucConfig()
        {
            InitializeComponent();
        }
    }
}
