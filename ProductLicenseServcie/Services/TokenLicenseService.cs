using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;


namespace ProductLicenseServcie.Services
{


    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "TokenLicenseService" in both code and config file together.
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, 
        InstanceContextMode = InstanceContextMode.Single)]
    public class TokenLicenseService : ITokenLicenseService
    {
        private ProductCatalogManager catalogMgr = null;
        private ProductLicenseManager licenseMgr = null;
        private static int _instanceCount = 0;

        public TokenLicenseService()
        {
            catalogMgr = new ProductCatalogManager();
            licenseMgr = new ProductLicenseManager();
            _instanceCount++;
            Debug.WriteLine("Leaving TokenLicenseService(), Instance Count = {0}", _instanceCount);

        }
        public void AcquireTokenLicense(string productName, string catalogVersion)
        {


            Debug.WriteLine("Entering AcquireTokenLicense(), Instance Count = {0}", _instanceCount);
            Debug.WriteLine("AcquireTokenLicense() - ProductName={0}, CatalogVersion={1}", productName, catalogVersion);

            ITokenCallbackService callback =
                OperationContext.Current.GetCallbackChannel<ITokenCallbackService>();
            if (callback == null)
                return;

            LicAcquisitionTokenStatusType licStatus = new LicAcquisitionTokenStatusType();
            licStatus.productName = productName;
            AcquisitionChannelStatusType status; 
            int requiredCount = catalogMgr.GetTokenCount(productName, catalogVersion, out status);
            if(status != AcquisitionChannelStatusType.OK)
            {
                licStatus.tokenStatus = status;
                callback.OnAcquisitionLicenseStatus(licStatus);
                return;
            }

            string acqID = productName + "-" + catalogVersion;
            licenseMgr.AcquireNewTokens(acqID, requiredCount, out status);
            if (status != AcquisitionChannelStatusType.OK)
            {
                licStatus.tokenStatus = status;
                callback.OnAcquisitionLicenseStatus(licStatus);
                return;
            }

            licStatus.tokenStatus = AcquisitionChannelStatusType.OK;
            callback.OnAcquisitionLicenseStatus(licStatus);

            //if (callback != null)
            //{
            //    LicAcquisitionTokenStatusType licStatus = new LicAcquisitionTokenStatusType();
            //    licStatus.productName = "WonderBrew";
            //    licStatus.tokenStatus = TokenStatusType.OK;

            //    for (int i = 0; i <= 10; i++)
            //    {
            //        callback.OnAcquisitionLicenseStatus(licStatus, LicenseSystemStatusType.OK);
            //        Thread.Sleep(10000);

            //    }
            //}

            return;
        }
    }

    
    public class ProductCatalogManager
    {
        //Product catalog list by token count.  List is synchronized with available
        //Token Catalog licenses on the configured LS. 
        private Dictionary<string, int> productCatalog = null;

        //Task/thread handle for product catalog synchronization task.
        private Task CatalogTask = null; 

        public ProductCatalogManager()
        {

            productCatalog = new Dictionary<string, int>();
            productCatalog.Add("wonderbrew1-2.0", 10);
            productCatalog.Add("wonderbrew2-2.0", 5);
            productCatalog.Add("wonderbrew3-1.0", 2);


            //Start catalog synchronization task
            CatalogTask = Task.Run(() => UpdateCatalogTask());

                   
        }

        public ProductCatalogManager(Dictionary<string, int> aCatalog) 
        {
            productCatalog = aCatalog;

            //Start catalog synchronization task
            CatalogTask = Task.Run(() => UpdateCatalogTask());
        }

        // feaature_id is the product or feature whose token count is being requested.
        // If feature_id is a product id then feature_id = <product-name>, "pro2" for example.
        // If feature_id is a feature id then feature_id = <product-name>-<feature-name>, "pro2-polymers"
        public int GetTokenCount(string feature_id, string catalog_version, out AcquisitionChannelStatusType status)
        {
            status = AcquisitionChannelStatusType.OK;

            int cntValue;

            if (productCatalog.TryGetValue(feature_id + "-" + catalog_version, out cntValue))
            {
                return cntValue;
            }

            else
                status = AcquisitionChannelStatusType.NOT_ENOUGH_TOKENS;          

            return -1;
        }

        private static void UpdateCatalogTask()
        {
            bool done = false;            

            while(!done)
            {
                //Retrieve catalogs from LS
                //Update catalog list every 3 minutes if there are new product catalogs
                Thread.Sleep(60*3000);
            }

        }

    }


    public class ProductLicenseManager
    {
        private Dictionary<string, int> ProductCountList = null;

        public ProductLicenseManager()
        {
            ProductCountList = new Dictionary<string, int>();
            
        }

        public int AcquireNewTokens(string id, int tokenCount,  out AcquisitionChannelStatusType status)
        {
            status = AcquisitionChannelStatusType.OK;
            int tempVal = 0;
            if(!ProductCountList.TryGetValue(id, out tempVal))
            {
                ProductCountList.Add(id, tokenCount);
            }
            return 1;
        }


    }
}
