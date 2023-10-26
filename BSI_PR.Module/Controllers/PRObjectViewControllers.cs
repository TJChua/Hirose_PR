using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Layout;
using DevExpress.ExpressApp.Model.NodeGenerators;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using BSI_PR.Module.BusinessObjects;

namespace BSI_PR.Module.Controllers
{
    // For more typical usage scenarios, be sure to check out https://documentation.devexpress.com/eXpressAppFramework/clsDevExpressExpressAppViewControllertopic.aspx.
    public partial class PRObjectViewControllers : ObjectViewController<ListView, PurchaseRequests>
    {
        GenControllers genCon;
        public PRObjectViewControllers()
        {
            InitializeComponent();
            // Target required Views (via the TargetXXX properties) and create their Actions.
        }
        protected override void OnActivated()
        {
            base.OnActivated();
            // Perform various tasks depending on the target View.
            NewObjectViewController controller = Frame.GetController<NewObjectViewController>();
            if (controller != null)
            {
                //controller.NewObjectAction.Execute += NewObjectAction_Execute;
                controller.ObjectCreated += Controller_ObjectCreated;
            }
        }

        private void Controller_ObjectCreated(object sender, ObjectCreatedEventArgs e)
        {
        //    if (e.CreatedObject is PurchaseRequests)
        //    {
        //        PurchaseRequests currentObject = (PurchaseRequests)e.CreatedObject;

        //        if (View.Id.Contains("Gift"))
        //        {
        //            currentObject.DocType = currentObject.Session.FindObject<DocTypes>(new BinaryOperator("BoCode", GeneralSettings.GIFT, BinaryOperatorType.Equal));
        //        }
        //        else if (View.Id.Contains("Travel"))
        //        {
        //            currentObject.DocType = currentObject.Session.FindObject<DocTypes>(new BinaryOperator("BoCode", GeneralSettings.TRAVEL, BinaryOperatorType.Equal));
        //        }

        //    }

        }

        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();
            // Access and customize the target View control.
            genCon = Frame.GetController<GenControllers>();
        }
        protected override void OnDeactivated()
        {
            NewObjectViewController controller = Frame.GetController<NewObjectViewController>();
            if (controller != null)
            {
                //controller.NewObjectAction.Execute -= NewObjectAction_Execute;
                controller.ObjectCreated -= Controller_ObjectCreated;
            }
            // Unsubscribe from previously subscribed events and release other references and resources.
            base.OnDeactivated();
        }
    }
}
