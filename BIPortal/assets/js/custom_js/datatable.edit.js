
function OnLastEdit(input, row, cell, options, onedit) {
    var cellIndex = cell.index();
    var rowData = row.data();
    var cellOption = options.find(function (e) {
        return e.column === cellIndex.column;
    });
    if (typeof cellOption === "undefined") {
        return onedit(row, cell, cellOption, input.text(),input.val());
        return;
    }
    switch (cellOption.type) {
        case 'select':
            return onedit(row, cell, cellOption, input.children("option").filter(":selected").text(), input.val());
            break;
        case 'text':
            return onedit(row, cell, cellOption, input.val(), input.val());
        default:
            return onedit(row, cell, cellOption, input.text());
            break;
    }
    return true;
}
function GenertateSelector(row, cell, options, self, onedit) {
    var cellIndex = cell.index();
    var rowData = row.data();
    var cellOption = options.find(function (e) {
        return e.column === cellIndex.column;
    });
    if (typeof cellOption === "undefined") {
        var a = $('<input class="td_edit" type="text"/>');
        a.val(cell.data());
        a.data('old-value', cell.data());
        $(self).empty().append(a);
        a.focus();
        a.keypress(function (e) {
            if (e.keyCode == 13) {
                onedit(cell, a, cellOption);
            }
        });
        return a;
    }
    switch (cellOption.type) {
        case 'datepicker':
            var a = $('<input class="td_edit" type="text"/>');
            a.val(cell.data());
            a.datepicker({
                format: cellOption.format,
                autoclose: true
            }).on('changeDate', function (e) {
                $(this).focus();
            });
            a.data('old-value', cell.data());
            $(self).empty().append(a);
            a.focus();
            a.keypress(function (e) {
                if (e.keyCode == 13) {
                    onedit(cell, a, cellOption);
                }
            });
            a.on('click', function (e) {
                a.datepicker('show');
            });
            return a;
        case 'select':
            var a = $('<select class="td_edit"></select>');
            var prev_val = rowData[cellOption.data];
            cellOption.source.forEach(function (e) {
                a.append($('<option value="' + e.value + '">' + e.display + '</option>'));
            });
            a.data('old-value', cell.data());
            a.val(rowData[cellOption.data]);
            $(self).empty().append(a);
            a.focus().select();
            a.trigger("click");
            a.on('change', function (e) {
                if (!onedit(cell, a, cellOption)){
                    $(this).val(prev_val);
                    return false;
                }
            })
            return a;
        default:
            var a = $('<input class="td_edit" type="text"/>');
            a.val(cell.data());
            a.data('old-value', cell.data());
            $(self).empty().append(a);
            a.focus();
            a.keypress(function (e) {
                if (e.keyCode == 13) {
                    onedit(cell, a, cellOption);
                }
            });
            return a;

    }
}