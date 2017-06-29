/*
- cart là 1 đối tượng có 2 property là 2 method:
+ init()
+ regEvents()

*/
var cart = {
    init: function () {
        cart.regEvents();
    },
    regEvents: function () {
        $('#btnContinue').off('click').on('click', function () {
            // redirect sang trang chủ
            window.location.href = "/";
        });

        $('#btnPayment').off('click').on('click', function () {
            // redirect sang trang chủ
            window.location.href = "/thanh-toan";
        });

        $('#btnUpdate').off('click').on('click', function () {
            var lstProduct = $('.txtQuantity');
            var listCart = [];
            $.each(lstProduct, function (i, item) {
                listCart.push({
                    Product: {
                        ID: $(item).data('id')
                    },
                    Quantity: $(item).val()
                });
            });

            $.ajax({
                // url có dạng: Tên Controller/Action
                url: '/Cart/Update',
                //  cartModel tên của param trong Action
                data: { cartModel: JSON.stringify(listCart) },
                // kiểu trả về json của hàm trên
                dataType: "json",
                type: "POST",
                success: function (response) {
                    // đặt log
                    console.log(response);
                    if (response.status == true) {
                        window.location.href = "/gio-hang";
                    }
                }
            })
        });

        $('#btnDelete').off('click').on('click', function () {
            $.ajax({
                // url có dạng: Tên Controller/Action
                url: '/Cart/DeleteAll',
                // kiểu trả về json của hàm trên
                dataType: "json",
                type: "POST",
                success: function (response) {
                    if (response.status == true) {
                        window.location.href = "/gio-hang";
                    }
                }
            })
        });

        $('.btn-deleteItem').off('click').on('click', function (e) {
            e.preventDefault();
            $.ajax({
                // url có dạng: Tên Controller/Action
                url: '/Cart/DeleteItem',
                data: {id: $(this).data('id')} ,
                // kiểu trả về json của hàm trên
                dataType: "json",
                type: "POST",
                success: function (response) {
                    if (response.status == true) {
                        window.location.href = "/gio-hang";
                    }
                }
            })
        });
    }
}
cart.init();
