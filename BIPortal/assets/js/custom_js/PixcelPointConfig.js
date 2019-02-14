
$(function () {
    var lastTD = null;
    var currentSeq = -1;
    var editOptions = [
        {
            column: 5,
            type: 'datepicker',
            format: 'dd/mm/yyyy',
            data: "FromDate"
        },

        {
            column: 3,
            type: 'select',
            //source: [
            //    { value: '1', display: 'có' },
            //    { value: '0', display: 'không' }
            //]
            source:
                $('#selectListIsUsage').data("selectconfig").map(x => {
                    var obj = {};
                    obj.value = x.ID;
                    obj.display = x.Status;
                    return obj;
                }),
            data: "IsRevenueOfOrg"
        }
        ,
        {
            column: 2,
            type: 'select',
            source:
            $('#listStationGroup').data("selectconfig").map(x => {
                var obj = {};
                obj.value = x.Code;
                obj.display = x.Name;
                return obj;
            }),
            data: "StationGroupCode"

        },
        {
            column: 4,
            type: 'select',
            source:
            $('#listJsonRegionName').data("selectconfig").map(x => {
                var region = {};
                region.value = x.BusinessKey;
                region.display = x.Name;
                return region;
            }),
            data: "RegionBusinessKey"
        },
        {
            column: 6,
            type: 'text',
            validate: function (value, text, row, tableData) {
                var sum = row.data();
                var regex = /^[-+]?[0-9]*\.?[0-9]+$/;

                if (parseFloat(value) > 100) {
                    alert("Giá trị tỷ lệ % phải nhỏ hơn hoặc bằng 100");
                    return false;
                }
                if (regex.test(value)) {
                    // text message
                }
                //if it's NOT valid
                else {
                    alert('Giá trị tỷ lệ phải là số !');
                    return false;
                }
                return true;
            }
        },
    ];
    var tableNeedConfig = $("#modal_info_need_config_table").DataTable({
        columns: [
            {
                data: null,
                defaultContent: '',
                className: '',
                orderable: false
            },
            { data: "Sequence" },
            { data: "StationGroup" },
            { data: "IsRevenueOfOrgText" },
            { data: "RegionName" },
            { data: "FromDate" },
            { data: "Value" }

        ],
        columnDefs: [
            {
                targets: [2, 3, 4, 5, 6],
                createdCell: function (td, cellData, rowData, row, col) {
                    $(td).attr("isedit", "1");
                }
            },
            {
                targets: [0],
                createdCell: function (td, cellData, rowData, row, col) {
                    if (rowData.hasOwnProperty("modal_is_new_sequence") && rowData["modal_is_new_sequence"]) {
                        var button = $('<a class="button button-small modal_table_cell_cancel" title="Hủy"></a>');
                        button.append($('<i class="fa fa-trash fa-lg"></i>'));
                        $(td).empty().append(button);
                        button.on('click', function (e) {
                            if ($(lastTD).parent('tr').get(0) === $(this).parent('td').parent('tr').get(0))
                                lastTD = null;
                            var rd = tableNeedConfig.row($(this).parent('td').parent('tr')).data();
                            var sequenceIndex = -1;
                            var datasrc = tableNeedConfig.columns().dataSrc();
                            for (var i = 0; i < datasrc.length; i++) {
                                if (datasrc[i] === "Sequence") {
                                    sequenceIndex = i;
                                }
                            }
                            var data = tableNeedConfig.rows().data();
                            for (var i = 0; i < data.length; i++) {
                                if (data[i].hasOwnProperty("modal_is_new_sequence")
                                    && data[i]["modal_is_new_sequence"]
                                    && data[i]["Sequence"] > rd["Sequence"]) {
                                    data[i]["Sequence"] = data[i]["Sequence"] - 1;
                                    if (sequenceIndex !== -1)
                                        tableNeedConfig.cell(i, sequenceIndex).data(data[i]["Sequence"]);
                                }
                            }
                            tableNeedConfig.row($(this).parent('td').parent('tr')).remove().draw();
                        });
                    }
                    else if (rowData.hasOwnProperty("model_is_edit") && rowData["model_is_edit"] && rowData.hasOwnProperty("modal_old_data")) {
                        var button = $('<a class="button button-small" title="Hủy"></a>');
                        button.append($('<i class="fa fa-remove fa-lg"></i>'));
                        $(td).empty().append(button);
                        button.on('click', function (e) {
                            if ($(lastTD).parent('tr').get(0) === $(this).parent('td').parent('tr').get(0))
                                lastTD = null;
                            var rd = tableNeedConfig.row($(this).parent('td').parent('tr')).data();
                            var old_data = JSON.parse(rd["modal_old_data"]);
                            tableListConfig.row.add(old_data);
                            tableNeedConfig.row($(this).parent('td').parent('tr')).remove().draw(false);
                            var dataListProduct = tableListConfig.rows().data();
                            for (var i = 0; i < dataListProduct.length; i++) {
                                dataListProduct[i]["modal_is_need_edit"] = true;
                            }
                            tableListConfig.draw();
                        });
                    }
                    if (tableNeedConfig.rows().data().length == 0) {
                        $("#modal_newProduct").off('click', onNewProduct);
                        $("#modal_newSequence").off('click', onNewSequence);
                        $("#modal_newSequence").on('click', onNewSequence);
                    }

                }
            }
        ]
    });
    var tableListConfig = $("#modal_list_config_table").DataTable({

        columns: [
            {
                data: null,
                orderable: false
            },
             { data: "Sequence" },
            { data: "StationGroup" },
            { data: "IsRevenueOfOrgText" },
            { data: "RegionName" },
            { data: "FromDate" },
            { data: "Value" }

        ],
        columnDefs: [
            {
                targets: [2, 3, 4, 5, 6],
                createdCell: function (td, cellData, rowData, row, col) {
                    $(td).attr("isedit", "1");
                }
            }
            //{
            //    targets: [0],
            //    createdCell: function (td, cellData, rowData, row, col) {
            //        var button = $('<a class="button button-small" title="Sửa"></a>');
            //        button.append($('<i class="fa fa-pencil fa-lg"></i>'));
            //        $(td).empty().append(button);
            //        button.on('click', function (e) {
            //            var rd = tableListConfig.row($(this).parent('td').parent('tr')).data();
            //            rd["model_is_edit"] = true;
            //            rd["modal_old_data"] = JSON.stringify(rd);
            //            tableNeedConfig.row.add(rd).draw(false);
            //            tableListConfig.row($(this).parent('td').parent('tr')).remove().draw(false);
            //        });
            //    }
            //}
        ],
        rowCallback: function (row, data, displayNum, displayIndex, dataIndex) {
            if (data.hasOwnProperty("modal_is_need_edit") && data["modal_is_need_edit"]) {
                var cell = row.cells[0];
                var button = $('<a class="button button-small" title="Sửa"></a>');
                button.append($('<i class="fa fa-pencil fa-lg"></i>'));
                $(cell).empty().append(button);
                button.on('click', function (e) {
                    //$.ajax({
                    //    method: "GET",
                    //    url: "/VGSConfigTemplate/GetInfoNeedProductConfig",
                    //    data: { productBusinessKey: businessKey, sequence: sequence },
                    //    success: (res) => {
                    //        //console.log(res)
                    //        if (res && res.data && res.data.length > 0) {
                    //            var product = res.data[0];
                    //            lastTD = null;
                    //            currentSeq = product.Sequence;
                    //            tableNeedConfig.clear().rows.add(res.data).draw();

                    //        }
                    //    }
                    //});

                    var rd = tableListConfig.row($(this).parent('td').parent('tr')).data();
                    var seq = rd["Sequence"];
                    var allData = tableListConfig.rows().data();
                    var listData = [];
                    var indexList = [];
                    for (var i = 0; i < allData.length; i++) {
                        if (allData[i]["Sequence"] === seq) {
                            indexList.push( i);
                            allData[i]["model_is_edit"] = true;
                            allData[i]["modal_old_data"] = JSON.stringify(allData[i]);
                            listData.push(allData[i]);
                        }
                    }
                    tableNeedConfig.rows.add(listData).draw(false);
                    for (var i = 0; i < indexList.length; i++) {
                        tableListConfig.row(indexList[i]).remove();
                    }
                    var dataListProduct = tableListConfig.rows().data();
                    for (var i = 0; i < dataListProduct.length; i++) {
                        delete dataListProduct[i].modal_is_need_edit;
                    }
                    tableListConfig.draw();
                    $("#modal_newProduct").on('click', onNewProduct);
                });
            }
            else {
                var cell = row.cells[0];
                $(cell).empty();
            }
        }
    });

    $('#modal_info_need_config_table').on('click', 'tbody td[isedit="1"]', function (e) {
        if (this === lastTD) return;
        //if (lastTD !== null) {
        //    var lastEdit = $(".td_edit", $(lastTD));
        //    var nodeName = lastEdit.prop('nodeName').toLowerCase();
        //    if (nodeName === 'select') {
        //        var lastRow = tableNeedConfig.row($(lastTD).parent('tr'));
        //        var lastCell = tableNeedConfig.cell(lastTD);

        //        OnLastEdit(lastEdit, lastRow, lastCell, editOptions, function (row, cell, option, text, value) {
        //            var rowData = row.data();
        //            if (option !== 'undefined' && option.type === 'select') {
        //                rowData[option.data] = value;
        //            }
                   
        //            cell.data(text);
                    
        //        });
        //    }
        //    else {
        //        tableNeedConfig.cell(lastEdit.parent('td')).data(lastEdit.val());
        //    }
        //    tableNeedConfig.cell(lastEdit.parent('td')).draw(false);
        //}
        if (lastTD !== null) {
            var lastEdit = $(".td_edit", $(lastTD));
            var nodeName = lastEdit.prop('nodeName').toLowerCase();
            var lastRow = tableNeedConfig.row($(lastTD).parent('tr'));
            var lastCell = tableNeedConfig.cell(lastTD);

            var x = OnLastEdit(lastEdit, lastRow, lastCell, editOptions, function (row, cell, option, text, value) {
                if (option.hasOwnProperty("validate") && option.validate(value, text, row, tableNeedConfig.rows().data())) {
                    var rowData = row.data();
                    if (option !== 'undefined' && option.type === 'select') {
                        rowData[option.data] = value;
                    }
                    //row.data(rowData);
                    cell.data(text);
                    tableNeedConfig.cell(lastEdit.parent('td')).draw(false);
                    return true;
                    //cell.draw('page');
                }
                else if (!option.hasOwnProperty("validate")) {
                    var rowData = row.data();
                    if (option !== 'undefined' && option.type === 'select') {
                        rowData[option.data] = value;
                    }
                    //row.data(rowData);
                    cell.data(text);
                    tableNeedConfig.cell(lastEdit.parent('td')).draw(false);
                    return true;
                    //cell.draw('page');
                }
                return false;
            });
            if (!x)
                return;
        }
        lastTD = this;
        //var a = GenertateSelector(tableNeedConfig.row($(this).parent('tr')), tableNeedConfig.cell(this), editOptions, this, function (cell, self, cellOption) {

        //    var nodeName = self.prop('nodeName').toLowerCase();
        //    if (cellOption !== 'undefined' && cellOption.type === 'select') {
        //        var r = tableNeedConfig.row(cell.index().row);
        //        var rData = r.data();
        //        rData[cellOption.data] = self.val();
        //        //r.data(rData);
        //        cell.data(self.children("option").filter(":selected").text());
        //    }
        //    else {
        //        cell.data(self.val());
        //    }
        //    //cell.draw('page');
        //    lastTD = null;
        //});
        var a = GenertateSelector(tableNeedConfig.row($(this).parent('tr')), tableNeedConfig.cell(this), editOptions, this, function (cell, self, cellOption) {
            if (cellOption.hasOwnProperty("validate") && cellOption.validate(self.val(), "", tableNeedConfig.row(cell.index().row), tableNeedConfig.rows().data())) {
                var nodeName = self.prop('nodeName').toLowerCase();
                if (cellOption !== 'undefined' && cellOption.type === 'select') {

                    var r = tableNeedConfig.row(cell.index().row);
                    var rData = r.data();
                    rData[cellOption.data] = self.val();
                    //r.data(rData);
                    cell.data(self.children("option").filter(":selected").text());
                }
                else {

                    cell.data(self.val());
                }
                //cell.draw('page');
                lastTD = null;
                return true;
            }
            else if (!cellOption.hasOwnProperty("validate")) {
                var nodeName = self.prop('nodeName').toLowerCase();
                if (cellOption !== 'undefined' && cellOption.type === 'select') {

                    var r = tableNeedConfig.row(cell.index().row);
                    var rData = r.data();
                    rData[cellOption.data] = self.val();
                    //r.data(rData);
                    cell.data(self.children("option").filter(":selected").text());
                }
                else {

                    cell.data(self.val());
                }
                //cell.draw('page');
                lastTD = null;
                return true;
            }
            return false;
        });
    });

    $("#dataConfig").on("click", ".create", function () {
        var businessKey = $(this).data("key");

        $("#modal_newSequence").data("businesskey", businessKey);
        $.when(
            $.ajax({
                method: "GET",
                url: "/PixelPointConfigTemplate/AddNewSequenceConfig",
                data: { stationBusinessKey: businessKey }, //, sequence: sequence
                success: (res) => {
                    console.log(res)
                    if (res && res.data && res.data.length > 0) {
                        var product = res.data[0];
                        lastTD = null;
                        currentSeq = product.Sequence;
                        tableNeedConfig.clear().rows.add(res.data).draw();

                    }
                }
            }),
            $.ajax({
                method: "GET",
                url: "/PixelPointConfigTemplate/GetListStationConfig",
                data: { stationBusinessKey: businessKey },
                success: (rsp) => {
                    //console.log(rsp)
                    if (rsp && rsp.data && rsp.data.length > 0) {
                        var product = rsp.data[0];
                        tableListConfig.clear().rows.add(rsp.data).draw();
                    }
                }
            }),
            $.ajax({
                method: "GET",
                url: "/PixelPointConfigTemplate/GetInfoOneStation",
                data: { stationBusinessKey: businessKey },
                success: (rs) => {
                    //console.log(rs);
                    if (rs && rs.data && rs.data.length) {
                        var product = rs.data[0];
                        $("#modal_station_storeName").text(product.StoreName);
                        $("#modal_station_revenueCenter").text(product.RevenueCenterName);
                        $("#modal_station_BusinessKey").text(product.StationBusinessKey);
                        $("#modal_station_Name").text(product.StationName);
                        $("#modal_station_active").text(product.IsActiveStatus);
                       
                    }
                }
            })
        ).done(() => {
            $("#modal_full").modal('show');
        })

    });


    $("#dataConfig").on("click", ".edit", function (e) {

        var sequence = $(this).data("sequence");
        var businessKey = $(this).data("key");
        $("#modal_newSequence").data("businesskey", businessKey);
        $.when(
            $.ajax({
                method: "GET",
                url: "/PixelPointConfigTemplate/GetInfoNeedStationConfig",
                data: { stationBusinessKey: businessKey, sequence: sequence },
                success: (res) => {
                    //console.log(res)
                    if (res && res.data && res.data.length > 0) {
                        var product = res.data[0];
                        lastTD = null;
                        currentSeq = product.Sequence;
                        tableNeedConfig.clear().rows.add(res.data).draw();

                    }
                }
            }),
            $.ajax({
                method: "GET",
                url: "/PixelPointConfigTemplate/GetListStationConfig",
                data: { stationBusinessKey: businessKey },
                success: (rsp) => {
                    //console.log(rsp)
                    if (rsp && rsp.data && rsp.data.length > 0) {
                        var product = rsp.data[0];
                        tableListConfig.clear().rows.add(rsp.data).draw();
                    }
                }
            }),
            $.ajax({
                method: "GET",
                url: "/PixelPointConfigTemplate/GetInfoOneStation",
                data: { stationBusinessKey: businessKey },
                success: (rs) => {
                    //console.log(rs);
                    if (rs && rs.data && rs.data.length) {
                        var product = rs.data[0];
                        $("#modal_station_storeName").text(product.StoreName);
                        $("#modal_station_revenueCenter").text(product.RevenueCenterName);
                        $("#modal_station_BusinessKey").text(product.StationBusinessKey);
                        $("#modal_station_Name").text(product.StationName);
                        $("#modal_station_active").text(product.IsActiveStatus);
                    }
                }
            })
        ).done(() => {
            $("#modal_full").modal('show');
        })
    });
    function onNewSequence(e) {
        $("#modal_newSequence").off();
        var data = tableNeedConfig.rows().data();
        if (data.length > 0) {
            $("#modal_newProduct").off();
            return;
        }
        $.ajax({
            method: "GET",
            url: "/PixelPointConfigTemplate/AddNewSequenceConfig",
            data: { stationBusinessKey: $("#modal_newSequence").data("businesskey") }, //, sequence: sequence
            success: (res) => {
                if (res && res.data && res.data.length > 0) {
                    var product = res.data[0];
                    lastTD = null;
                    currentSeq = product.Sequence;
                    tableNeedConfig.clear().rows.add(res.data).draw();
                    var dataListProduct = tableListConfig.rows().data();
                    for (var i = 0; i < dataListProduct.length; i++) {
                        delete dataListProduct[i].modal_is_need_edit;
                    }
                    tableListConfig.draw();
                    $("#modal_newProduct").off();
                    $("#modal_newProduct").on('click', onNewProduct);
                }
            }
        })
    }
    $("#modal_newSequence").off('click', onNewSequence);

    $("#modal_save").on('click', function (e) {
        var data = tableNeedConfig.rows().data();
        var businessKey = $(this).data("key");
        console.log(businessKey);
        var arr = [];
        for (var i = 0; i < data.length; i++) {
            arr.push(data[i]);
        }

        var sum = 0;

        for (var v = 0; v < data.length; v++) {
            var x = data[v]["Value"];
            sum += parseFloat(x);
        }
        if (sum > 100) {
            alert("Giá trị tỷ lệ Tổng phải nhỏ hơn hoặc bằng 100");
            return false;
        }


        $.ajax({
            method: 'POST',
            url: '/PixelPointConfigTemplate/Save',
            data: { Models: arr },
            success: function (rs) {
                if (rs.success) {
                    alert("Lưu dữ liệu thành công!");
                    var dataListProduct = rs.data;
                    for (var i = 0; i < dataListProduct.length; i++) {
                        dataListProduct[i]["modal_is_need_edit"] = true;
                    }
                    tableListConfig.clear().rows.add(dataListProduct).draw();
                    tableNeedConfig.clear().draw();
                    lastTD = null;
                    $("#modal_newProduct").off('click', onNewProduct);
                    $("#modal_newSequence").off('click', onNewSequence);
                    $("#modal_newSequence").on('click', onNewSequence);
                }
                else {
                    alert("Dữ liệu chưa được lưu lại!");
                }
            }
        })

    });

    function onNewProduct(e) {
        var data = tableNeedConfig.rows().data();
        if (data.length > 0) {
            $("#modal_newSequence").off();
            var maxData = data[0];
            maxData = JSON.parse(JSON.stringify(maxData));
            maxData['modal_is_new_sequence'] = true;
            if (maxData.hasOwnProperty("ID"))
                delete maxData["ID"];
            tableNeedConfig.row.add(maxData).draw(false);
        }

    }
    $("#modal_newProduct").on('click', onNewProduct);
    $('#modal_cancle').on('click', function (e) {
        var data = tableNeedConfig.rows().data();
        for (var i = 0; i < data.length; i++) {
            var rowData = data[i];
            if (rowData.hasOwnProperty('model_is_edit') && rowData['model_is_edit']) {
                var oldData = JSON.parse(rowData['modal_old_data']);
                tableListConfig.row.add(oldData).draw();
            }
        }
        var dataListProduct = tableListConfig.rows().data();
        for (var i = 0; i < dataListProduct.length; i++) {
            dataListProduct[i]["modal_is_need_edit"] = true;
        }
        tableListConfig.draw();
        tableNeedConfig.clear().draw();
        lastTD = null;
        $("#modal_newProduct").on('click', onNewProduct);
        $("#modal_newSequence").off('click', onNewSequence);
        $("#modal_newSequence").on('click', onNewSequence);
    })

    $('select').filter('[name=StoreBusinessKey],[name=RevenueCenterBusinessKey],[name=StationBusinessKey],[name=IsActive]').each(function (i, e) {
        $(this).val($(this).data('value')).trigger('change');
        console.log($(this).val);
    })

})