using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Reporting.WinForms;

namespace Common61011
{
    public class ReportViewerMessagesZhcn : IReportViewerMessages
    {
        public string BackButtonT;
        public string CurrentPageT;
        public string ExportButtonT;
        public string FindButtonT;
        public string FindNextButtonT;
        public string FirstPageButtonT;
        public string LastPageButtonT;
        public string NextPageButtonT;
        public string RefreshButtonT;
        public string SearchTextBoxT;
        public string PreviousPageButtonT;
        public string PrintButtonT;
        public string ZoomControlT;
        public string PageWidth;
        public string WholePage;
        public string TotalPagesT;
        public string StopButtonT;
        public string PrintLayoutButtonT;
        public string PageSetupButtonT;
        public string FindButton;
        public string FindNextButton;
        public string ExportButton;

        #region   IReportViewerMessages   Members
        public string BackButtonToolTip
        {
            get { return (BackButtonT); }
        }
        public string CurrentPageTextBoxToolTip
        {
            get { return (CurrentPageT); }
        }
        public string ExportButtonText
        {
            get { return (ExportButtonT); }
        }
        public string ExportButtonToolTip
        {
            get { return (ExportButtonT); }
        }
        public string FindButtonText
        {
            get { return (FindButton); }
        }
        public string FindButtonToolTip
        {
            get { return (FindButtonT); }
        }
        public string FindNextButtonText
        {
            get { return (FindNextButton); }
        }
        public string FindNextButtonToolTip
        {
            get { return (FindNextButtonT); }
        }
        public string FirstPageButtonToolTip
        {
            get { return (FirstPageButtonT); }
        }
        public string LastPageButtonToolTip
        {
            get { return (LastPageButtonT); }
        }
        public string NextPageButtonToolTip
        {
            get { return (NextPageButtonT); }
        }
        public string PageOf
        {
            get { return ("/ "); }
        }
        public string RefreshButtonToolTip
        {
            get { return (RefreshButtonT); }
        }
        public string SearchTextBoxToolTip
        {
            get { return (SearchTextBoxT); }
        }
        public string PreviousPageButtonToolTip
        {
            get { return (PreviousPageButtonT); }
        }
        public string PrintButtonToolTip
        {
            get { return (PrintButtonT); }
        }
        public string ZoomControlToolTip
        {
            get { return (ZoomControlT); }
        }
        public string ZoomToPageWidth
        {
            get { return (PageWidth); }
        }
        public string ZoomToWholePage
        {
            get { return (WholePage); }
        }
        public string TotalPagesToolTip
        {
            get { return (TotalPagesT); }
        }
        public string StopButtonToolTip
        {
            get { return (StopButtonT); }
        }
        public string PrintLayoutButtonToolTip
        {
            get { return (PrintLayoutButtonT); }
        }
        public string PageSetupButtonToolTip
        {
            get { return (PageSetupButtonT); }
        }



        public string ChangeCredentialsToolTip
        {
            get { return ("ChangeCredentialsToolTip. "); }
        }      
        public string DocumentMap
        {
            get { return ("文档视图 "); }
        }
        public string DocumentMapButtonToolTip
        {
            get { return ("文档视图. "); }
        }
        public string ExportFormatsToolTip
        {
            get { return ("选择格式. "); }
        }
        public string FalseValueText
        {
            get { return ("不正确的值. "); }
        }  
        public string InvalidPageNumber
        {
            get { return ("页面数不对 "); }
        }     
        public string NoMoreMatches
        {
            get { return ("无匹配项 "); }
        }
        public string NullCheckBoxText
        {
            get { return ("空值 "); }
        }
        public string NullValueText
        {
            get { return ("空值 "); }
        }    
        public string ParameterAreaButtonToolTip
        {
            get { return ("参数设置 "); }
        }
        public string PasswordPrompt
        {
            get { return ("PasswordPrompt "); }
        }   
        public string ProgressText
        {
            get { return ("...... "); }
        }   
        public string SelectAValue
        {
            get { return ("SelectAValue "); }
        }
        public string SelectAll
        {
            get { return ("全选 "); }
        }
        public string SelectFormat
        {
            get { return ("选择格式 "); }
        }
        public string TextNotFound
        {
            get { return ("未找到 "); }
        }
        public string TodayIs
        {
            get { return ("TodayIs "); }
        }
        public string TrueValueText
        {
            get { return ("TrueValueText "); }
        }
        public string UserNamePrompt
        {
            get { return ("UserNamePrompt "); }
        }
        public string ViewReportButtonText
        {
            get { return ("查看报表 "); }
        }
        public string ViewReportButtonToolTip
        {
            get { return ("ViewReportButtonToolTip "); }
        }

        public string ZoomMenuItemText
        {
            get { return (ZoomControlT); }
        }
        public string RefreshMenuItemText
        {
            get { return (RefreshButtonT); }
        }
        public string StopMenuItemText
        {
            get { return (StopButtonT); }
        }
        public string PrintMenuItemText
        {
            get { return (PrintButtonT); }
        }
        public string BackMenuItemText
        {
            get { return (BackButtonT); }
        }
        public string ExportMenuItemText
        {
            get { return (ExportButtonT); }
        }
    
        public string DocumentMapMenuItemText
        {
            get { return (PageWidth); }
        }
        public string PrintLayoutMenuItemText
        {
            get { return (PrintLayoutButtonT); }
        }    

        public string PageSetupMenuItemText
        {
            get { return (PageSetupButtonT); }
        }
        public string NullCheckBoxToolTip
        {
            get { return ("NullCheckBoxToolTip "); }
        }
        public string ChangeCredentialsText
        {
            get { return ("更改 "); }
        }
        #endregion
    }
}
