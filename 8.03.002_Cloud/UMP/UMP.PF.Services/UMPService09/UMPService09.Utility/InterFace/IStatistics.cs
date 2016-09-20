using System;
using UMPService09.Model;

namespace UMPService09.Utility.InterFace
{
    public interface IStatistics
    {
        void DoAction(DataBaseConfig ADataBaseConfig, ServiceConfigInfo AServiceConfigInfo,GlobalSetting AGlobalSetting);
    }
}
;