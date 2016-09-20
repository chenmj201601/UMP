using VoiceCyber.UMP.Common11101;

namespace UMPS1110.Models.ConfigObjects
{
    public class AlarmProcessObject : ConfigObject
    {
        public const int PRO_PROCESSNAME = 12;

        public string ProcessName { get; set; }

        public override void GetBasicPropertyValues()
        {
            base.GetBasicPropertyValues();

            ResourceProperty propertyValue;
            for (int i = 0; i < ListProperties.Count; i++)
            {
                propertyValue = ListProperties[i];
                switch (propertyValue.PropertyID)
                {
                    case PRO_PROCESSNAME:
                        ProcessName = propertyValue.Value;
                        break;
                }
            }

            GetNameAndDescription();
        }

        public override void GetNameAndDescription()
        {
            if (ProcessName != null && ProcessName != string.Empty)
            {
                Name = string.Format("[{0}]", ProcessName);
                Description = string.Format("[{0}]{1}", ProcessName, ObjectID);
            }
            else
            {
                base.GetNameAndDescription();
            }
        }
    }
}
