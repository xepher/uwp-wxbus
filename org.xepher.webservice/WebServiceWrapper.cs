using org.xepher.webservice.BusTravelGuideServiceReference;

namespace org.xepher.webservice
{
    public class WebServiceWrapper
    {
        public BusTravelGuideSoapClient soapClient;

        public WebServiceWrapper()
        {
            soapClient = new BusTravelGuideSoapClient();
        }

        public void GetBusALStationInfoCommonAsync(int routeid, int segmentid, int stationseq, string fdisMsg)
        {
            soapClient.getBusALStationInfoCommonAsync(routeid.ToString(), segmentid.ToString(), stationseq.ToString(),
                                                      fdisMsg);
        }
    }
}
