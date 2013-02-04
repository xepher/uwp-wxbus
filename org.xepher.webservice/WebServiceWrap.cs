using org.xepher.webservice.BusTravelGuideServiceReference;

namespace org.xepher.webservice
{
    public class WebServiceWrap
    {
        public BusTravelGuideSoapClient soapClient = new BusTravelGuideSoapClient();

        public void GetBusALStationInfoCommon(int routeid, int segmentid,int stationseq,string fdisMsg)
        {
            soapClient.getBusALStationInfoCommonAsync(routeid.ToString(), segmentid.ToString(), stationseq.ToString(),
                                                      fdisMsg);
        }
    }
}
