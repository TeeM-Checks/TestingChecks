<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ModelPopupRelation.ascx.cs" Inherits="NewBilletterie.Controls.ModelPopupRelation" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<script language="javascript" type="text/javascript">
    //The endRequest event is raised after an asynchronous postback is finished and control has been returned to the browser. 
    Sys.WebForms.PageRequestManager.getInstance().add_endRequest(EndRequestHandler<%=FunctionUniqueName %>);//Set The name of the handler function that will be called.
    function EndRequestHandler<%=FunctionUniqueName %>(sender, args)
    {
        if (args.get_error() == undefined)
        {              
            var Start = <%=Start.ToString().ToLower() %>;     
            if(Start == false)return;
            CreateRelation<%=FunctionUniqueName %>('<%=ParentModelPopupID  %>', '<%=ChildModelPopupID %>');
     }
 }   
   
 //Set z-Index of the Backside(Model popup) and front Side (model popup) 
 function    CreateRelation<%=FunctionUniqueName %>(ParentModelPopupID, ChildModelPopupID)
    {
        try
        {
            var ParentModelPopup = Get_PopUpContrl<%=FunctionUniqueName %>(ParentModelPopupID);
            var ChildModelPopup = Get_PopUpContrl<%=FunctionUniqueName %>(ChildModelPopupID);           
            if(ParentModelPopup != null && ChildModelPopup != null)
            {        
                /* Note:         
                _backgroundElement = the div html control that covers the back side controls of the model popup
                Example: <div style="left: 0px; top: 0px; width: 1920px; height: 455px; position: fixed; z-index: 10000;" id="mpeFirstDialogBox_backgroundElement" class="ModalPoupBackgroundCssClass"></div>
                _foregroundElement = the div html control that shows the details of the popup. Server side you can see this  as a property of ModalPopupExtender "ModalPopupExtender.PopupControlID" 
                Example: <div id="pnlFirstDialogBox" style="left: 624px; top: 27.5px; width: 35%; height: 400px; position: fixed; z-index: 10001; background-color: white;">[includes detail part as you specified]</div>
                */
                //get zIndex of the parent modelpopup control's _backgroundElement and set this zIndex to the child model popup control's _backgroundElement  with increment on 1
                ChildModelPopup._backgroundElement.style.zIndex = ParentModelPopup._backgroundElement.style.zIndex + 1;
                //get zIndex of the parent modelpopup control's _foregroundElement and set this zIndex to the child model popup control's _foregroundElement  with increment on 2
                ChildModelPopup._foregroundElement.style.zIndex = ParentModelPopup._backgroundElement.style.zIndex + 2;                  
            }           
            //ChildModelPopup.show();  
        }
        catch (ex)
        {
            alert('Exception in CreateRelation<%=FunctionUniqueName %>:' + ex.message);
        }
    }
    //get the model popup
    function Get_PopUpContrl<%=FunctionUniqueName %>(PopupControlID)
    {
        if (PopupControlID == null) throw new Error(0, 'Wrong value of PopupControlID!');
        var ModelPopUp = $find(PopupControlID);
        if (ModelPopUp == null) throw new Error(0, 'Not Found: ' + PopupControlID + '!');         
        return ModelPopUp;
    }    
</script>
