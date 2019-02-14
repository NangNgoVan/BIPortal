$(document).ready(
    function () {

        $(".tree-checkbox-hierarchical").fancytree({
            checkbox: true,
            selectMode: 3,
            select: function (event, data) {
                // A node was activated: display its title:

                GetSelectedTreeNode("readdyTree", "StrAllowedMenus");
                //var node = data.node;
                //if (node.selected) {
                //    var LstSelectedMenu = $("#LstSelectedMenu").val();
                //    LstSelectedMenu += ","+ node.key;
                //    $("#LstSelectedMenu").val(LstSelectedMenu)
                //    alert(LstSelectedMenu);
                //}
                    //console.log(node.key);
                //else
                //    console.log("ko duowc chon");
            },

            
        });

        //Dropdown hover Menu
        //$('.dropdown').hover(function () {
        //    $(this).addClass('open')

        //}, function () {
        //    $(this).removeClass('open');
        //})

    
       
   }

);

function GetSelectedTreeNode(treeID,tagInput) {
   // alert(treeID);
    var tree = $("#" + treeID).fancytree("getTree");

    selNodes = tree.getSelectedNodes();
    var lsSelected = "";
    for (i = 0; i < selNodes.length; i++) {
        lsSelected += "," +selNodes[i].key;
    }
    if (lsSelected.length>0)
    lsSelected = lsSelected.substring(1);
    $("#" + tagInput).val(lsSelected);

}

/**
 * *************************************************************
 * *************************************************************
    VGS- CONFIG FUNCTION
 * *************************************************************
 * *************************************************************
 */
var lstNotYetVGS2Add = {};
function getSelectedNotYetProduct(source) {
    lstNotYetVGS2Add = {};
    $('#dvAjax').html('');
    var lstSelectedNotYet = [];

    var $lstChkbox = $('input[name=chknotyet]:checked');

    $lstChkbox.each(function () {
        //lstSelectedNotYet.push
        //alert($(this).val());
        notYetProduct = source[$(this).val()];
        lstSelectedNotYet.push(notYetProduct);

        lstNotYetVGS2Add[$(this).val()] = {
            'ID': '', 'OrgID': '', 'ProductBusinessKey': $(this).val(), 'Sequence': '1',
            'ProductTypeCode': '', 'IsCombo': '', 'IsUsage': '', 'LocationBusinessKey': '',
            'FromDate': '', 'ToDate': '', 'Percent': ''

        };
        //console.log(lstSelectedNotYet);
    });
    console.log("-------------------------------loading form to edit data -------------------------");
    console.log(lstNotYetVGS2Add);
    $.ajax({
        type: "POST",
        url: "/ConfigTemplate/ConfigNotYetVGSAjax",
        data: {
            lst: lstSelectedNotYet
        },
        datatype: "html",
        contentType: 'application/x-www-form-urlencoded',
        success: function (output) {
            $('#dvAjax').html(output);
            $(".FromDate").datepicker({ dateFormat: 'yy/mm/dd' });
            $(".ToDate").datepicker({ dateFormat: 'yy/mm/dd' });



            $(document).on("click", "#btnSaveNotYet", function () {
                saveNotYetVGS();
            });
            //$('#btnSaveNotYet').click(function () {
            //    saveNotYetVGS();
            //});
        },
        error: function () {
            alert("Error occured!!")
        }
    });

    $('#dvAjax').dialog({
        title: '',
        show: true,
        modal: true,
        width: 1500,
        height: 500,

    });
    console.log("-------------------------------END loading form to edit data -------------------------");

}

function saveNotYetVGS() {
    //lstNotYetVGS2Add = {};
    var lstNotYetVGS2AddSubmit = [];

    $('#dvAjax tr ').each(function () {
        //console.log(this.id);
        //console.log(lstNotYetVGS2Add);

        if (this.id) {
            var id = this.id;
            // console.log(id);
            //console.log($(this).find('#ProductType').val());
            //console.log(lstNotYetVGS2Add[id]['ProductBusinessKey']);
            lstNotYetVGS2Add[id]['ProductTypeCode'] = $(this).find('#ProductType').val();
            lstNotYetVGS2Add[id]['IsCombo'] = $(this).find('#IsCombo').val();
            lstNotYetVGS2Add[id]['IsUsage'] = $(this).find('#IsUsage').val();
            lstNotYetVGS2Add[id]['LocationBusinessKey'] = $(this).find('#LocationBusinessKey').val();
            lstNotYetVGS2Add[id]['FromDate'] = $(this).find('.FromDate').val();
            lstNotYetVGS2Add[id]['ToDate'] = $(this).find('.ToDate').val();
            lstNotYetVGS2Add[id]['Percent'] = $(this).find('.Percent').val();

            lstNotYetVGS2AddSubmit.push(lstNotYetVGS2Add[id]);
        }
        // console.log(lstNotYetVGS2Add[id]);


    });

    console.log('-------------------------------------SAVEDATA--------------------')
    console.log(lstNotYetVGS2AddSubmit);
    $.ajax({
        type: "POST",
        url: "/ConfigTemplate/ConfigNotYetVGSSubmit",
        data: {
            lst: lstNotYetVGS2AddSubmit
        },
        datatype: "html",
        contentType: 'application/x-www-form-urlencoded',
        success: function (output) {
            $('#dvAjax').html(output);

            // console.log(lstNotYetVGS2Add);

        },
        error: function () {
            alert("Error occured!!")
        }
    });
    console.log('-------------------------------------END SAVEDATA--------------------');

    location.reload();
}


/**
 * *************************************************************
 * *************************************************************
    VGS- CONFIG FUNCTION ----EDIT THE CONFIG VGS
 * *************************************************************
 * *************************************************************
 */


$(document).ready(function () {
    $('.vsg_config_row').hover(function () {

        var id = this.id;
        //console.log($(this).attr("productkey"));
        $(this).addClass("vgs_hovered");

        //$(this).append(str_popup);

        showVGSMenuConfig(id, $(this).attr("productkey"));
        //console.log($(this).position());
        //console.log(this.className);
        $('.vsg_config_row').each(function () {
            if (this.id != id) $(this).removeClass("vgs_hovered");
            // $(this).remove('#vgs_row_edit');
        });

    });


    $('#vgs_edit_submit').click(function () {
        showPopupEditVGS();
    });
});
function showVGSMenuConfig(configID, productKey) {
    var position = $('#' + configID).offset();


    var width = $('#' + configID).width() / 3;
    $('#vgs_row_edit').removeClass('vgs_row_edit_hidden');

    $('#vgs_row_edit input[name="config_id"] ').val(configID);
    $('#vgs_row_edit input[name="config_productkey"] ').val(productKey);

    //console.log("before:");
    //console.log($('#vgs_row_edit').offset());
    $('#vgs_row_edit').offset({ top: position.top + 5, left: position.left + width });

    //console.log("alfer:");
    //console.log($('#vgs_row_edit').offset());
    $('#vgs_row_edit').addClass('vgs_row_edit_show');


    //$('#vgs_row_edit').dialog({
    //    resizable: false,
    //    height: "auto",
    //    width: width / 2,
    //    modal: true,
    //    position: {
    //        top: position.top,
    //        left: position.left + width /2
    //    }
    //});
}

function showPopupEditVGS() {
    $('#dvAjax').html('');
    configID = $('#vgs_row_edit input[name="config_id"] ').val();
    productKey = $('#vgs_row_edit input[name="config_productkey"] ').val();
    action = $('#vgs_row_edit input[name="vgs_row_edit"]:checked ').val();



    console.log('-------------action---------------');
    console.log(action);
    $.ajax({
        type: "POST",
        url: "/ConfigTemplate/VGSEditConfigForm",
        data: {
            ConfigID: configID,
            ProductKey: productKey,
            Action: action
        },
        datatype: "html",
        contentType: 'application/x-www-form-urlencoded',
        success: function (output) {
            $('#dvAjax').html(output);
            $(".FromDate").datepicker({ dateFormat: 'yy/mm/dd' });
            $(".ToDate").datepicker({ dateFormat: 'yy/mm/dd' });


            $(document).on("click", ".vgs_td_delete", function (e) {

                $(this).closest('tr').remove();
            });
            $(document).on("dblclick", "#vgs_clone_row", function (e) {
                e.stopImmediatePropagation();
                e.preventDefault();
                addNewTableRow();

            });
            $(document).on("click", "#btnSave", function (e) {
                e.stopImmediatePropagation();
                e.preventDefault();
                saveEditableSelectedVGS();

            });
            // console.log(lstNotYetVGS2Add);

        },
        error: function () {
            alert("Error occured!!")
        }
    });

    $('#dvAjax_container').dialog({
        title: '',
        show: true,
        modal: true,
        width: 1500,
        height: 500,

    });
}


/*********************************************************************************************************/
function saveEditableSelectedVGS() {
    var lstEditableSelectedVGS = [];

    $('.vgs_selected_edit').each(function () {
        lstEditableSelectedVGS.push({
            'ID': $(this).attr('config_id'),
            'OrgID': '',
            'ProductBusinessKey': $(this).attr('productbusinesskey'),
            'Sequence': $(this).attr('sequence'),
            'ProductTypeCode': $(this).find('#ProductType').val(),
            'IsCombo': $(this).find('#IsCombo').val(),
            'IsUsage': $(this).find('#IsUsage').val(),
            'LocationBusinessKey': $(this).find('#LocationBusinessKey').val(),
            'FromDate': $(this).find('.FromDate').val(),
            'ToDate': $(this).find('.ToDate').val(),
            'Percent': $(this).find('.Percent').val()

        });
    });

    console.log('-------------------------------------SAVEDATA saveEditableSelectedVGS--------------------')
    console.log(lstEditableSelectedVGS);
    $.ajax({
        type: "POST",
        url: "/ConfigTemplate/ConfigNotYetVGSSubmit",
        data: {
            lst: lstEditableSelectedVGS
        },
        datatype: "html",
        contentType: 'application/x-www-form-urlencoded',
        beforeSend: function () {

            loading();
        },
        complete: function () {
            unloading();
        },
        success: function (output) {
            $('#dvAjax').html(output);

            // console.log(lstNotYetVGS2Add);

        },
        error: function () {
            alert("Error occured!!")
        }
    });
    console.log('-------------------------------------END SAVEDATA saveEditableSelectedVGS--------------------');
}



/*********************ADD A NEW ROW TO THE TABLE**********************************************************/

function addNewTableRow() {
    var count = $(".vgs_tr_0").length;
    console.log('count=' + count);
    var index = 0;
    var added = false;
    $(".vgs_tr_0").each(function () {
        index++;
        console.log('index=' + index);
        if (index == count && added == false) {
            added = true;
            var r = confirm("Ban co muon them mot dong khong?");
            if (r == true) {
                var currentRow = $(this).clone();
                var tdindex = parseInt(currentRow.find('.vga_td_index').html());
                currentRow.find('.vga_td_index').html(tdindex + 1);
                var fromdate_id = currentRow.find('.FromDate').attr('id') + Math.floor((Math.random() * 10000) + 1);;

                currentRow.find('.FromDate').attr('id', 'fromdatehtml_' + fromdate_id);
                currentRow.insertAfter($(this));

                $('.FromDate').removeClass('hasDatepicker').datepicker({ dateFormat: 'yy/mm/dd' });

                //$(document).on('focus', 'fromdatehtml_' + fromdate_id, function () {
                //    $(this).datepicker({ dateFormat: 'yy/mm/dd' });
                //});



            }

        }

    })

}


function loading() {
    $('#loader').addClass('loading');
    $('#loader').removeClass('unloading');
}

function unloading() {
    $('#loader').removeClass('loading');
    $('#loader').addClass('unloading');
}
/*********************end:  ADD A NEW ROW TO THE TABLE**********************************************************/
