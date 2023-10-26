using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Web.Editors.ASPx;
using System;
using DevExpress.ExpressApp.Model;
using System.Web.UI.WebControls;
using DevExpress.Web;

namespace BSI_PR.Module.Web.Editors
{
  
        [PropertyEditor(typeof(int), true)]
        public class MyIntPropertyEditor : ASPxIntPropertyEditor
        {
            public MyIntPropertyEditor(Type objectType, IModelMemberViewItem model) : base(objectType, model) { }

            protected override void SetupControl(WebControl control)
            {
                base.SetupControl(control);
                var spinEditor = control as ASPxSpinEdit;
                if (spinEditor == null) return;
                spinEditor.SpinButtons.ShowIncrementButtons = false;
                spinEditor.AllowMouseWheel = false;
                spinEditor.SelectInputTextOnClick = true;
            }
        }
        [PropertyEditor(typeof(decimal), true)]
        public class MyDecPropertyEditor : ASPxDecimalPropertyEditor
        {
            public MyDecPropertyEditor(Type objectType, IModelMemberViewItem model) : base(objectType, model) { }

            protected override void SetupControl(WebControl control)
            {
                base.SetupControl(control);
                var spinEditor = control as ASPxSpinEdit;
                if (spinEditor == null) return;
                spinEditor.SpinButtons.ShowIncrementButtons = false;
                spinEditor.AllowMouseWheel = false;
                spinEditor.SelectInputTextOnClick = true;

            }
        }
        [PropertyEditor(typeof(double), true)]
        public class MyDouPropertyEditor : ASPxDecimalPropertyEditor
        {
            public MyDouPropertyEditor(Type objectType, IModelMemberViewItem model) : base(objectType, model) { }

            protected override void SetupControl(WebControl control)
            {
                base.SetupControl(control);
                var spinEditor = control as ASPxSpinEdit;
                if (spinEditor == null) return;
                spinEditor.SpinButtons.ShowIncrementButtons = false;
                spinEditor.AllowMouseWheel = false;
                spinEditor.SelectInputTextOnClick = true;

            }
        }

        //[PropertyEditor(typeof(decimal), "VPDec", false)]
        //public class MyDecPropertyEditorVP : ASPxDecimalPropertyEditor
        //{
        //    public MyDecPropertyEditorVP(Type objectType, IModelMemberViewItem model) : base(objectType, model)
        //    { }

        //    protected override void SetupControl(WebControl control)
        //    {
        //        base.SetupControl(control);
        //        var spinEditor = control as ASPxSpinEdit;
        //        if (spinEditor == null) return;
        //        spinEditor.SpinButtons.ShowIncrementButtons = false;
        //        spinEditor.AllowMouseWheel = false;
        //        spinEditor.SelectInputTextOnClick = true;

        //    }
        //}

        //[PropertyEditor(typeof(double), "VPDou", false)]
        //public class MyDouPropertyEditorVP : ASPxDecimalPropertyEditor
        //{
        //    public MyDouPropertyEditorVP(Type objectType, IModelMemberViewItem model) : base(objectType, model)
        //    { }

        //    protected override void SetupControl(WebControl control)
        //    {
        //        base.SetupControl(control);
        //        var spinEditor = control as ASPxSpinEdit;
        //        if (spinEditor == null) return;
        //        spinEditor.SpinButtons.ShowIncrementButtons = false;
        //        spinEditor.AllowMouseWheel = false;
        //        spinEditor.SelectInputTextOnClick = true;

        //    }
        //}
    
}
