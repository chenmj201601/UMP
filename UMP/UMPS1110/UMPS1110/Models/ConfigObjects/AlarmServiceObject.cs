using VoiceCyber.UMP.Common11101;

namespace UMPS1110.Models.ConfigObjects
{
    public class AlarmServiceObject : ConfigObject
    {
        public const int PRO_SERVICENAME = 12;

        public string ServerName { get; set; }

        public override void GetBasicPropertyValues()
        {
            base.GetBasicPropertyValues();

            ResourceProperty propertyValue;
            for (int i = 0; i < ListProperties.Count; i++)
            {
                propertyValue = ListProperties[i];
                switch (propertyValue.PropertyID)
                {
                    case PRO_SERVICENAME:
                        ServerName = propertyValue.Value;
                        break;
                }
            }

            GetNameAndDescription();
        }

        public override void GetNameAndDescription()
        {
            if (ServerName != null && ServerName != string.Empty)
            {
                Name = string.Format("[{0}]",ServerName);
                Description = string.Format("[{0}]{1}",ServerName, ObjectID);
            }
            else
            {
                base.GetNameAndDescription();
            }
        }
    }
}
