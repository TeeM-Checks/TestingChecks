using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NewBilletterie.Controls
{
    public partial class ModelPopupRelation : System.Web.UI.UserControl
    {
        //protected void Page_Load(object sender, EventArgs e)
        //{

        //}

        /// <summary>
        /// Gets or sets the parent model popup ID.
        /// </summary>
        /// <value>The parent model popup ID.</value>
        public String ParentModelPopupID
        {
            get { return _ParentModelPopupID; }
            set { _ParentModelPopupID = value; }
        }private String _ParentModelPopupID = String.Empty;

        /// <summary>
        /// Gets or sets the child model popup ID.
        /// </summary>
        /// <value>The child model popup ID.</value>
        public String ChildModelPopupID
        {
            get { return _ChildModelPopupID; }
            set { _ChildModelPopupID = value; }
        }private String _ChildModelPopupID = String.Empty;


        /// <summary>
        /// Gets the name of the function unique.
        /// </summary>
        /// <value>The name of the function unique.</value>
        public String FunctionUniqueName
        {
            get { return _FunctionUniqueName; }
        }private String _FunctionUniqueName = String.Empty;


        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="ModelPopupRelation"/> is start.
        /// </summary>
        /// <value><c>true</c> if start; otherwise, <c>false</c>.</value>
        public Boolean Start
        {
            get { return _Start; }
            set { _Start = value; }
        }private Boolean _Start = true;

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            //if parent and child popup name  are not provided set control to not work.
            if (ParentModelPopupID.Trim().Length == 0 || ChildModelPopupID.Trim().Length == 0)
            {
                Start = false;
            }
            else
            {
                /*
                 * Prepare Variable to make all the javascript functions unique. 
                 * if we use multiple modelPopupRelation because 
                 * It will generate multiple <script></script> tages with same name functions
                 */
                _FunctionUniqueName = String.Format("_{0}_{1}_{2}", ParentModelPopupID, ChildModelPopupID, Guid.NewGuid().ToString().Substring(0, 8).ToString());
            }
        }


        /// <summary>
        /// Gets the control client ID.
        /// </summary>
        /// <param name="modelPopupDialogBoxID">The model popup dialog box ID.</param>
        /// <returns></returns>
        private String GetControlClientID(String modelPopupDialogBoxID)
        {
            AjaxControlToolkit.ModalPopupExtender _ModalPopupExtender = null;//keeps ModalPopupExtender 
            String ControlClientID = String.Empty;//keeps Control Client ID 

            Object mpe = ModalPopupExtenderControlSearch(this.Page.Form, modelPopupDialogBoxID);//searche the ModalPopupExtender
            if (mpe == null) throw new Exception("ModalPopupExtender Named " + modelPopupDialogBoxID + " not found!");//through exception if "ModalPopupExtender" not found
            _ModalPopupExtender = mpe as AjaxControlToolkit.ModalPopupExtender;
            if (_ModalPopupExtender.BehaviorID.Length == 0)//if BehaviorID is not specified
                ControlClientID = _ModalPopupExtender.ClientID;// In case of Client ID
            else
                ControlClientID = _ModalPopupExtender.BehaviorID;// In Case of Behavior ID
            return ControlClientID;

        }

        /// <summary>
        /// Modals the popup extender control search.
        /// </summary>
        /// <param name="_Control">The _ control.</param>
        /// <param name="ModalPopupExtenderID">The modal popup extender ID.</param>
        /// <returns></returns>
        private Control ModalPopupExtenderControlSearch(Control _Control, String ModalPopupExtenderID)
        {
            Stack<Control> controlStack = new Stack<Control>();//to keep record of all the controls for searching
            controlStack.Push(_Control);//add the control in the stack
            //loop through all the stack elements
            while (controlStack.Count > 0)
            {
                Control current_Control = controlStack.Pop();//get control to match the id
                if (current_Control.ID == ModalPopupExtenderID)
                    return current_Control;//return on match
                //loop through all the controls of the current control if any and add to control stack for further searching
                foreach (Control control in current_Control.Controls)
                {
                    controlStack.Push(control);
                }
            }
            return null;//return null if not match
        }
   
    




    }
}