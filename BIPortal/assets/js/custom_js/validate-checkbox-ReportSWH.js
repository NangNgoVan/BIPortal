function onSubmit() {
    var fieldsOrganization = $("input[name='Organization']").serializeArray();
     if (fieldsOrganization.length == 0) {
        alert('Vui lòng chọn thông tin mã đơn vị !');
        // cancel submit
        return false;
    }
}

// register event on form, not submit button
$('#subscribeForm').submit(onSubmit);