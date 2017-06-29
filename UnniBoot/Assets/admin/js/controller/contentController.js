var proc = {
    init: function () {
        proc.registerEvents();
    },
    registerEvents: function () {
        $('.btn-active').off('click').on('click', function (e) {

            e.preventDefault();
                    
            var btn = $(this);
             // lấy ra id của button hiện tại từ data-...
            // có thể thay bằng prop('id') properties  
            var procId = btn.data('id');

            $.ajax({
                // url: tên Controler/ Action
                url: "/Admin/Content/ChangeStatus",
                // id ở đây là tên tham số của hàm ChangeStatus
                data: { id: procId },
                // kiểu trả về json của hàm trên
                dataType: "json",
                type:"POST",               
                success: function (response) {
                    // đặt log
                    console.log(response);
                    if (response.status == true)
                        btn.text('Actived')
                    else
                        btn.text('Blocked')
                }
            });
        });
    }
}
proc.init();
