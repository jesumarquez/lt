<%@ Control Language="C#" AutoEventWireup="True" CodeBehind="Menu.ascx.cs" Inherits="Logictracker.App_Controls.Menu" %>
<asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
<ContentTemplate>
    
<div class="menu_container">
    <div style="position: relative;">
        <asp:TextBox runat="server" id="txtSearchMenu" CssClass="menu_search"></asp:TextBox>
        <div id="search_clear" class="menu_search_clear"></div>
        <cwc:ResourceWatermarkExtender runat="server" BehaviorID="txtSearchMenuWatermark" TargetControlID="txtSearchMenu" WatermarkCssClass="menu_search_watermarked" ResourceName="Labels" VariableName="BUSCAR_EN_MENU" />
    </div>
    <div id="search_results_container" class="menu_group" style="display: none;">
        <div id="search_results" runat="server" class="menu_subgroup">
            <div id="search_results_none" class="menu_search_no_results"><cwc:ResourceLabel runat="server" ResourceName="Labels" VariableName="MENU_SEARCH_NO_RESULTS"></cwc:ResourceLabel></div>

            
        </div>
    </div>
<div id="standard_menu">
    <AjaxToolkit:Accordion ID="accordion" runat="server" SkinID="Menu">
        <Panes>
            <AjaxToolkit:AccordionPane runat="server" ID="acpPanel" Visible="false" SkinID="Menu">
                <Header><asp:Label ID="lblHeader" runat="server"></asp:Label></Header>
                <Content>
                <asp:Panel ID="panelContent" runat="server"></asp:Panel>
                </Content>
            </AjaxToolkit:AccordionPane>
        </Panes>
    </AjaxToolkit:Accordion>
</div>

</div>   

<script type="text/javascript">
    $addHandler($get('<%=txtSearchMenu.ClientID %>'), 'keyup', search_menu);
    $addHandler($get('search_clear'), 'click', clear);
    $addHandler(document, 'keypress', onkey);
    
    function search_menu() {
        var txtSearchMenu = $get('<%=txtSearchMenu.ClientID %>');
        var search_results = $get('<%=search_results.ClientID %>');
        var search_results_container = $get('search_results_container');
        var standard_menu = $get('standard_menu');
        var search_clear = $get('search_clear');
        var text = RemoverAcentos(txtSearchMenu.value);
        if (text.length > 0) {
            var modules = search_results.getElementsByTagName('div');
            var matchCount = 0;
            for (var i = 0; i < modules.length; i++) {
                var module = modules[i];
                var title = module.getAttribute('title');
                if(!title) continue;
                var match = RemoverAcentos(title).indexOf(text) > -1;
                module.style.display = match ? '' : 'none';
                if (match) matchCount++;
            }
            $get('search_results_none').style.display = matchCount > 0 ? 'none' : '';
            search_results_container.style.display = '';
            standard_menu.style.display = 'none';
            search_clear.className = 'menu_search_clear_active';
        } else {
            search_results_container.style.display = 'none';
            standard_menu.style.display = '';
            search_clear.className = 'menu_search_clear';
        }
    }
    function clear() {
        var txtSearchMenu = $get('<%=txtSearchMenu.ClientID %>');
        txtSearchMenu.value = '';
        search_menu();
        var wm = $find('txtSearchMenuWatermark');
        wm._applyWatermark();
        txtSearchMenu.blur();
    }
    function RemoverAcentos(text) {
        text = text.toLowerCase();
        var text2 = '';
        for(var i = 0; i < text.length; i++)
        {
            var current = text.charAt(i);
            switch (current)
            {
            		case 'á':
            		case 'ä':
            		case 'à':
            		case 'ã': text2+='a'; break;
            		case 'é':
            		case 'ë':
            		case 'è': text2+='e'; break;
            		case 'í':
            		case 'ï':
            		case 'ì': text2+='i'; break;
            		case 'ó':
            		case 'ö':
            		case 'ò': text2+='o'; break;
            		case 'ú':
            		case 'ù':
            		case 'ü': text2+='u'; break;
            		case 'ÿ':
            		case 'ý': text2+='y'; break;
            		default:  text2+=current; break;
            	}
            }
            return text2;
        }

    function onkey(e) {
        if (e.ctrlKey && String.fromCharCode(e.charCode) == 'b') {
            e.rawEvent.preventDefault();
            var txtSearchMenu = $get('<%=txtSearchMenu.ClientID %>');
            txtSearchMenu.focus();
        } else if (e.charCode == 27) {
            e.rawEvent.preventDefault();
            var txtSearchMenu = $get('<%=txtSearchMenu.ClientID %>');
            if (document.activeElement == txtSearchMenu) {
                e.rawEvent.preventDefault();
                clear();
            }
        }
        else if (e.charCode == 13) {
            var txtSearchMenu = $get('<%=txtSearchMenu.ClientID %>');
            if (document.activeElement == txtSearchMenu) {
                e.rawEvent.preventDefault();
            }
        }
    }

</script>

</ContentTemplate>
</asp:UpdatePanel>

