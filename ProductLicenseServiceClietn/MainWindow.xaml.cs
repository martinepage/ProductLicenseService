using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ServiceModel;
using System.ServiceModel.Channels;
using ProductLicenseServiceClietn.ServiceReference1;



namespace ProductLicenseServiceClietn
{
    [CallbackBehavior(UseSynchronizationContext = false)]
    public class MyCallback : ITokenLicenseServiceCallback
    {
        public void OnAcquisitionLicenseStatus(LicAcquisitionTokenStatusType lt)
        {
            MessageBox.Show("Callback Reached");

        }

    }

    public class MyServiceClient: DuplexClientBase<ITokenLicenseService>, ITokenLicenseService
    {
        public MyServiceClient(InstanceContext callbackCntx)
            : base(callbackCntx)
        {

        }

        
         public void AcquireTokenLicense(string productName, string producVersion)
        {
            base.Channel.AcquireTokenLicense(productName, producVersion);
        }

         public Task AcquireTokenLicenseAsync(string productName, string producVersion)
        {
            
            Task t = base.Channel.AcquireTokenLicenseAsync(productName, producVersion);
            return t;
        }
    }
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    [CallbackBehavior(UseSynchronizationContext = false)]
    public partial class MainWindow : Window, ITokenLicenseServiceCallback
    {

        SynchronizationContext syncContext = null;
        

        public MainWindow()
        {
            InitializeComponent();
            syncContext = SynchronizationContext.Current;            
            
        }

        

        private void textBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        public void OnAcquisitionLicenseStatus(LicAcquisitionTokenStatusType lt)
        {

            string StatusString = "Acquisition Status= " + lt.tokenStatus.ToString() + "\nProduct Name= " + lt.productName.ToString() +
                "\n-----------------------------\n";

            SendOrPostCallback updateStatusBox = new SendOrPostCallback(arg =>
            {
                StatusBox.AppendText(StatusString);
            });

            syncContext.Send(updateStatusBox, null);

            //MessageBox.Show("Callback Reached");

        }

        public void OnTaskContinuation()
        {
            MessageBox.Show("Async Acquisition completed!!");
        }

        private async void Acquire_Click(object sender, RoutedEventArgs e)
        {
            //Method #1
            //ITokenLicenseServiceCallback callback = new MyCallback();
            //var instanceContext = new InstanceContext(callback);
            //MyServiceClient proxy = new MyServiceClient(instanceContext);
            //proxy.AcquireTokenLicense("WonderBrew", "12.0");


            //Method #2
            //ITokenLicenseServiceCallback callback = new MyCallback();
            //var instanceContext = new InstanceContext(callback);
            //var client = new ProductLicenseServiceClietn.ServiceReference1.TokenLicenseServiceClient(instanceContext);
            //client.AcquireTokenLicense("WonderBrew", "12.0");


            //Method #3
            //string productName = this.ProductName.Text;
            //string catalogVersion = this.CatalogVersion.Text;
            //await Task.Run(() =>
            //{
            //    ITokenLicenseServiceCallback callback = this;
            //    var instanceContext = new InstanceContext(callback);
            //    MyServiceClient proxy = new MyServiceClient(instanceContext);
            //    proxy.AcquireTokenLicense(productName, catalogVersion);

            //});

            //Method #4
            string productName = this.ProductName.Text;
            string catalogVersion = this.CatalogVersion.Text;
            ITokenLicenseServiceCallback callback = this;
            var instanceContext = new InstanceContext(callback);
            MyServiceClient proxy = new MyServiceClient(instanceContext);
            Task t = proxy.AcquireTokenLicenseAsync(productName, catalogVersion);
            t.ContinueWith((Task t1) =>
           {
               OnTaskContinuation();
           }       
            
            );

            

            


        }

        private void ReleaseLicense_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Relaase License Event");
        }
    }

    
}
