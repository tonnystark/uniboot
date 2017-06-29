var user = {
    init: function () {
        user.loadProvince();
        user.registerEvent();
    },
    registerEvent:function(){
        $('#ddlProvince').off('change').on('change', function () {
            var id = $(this).val();
            if (id != '') {
                user.loadDistrict(id);
            }
            else
            {
                $('#ddlDistrict').html('');
            }
        });
        $('#ddlDistrict').off('change').on('change', function () {
            var id = $(this).val();
            if (id != '') {
                user.loadWard(id);
            }
            else {
                $('#ddlWard').html('');
            }
        });
    },
    loadProvince: function () {      
        $.ajax({
            url: '/User/LoadProvince',
            type: "POST",
            dataType: "json",
            success: function (res) {
                var html = "<option value=''>Chọn tỉnh thành</option>";
                if (res.status == true) {
                    // danh sách các tỉnh thành trả về từ server
                    var data = res.data;
                    $.each(data, function (i, item) {
                        html += '<option value="' + item.ID + '" >' + item.Name + "</option>";
                    });
                    $('#ddlProvince').html(html);
                }
            }
        });
    },
    loadDistrict: function (provinceID) {
        $.ajax({
            url: '/User/LoadDistrict',
            type: "POST",
            dataType: "json",
            // truyền tham số vào cho hàm ở Controller
            data:{provinceID: provinceID},
            success: function (res) {
                var html = "<option>Chọn quận huyện</option>";
                if (res.status == true) {
                    // danh sách các tỉnh thành trả về từ server
                    var data = res.data;
                    $.each(data, function (i, item) {
                        html += '<option value="' + item.ID + '" >' + item.Name + "</option>";
                    });
                    $('#ddlDistrict').html(html);
                }
            }
        });
    },
     loadWard: function (districtID) {
        $.ajax({
            url: '/User/LoadWard',
            type: "POST",
            dataType: "json",
            // truyền tham số vào cho hàm ở Controller
            data: { districtID: districtID },
            success: function (res) {
                var html = "<option>Chọn xã/ thị trấn</option>";
                if (res.status == true) {
                    // danh sách các tỉnh thành trả về từ server
                    var data = res.data;
                    $.each(data, function (i, item) {
                        html += '<option value="' + item.ID + '" >' + item.Name + "</option>";
                    });
                    $('#ddlWard').html(html);
                }
            }
        });
    }
}
user.init();
