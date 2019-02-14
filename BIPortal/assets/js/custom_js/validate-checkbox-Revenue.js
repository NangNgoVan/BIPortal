function onSubmit() {
    var fields = $("input[name='GetColumn']").serializeArray();
    var fieldsBusinessType = $("input[name='BusinessType']").serializeArray();

    if (fieldsBusinessType.length == 0) {
        alert('Vui lòng chọn loại hình doanh thu !');
        // cancel submit
        return false;
    }
    if (fields.length == 0) {
        alert('Vui lòng chọn thông tin các cột hiển thị !');
        // cancel submit
        return false;
    }
}

// register event on form, not submit button
$('#subscribeForm').submit(onSubmit);