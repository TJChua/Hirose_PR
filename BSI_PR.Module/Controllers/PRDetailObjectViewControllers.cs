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
using DevExpress.Xpo;

namespace BSI_PR.Module.Controllers
{
    // For more typical usage scenarios, be sure to check out https://documentation.devexpress.com/eXpressAppFramework/clsDevExpressExpressAppViewControllertopic.aspx.

    public partial class PRDetailObjectViewControllers : ObjectViewController<ListView, PurchaseRequestDetails>
    {
        private object e;

        public PRDetailObjectViewControllers()
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


            //IObjectSpace os = Application.CreateObjectSpace();
            //PurchaseRequests findobj = os.FindObject<PurchaseRequests>(CriteriaOperator.Parse("Oid", Oid);




        }
        private void Controller_ObjectCreated(object sender, ObjectCreatedEventArgs e)
        {
            if (e.CreatedObject is PurchaseRequestDetails && View.IsRoot == false)
            {
                PurchaseRequestDetails currentObject = (PurchaseRequestDetails)e.CreatedObject;

                ListView lv = ((ListView)View);
                if (lv.CollectionSource is PropertyCollectionSource)
                {
                    PropertyCollectionSource collectionSource = (PropertyCollectionSource)lv.CollectionSource;
                    if (collectionSource.MasterObject != null)
                    {

                        if (collectionSource.MasterObjectType == typeof(PurchaseRequests))
                        {

                            PurchaseRequests masterobject = (PurchaseRequests)collectionSource.MasterObject;
                            if (masterobject.DocType != null)
                                currentObject.DocType = currentObject.Session.FindObject<DocTypes>(new BinaryOperator("BoCode", masterobject.DocType.BoCode, BinaryOperatorType.Equal));
                        }
                    }
                }

            }


        }
        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();
            // Access and customize the target View control.
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
