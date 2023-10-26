using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Security;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Integration
{
    public partial class Integration : Form
    {
        public string defuserid { get; set; }
        public string defpassword { get; set; }
        public bool autopostafterlogin { get; set; }
        public bool autologin { get; set; }

        private IObjectSpace securedObjectSpace;
        public SecurityStrategyComplex Security { get; set; }
        public IObjectSpaceProvider ObjectSpaceProvider { get; set; }

        public Integration(SecurityStrategyComplex security, IObjectSpaceProvider objectSpaceProvider)
        {
            InitializeComponent();
            Security = security;
            ObjectSpaceProvider = objectSpaceProvider;
        }

        private void Integration_Load(object sender, EventArgs e)
        {
            Code obj = new Code(Security, ObjectSpaceProvider);
            this.Close();

            Application.Exit();
            GC.Collect();
        }
    }
}
