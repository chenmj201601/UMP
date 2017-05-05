using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PFShareControls
{
    public partial class UCIPAddressTextBox : UserControl, PFShareInterface
    {
        public event EventHandler<OperationEventArgs> IOperationEvent;

        public UCIPAddressTextBox()
        {
            InitializeComponent();

            // 处理粘贴
            DataObject.AddPastingHandler(this, new DataObjectPastingEventHandler(IPTextBox_Paste));

            // 设置textBox次序
            IPAddress1.SetNeighbour(null, IPAddress2);
            IPAddress2.SetNeighbour(IPAddress1, IPAddress3);
            IPAddress3.SetNeighbour(IPAddress2, IPAddress4);
            IPAddress4.SetNeighbour(IPAddress3, null);

            ButtonMoreInformation.Click += ButtonMoreInformation_Click;
        }

        private void ButtonMoreInformation_Click(object sender, RoutedEventArgs e)
        {
            if (IOperationEvent != null)
            {
                OperationEventArgs LEventArgs = new OperationEventArgs();
                LEventArgs.StrObjectTag = "M-IP";
                LEventArgs.ObjectSource = GetIP();
                IOperationEvent(this, LEventArgs);
            }
        }

        // 处理粘贴 类似ip的形式才能粘贴
        private void IPTextBox_Paste(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(string)))
            {
                string LStrValue = e.DataObject.GetData(typeof(string)).ToString();
                SetIP(LStrValue);
            }
            e.CancelCommand();
        }

        public string GetIP()
        {
            return IPAddress1.Text.Trim() + "." + IPAddress2.Text.Trim() + "." + IPAddress3.Text.Trim() + "." + IPAddress4.Text.Trim();
        }

        // 设置ip
        public bool SetIP(string AStrIPAddress)
        {
            string[] LStrArrayIPs = AStrIPAddress.Split('.');
            if (LStrArrayIPs.Length == 4)
            {
                int LIntOutData;
                for (int LIntLoopArray = 0; LIntLoopArray < LStrArrayIPs.Length; ++LIntLoopArray)
                {
                    if (!Int32.TryParse(LStrArrayIPs[LIntLoopArray], out LIntOutData) || LIntOutData > 255 || LIntOutData < 0) { return false; }
                }
                IPAddress1.Text = LStrArrayIPs[0];
                IPAddress2.Text = LStrArrayIPs[1];
                IPAddress3.Text = LStrArrayIPs[2];
                IPAddress4.Text = LStrArrayIPs[3];
                return true;
            }
            return false;
        }

    }
}
