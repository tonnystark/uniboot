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
                url: "/Admin/Product/ChangeStatus",
                // id ở đây là tên tham số của hàm ChangeStatus
                data: { id: procId },
                // kiểu trả về json của hàm trên
                dataType: "json",
                type: "POST",
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

        $('.btn-images').off('click').on('click', function (e) {
            e.preventDefault();
            // hiện lên model
            $('#imgManager').modal('show');
            $('#hidProductID').val($(this).data('id'));
            proc.loadImage();
        });

        $('#btn-chooseImg').off('click').on('click', function (e) {
            e.preventDefault();
            var finder = new CKFinder();
            finder.selectActionFunction = function (url) {
                $('#imgList').append('<div style="float:left"><img src="' + url + '" width="100"/><a href="#" class="btn-delImg"><i class="fa fa-times"></i></a></div>');

                $('.btn-delImg').off('click').on('click', function (e) {
                    // chặn link chạy
                    e.preventDefault();
                    // parent là phần tử trên nó 1 cấp, ở đây là "<img>"
                    $(this).parent().remove();
                });
            };
            finder.popup();
        });

        $('#btn-saveImg').off('click').on('click', function () {
            // mảng chứa image
            var imgArr = [];

            // lấy ra id của product hiện tại
            var id = $('#hidProductID').val();

            // lặp qua imgList chọn ra các thẻ <img>
            $.each($('#imgList img'), function (i, item) {
                // thêm vào mảng imgArr thuộc tính 'src' của từng phầntử img
                imgArr.push($(item).prop('src'));
            });
            $.ajax({
                url: '/Admin/Product/SaveImages',
                type: 'POST',
                dataType: 'json',
                // chuyển jSon sang String
                data:
                    {
                        images: JSON.stringify(imgArr),
                        productID: id
                    },
                success: function (res) {
                    if (res.status) {
                        // đóng  model
                        $('#imgManager').modal('hide');
                        $('#imgList').html('');
                        alert('Lưu ảnh thành công');
                    }                    
                }
            });
        });
    },
    loadImage: function () {
              $.ajax({
              url: '/Admin/Product/LoadImages',
                type: 'GET',
                dataType: 'json',
                // chuyển jSon sang String
                data:
                    {                      
                        id: $('#hidProductID').val()
                    },
                success: function (res) {
                    if (res.status) {
                        var data = res.data;
                        var html = '';
                        $.each(data, function (i, item) {
                            html += '<div style="float:left"><img src="' + item + '" width="100"/><a href="#" class="btn-delImg"><i class="fa fa-times"></i></a></div>';
                        })

                        $('#imgList').html(html);

                        $('.btn-delImg').off('click').on('click', function (e) {
                            // chặn link chạy
                            e.preventDefault();
                            // parent là phần tử trên nó 1 cấp, ở đây là "<img>"
                            $(this).parent().remove();
                        });
                    }
                }
            });
    }
}
proc.init();
