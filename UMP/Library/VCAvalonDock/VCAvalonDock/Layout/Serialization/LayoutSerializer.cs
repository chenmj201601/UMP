//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    f5aaa145-fc9b-4c8d-960b-fdec1a395e9b
//        CLR Version:              4.0.30319.18444
//        Name:                     LayoutSerializer
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.Wpf.AvalonDock.Layout.Serialization
//        File Name:                LayoutSerializer
//
//        created by Charley at 2014/7/22 9:35:29
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VoiceCyber.Wpf.AvalonDock.Layout.Serialization
{
    public abstract class LayoutSerializer
    {
        DockingManager _manager;

        public LayoutSerializer(DockingManager manager)
        {
            if (manager == null)
                throw new ArgumentNullException("manager");

            _manager = manager;
            _previousAnchorables = _manager.Layout.Descendents().OfType<LayoutAnchorable>().ToArray();
            _previousDocuments = _manager.Layout.Descendents().OfType<LayoutDocument>().ToArray();
        }

        LayoutAnchorable[] _previousAnchorables = null;
        LayoutDocument[] _previousDocuments = null;

        public DockingManager Manager
        {
            get { return _manager; }
        }

        public event EventHandler<LayoutSerializationCallbackEventArgs> LayoutSerializationCallback;

        protected virtual void FixupLayout(LayoutRoot layout)
        {
            //fix container panes
            foreach (var lcToAttach in layout.Descendents().OfType<ILayoutPreviousContainer>().Where(lc => lc.PreviousContainerId != null))
            {
                var paneContainerToAttach = layout.Descendents().OfType<ILayoutPaneSerializable>().FirstOrDefault(lps => lps.Id == lcToAttach.PreviousContainerId);
                if (paneContainerToAttach == null)
                    throw new ArgumentException(string.Format("Unable to find a pane with id ='{0}'", lcToAttach.PreviousContainerId));

                lcToAttach.PreviousContainer = paneContainerToAttach as ILayoutContainer;
            }


            //now fix the content of the layoutcontents
            foreach (var lcToFix in layout.Descendents().OfType<LayoutAnchorable>().Where(lc => lc.Content == null).ToArray())
            {
                LayoutAnchorable previousAchorable = null;
                if (lcToFix.ContentId != null)
                {
                    //try find the content in replaced layout
                    previousAchorable = _previousAnchorables.FirstOrDefault(a => a.ContentId == lcToFix.ContentId);
                }

                if (LayoutSerializationCallback != null)
                {
                    var args = new LayoutSerializationCallbackEventArgs(lcToFix, previousAchorable != null ? previousAchorable.Content : null);
                    LayoutSerializationCallback(this, args);
                    if (args.Cancel)
                        lcToFix.Close();
                    else if (args.Content != null)
                        lcToFix.Content = args.Content;
                    else if (args.Model.Content != null)
                        lcToFix.Hide(false);
                }
                else if (previousAchorable == null)
                    lcToFix.Hide(false);
                else
                {
                    lcToFix.Content = previousAchorable.Content;
                    lcToFix.IconSource = previousAchorable.IconSource;
                }
            }


            foreach (var lcToFix in layout.Descendents().OfType<LayoutDocument>().Where(lc => lc.Content == null).ToArray())
            {
                LayoutDocument previousDocument = null;
                if (lcToFix.ContentId != null)
                {
                    //try find the content in replaced layout
                    previousDocument = _previousDocuments.FirstOrDefault(a => a.ContentId == lcToFix.ContentId);
                }

                if (LayoutSerializationCallback != null)
                {
                    var args = new LayoutSerializationCallbackEventArgs(lcToFix, previousDocument != null ? previousDocument.Content : null);
                    LayoutSerializationCallback(this, args);

                    if (args.Cancel)
                        lcToFix.Close();
                    else if (args.Content != null)
                        lcToFix.Content = args.Content;
                    else if (args.Model.Content != null)
                        lcToFix.Close();
                }
                else if (previousDocument == null)
                    lcToFix.Close();
                else
                    lcToFix.Content = previousDocument.Content;
            }


            layout.CollectGarbage();
        }

        protected void StartDeserialization()
        {
            Manager.SuspendDocumentsSourceBinding = true;
            Manager.SuspendAnchorablesSourceBinding = true;
        }

        protected void EndDeserialization()
        {
            Manager.SuspendDocumentsSourceBinding = false;
            Manager.SuspendAnchorablesSourceBinding = false;
        }
    }
}
