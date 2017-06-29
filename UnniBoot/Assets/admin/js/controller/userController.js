var user = {
    init: function () {
        user.registerEvents();
    },
    registerEvents: function () {
        $('.btn-active').off('click').on('click', function (e) {

            e.preventDefault();
                    
            var btn = $(this);
             // lấy ra id của button hiện tại từ data-...
            // có thể thay bằng prop('id') properties  
            var userId = btn.data('id');

            $.ajax({
                // url: tên Controler/ Action
                url: "/Admin/User/ChangeStatus",
                // id ở đây là tên tham số của hàm ChangeStatus
                data: { id: userId },
                // kiểu trả về json của hàm trên
                dataType: "json",
                type:"POST",               
                success: function (response) {
                    // đặt log
                    console.log(response);
                    if (response.status == true)
                        btn.text('Actived')
                    else
                        btn.text('Block')
                }
            });
        });
    }
}
user.init();
