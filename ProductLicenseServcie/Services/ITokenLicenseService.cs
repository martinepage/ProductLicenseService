using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace ProductLicenseServcie.Services
{


    public enum AcquisitionChannelStatusType
    {
        OK = 100,
        NO_TOKENS_AVAILABLE,
        NOT_ENOUGH_TOKENS,
        TOKEN_CATALOG_NOT_FOUND,
        TOKEN_COST_COULD_NOT_DETERMINED,
        SERVER_UNAVAILABLE,

    }

    public class LicAcquisitionTokenStatusType
    {
        public string productName { get; set; }
        public AcquisitionChannelStatusType tokenStatus { get; set; }
    }

    

    public interface ITokenCallbackService
    {
        //SUMMARY:
        //  Periodically returns status information to clients.  We can do this for
        //  each connected client on a timer bases or we can only send status when
        //  the status changes for a particular client.  Which ever is easier.
        [OperationContract(IsOneWay = true)]
        void OnAcquisitionLicenseStatus(LicAcquisitionTokenStatusType LicAcquisitionTokenStatus);

    }

    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "ITokenLicenseService" in both code and config file together.
    [ServiceContract(CallbackContract = typeof(ITokenCallbackService))]
    public interface ITokenLicenseService
    {
        // SUMMARY:
        //  Invoke acquisition asynchronously so no return value.  The callback
        //  interface above will contain staus of acquisition request.
        [OperationContract]
        void AcquireTokenLicense(string productName, string catalogVersion);

    }
}
