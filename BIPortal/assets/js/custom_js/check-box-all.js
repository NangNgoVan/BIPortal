

$(document).ready(function () {
    $("#checkedAll").change(function () {
        if (this.checked) {
            $('[id*="checkSingle"]').each(function () {
                this.checked = true;
            })
        } else {
            $('[id*="checkSingle"]').each(function () {
                this.checked = false;
            })
        }
    });

    $('[id*="checkSingle"]').click(function () {
        if ($(this).is(":checked")) {
            var isAllChecked = 0;
            $('[id*="checkSingle"]').each(function () {
                if (!this.checked)
                    isAllChecked = 1;
            })
            if (isAllChecked == 0) { $("#checkedAll").prop("checked", true); }
        } else {
            $("#checkedAll").prop("checked", false);
        }
    });
});