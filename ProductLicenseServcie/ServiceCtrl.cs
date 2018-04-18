using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using ProductLicenseServcie.Services;

namespace ProductLicenseServcie
{
    public partial class TokenLicenseSvc : ServiceBase
    {
        internal static ServiceHost myServiceHost = null;
        public TokenLicenseSvc()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            if (myServiceHost != null)
            {
                myServiceHost.Close();
            }

            myServiceHost = new ServiceHost(typeof(TokenLicenseService));
            myServiceHost.Open();
        }

        protected override void OnStop()
        {
            myServiceHost.Close();
        }
    }
}
